using System.Globalization;
using DocsManager.Controllers.Types;
using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.User;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DocsManager.Services.Invoice;

public class InvoiceService(DocsManagementContext context, IntegerToWordsConverter.IntegerToWordsConverter itwc, IUserService userService)
    : IInvoiceService
{
    private const int InvoicePageSize = 10;
    private const double VatRate = 0.21;

    public async Task<InvoicesGridDto> GetInvoiceForGrid(Guid userId, int page)
    {
        page *= InvoicePageSize;
        var invoices = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == userId)
            .OrderBy(x => x.InvoiceId)
            .Skip(page)
            .Take(InvoicePageSize)
            .Select(x =>
                new InvoiceListDto(
                    x.SeriesNumber,
                    x.InvoiceDate,
                    x.IsPayed,
                    x.InvoiceClient.BuyerName,
                    x.Items.Sum(item => item.PriceOfUnit * item.Units)))
            .ToListAsync();
        var sum = invoices.Sum(invoiceListDto => invoiceListDto.TotalSum);
        return new InvoicesGridDto(invoices, sum);
    }

    public async Task<InvoiceDto?> GetInvoice(int id, BearerUser user)
    {
        var invoice = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == user.UserId)
            .Where(invoice => invoice.SeriesNumber == id)
            .Select(x => new InvoiceDto
            {
                SeriesNumber = x.SeriesNumber,
                Address = x.InvoiceUser.Address,
                BankName = x.InvoiceUser.BankName,
                BankNumber = x.InvoiceUser.BankNumber,
                BuyerAddress = x.InvoiceClient.BuyerAddress,
                BuyerCode = x.InvoiceClient.BuyerCode,
                VatCode = x.InvoiceClient.VatCode,
                BuyerName = x.InvoiceClient.BuyerName,
                PersonalId = x.InvoiceUser.PersonalId,
                Date = x.InvoiceDate.ToString("yyyy MM dd"),
                FreelanceWorkId = x.InvoiceUser.FreelanceWorkId,
                Name = user.Name + " " + user.LastName,
                NameWithInitials = $"{user.Name.First()}{user.LastName.First()}",
                Products = x.Items.Select(x => new ItemDto(x)).ToList(),
                UserVatCode = x.InvoiceUser.VatCode,
                VatRate = VatRate
            })
            .FirstOrDefaultAsync();
        if (invoice == null) return null;
        var totalCost = invoice.Products.Sum(product => product.TotalPrice);

        if(invoice.UserVatCode != null)
        {
            var pvm = (totalCost * (decimal)VatRate);
            invoice.Pvm = pvm.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT"));
            invoice.TotalWithoutVat  = totalCost.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT"));
            totalCost += pvm;
        }

        invoice.TotalMoney = totalCost.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT"));

        invoice.SumInWords = itwc.ConvertSumToWords(totalCost);

        return invoice;
    }

    public async Task<Result<Models.Invoice>> InsertInvoice(InvoicePostDto invoicePostDto, Guid userId)
    {
        var userModel = await context.Users.FindAsync(userId);
        var clientModel = await context.Clients.FindAsync(invoicePostDto.ClientId);
        if (userModel == null || clientModel == null) return Result.Fail("User or client does not exist");

        var invoice = new Models.Invoice
        {
            InvoiceUser = userModel,
            InvoiceDate = DateTime.Parse(invoicePostDto.InvoiceDate).ToUniversalTime(),
            InvoiceClient = clientModel,
            SeriesNumber = invoicePostDto.SeriesNumber
        };
        context.Invoices.Add(invoice);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is not PostgresException sqlException) throw;
            if(sqlException.SqlState == "23505")
            {
                return Result.Fail("Invoice with invoice number already exists");
            }

            throw;
        }


        foreach (var itemPostDto in invoicePostDto.Items)
        {
            var itemPost = new InvoiceItem
            {
                InvoiceId = invoice.InvoiceId,
                Name = itemPostDto.Name,
                PriceOfUnit = itemPostDto.PriceOfUnit,
                Units = itemPostDto.Units,
                UnitOfMeasurement = itemPostDto.UnitOfMeasurement
            };
            context.InvoiceItems.Add(itemPost);
        }

        await context.SaveChangesAsync();

        return Result.Ok(invoice);
    }

    public async Task<bool> DeleteInvoice(int id, Guid userId)
    {
        var invoice = await context.Invoices
            .Where(invoice => invoice.SeriesNumber == id)
            .Where(invoice => invoice.InvoiceUserId == userId)
            .FirstOrDefaultAsync();
        if (invoice == null) return false;

        context.Invoices.Remove(invoice);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int?> GetLatestUserInvoice(Guid userId)
    {
        var last = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == userId)
            .OrderByDescending(invoice => invoice.SeriesNumber)
            .Select(invoice => invoice.SeriesNumber)
            .FirstOrDefaultAsync();
        if (last == default) return null;
        return last;
    }

    public async Task<bool> AddItemToInvoice(int id, ItemPostDto itemPost, Guid userId)
    {
        var invoice = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == userId)
            .Where(invoice => invoice.SeriesNumber == id)
            .FirstOrDefaultAsync();
        if (invoice == null) return false;

        var invoiceItem = new InvoiceItem
        {
            InvoiceId = invoice.InvoiceId,
            PriceOfUnit = itemPost.PriceOfUnit,
            UnitOfMeasurement = itemPost.UnitOfMeasurement,
            Units = itemPost.Units,
            Name = itemPost.Name
        };

        context.InvoiceItems.Add(invoiceItem);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetPayed(int id, IsPayedDto isPayed, Guid userId)
    {
        var invoice = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == userId)
            .Where(invoice => invoice.SeriesNumber == id)
            .FirstOrDefaultAsync();
        if (invoice == null) return false;

        invoice.IsPayed = isPayed.isPayed;

        context.Entry(invoice).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InvoiceExists(invoice.InvoiceId))
                return false;
            throw;
        }

        return true;
    }

    public async Task<bool> RemoveItemFromInvoice(int invoiceId, int invoiceItemId, Guid userId)
    {
        var invoiceItem = await context.InvoiceItems
            .Where(invoiceItem => invoiceItem.Invoice.SeriesNumber == invoiceId)
            .Where(invoiceItem => invoiceItem.InvoiceItemId == invoiceItemId)
            .Where(invoiceItem => invoiceItem.Invoice.InvoiceUserId == userId)
            .FirstOrDefaultAsync();
        if (invoiceItem == null) return false;

        context.InvoiceItems.Remove(invoiceItem);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<int> GetInvoiceCount(Guid userId)
    {
        var count = await context.Invoices.Where(invoice => invoice.InvoiceUserId == userId).CountAsync();
        return count;
    }
    
    public async Task<Result> UpdateInvoice(InvoiceUpdatePost invoicePostDto, int invoiceId, Guid userId)
    {
        var invoice = await context.Invoices
            .Where(invoice => invoice.SeriesNumber == invoiceId)
            .Where(invoice => invoice.InvoiceUserId == userId)
            .FirstOrDefaultAsync();
        if (invoice == null) return Result.Fail("Invoice not found");
        invoice.SeriesNumber = invoicePostDto.SeriesNumber;
        invoice.InvoiceDate = DateTime.Parse(invoicePostDto.InvoiceDate).ToUniversalTime();
        invoice.InvoiceClientId = invoicePostDto.InvoiceClientId;
        context.Entry(invoice).State = EntityState.Modified;
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case DbUpdateConcurrencyException when !InvoiceExists(invoice.InvoiceId):
                    return Result.Fail("Invoice got deleted");
                case DbUpdateConcurrencyException:
                    throw;
                case DbUpdateException:
                {
                    if (ex.InnerException is not PostgresException sqlException) throw;
                    if(sqlException.SqlState == "23505")
                    {
                        return Result.Fail("Invoice with invoice number already exists");
                    }

                    break;
                }
            }

            throw;
        }

        return Result.Ok();
    }
    
    public async Task<Result<Models.Invoice>> GetShortInvoice(int id, Guid userId)
    {
        var invoice = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == userId)
            .Where(invoice => invoice.SeriesNumber == id)
            .FirstOrDefaultAsync();
        if (invoice == null) return Result.Fail("Invoice not found");
        return Result.Ok(invoice);
    }

    private bool InvoiceExists(int id)
    {
        return context.Invoices.Any(e => e.InvoiceId == id);
    }
}
