using System.Security.Claims;
using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientController(IClientService clientService) : ControllerWithUser
{
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetClients([FromQuery(Name = "page")] int page)
    {
        return Ok(await clientService.GetClients(page));
    }

    [HttpGet("select")]
    public async Task<ActionResult<IEnumerable<ClientDTO>>> GetClientsSelect()
    {
        return Ok(await clientService.GetSelectableClients());
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetClient(int id)
    {
        var client = await clientService.GetClient(id);
        if (client == null) return NotFound("Client not found");
        return Ok(client);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> PutClient(int id, Client client)
    {
        if (id != client.ClientId) return BadRequest();
        var result = await clientService.UpdateClient(client);
        return result ? NoContent() : NotFound("Client not found");

    }
    
    [HttpPost]
    public async Task<ActionResult<Client>> PostClient(Client client)
    {
        var insertClient = await clientService.InsertClient(client);
        return CreatedAtAction("GetClient", new { id = client.ClientId }, client);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var userId = GetUserGuid();
        if (userId == null) NotFound();
        var result = await clientService.DeleteClient(id, userId.Value);
        return result switch
        {
            ClientDeleteResult.Success => NoContent(),
            ClientDeleteResult.NoClient => NotFound("Client not found"),
            ClientDeleteResult.HasNonUserInvoices => UnprocessableEntity(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetClientCount()
    {
        return await clientService.GetClientCount();
    }
}