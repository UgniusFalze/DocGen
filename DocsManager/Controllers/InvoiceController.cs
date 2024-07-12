using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.Invoice;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoiceController(IInvoiceService invoiceService) : ControllerWithUser
{
    [HttpGet]
    public async Task<ActionResult<InvoicesGridDto>> GetInvoices([FromQuery(Name = "page")] int page)
    {
        var user = GetUserGuid();

        if (user == null) return NotFound("User not found");
        return Ok(await invoiceService.GetInvoiceForGrid(user.Value, page));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var invoice = await invoiceService.GetInvoice(id, user.Value);
        return invoice == null ? NotFound("Invoice not found") : Ok(invoice);
    }
    
    [HttpPost]
    public async Task<ActionResult<Invoice>> PostInvoice(InvoicePostDto invoicePost)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.InsertInvoice(invoicePost, user.Value);
        
        return result == null ? NotFound("Client or user not found") : CreatedAtAction("GetInvoice", new { id = result.InvoiceId }, result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.DeleteInvoice(id, user.Value);
        return result ? NoContent() : NotFound("Invoice not found");
    }


    [HttpGet("last")]
    public async Task<ActionResult<int>> GetLatestInvoice()
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.GetLatestUserInvoice(user.Value);
        return result ?? 1;
    }
    
    [HttpPost("{id}/addItem")]
    public async Task<IActionResult> AddItem(int id, ItemPostDto itemPost)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.AddItemToInvoice(id, itemPost, user.Value);
        return result ? NoContent() : NotFound("Invoice not found");
    }
    
    [HttpDelete("{id}/deleteItem/{itemId}")]
    public async Task<IActionResult> DeleteItem(int id, int itemId)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.RemoveItemFromInvoice(id, itemId, user.Value);
        return result ? NoContent() : NotFound("Invoice item not found");
    }
    
    [HttpPost("{id}/setPayed")]
    public async Task<IActionResult> SetPayed(int id, IsPayedDto isPayed)
    {
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.SetPayed(id, isPayed, user.Value);
        return result ? NoContent() : NotFound("Invoice not found");
    }
    
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetInvoiceCount(){
        var user = GetUserGuid();
        if (user == null) return NotFound("User not found");
        var result = await invoiceService.GetInvoiceCount(user.Value);
        return result;
    }
}