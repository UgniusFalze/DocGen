using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.Errors;
using DocsManager.Services.Invoice;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController(IInvoiceService invoiceService) : ControllerWithUser
{
    /// <summary>
    ///     Gets user invoices
    /// </summary>
    /// <param name="page">User invoices paging, each page consists of 10 records</param>
    /// <returns>User invoices</returns>
    /// <response code="200">Returns user invoices</response>
    /// <response code="404">If user is not found</response>
    [HttpGet]
    public async Task<ActionResult<InvoicesGridDto>> GetInvoices([FromQuery(Name = "page")] int page)
    {
        var user = GetCurrentUser();

        if (user == null) return NotFound("User not found");
        return Ok(await invoiceService.GetInvoiceForGrid(user.Value.UserId, page));
    }

    /// <summary>
    ///     Gets user invoice based on its series number
    /// </summary>
    /// <param name="id">Invoice series number for user</param>
    /// <returns>Matching invoice</returns>
    /// <response code="404">If user or invoice is not found</response>
    /// <response code="200">Returns matching invoice</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var invoice = await invoiceService.GetInvoice(id, user.Value);
        return invoice == null ? NotFound("Invoice not found") : Ok(invoice);
    }

    /// <summary>
    ///     Inserts a new invoice
    /// </summary>
    /// <param name="invoicePost"></param>
    /// <returns>Newly created invoice</returns>
    /// <response code="200">Returns newly created invoice</response>
    /// <response code="404">If user or provided client is not found</response>
    /// <response code="422">If invoice with number exists</response>
    [HttpPost]
    public async Task<ActionResult<Invoice>> PostInvoice(InvoicePostDto invoicePost)
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.InsertInvoice(invoicePost, user.Value.UserId);
        if (result.IsFailed)
            return result.Errors.First().Metadata.First().Value switch
            {
                DuplicationResultCode.NotFound => NotFound("User or client does not exist"),
                DuplicationResultCode.Duplication => UnprocessableEntity(
                    "Invoice with invoice number already exists"),
                _ => Problem("Error")
            };
        return Ok(result.Value);
    }

    /// <summary>
    ///     Deletes invoice
    /// </summary>
    /// <param name="id">Invoice series number</param>
    /// <returns></returns>
    /// <response code="204">Invoice was successfully deleted</response>
    /// <response code="404">If user or invoice is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.DeleteInvoice(id, user.Value.UserId);
        return result ? NoContent() : NotFound("Invoice not found");
    }

    /// <summary>
    ///     Gets latest invoice's series number
    /// </summary>
    /// <returns>Users latest invoice's series number</returns>
    /// <response code="200">Returns latest invoice number</response>
    /// <response code="404">If user is not found</response>
    [HttpGet("last")]
    public async Task<ActionResult<int>> GetLatestInvoice()
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.GetLatestUserInvoice(user.Value.UserId);
        return result ?? 1;
    }

    /// <summary>
    ///     Inserts a new item for invoice
    /// </summary>
    /// <param name="id">Invoice series number</param>
    /// <param name="itemPost"></param>
    /// <returns></returns>
    /// <response code="204">Invoice item was successfully added</response>
    /// <response code="404">If user or invoice is not found</response>
    [HttpPost("{id}/addItem")]
    public async Task<IActionResult> AddItem(int id, ItemPostDto itemPost)
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.AddItemToInvoice(id, itemPost, user.Value.UserId);
        return result ? NoContent() : NotFound("Invoice not found");
    }

    /// <summary>
    ///     Deletes invoice item
    /// </summary>
    /// <param name="id">Invoice series number</param>
    /// <param name="itemId"></param>
    /// <returns></returns>
    /// <response code="204">Invoice item was successfully deleted</response>
    /// <response code="404">If user or invoice is not found</response>
    [HttpDelete("{id}/deleteItem/{itemId}")]
    public async Task<IActionResult> DeleteItem(int id, int itemId)
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.RemoveItemFromInvoice(id, itemId, user.Value.UserId);
        return result ? NoContent() : NotFound("Invoice item not found");
    }

    /// <summary>
    ///     Sets invoice paid status
    /// </summary>
    /// <param name="id">Invoice series number</param>
    /// <param name="isPayed"></param>
    /// <returns></returns>
    /// <response code="204">Invoice paid status successfully updated</response>
    /// <response code="404">If user or invoice is not found</response>
    [HttpPost("{id}/setPayed")]
    public async Task<IActionResult> SetPayed(int id, IsPayedDto isPayed)
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.SetPayed(id, isPayed, user.Value.UserId);
        return result ? NoContent() : NotFound("Invoice not found");
    }

    /// <summary>
    ///     Updates invoice
    /// </summary>
    /// <param name="id"></param>
    /// <param name="post"></param>
    /// <returns></returns>
    /// <response code="204">Invoice updated successfully</response>
    /// <response code="422">If invoice with number exists</response>
    /// <response code="404">If user or invoice is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInvoice(int id, InvoiceUpdatePost post)
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.UpdateInvoice(post, id, user.Value.UserId);
        if (result.IsFailed)
            return result.Errors.First().Metadata.First().Value switch
            {
                DuplicationResultCode.NotFound => NotFound("Invoice does not exist"),
                DuplicationResultCode.Duplication => UnprocessableEntity(
                    "Invoice with invoice number already exists"),
                _ => Problem("Error")
            };

        return NoContent();
    }

    /// <summary>
    ///     Gets invoice without any items
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Invoice from id</returns>
    /// <response code="200">Returns invoice from id</response>
    /// <response code="404">If invoice not found</response>
    [HttpGet("shortInvoice/{id}")]
    public async Task<ActionResult<Invoice>> GetShortInvoice(int id)
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.GetShortInvoice(id, user.Value.UserId);
        if (result.IsFailed) return NotFound(result.Errors.First().Message);
        return Ok(result.Value);
    }

    /// <summary>
    ///     Gets user invoices count
    /// </summary>
    /// <returns>Invoice count</returns>
    /// <response code="200">Return invoices count</response>
    /// <response code="404">If user is not found</response>
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetInvoiceCount()
    {
        var user = GetCurrentUser();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.GetInvoiceCount(user.Value.UserId);
        return result;
    }
}