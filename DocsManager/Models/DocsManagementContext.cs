using Microsoft.EntityFrameworkCore;

namespace DocsManager.Models;

public class DocsManagementContext(DbContextOptions<DocsManagementContext> options) : DbContext(options)
{
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Invoice>()
            .Property(b => b.IsPayed)
            .HasDefaultValue(false);
        builder.Entity<Client>()
            .HasIndex(c => c.BuyerCode)
            .IsUnique();
    }
}