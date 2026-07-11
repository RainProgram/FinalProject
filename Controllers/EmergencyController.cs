using EventRescue.Data;
using EventRescue.Models;
using EventRescue.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventRescue.Controllers
{
    [Authorize]
    public class EmergencyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmergencyController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //==================================================
        // Index : شاشة الرادار الحي للطوارئ
        // كل طلبات الفزعة (خلال أقل من 24 ساعة) في منطقة الفني
        // بجميع التخصصات — خدمات وتأجير — للقبول السريع
        //==================================================
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.AccountType != UserType.Provider)
            {
                TempData["ErrorMessage"] = "هذه الصفحة مخصصة لمزودي الخدمة";
                return RedirectToAction("Index", "Home");
            }

            // نافذة الطوارئ: من هذه اللحظة حتى 24 ساعة قادمة
            var now = DateTime.Now;
            var next24Hours = now.AddHours(24);

            // الفلتر: منطقة الفني فقط + متاح + موعده خلال 24 ساعة
            // (بدون فلترة تخصص — الطوارئ تعرض كل الأقسام حسب ملف المدرب)
            var emergencyRequests = await _context.EventRequests
                .Include(r => r.Category)
                .Include(r => r.Region)
                .Include(r => r.ProviderOffers)
                .Where(r => r.RegionId == currentUser.RegionId
                         && r.Status == EventStatus.Pending
                         && r.EventDate >= now
                         && r.EventDate <= next24Hours)
                .OrderBy(r => r.EventDate)     // الأعجل أولاً
                .ToListAsync();

            ViewBag.CurrentProviderId = currentUser.Id;

            // حالة توفر الفني الحالية — نعرضها له بالصفحة
            ViewBag.IsAvailableNow = currentUser.IsAvailableNow;

            return View(emergencyRequests);
        }

        //==================================================
        // ToggleAvailability : زر فوري (بدون فيو)
        // الفني يغير حالته: متاح الآن / غير متاح لاستقبال الفزعات
        //==================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.AccountType != UserType.Provider)
            {
                return RedirectToAction("Index", "Home");
            }

            // عكس الحالة الحالية وحفظها
            currentUser.IsAvailableNow = !currentUser.IsAvailableNow;
            await _userManager.UpdateAsync(currentUser);

            TempData["SuccessMessage"] = currentUser.IsAvailableNow
                ? "أنت الآن متاح لاستقبال طلبات الفزعة العاجلة"
                : "تم إيقاف استقبال طلبات الفزعة";

            return RedirectToAction(nameof(Index));
        }
    }
}
