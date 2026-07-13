using EventRescue.Models;

namespace EventRescue.Models
{
    // ViewModel لصفحة إحصائيات الإدارة العامة
    public class AdminDashboardViewModel
    {
        // ============ إحصائيات المستخدمين ============
        public int TotalUsers { get; set; }
        public int TotalClients { get; set; }
        public int TotalProviders { get; set; }
        public int BlockedUsers { get; set; }

        // ============ إحصائيات الطلبات ============
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int AcceptedRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int CanceledRequests { get; set; }

        // ============ إحصائيات العروض ============
        public int TotalOffers { get; set; }

        // ============ آخر الطلبات (لعرض سريع) ============
        public List<EventRequest> RecentRequests { get; set; } = new();
    }
}
