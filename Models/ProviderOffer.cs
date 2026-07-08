using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventRescue.Models.Enums;


namespace EventRescue.Models
{
    public class ProviderOffer
    {
        [Key]
        public int Id { get; set; }


        // ================= بيانات العرض =================

        [Required(ErrorMessage = "يرجى تحديد سعرالخدمة .")]
        [Range(1, 100000, ErrorMessage = "يرجى إدخال سعر منطقي بين 1 و 100,000 ريال.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime OfferDate { get; set; } = DateTime.Now;
        public bool IsAccepted { get; set; } = false;
        

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