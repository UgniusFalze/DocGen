using DocsManager.Models.Dto;

namespace DocsManager.Services.Invoice;

public interface IInvoiceService
{
    public Task<InvoicesGridDto> GetInvoiceForGrid(Guid userId, int page);

    public Task<int> GetInvoiceCount(Guid userId);

    public Task<InvoiceDto?> GetInvoice(int id, Guid userId);

    public Task<Models.Invoice?> InsertInvoice(InvoicePostDto invoicePostDto, Guid userId);

    public Task<bool> DeleteInvoice(int id, Guid userId);

    public Task<int?> GetLatestUserInvoice(Guid userId);

    public Task<bool> AddItemToInvoice(int id, ItemPostDto itemPost, Guid userId);

    public Task<bool> SetPayed(int id, IsPayedDto isPayed, Guid userId);

    public Task<bool> RemoveItemFromInvoice(int invoiceId, int itemId, Guid userId);
}