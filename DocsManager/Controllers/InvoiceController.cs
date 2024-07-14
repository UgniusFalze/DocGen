using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.Invoice;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController(IInvoiceService invoiceService) : ControllerWithUser
{
    /// <summary>
    /// Gets user invoices
    /// </summary>
    /// <param name="page">User invoices paging, each page consists of 10 records</param>
    /// <returns>User invoices</returns>
    /// <response code="200">Returns user invoices</response>
    /// <response code="404">If user is not found</response>
    [HttpGet]
    public async Task<ActionResult<InvoicesGridDto>> GetInvoices([FromQuery(Name = "page")] int page)
    {
        var user = GetUserGuid();

        if (user == null) return NotFound("User not found");
        return Ok(await invoiceService.GetInvoiceForGrid(user.Value, page));
    }
    
    /// <summary>
    /// Gets user invoice based on its series number
    /// </summary>
    /// <param name="id">Invoice series number for user</param>
    /// <returns>Matching invoice</returns>
    /// <response code="404">If user or invoice is not found</response>
    /// <response code="200">Returns matching invoice</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var invoice = await invoiceService.GetInvoice(id, user.Value);
        return invoice == null ? NotFound("Invoice not found") : Ok(invoice);
    }
    
    /// <summary>
    /// Inserts a new invoice
    /// </summary>
    /// <param name="invoicePost"></param>
    /// <returns>Newly created invoice</returns>
    /// <response code="200">Returns newly created invoice</response>
    /// <response code="404">If user or provided client is not found</response>
    [HttpPost]
    public async Task<ActionResult<Invoice>> PostInvoice(InvoicePostDto invoicePost)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.InsertInvoice(invoicePost, user.Value);
        
        return result == null ? NotFound("Client or user not found") : CreatedAtAction("GetInvoice", new { id = result.InvoiceId }, result);
    }
    
    /// <summary>
    /// Deletes invoice
    /// </summary>
    /// <param name="id">Invoice series number</param>
    /// <returns></returns>
    /// <response code="204">Invoice was successfully deleted</response>
    /// <response code="404">If user or invoice is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.DeleteInvoice(id, user.Value);
        return result ? NoContent() : NotFound("Invoice not found");
    }

    /// <summary>
    /// Gets latest invoice's series number
    /// </summary>
    /// <returns>Users latest invoice's series number</returns>
    /// <response code="200">Returns latest invoice number</response>
    /// <response code="404">If user is not found</response>
    [HttpGet("last")]
    public async Task<ActionResult<int>> GetLatestInvoice()
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.GetLatestUserInvoice(user.Value);
        return result ?? 1;
    }
    
    /// <summary>
    /// Inserts a new item for invoice
    /// </summary>
    /// <param name="id">Invoice series number</param>
    /// <param name="itemPost"></param>
    /// <returns></returns>
    /// <response code="204">Invoice item was successfully added</response>
    /// <response code="404">If user or invoice is not found</response>
    [HttpPost("{id}/addItem")]
    public async Task<IActionResult> AddItem(int id, ItemPostDto itemPost)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.AddItemToInvoice(id, itemPost, user.Value);
        return result ? NoContent() : NotFound("Invoice not found");
    }
    
    /// <summary>
    /// Deletes invoice item
    /// </summary>
    /// <param name="id">Invoice series number</param>
    /// <param name="itemId"></param>
    /// <returns></returns>
    /// <response code="204">Invoice item was successfully deleted</response>
    /// <response code="404">If user or invoice is not found</response>
    [HttpDelete("{id}/deleteItem/{itemId}")]
    public async Task<IActionResult> DeleteItem(int id, int itemId)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.RemoveItemFromInvoice(id, itemId, user.Value);
        return result ? NoContent() : NotFound("Invoice item not found");
    }
    
    /// <summary>
    /// Sets invoice paid status
    /// </summary>
    /// <param name="id">Invoice series number</param>
    /// <param name="isPayed"></param>
    /// <returns></returns>
    /// <response code="204">Invoice paid status successfully updated</response>
    /// <response code="404">If user or invoice is not found</response>
    [HttpPost("{id}/setPayed")]
    public async Task<IActionResult> SetPayed(int id, IsPayedDto isPayed)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.SetPayed(id, isPayed, user.Value);
        return result ? NoContent() : NotFound("Invoice not found");
    }
    
    /// <summary>
    /// Gets user invoices count
    /// </summary>
    /// <returns>Invoice count</returns>
    /// <response code="200">Return invoices count</response>
    /// <response code="404">If user is not found</response>
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetInvoiceCount(){
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.GetInvoiceCount(user.Value);
        return result;
    }
}