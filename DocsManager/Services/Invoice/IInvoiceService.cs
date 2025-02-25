using DocsManager.Controllers.Types;
using DocsManager.Models.Dto;
using FluentResults;

namespace DocsManager.Services.Invoice;

public interface IInvoiceService
{
    public Task<InvoicesGridDto> GetInvoiceForGrid(Guid userId, int page);

    public Task<int> GetInvoiceCount(Guid userId);

    public Task<InvoiceDto?> GetInvoice(int id, BearerUser user);

    public Task<Result<Models.Invoice>> GetShortInvoice(int id, Guid user);

    public Task<Result<Models.Invoice>> InsertInvoice(InvoicePostDto invoicePostDto, Guid userId);

    public Task<bool> DeleteInvoice(int id, Guid userId);

    public Task<int?> GetLatestUserInvoice(Guid userId);

    public Task<bool> AddItemToInvoice(int id, ItemPostDto itemPost, Guid userId);

    public Task<bool> SetPayed(int id, IsPayedDto isPayed, Guid userId);

    public Task<bool> RemoveItemFromInvoice(int invoiceId, int itemId, Guid userId);

    public Task<Result> UpdateInvoice(InvoiceUpdatePost invoiceUpdatePost, int invoiceId, Guid userId);
}