using DocsManager.Models.docs;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Models;

public class DocsManagementContext(DbContextOptions<DocsManagementContext> options) : DbContext(options)
{
    public DbSet<Template> Templates { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Template>()
            .HasIndex(t => t.TemplateModel)
            .IsUnique();
    }
}