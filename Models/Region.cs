using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventRescue.Models
{
 public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنطقة مطلوب.")]
        [StringLength(50, ErrorMessage = "اسم المنطقة لا يجب أن يتجاوز 50 حرفاً.")]
        public string Name { get; set; } = null!;
        
        // المستخدمون المنتمون لهذه المنطقة
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
     
        // الطلبات الموجودة في هذه المنطقة
        public virtual ICollection<EventRequest> EventRequests { get; set; } = new List<EventRequest>();
    
    }
}