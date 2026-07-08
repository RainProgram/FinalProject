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

        // ================= الجداول =================

        public DbSet<Category> Categories { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<EventRequest> EventRequests { get; set; }

        public DbSet<ProviderOffer> ProviderOffers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ==========================================
            // ApplicationUser -> Region
            // ==========================================

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Region)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RegionId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // ApplicationUser -> Category (Specialty)
            // ==========================================

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Specialty)
                .WithMany(c => c.Providers)
                .HasForeignKey(u => u.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);


            // ==========================================
            // EventRequest -> User (منشئ الطلب)
            // ==========================================

            builder.Entity<EventRequest>()
                .HasOne(r => r.User)
                .WithMany(u => u.CreatedRequests)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // EventRequest -> AcceptedProvider
            // ==========================================

            builder.Entity<EventRequest>()
                .HasOne(r => r.AcceptedProvider)
                .WithMany(u => u.AcceptedRequests)
                .HasForeignKey(r => r.AcceptedProviderId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // EventRequest -> Category
            // ==========================================

            builder.Entity<EventRequest>()
                .HasOne(r => r.Category)
                .WithMany(c => c.EventRequests)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // EventRequest -> Region
            // ==========================================

            builder.Entity<EventRequest>()
                .HasOne(r => r.Region)
                .WithMany(rg => rg.EventRequests)
                .HasForeignKey(r => r.RegionId)
                .OnDelete(DeleteBehavior.Restrict);


            // ==========================================
            // ProviderOffer -> EventRequest
            // ==========================================

            builder.Entity<ProviderOffer>()
                .HasOne(o => o.EventRequest)
                .WithMany(r => r.ProviderOffers)
                .HasForeignKey(o => o.EventRequestId)
                .OnDelete(DeleteBehavior.Cascade);


            // ==========================================
            // ProviderOffer -> Provider
            // ==========================================

            builder.Entity<ProviderOffer>()
                .HasOne(o => o.Provider)
                .WithMany(u => u.SuppliedOffers)
                .HasForeignKey(o => o.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================================================
            // منع المزود من تقديم أكثر من عرض واحد لنفس الطلب في قاعدة البيانات
            // =======================================================
            builder.Entity<ProviderOffer>()
                .HasIndex(o => new { o.ProviderId, o.EventRequestId })
                .IsUnique(); 
        }
    }
}