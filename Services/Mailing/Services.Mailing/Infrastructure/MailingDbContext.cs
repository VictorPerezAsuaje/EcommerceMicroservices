using Microsoft.EntityFrameworkCore;


namespace Services.Mailing.Infrastructure;

public class MailingDbContext : DbContext
{
    public MailingDbContext(DbContextOptions<MailingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
