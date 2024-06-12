using System.Security.Claims;
using DocsManager.Models;
using DocsManager.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocsManager;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly DocsManagementContext _context;
    private const int CLIENT_PAGE_SIZE = 10;

    private Guid? GetUserGuid()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user == null) return null;

        return Guid.Parse(user);
    }
    public ClientController(DocsManagementContext context)
    {
        _context = context;
    }

    // GET: api/Client
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetClients([FromQuery(Name = "page")] int page)
    {
        page = page * CLIENT_PAGE_SIZE;
        var clients = _context.Clients.Skip(page).Take(CLIENT_PAGE_SIZE).ToListAsync();
        return await clients;
    }

    [HttpGet("select")]
    public async Task<ActionResult<IEnumerable<ClientDTO>>> GetClientsSelect()
    {
        var clients = _context.Clients.Select(client => new ClientDTO(client.ClientId, client.BuyerName));
        return await clients.ToListAsync();
    }

    // GET: api/Client/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetClient(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client == null) return NotFound();

        return client;
    }

    // PUT: api/Client/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutClient(int id, Client client)
    {
        if (id != client.ClientId) return BadRequest();

        _context.Entry(client).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/Client
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Client>> PostClient(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetClient", new { id = client.ClientId }, client);
    }

    // DELETE: api/Client/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var userId = GetUserGuid();
        if (userId == null) return NotFound();
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return NotFound();
        var hasNonUserInvoices = await _context.Invoices.AnyAsync(invoice => invoice.InvoiceClientId == id && invoice.InvoiceUserId != userId);
        if (hasNonUserInvoices) return UnprocessableEntity();
        
        
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClientExists(int id)
    {
        return _context.Clients.Any(e => e.ClientId == id);
    }
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetClientCount()
    {
        return await _context.Clients.CountAsync();
    }
}