using DocsManager.Models.docs;
using Microsoft.EntityFrameworkCore;

namespace DocsManager.Models;

public class DocsManagementContext(DbContextOptions<DocsManagementContext> options) : DbContext(options)
{
    public DbSet<Template> Templates { get; set; }
}