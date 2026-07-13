using EventRescue.Models;
using EventRescue.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventRescue.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<AppDbContext>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // ==========================================
            // Regions
            // ==========================================

            if (!context.Regions.Any())
            {
                context.Regions.AddRange(

                    new Region
                    {
                        Name = "الرياض"
                    },

                    new Region
                    {
                        Name = "جدة"
                    },

                    new Region
                    {
                        Name = "الدمام"
                    },

                    new Region
                    {
                        Name = "مكة"
                    },

                    new Region
                    {
                        Name = "المدينة المنورة"
                    }

                );

                await context.SaveChangesAsync();
            }

            // ==========================================
            // Categories
            // ==========================================

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(

                    new Category
                    {
                        Name = "قهوجي أو قهوجية",
                        Type = "Services",
                        Icon = "bi bi-cup-hot-fill"
                    },

                    new Category
                    {
                        Name = "مقدمات ضيافة",
                        Type = "Services",
                        Icon = "bi bi-person-heart"
                    },

                    new Category
                    {
                        Name = "عاملة تنظيف",
                        Type = "Services",
                        Icon = "bi bi-stars"
                    },

                    new Category
                    {
                        Name = "فني إصلاح أثاث",
                        Type = "Services",
                        Icon = "bi bi-tools"
                    },

                    new Category
                    {
                        Name = "منسق ورد",
                        Type = "Services",
                        Icon = "bi bi-flower1"
                    },

                    new Category
                    {
                        Name = "مصور مناسبات",
                        Type = "Services",
                        Icon = "bi bi-camera-fill"
                    },

                    new Category
                    {
                        Name = "شيف منزلي",
                        Type = "Services",
                        Icon = "bi bi-egg-fried"
                    },

                    new Category
                    {
                        Name = "سائق توصيل",
                        Type = "Services",
                        Icon = "bi bi-truck"
                    },

                    new Category
                    {
                        Name = "كراسي وطاولات",
                        Type = "Rentals",
                        Icon = "bi bi-intersect"
                    },

                    new Category
                    {
                        Name = "كنب",
                        Type = "Rentals",
                        Icon = "bi bi-diamond-half"
                    },

                    new Category
                    {
                        Name = "خيام",
                        Type = "Rentals",
                        Icon = "bi bi-house-heart-fill"
                    },

                    new Category
                    {
                        Name = "سخانات ومبردات",
                        Type = "Rentals",
                        Icon = "bi bi-cookie"
                    }

                );

                await context.SaveChangesAsync();
            }

            // ==========================================
            // Admin
            // ==========================================

                // تأكد أن دور Admin موجود
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                var admin = await userManager.FindByEmailAsync("admin@test.com");

                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = "admin@test.com",
                        Email = "admin@test.com",
                        FullName = "مدير النظام",

                        AccountType = UserType.Provider,

                        RegionId = context.Regions
                            .First(r => r.Name == "الرياض").Id,

                        IsAvailableNow = false,
                        IsBlocked = false
                    };

                    var result = await userManager.CreateAsync(admin, "Admin@123");

                    if (!result.Succeeded)
                    {
                        Console.WriteLine(
                            string.Join(", ", result.Errors.Select(e => e.Description))
                        );
                    }
                }

                // هنا يتم ربط المستخدم الموجود أو الجديد بالدور
                if (!await userManager.IsInRoleAsync(admin, "Admin"))
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

            // ==========================================
            // Clients
            // ==========================================

            var clients = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "client1@test.com",
                    Email = "client1@test.com",
                    FullName = "محمد خالد",
                    AccountType = UserType.Client,
                    RegionId = context.Regions.First(r => r.Name == "الرياض").Id,
                    IsAvailableNow = false,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "client2@test.com",
                    Email = "client2@test.com",
                    FullName = "فاطمة ابراهيم",
                    AccountType = UserType.Client,
                    RegionId = context.Regions.First(r => r.Name == "جدة").Id,
                    IsAvailableNow = false,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "client3@test.com",
                    Email = "client3@test.com",
                    FullName = "عبدالله محمد",
                    AccountType = UserType.Client,
                    RegionId = context.Regions.First(r => r.Name == "الدمام").Id,
                    IsAvailableNow = false,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "client4@test.com",
                    Email = "client4@test.com",
                    FullName = "نورة علي",
                    AccountType = UserType.Client,
                    RegionId = context.Regions.First(r => r.Name == "مكة").Id,
                    IsAvailableNow = false,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "client5@test.com",
                    Email = "client5@test.com",
                    FullName = "سلمان فهد",
                    AccountType = UserType.Client,
                    RegionId = context.Regions.First(r => r.Name == "المدينة المنورة").Id,
                    IsAvailableNow = false,
                    IsBlocked = false
                }
            };

            foreach (var client in clients)
            {
                if (await userManager.FindByEmailAsync(client.Email!) == null)
                {
                    await userManager.CreateAsync(client, "Client@123");
                }
            }

            // ==========================================
            // Providers
            // ==========================================

            var providers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "qahwa@test.com",
                    Email = "qahwa@test.com",
                    FullName = "محمد سعيد",
                    AccountType = UserType.Provider,
                    CategoryId = 1,
                    RegionId = context.Regions.First(r => r.Name == "الرياض").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "hospitality@test.com",
                    Email = "hospitality@test.com",
                    FullName = "أحمد علي",
                    AccountType = UserType.Provider,
                    CategoryId = 2,
                    RegionId = context.Regions.First(r => r.Name == "جدة").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "clean@test.com",
                    Email = "clean@test.com",
                    FullName = "سارة محمد",
                    AccountType = UserType.Provider,
                    CategoryId = 3,
                    RegionId = context.Regions.First(r => r.Name == "الدمام").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "furniture@test.com",
                    Email = "furniture@test.com",
                    FullName = "خالد محمد",
                    AccountType = UserType.Provider,
                    CategoryId = 4,
                    RegionId = context.Regions.First(r => r.Name == "مكة").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "flowers@test.com",
                    Email = "flowers@test.com",
                    FullName = "نورة محمد",
                    AccountType = UserType.Provider,
                    CategoryId = 5,
                    RegionId = context.Regions.First(r => r.Name == "المدينة المنورة").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "photo@test.com",
                    Email = "photo@test.com",
                    FullName = "عبدالله محمد",
                    AccountType = UserType.Provider,
                    CategoryId = 6,
                    RegionId = context.Regions.First(r => r.Name == "الرياض").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                                new ApplicationUser
                {
                    UserName = "chef@test.com",
                    Email = "chef@test.com",
                    FullName = "يوسف أحمد",
                    AccountType = UserType.Provider,
                    CategoryId = 7,
                    RegionId = context.Regions.First(r => r.Name == "جدة").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "driver@test.com",
                    Email = "driver@test.com",
                    FullName = "سلمان محمد",
                    AccountType = UserType.Provider,
                    CategoryId = 8,
                    RegionId = context.Regions.First(r => r.Name == "الدمام").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "chairs@test.com",
                    Email = "chairs@test.com",
                    FullName = "فهد أحمد",
                    AccountType = UserType.Provider,
                    CategoryId = 9,
                    RegionId = context.Regions.First(r => r.Name == "مكة").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "sofa@test.com",
                    Email = "sofa@test.com",
                    FullName = "تركي محمد",
                    AccountType = UserType.Provider,
                    CategoryId = 10,
                    RegionId = context.Regions.First(r => r.Name == "المدينة المنورة").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "tent@test.com",
                    Email = "tent@test.com",
                    FullName = "ماجد أحمد",
                    AccountType = UserType.Provider,
                    CategoryId = 11,
                    RegionId = context.Regions.First(r => r.Name == "الرياض").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                },

                new ApplicationUser
                {
                    UserName = "cooler@test.com",
                    Email = "cooler@test.com",
                    FullName = "راشد سلطان",
                    AccountType = UserType.Provider,
                    CategoryId = 12,
                    RegionId = context.Regions.First(r => r.Name == "جدة").Id,
                    IsAvailableNow = true,
                    IsBlocked = false
                }
            };

            foreach (var provider in providers)
            {
                if (await userManager.FindByEmailAsync(provider.Email!) == null)
                {
                    await userManager.CreateAsync(provider, "Provider@123");
                }
            }

            await context.SaveChangesAsync();

            // ==========================================
            // تحميل المستخدمين
            // ==========================================

            var client1 = await userManager.FindByEmailAsync("client1@test.com");
            var client2 = await userManager.FindByEmailAsync("client2@test.com");
            var client3 = await userManager.FindByEmailAsync("client3@test.com");
            var client4 = await userManager.FindByEmailAsync("client4@test.com");
            var client5 = await userManager.FindByEmailAsync("client5@test.com");

            // ==========================================
            // Event Requests
            // ==========================================

            if (!context.EventRequests.Any())
            {
                var requests = new List<EventRequest>
                {
                    new EventRequest
                    {
                        UserId = client1!.Id,
                        CategoryId = 1,
                        Title = "قهوجي لحفل زفاف",
                        Description = "أحتاج قهوجي لتقديم القهوة العربية في حفل زفاف.",
                        RegionId = context.Regions.First(r => r.Name == "الرياض").Id,
                        Address = "حي الياسمين",
                        EventDate = DateTime.Now.AddDays(3),
                        ImagePath = "images/qahwa.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client2!.Id,
                        CategoryId = 2,
                        Title = "طاقم ضيافة",
                        Description = "مطلوب طاقم ضيافة لاستقبال الضيوف.",
                        RegionId = context.Regions.First(r => r.Name == "جدة").Id,
                        Address = "حي الزهراء",
                        EventDate = DateTime.Now.AddDays(5),
                        ImagePath = "images/hospitality.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client3!.Id,
                        CategoryId = 3,
                        Title = "تنظيف مجلس",
                        Description = "أحتاج تنظيف المجلس قبل المناسبة.",
                        RegionId = context.Regions.First(r => r.Name == "الدمام").Id,
                        Address = "حي الفيصلية",
                        EventDate = DateTime.Now.AddDays(2),
                        ImagePath = "images/cleaning.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client4!.Id,
                        CategoryId = 4,
                        Title = "إصلاح كنب",
                        Description = "يوجد كنب يحتاج إلى إصلاح.",
                        RegionId = context.Regions.First(r => r.Name == "مكة").Id,
                        Address = "حي العزيزية",
                        EventDate = DateTime.Now.AddDays(4),
                        ImagePath = "images/furniture.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client5!.Id,
                        CategoryId = 5,
                        Title = "تنسيق ورد",
                        Description = "أرغب بتنسيق ورد طبيعي.",
                        RegionId = context.Regions.First(r => r.Name == "المدينة المنورة").Id,
                        Address = "حي قباء",
                        EventDate = DateTime.Now.AddDays(6),
                        ImagePath = "images/flowers.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client1!.Id,
                        CategoryId = 6,
                        Title = "مصور احترافي",
                        Description = "مطلوب مصور احترافي لتغطية المناسبة.",
                        RegionId = context.Regions.First(r => r.Name == "الرياض").Id,
                        Address = "حي قرطبة",
                        EventDate = DateTime.Now.AddDays(7),
                        ImagePath = "images/photo.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client2!.Id,
                        CategoryId = 7,
                        Title = "شيف منزلي",
                        Description = "أحتاج شيف لإعداد البوفيه.",
                        RegionId = context.Regions.First(r => r.Name == "جدة").Id,
                        Address = "حي الروضة",
                        EventDate = DateTime.Now.AddDays(3),
                        ImagePath = "images/chef.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client3!.Id,
                        CategoryId = 8,
                        Title = "سائق",
                        Description = "مطلوب سائق لنقل الضيوف.",
                        RegionId = context.Regions.First(r => r.Name == "الدمام").Id,
                        Address = "حي الشاطئ",
                        EventDate = DateTime.Now.AddDays(1),
                        ImagePath = "images/driver.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client4!.Id,
                        CategoryId = 9,
                        Title = "تأجير كراسي وطاولات",
                        Description = "أحتاج كراسي وطاولات للمناسبة.",
                        RegionId = context.Regions.First(r => r.Name == "مكة").Id,
                        Address = "حي الشرائع",
                        EventDate = DateTime.Now.AddDays(5),
                        ImagePath = "images/chairs.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client5!.Id,
                        CategoryId = 10,
                        Title = "تأجير كنب",
                        Description = "مطلوب أطقم كنب للمناسبة.",
                        RegionId = context.Regions.First(r => r.Name == "المدينة المنورة").Id,
                        Address = "حي العريض",
                        EventDate = DateTime.Now.AddDays(4),
                        ImagePath = "images/sofa.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client1!.Id,
                        CategoryId = 11,
                        Title = "تأجير خيمة",
                        Description = "أحتاج خيمة كبيرة للمناسبة.",
                        RegionId = context.Regions.First(r => r.Name == "الرياض").Id,
                        Address = "حي النرجس",
                        EventDate = DateTime.Now.AddDays(8),
                        ImagePath = "images/tent.jpg"
                    },

                    new EventRequest
                    {
                        UserId = client2!.Id,
                        CategoryId = 12,
                        Title = "مبردات وسخانات",
                        Description = "مطلوب مبردات وسخانات للمشروبات.",
                        RegionId = context.Regions.First(r => r.Name == "جدة").Id,
                        Address = "حي الحمدانية",
                        EventDate = DateTime.Now.AddDays(2),
                        ImagePath = "images/cooler.jpg"
                    }
                };

                context.EventRequests.AddRange(requests);

                await context.SaveChangesAsync();
            }

            // ==========================================
            // Provider Offers
            // ==========================================

            if (!context.ProviderOffers.Any())
            {
                var requests = context.EventRequests.ToList();

                var providersInDb = context.Users
                    .Where(u =>
                        u.AccountType == UserType.Provider &&
                        !u.IsBlocked)
                    .ToList();

                var offers = new List<ProviderOffer>();

                foreach (var request in requests)
                {
                    // اختيار المزود المطابق لنفس المدينة ونفس التخصص
                    var provider = providersInDb.FirstOrDefault(p =>
                        p.RegionId == request.RegionId &&
                        p.CategoryId == request.CategoryId);

                    if (provider == null)
                        continue;

                    offers.Add(new ProviderOffer
                    {
                        EventRequestId = request.Id,
                        ProviderId = provider.Id,

                        Price = request.CategoryId switch
                        {
                            1 => 300,
                            2 => 350,
                            3 => 200,
                            4 => 450,
                            5 => 250,
                            6 => 600,
                            7 => 700,
                            8 => 150,
                            9 => 500,
                            10 => 800,
                            11 => 1000,
                            12 => 400,
                            _ => 300
                        },

                        OfferDate = DateTime.Now,
                        IsAccepted = false
                    });
                }

                context.ProviderOffers.AddRange(offers);

                await context.SaveChangesAsync();
            }
        }
    }
}