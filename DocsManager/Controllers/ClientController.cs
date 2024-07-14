using DocsManager.Models;
using DocsManager.Models.Dto;
using DocsManager.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace DocsManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientController(IClientService clientService) : ControllerWithUser
{
    /// <summary>
    ///     Get filtered clients
    /// </summary>
    /// <param name="page">Client record pagination, each page consists of 10 clients</param>
    /// <param name="search">Search parameter based on clients full name</param>
    /// <returns>Returns clients based on filters</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetClients([FromQuery(Name = "page")] int page,
        [FromQuery(Name = "search")] string? search)
    {
        return Ok(await clientService.GetClients(page, search));
    }

    /// <summary>
    ///     Get all clients with names and ids
    /// </summary>
    /// <returns>Returns clients with names and ids</returns>
    [HttpGet("select")]
    public async Task<ActionResult<IEnumerable<ClientDTO>>> GetClientsSelect()
    {
        return Ok(await clientService.GetSelectableClients());
    }

    /// <summary>
    ///     Gets client from id
    /// </summary>
    /// <param name="id">Id of the client</param>
    /// <returns>Client with specified id</returns>
    /// <response code="200">Returns the client from id</response>
    /// <response code="404">If client is not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetClient(int id)
    {
        var client = await clientService.GetClient(id);
        if (client == null) return NotFound("Client not found");
        return Ok(client);
    }

    /// <summary>
    ///     Updates selected client
    /// </summary>
    /// <param name="id">Id of the client</param>
    /// <param name="client"></param>
    /// <response code="204">If client was successfully updated</response>
    /// <response code="400">If client's id does not match the posted client's id</response>
    /// <response code="404">If client is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutClient(int id, Client client)
    {
        if (id != client.ClientId) return BadRequest();
        var result = await clientService.UpdateClient(client);
        return result ? NoContent() : NotFound("Client not found");
    }

    /// <summary>
    ///     Inserts a new client
    /// </summary>
    /// <param name="client"></param>
    /// <returns>A newly created client</returns>
    /// <response code="200">Returns the created client</response>
    /// <response code="422">If client with code exists</response>
    [HttpPost]
    public async Task<ActionResult<Client>> PostClient(Client client)
    {
        var insertClient = await clientService.InsertClient(client);
        if (insertClient.IsFailed) return UnprocessableEntity(insertClient.Errors.First().Message);
        return CreatedAtAction("GetClient", new { id = insertClient.Value.ClientId }, insertClient);
    }

    /// <summary>
    ///     Deletes a client
    /// </summary>
    /// <param name="id">Client id</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">C# enums are not as robust, so need catch standard value</exception>
    /// <response code="204">Successfully deleted client</response>
    /// <response code="404">Client is not found</response>
    /// <response code="422">If client has invoices with other users</response>
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
            ClientDeleteResult.HasNonUserInvoices => UnprocessableEntity("Client has non-user invoices"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    ///     Gets client count
    /// </summary>
    /// <returns></returns>
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetClientCount()
    {
        return await clientService.GetClientCount();
    }
}