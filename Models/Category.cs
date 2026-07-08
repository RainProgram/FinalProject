using System.ComponentModel.DataAnnotations;

namespace EventRescue.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم القسم مطلوب.")]
        public string Name { get; set; }  = null!;

        [Required(ErrorMessage = "نوع القسم رئيسي مطلوب.")]
        [StringLength(30)]
        public string Type { get; set; }  = null!; // Services / Rentals

        public string Icon { get; set; } = null!;


        // العلاقات

        // القسم يحتوي على العديد من الطلبات
        public virtual ICollection<EventRequest> EventRequests { get; set; }
            = new List<EventRequest>();

        // القسم يحتوي على العديد من مزودي الخدمة
        public virtual ICollection<ApplicationUser> Providers { get; set; }
            = new List<ApplicationUser>();
    }
}