using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventRescue.Models
{
    public class EventRequest
    {
        [Key]
        public int Id { get; set; }

        // ================= العميل =================

        [Required]
        public string ClientId { get; set; } = null!;

        [ForeignKey(nameof(ClientId))]
        public virtual ApplicationUser? Client { get; set; }


        // ================= القسم =================

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }


        // ================= المزود المقبول =================

        // يبقى فارغاً حتى يقبل العميل أحد العروض
        public string? AcceptedProviderId { get; set; }

        [ForeignKey(nameof(AcceptedProviderId))]
        public virtual ApplicationUser? AcceptedProvider { get; set; }


        // ================= بيانات الطلب =================

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string ImagePath { get; set; } = null!;

        [Required]
        public string Region { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        public DateTime EventDate { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        // ================= العلاقات =================

        public virtual ICollection<ProviderOffer> ProviderOffers { get; set; }
            = new List<ProviderOffer>();
    }
}