using System.ComponentModel.DataAnnotations;
using EventRescue.Models.Enums;

namespace EventRescue.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [StringLength(50, ErrorMessage = "الاسم يجب أن لا يتجاوز 50 حرفاً")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, ErrorMessage = "كلمة المرور يجب أن تكون على الأقل 4 رموز", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقتين")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "يرجى اختيار المدينة")]
        [Display(Name = "المدينة")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "يرجى اختيار نوع الحساب")]
        [Display(Name = "نوع الحساب")]
        public UserType AccountType { get; set; }

        [Display(Name = "التخصص")]
        public int? CategoryId { get; set; }
    }
}