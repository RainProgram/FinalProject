using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventRescue.Models
{
    public class ProviderOffer
    {
        [Key]
        public int Id { get; set; }


        // ================= بيانات العرض =================

        [Required]

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime OfferDate { get; set; } = DateTime.Now;


        // ================= الطلب =================

        [Required]
        public int EventRequestId { get; set; } 

        [ForeignKey(nameof(EventRequestId))]
        public virtual EventRequest? EventRequest { get; set; }


        // ================= المزود =================

        [Required]
        public string ProviderId { get; set; } = null!;

        [ForeignKey(nameof(ProviderId))]
        public virtual ApplicationUser? Provider { get; set; }
    }
}