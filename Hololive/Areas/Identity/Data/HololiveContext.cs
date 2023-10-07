using Hololive.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hololive.Data;

public class HololiveContext : IdentityDbContext<HololiveUser>
{
    public HololiveContext(DbContextOptions<HololiveContext> options)
        : base(options)
    {
    }

    public DbSet<Hololive.Models.Voucher> Voucher { get; set; }

    public DbSet<Hololive.Models.Customer> Customer { get; set; }

    public DbSet<Hololive.Models.Transaction> Transaction { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
