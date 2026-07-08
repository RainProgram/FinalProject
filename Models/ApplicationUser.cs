using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EventRescue.Models.Enums;


namespace EventRescue.Models
{
    public class ApplicationUser : IdentityUser
    {
        // البيانات الأساسية
        // الاسم الكامل للمستخدم
        [Required(ErrorMessage = "الاسم الكامل مطلوب.")]
        [StringLength(50, ErrorMessage = "الاسم يجب أن لا يتجاوز 50 حرفاً.")]
        public string FullName { get; set; } = null!; 

        // المنطقة/المدينة (جدة، الرياض ...) - علاقة مع جدول Regions
        [Required(ErrorMessage = "يرجى تحديد المدينة.")]
        public int RegionId { get; set; }
        [ForeignKey(nameof(RegionId))]
        public virtual Region? Region { get; set; }

        // نوع الحساب: مستفيد أو مزود خدمة (يحدد عند التسجيل)
        [Required(ErrorMessage = "يرجى اختيار نوع الحساب.")]
        public UserType AccountType { get; set; }

       // تخصص مزود الخدمة: حقل مباشر يشير لقسم واحد فقط .
        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Specialty { get; set; }

        // متاح حالياً
        public bool IsAvailableNow { get; set; } = true;

        //حالة الحظر 
        public bool IsBlocked { get; set; } = false;


        // ================= العلاقات =================


        // الطلبات التي أنشأها العميل
        public virtual ICollection<EventRequest> CreatedRequests { get; set; }
        = new List<EventRequest>();


        // العروض التي قدمها المزود
        public virtual ICollection<ProviderOffer> SuppliedOffers { get; set; }
            = new List<ProviderOffer>();


        // الطلبات التي رسا عليهـا (AcceptedProvider)
        public virtual ICollection<EventRequest> AcceptedRequests { get; set; }
            = new List<EventRequest>();
    }
}