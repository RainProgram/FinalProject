using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventRescue.Models
{
    public class ApplicationUser : IdentityUser
    {
        // البيانات الأساسية

        public string Name { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Region { get; set; } = null!;

        public bool IsAvailableNow { get; set; }


        // تخصص المزود

        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }


        // ================= العلاقات =================


        // الطلبات التي أنشأها العميل
        public virtual ICollection<EventRequest> EventRequests { get; set; }
            = new List<EventRequest>();


        // العروض التي قدمها المزود
        public virtual ICollection<ProviderOffer> SuppliedOffers { get; set; }
            = new List<ProviderOffer>();


        // الطلبات التي رسا عليهـا (AcceptedProvider)
        public virtual ICollection<EventRequest> AcceptedRequests { get; set; }
            = new List<EventRequest>();
    }
}