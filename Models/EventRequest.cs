using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EventRescue.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace EventRescue.Models
{
    public class EventRequest
    {
        [Key]
        public int Id { get; set; }

        // ================= العميل =================

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }


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

        [Required(ErrorMessage = "عنوان المناسبة أو الطلب مطلوب.")]
        [StringLength(50, ErrorMessage = "العنوان طويل جداً.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "يرجى كتابة تفاصيل ووصف الطلب.")]
        [StringLength(500, ErrorMessage = "الوصف لا يمكن أن يتجاوز 500 حرف.")]
        public string Description { get; set; } = null!;


        [Required(ErrorMessage = "المدينة مطلوبة لتحديد مكان المناسبة.")]
        public int RegionId { get; set; }
        [ForeignKey(nameof(RegionId))]
         public virtual Region? Region { get; set; }


        [Required(ErrorMessage = "يرجى كتابة اسم الحي.")]
        public string Address { get; set; } = null!; //الحي

        
        [Required(ErrorMessage = "يرجى تحديد تاريخ المناسبة.")]
        public DateTime EventDate { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Pending; // حالة الطلب : مكتمل, متاح 
       
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? ImagePath { get; set; } 
        
        [NotMapped]
        public IFormFile? ImageFile {get ; set ;} 


        // ================= العلاقات =================

        public virtual ICollection<ProviderOffer> ProviderOffers { get; set; }
            = new List<ProviderOffer>();
    }
}