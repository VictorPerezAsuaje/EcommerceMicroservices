using Microsoft.EntityFrameworkCore;
using Services.Mailing.Infrastructure;


namespace Services.Mailing.Tests.Utilities
{
    internal class DbInitializer
    {

        public static void SeedData(MailingDbContext context)
        {
            context.SaveChanges();
        }
    }

}
