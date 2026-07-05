using EventRescue.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventRescue.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ===================== DbSets =====================

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<EventRequest> EventRequests { get; set; } = null!;

        public DbSet<ProviderOffer> ProviderOffers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //==================================================
            // Category (1) -------- (*) EventRequest
            //==================================================

            builder.Entity<EventRequest>()
                .HasOne(e => e.Category)
                .WithMany(c => c.EventRequests)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            //==================================================
            // Category (1) -------- (*) Providers
            //==================================================

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Category)
                .WithMany(c => c.Providers)
                .HasForeignKey(u => u.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            //==================================================
            // Client (1) -------- (*) EventRequest
            //==================================================

            builder.Entity<EventRequest>()
                .HasOne(e => e.Client)
                .WithMany(u => u.EventRequests)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);


            //==================================================
            // Accepted Provider (1) -------- (*) EventRequest
            //==================================================

            builder.Entity<EventRequest>()
                .HasOne(e => e.AcceptedProvider)
                .WithMany(u => u.AcceptedRequests)
                .HasForeignKey(e => e.AcceptedProviderId)
                .OnDelete(DeleteBehavior.Restrict);


            //==================================================
            // EventRequest (1) -------- (*) ProviderOffer
            //==================================================

            builder.Entity<ProviderOffer>()
                .HasOne(o => o.EventRequest)
                .WithMany(e => e.ProviderOffers)
                .HasForeignKey(o => o.EventRequestId)
                .OnDelete(DeleteBehavior.Cascade);


            //==================================================
            // Provider (1) -------- (*) ProviderOffer
            //==================================================

            builder.Entity<ProviderOffer>()
                .HasOne(o => o.Provider)
                .WithMany(u => u.SuppliedOffers)
                .HasForeignKey(o => o.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);


            //==================================================
            // منع المزود من تقديم أكثر من عرض لنفس الطلب
            //==================================================

            builder.Entity<ProviderOffer>()
                .HasIndex(o => new
                {
                    o.EventRequestId,
                    o.ProviderId
                })
                .IsUnique();


            //==================================================
            // البيانات الأولية للأقسام (Data Seeding)
            //==================================================

            builder.Entity<Category>().HasData(

                // أولاً: قسم الخدمات والضيافة (Services)

                new Category
                {
                    Id = 1,
                    Name = "قهوجي أو قهوجية",
                    Type = "Services",
                    Icon = "bi bi-cup-hot-fill"
                },

                new Category
                {
                    Id = 2,
                    Name = "مقدمات ضيافة",
                    Type = "Services",
                    Icon = "bi bi-person-heart-fill"
                },

                new Category
                {
                    Id = 3,
                    Name = "عاملة تنظيف",
                    Type = "Services",
                    Icon = "bi bi-stars"
                },

                new Category
                {
                    Id = 4,
                    Name = "فني إصلاح أثاث",
                    Type = "Services",
                    Icon = "bi bi-tools"
                },

                new Category
                {
                    Id = 5,
                    Name = "منسق ورد",
                    Type = "Services",
                    Icon = "bi bi-flower1"
                },

                new Category
                {
                    Id = 6,
                    Name = "مصور مناسبات",
                    Type = "Services",
                    Icon = "bi bi-camera-fill"
                },

                new Category
                {
                    Id = 7,
                    Name = "شيف منزلي",
                    Type = "Services",
                    Icon = "bi bi-egg-fried"
                },

                new Category
                {
                    Id = 8,
                    Name = "سائق توصيل",
                    Type = "Services",
                    Icon = "bi bi-truck"
                },

                // ثانياً: قسم التأجير والمستلزمات (Rentals)

                new Category
                {
                    Id = 9,
                    Name = "كراسي وطاولات",
                    Type = "Rentals",
                    Icon = "bi bi-grid-3x3-gap-fill"
                },

                new Category
                {
                    Id = 10,
                    Name = "كنب",
                    Type = "Rentals",
                    Icon = "bi bi-house-heart-fill"
                },

                new Category
                {
                    Id = 11,
                    Name = "خيام",
                    Type = "Rentals",
                    Icon = "bi bi-house"
                },

                new Category
                {
                    Id = 12,
                    Name = "سخانات ومبردات",
                    Type = "Rentals",
                    Icon = "bi bi-thermometer-half"
                }
            );
        }
    }
}