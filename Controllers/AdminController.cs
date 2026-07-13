using EventRescue.Data;
using EventRescue.Models;
using EventRescue.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventRescue.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =====================================================
        // GET: /Admin/Dashboard
        // لوحة إحصائيات عامة: أعداد المستخدمين والطلبات الحالية
        // =====================================================
        public async Task<IActionResult> Dashboard()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalClients = await _context.Users.CountAsync(u => u.AccountType == UserType.Client),
                TotalProviders = await _context.Users.CountAsync(u => u.AccountType == UserType.Provider),
                BlockedUsers = await _context.Users.CountAsync(u => u.IsBlocked),

                TotalRequests = await _context.EventRequests.CountAsync(),
                PendingRequests = await _context.EventRequests.CountAsync(r => r.Status == EventStatus.Pending),
                AcceptedRequests = await _context.EventRequests.CountAsync(r => r.Status == EventStatus.Accepted),
                CompletedRequests = await _context.EventRequests.CountAsync(r => r.Status == EventStatus.Completed),
                CanceledRequests = await _context.EventRequests.CountAsync(r => r.Status == EventStatus.Canceled),

                TotalOffers = await _context.ProviderOffers.CountAsync(),

                RecentRequests = await _context.EventRequests
                    .Include(r => r.User)
                    .Include(r => r.Category)
                    .Include(r => r.Region)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(vm);
        }

        // =====================================================
        // GET: /Admin/ManageUsers?type=client|provider|blocked
        // جدول شامل للتحكم بالمستفيدين والمزودين + خاصية الحظر
        // =====================================================
        public async Task<IActionResult> ManageUsers(string? type)
        {
            // كل مستخدمي رول Admin حاليًا (نجيبها مرة واحدة بدل ما نستعلم لكل يوزر)
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminIds = adminUsers.Select(u => u.Id).ToHashSet();

            var query = _context.Users
                .Include(u => u.Region)
                .Include(u => u.Specialty)
                .AsQueryable();

            if (type == "client")
            {
                query = query.Where(u => u.AccountType == UserType.Client);
            }
            else if (type == "provider")
            {
                query = query.Where(u => u.AccountType == UserType.Provider);
            }
            else if (type == "blocked")
            {
                query = query.Where(u => u.IsBlocked);
            }
            else if (type == "admin")
            {
                query = query.Where(u => adminIds.Contains(u.Id));
            }

            var users = await query
                .OrderBy(u => u.AccountType)
                .ThenBy(u => u.FullName)
                .ToListAsync();

            ViewBag.SelectedType = type; // لتفعيل زر الفلتر النشط بالـ View
            ViewBag.AdminIds = adminIds; // عشان الـ View يعرف مين أدمن حاليًا
            ViewBag.CurrentUserId = _userManager.GetUserId(User); // عشان نمنع الأدمن من إزالة صلاحيته عن نفسه

            return View(users);
        }

        // =====================================================
        // POST: /Admin/ToggleBlock
        // حظر أو فك حظر مستخدم من صفحة إدارة المستخدمين
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlock(string userId, string? type)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                TempData["Error"] = "المستخدم غير موجود.";
                return RedirectToAction(nameof(ManageUsers), new { type });
            }

            user.IsBlocked = !user.IsBlocked;
            await _context.SaveChangesAsync();

            TempData["Success"] = user.IsBlocked
                ? $"تم حظر المستخدم {user.FullName}."
                : $"تم فك الحظر عن المستخدم {user.FullName}.";

            return RedirectToAction(nameof(ManageUsers), new { type });
        }

        // =====================================================
        // POST: /Admin/ToggleAdminRole
        // تعيين مستخدم كأدمن أو إزالة صلاحية الأدمن عنه
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdminRole(string userId, string? type)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                TempData["Error"] = "المستخدم غير موجود.";
                return RedirectToAction(nameof(ManageUsers), new { type });
            }

            var currentUserId = _userManager.GetUserId(User);

            var isCurrentlyAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // منع الأدمن من إزالة صلاحيته عن نفسه (تفادي قفل النظام على الجميع)
            if (isCurrentlyAdmin && userId == currentUserId)
            {
                TempData["Error"] = "لا يمكنك إزالة صلاحية الأدمن عن حسابك الحالي.";
                return RedirectToAction(nameof(ManageUsers), new { type });
            }

            if (isCurrentlyAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                TempData["Success"] = $"تم إزالة صلاحية الأدمن عن {user.FullName}.";
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["Success"] = $"تم تعيين {user.FullName} كأدمن.";
            }

            return RedirectToAction(nameof(ManageUsers), new { type });
        }

        // =====================================================
        // GET: /Admin/Requests?status=Pending
        // قائمة الطلبات، قابلة للفلترة حسب الحالة (مربوطة من كروت الداشبورد)
        // =====================================================
        public async Task<IActionResult> Requests(EventStatus? status)
        {
            var query = _context.EventRequests
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Region)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            var requests = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.SelectedStatus = status;

            return View(requests);
        }

        // =====================================================
        // GET: /Admin/Offers
        // قائمة كل العروض المقدمة من المزودين
        // =====================================================
        public async Task<IActionResult> Offers()
        {
            var offers = await _context.ProviderOffers
                .Include(o => o.EventRequest)
                .Include(o => o.Provider)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return View(offers);
        }
    }
}
