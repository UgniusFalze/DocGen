using System.Globalization;
using System.Security.Claims;
using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.IntegerToWordsConverter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController(DocsManagementContext context, IntegerToWordsConverter itwc) : ControllerBase
{
    private Guid? GetUserGuid()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null) return null;

        return Guid.Parse(user);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<InvoicesGridDto>> GetInvoices()
    {
        var user = GetUserGuid();

        if (user == null) return NotFound();

        var invoices = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == user)
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
    
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound();
        var invoice = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == user)
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
                Date = x.InvoiceDate.ToString("yyyy MM dd"),
                FreelanceWorkId = x.InvoiceUser.FreelanceWorkId,
                Name = x.InvoiceUser.FirstName + " " + x.InvoiceUser.LastName,
                Products = x.Items.Select(x => new ItemDto(x)).ToList(),
                NameWithInitials = x.InvoiceUser.FirstName.First() + ". " + x.InvoiceUser.LastName
            })
            .FirstOrDefaultAsync();
        if (invoice == null) return NotFound();
        var totalCost = invoice.Products.Sum(product => product.TotalPrice);
        invoice.TotalMoney = totalCost.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT"));
        invoice.SumInWords = itwc.ConvertSumToWords(totalCost);
        
        return invoice;
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
    {
        if (id != invoice.InvoiceId) return BadRequest();

        context.Entry(invoice).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InvoiceExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Invoice>> PostInvoice(InvoicePostDto invoicePost)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null) return NotFound();

        var userGuid = Guid.Parse(user);

        var userModel = await context.Users.FindAsync(userGuid);
        var clientModel = await context.Clients.FindAsync(invoicePost.ClientId);

        if (userModel == null || clientModel == null) return NotFound();
        var invoice = new Invoice
        {
            InvoiceUser = userModel,
            InvoiceDate = DateTime.Parse(invoicePost.InvoiceDate).ToUniversalTime(),
            InvoiceClient = clientModel,
            SeriesNumber = invoicePost.SeriesNumber
        };

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();
        foreach (var itemPostDto in invoicePost.Items)
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
        return CreatedAtAction("GetInvoice", new { id = invoice.InvoiceId }, invoice);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var invoice = await context.Invoices.FindAsync(id);
        if (invoice == null) return NotFound();

        context.Invoices.Remove(invoice);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool InvoiceExists(int id)
    {
        return context.Invoices.Any(e => e.InvoiceId == id);
    }


    [HttpGet("last")]
    public async Task<ActionResult<int>> GetLatestInvoice()
    {
        var user = GetUserGuid();
        if (user == null) return NotFound();

        var last = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == user)
            .OrderByDescending(invoice => invoice.SeriesNumber)
            .Select(invoice => invoice.SeriesNumber)
            .FirstOrDefaultAsync();
        return last;
    }

    [Authorize]
    [HttpPost("{id}/AddItem")]
    public async Task<IActionResult> AddItem(int id, ItemPostDto itemPost)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound();

        var invoice = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == user)
            .Where(invoice => invoice.SeriesNumber == id)
            .FirstOrDefaultAsync();
        if (invoice == null)
        {
            return NotFound();
        }

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
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("{id}/setPayed")]
    public async Task<IActionResult> SetPayed(int id, IsPayedDto isPayed)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var invoice = await context.Invoices
            .Where(invoice => invoice.InvoiceUserId == user)
            .Where(invoice => invoice.SeriesNumber == id)
            .FirstOrDefaultAsync();
        if (invoice == null)
        {
            return NotFound("Invoice not found");
        }

        invoice.IsPayed = isPayed.isPayed;
        
        context.Entry(invoice).State = EntityState.Modified;
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InvoiceExists(id))
                return NotFound("Invoice not found");
            throw;
        }

        return NoContent();
    }   
}