using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocsManager.Models;
using DocsManager.Models.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DocsManager
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly DocsManagementContext _context;

        public InvoiceController(DocsManagementContext context)
        {
            _context = context;
        }

        private Guid? GetUserGuid()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return null;
            }

            return Guid.Parse(user);
        }

        // GET: api/Invoice
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceListDto>>> GetInvoices()
        {
            
            var user = GetUserGuid();

            if (user == null)
            {
                return NotFound();
            }

            var invoices = _context.Invoices
                .Where(invoice => invoice.InvoiceUserId == user)
                .Select(x =>
                    new InvoiceListDto(x.InvoiceId, x.InvoiceDate, x.InvoiceClient.BuyerName ));

            return await invoices.ToListAsync();
        }

        // GET: api/Invoice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        // PUT: api/Invoice/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                return BadRequest();
            }

            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Invoice
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(InvoicePostDto invoicePost)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user == null)
            {
                return NotFound();
            }

            var userGuid = Guid.Parse(user);
            
            var userModel = await _context.Users.FindAsync(userGuid);
            var clientModel = await _context.Clients.FindAsync(invoicePost.ClientId);

            if (userModel == null || clientModel == null)
            {
                return NotFound();
            }
            var invoice = new Invoice
            {
                InvoiceUser = userModel,
                InvoiceDate = DateTime.Parse(invoicePost.InvoiceDate).ToUniversalTime(),
                InvoiceClient = clientModel
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvoice", new { id = invoice.InvoiceId }, invoice);
        }

        // DELETE: api/Invoice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }

        
        [HttpGet("last")]
        public async Task<ActionResult<int>> GetLatestInvoice()
        {
            var user = GetUserGuid();
            if (user == null)
            {
                return NotFound();
            }

            var last = await _context.Invoices
                .Where(invoice => invoice.InvoiceUserId == user)
                .Select(invoice => invoice.SeriesNumber)
                .FirstOrDefaultAsync();
            return last;
        }
    }
}
