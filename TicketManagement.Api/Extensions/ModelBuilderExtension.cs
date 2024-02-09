using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using TicketManagement.Api.Enums;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Extensions;

public static class ModelBuilderExtension
{
    private const int MAX_EVENTS_QUANTITY = 1000;
    private const int MAX_USERS_QUANTITY = 10;

    private static Faker Faker { get; set; }
    private static PasswordHasher<ApplicationUser> PasswordHasher { get; set; }
    private static List<Event> Events { get; set; }
    private static List<IdentityRole> Roles { get; set; }
    private static List<IdentityUserRole<string>> UserRoles { get; set; }
    private static List<ApplicationUser> Users { get; set; }
    private static List<Category> Categories { get; set; }
    private static List<Album> Albums { get; set; }

    public static async void Seed(this ModelBuilder modelBuilder)
    {
        Faker = new Faker();

        await SeedRoles(modelBuilder);
        await SeedUsers(modelBuilder);
        await SeedUserRoles(modelBuilder);
        await SeedCategories(modelBuilder);
        await SeedEvents(modelBuilder);
        await SeedAlbums(modelBuilder);
    }

    private static async Task SeedRoles(ModelBuilder builder)
    {
        Roles = new List<IdentityRole>
        {
            new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = UserRole.ADMIN.GetDisplayName(),
                ConcurrencyStamp = "1",
                NormalizedName = UserRole.ADMIN.GetDisplayName().Normalize()
            },
            new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = UserRole.CUSTOMER.GetDisplayName(),
                ConcurrencyStamp = "2",
                NormalizedName = UserRole.CUSTOMER.GetDisplayName().Normalize()
            },
            new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = UserRole.ORGANIZER.GetDisplayName(),
                ConcurrencyStamp = "3",
                NormalizedName = UserRole.ORGANIZER.GetDisplayName().Normalize()
            },
        };

        builder.Entity<IdentityRole>().HasData(Roles);
    }

    private static async Task SeedUsers(ModelBuilder builder)
    {
        PasswordHasher = new PasswordHasher<ApplicationUser>();
        Users = new List<ApplicationUser>();
        UserRoles = new List<IdentityUserRole<string>>();

        ApplicationUser admin = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Admin",
            Email = "admin@gmail.com",
            NormalizedEmail = "admin@gmail.com",
            UserName = "admin",
            NormalizedUserName = "admin",
            LockoutEnabled = false,
            PhoneNumber = "0829440357",
            DOB = DateTime.Now,
            Gender = UserGender.MALE,
            Avatar = "https://i.pravatar.cc/700",
            Status = UserStatus.ACTIVE,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
        admin.PasswordHash = PasswordHasher.HashPassword(admin, "Admin*123");
        var adminRole = new IdentityUserRole<string>()
        {
            UserId = admin.Id, RoleId = Roles[0].Id
        };
        UserRoles.Add(adminRole);

        var userFaker = new Faker<ApplicationUser>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid().ToString())
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.LockoutEnabled, f => false)
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("###-###-####"))
            .RuleFor(u => u.DOB, _ => DateTime.Now)
            .RuleFor(u => u.Gender, f => f.PickRandom<UserGender>())
            .RuleFor(u => u.Avatar, f => f.Person.Avatar)
            .RuleFor(u => u.Status, _ => UserStatus.ACTIVE)
            .RuleFor(u => u.CreatedAt, _ => DateTime.Now)
            .RuleFor(u => u.UpdatedAt, _ => DateTime.Now);

        for (int userIndex = 0; userIndex < MAX_USERS_QUANTITY; userIndex++)
        {
            var customer = userFaker.Generate();
            customer.PasswordHash = PasswordHasher.HashPassword(customer, "Customer*123");
            Users.Add(customer);
            var customerRole = new IdentityUserRole<string>()
            {
                UserId = customer.Id, RoleId = Roles[1].Id
            };
            UserRoles.Add(customerRole);

            var organizer = userFaker.Generate();
            organizer.PasswordHash = PasswordHasher.HashPassword(organizer, "Organizer*123");
            Users.Add(organizer);
            var organizerRole = new IdentityUserRole<string>()
            {
                UserId = organizer.Id, RoleId = Roles[2].Id
            };
            UserRoles.Add(organizerRole);
        }

        var usersWithAdmin = Users;
        usersWithAdmin.Add(admin);

        builder.Entity<ApplicationUser>().HasData(usersWithAdmin);
    }

    private static async Task SeedUserRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityUserRole<string>>().HasData(UserRoles);
    }

    private static async Task SeedCategories(ModelBuilder builder)
    {
        Categories = new List<Category>()
        {
            new Category()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Âm nhạc",
                Color = "#264653",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Category()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Thể thao",
                Color = "#2a9d8f",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Category()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Hội họa",
                Color = "#e9c46a",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Category()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Doanh nghệp",
                Color = "#f4a261",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Category()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Nhiếp ảnh",
                Color = "#e76f51",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        };

        builder.Entity<Category>().HasData(Categories);
    }

    private static async Task SeedEvents(ModelBuilder builder)
    {
        Events = new List<Event>();
        Albums = new List<Album>();

        var fakerEvent = new Faker<Event>()
            .RuleFor(e => e.Id, f => Guid.NewGuid().ToString())
            .RuleFor(e => e.CoverImage, f => f.Image.PicsumUrl())
            .RuleFor(e => e.Name, f => f.Commerce.ProductName())
            .RuleFor(e => e.Description, f => f.Commerce.ProductDescription())
            .RuleFor(e => e.CategoryId, f => f.PickRandom<Category>(Categories).Id)
            .RuleFor(e => e.Location, f => f.Address.FullAddress())
            .RuleFor(e => e.CreatorId, f => f.PickRandom<ApplicationUser>(Users).Id)
            .RuleFor(e => e.IsPromotion, f => f.Random.Bool())
            .RuleFor(e => e.PromotionPlan, f => f.Random.Number(0, 100))
            .RuleFor(e => e.Favourite, f => f.Random.Number(0, 1000))
            .RuleFor(e => e.Share, f => f.Random.Number(0, 1000))
            .RuleFor(e => e.CreatedAt, f => DateTime.Now)
            .RuleFor(e => e.UpdatedAt, f => DateTime.Now)
            .RuleFor(e => e.TicketPrice, f => decimal.Parse(f.Commerce.Price(100000, 10000000)))
            .RuleFor(e => e.TicketQuantity, f => f.Random.Number(0, 10000));

        for (int eventIndex = 0; eventIndex < MAX_EVENTS_QUANTITY; eventIndex++)
        {
            var eventStartTime = DateTime.Now.Subtract(TimeSpan.FromDays(Faker.Random.Number(0, 60)));
            var eventEndTime = DateTime.Now.Add(TimeSpan.FromDays(Faker.Random.Number(1, 60)));
            var eventItem = fakerEvent.Generate();
            eventItem.StartTime = eventStartTime;
            eventItem.EndTime = eventEndTime;
            eventItem.TicketStartTime = eventStartTime;
            eventItem.TicketCloseTime = eventEndTime;
            Events.Add(eventItem);

            var fakerAlbum = new Faker<Album>()
                .RuleFor(a => a.Id, f => Guid.NewGuid().ToString())
                .RuleFor(a => a.EventId, f => eventItem.Id)
                .RuleFor(a => a.Uri, f => f.Image.PicsumUrl());

            var albums = fakerAlbum.Generate(5);
            Albums.AddRange(albums);
        }

        builder.Entity<Event>().HasData(Events);
    }

    private static async Task SeedAlbums(ModelBuilder builder)
    {
        builder.Entity<Album>().HasData(Albums);
    }
}