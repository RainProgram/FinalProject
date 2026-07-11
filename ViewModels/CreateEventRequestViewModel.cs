using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EventRescue.ViewModels
{
    public class CreateEventRequestViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار نوع الخدمة")]
        [Display(Name = "نوع الخدمة")]
         public int CategoryId { get; set; }

        [Required(ErrorMessage = "عنوان الطلب مطلوب")]
        [StringLength(50, ErrorMessage = "العنوان يجب أن لا يتجاوز 50 حرفاً")]
        [Display(Name = "عنوان الطلب")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "وصف المشكلة مطلوب")]
        [StringLength(300, ErrorMessage = "الوصف يجب أن لا يتجاوز 300 حرف")]
        [Display(Name = "وصف المشكلة")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "تاريخ المناسبة مطلوب")]
        [DataType(DataType.DateTime)]   // تاريخ + وقت لتصنيف الفزعة العاجلة بدقة
        [Display(Name = "تاريخ ووقت المناسبة")]
        public DateTime EventDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(150, ErrorMessage = "العنوان يجب أن لا يتجاوز 150 حرفاً")]
        [Display(Name = "العنوان / الحي")]
        public string Address { get; set; } = null!;

        [Display(Name = "صورة المشكلة")]
        public IFormFile? ImageFile { get; set; }
    }
}