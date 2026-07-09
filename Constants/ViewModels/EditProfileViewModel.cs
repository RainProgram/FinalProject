using System.ComponentModel.DataAnnotations;

namespace EventRescue.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [StringLength(50, ErrorMessage = "الاسم يجب أن لا يتجاوز 50 حرفاً")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; } = null!;

        [Phone(ErrorMessage = "رقم الجوال غير صحيح")]
        [Display(Name = "رقم الجوال")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "يرجى اختيار المدينة")]
        [Display(Name = "المدينة")]
        public int RegionId { get; set; }

        [Display(Name = "التخصص")]
        public int? CategoryId { get; set; }

        [Display(Name = "متاح الآن")]
        public bool IsAvailableNow { get; set; }
    }
}