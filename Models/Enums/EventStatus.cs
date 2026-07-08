using System.ComponentModel.DataAnnotations;

namespace EventRescue.Models.Enums
{
    public enum EventStatus
    {
        [Display(Name = "متاح")]
        Pending = 0,
        
        [Display(Name = "مقبول")]
        Accepted = 1,
        
        [Display(Name = "مكتمل")]
        Completed = 2,
        
        [Display(Name = "ملغي")]
        Canceled = 3
    }
}