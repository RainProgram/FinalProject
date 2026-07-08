using System.ComponentModel.DataAnnotations;

namespace EventRescue.Models.Enums 
{
    public enum UserType
    {
        [Display(Name = "مستفيد")]
        Client = 0, // القيمة المخزنة بقاعدة البيانات 0
        
        [Display(Name = "مزود خدمة")]
        Provider = 1 // القيمة المخزنة بقاعدة البيانات 1
    }
}