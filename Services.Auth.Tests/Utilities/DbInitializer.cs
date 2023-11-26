using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Auth.Domain;
using Services.Auth.Infrastructure;


namespace Services.Auth.Tests.Utilities;

internal class DbInitializer 
{
    private static List<AppUser> _Users = new List<AppUser>();
    public static List<AppUser> Users()
    {
        if (_Users.Count > 1)
            return _Users;

        var users = new List<AppUser>();
        var hasher = new PasswordHasher<AppUser>();

        var client = new AppUser()
        {
            UserName = "client@client",
            Email = "client@client",
            NormalizedEmail = "client@client".ToUpper(),
            FirstName = "Client FirstName",
            LastName = "Client LastName",
            PhoneNumber = "123456789"
        };

        var admin = new AppUser()
        {
            UserName = "admin@admin",
            Email = "admin@admin",
            NormalizedEmail = "admin@admin".ToUpper(),
            FirstName = "Admin FirstName",
            LastName = "Admin LastName",
            PhoneNumber = "987654321"
        };

        client.PasswordHash = hasher.HashPassword(client, "C1ientP@ssw0rd");
        admin.PasswordHash = hasher.HashPassword(admin, "Adm1nP@ssw0rd");

        users.Add(client);
        users.Add(admin);
        _Users = users;
        return _Users;
    }

    public static void SeedData(AuthDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Users.Any())        
            context.Users.AddRange(Users());

        context.SaveChanges();
    }
}

