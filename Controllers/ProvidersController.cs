using EventRescue.Data;
using EventRescue.Models;
using EventRescue.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventRescue.Controllers
{
    // [Authorize] = ممنوع تفتح أي صفحة هنا بدون تسجيل دخول
    [Authorize]
    public class ProvidersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // الحقن: النظام يعطينا بوابة قاعدة البيانات ومدير المستخدمين جاهزين
        public ProvidersController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //==================================================
        // AvailableRequests : رادار الطلبات المتاحة
        // يعرض للمزود الطلبات في نفس مدينته + نفس تخصصه فقط
        //==================================================
        public async Task<IActionResult> AvailableRequests()
        {
            // 1) نجيب المستخدم المسجل دخوله حالياً
            var currentUser = await _userManager.GetUserAsync(User);

            // 2) حماية: لازم يكون مزود خدمة وله تخصص محدد
            if (currentUser == null
                || currentUser.AccountType != UserType.Provider
                || currentUser.CategoryId == null)
            {
                TempData["ErrorMessage"] = "هذه الصفحة مخصصة لمزودي الخدمة";
                return RedirectToAction("Index", "Home");
            }

            // 3) الفلترة: مدينة المزود + تخصصه + الطلبات المتاحة فقط
            var availableRequests = await _context.EventRequests
                .Include(r => r.Category)          // نحمل بيانات القسم مع الطلب
                .Include(r => r.Region)            // نحمل اسم المدينة
                .Include(r => r.User)              // نحمل بيانات العميل صاحب الطلب
                .Include(r => r.ProviderOffers)    // نحمل العروض المقدمة على الطلب
                .Where(r => r.RegionId == currentUser.RegionId
                         && r.CategoryId == currentUser.CategoryId
                         && r.Status == EventStatus.Pending)
                .OrderBy(r => r.EventDate)         // الأقرب موعداً أولاً
                .ToListAsync();

            // 4) نمرر معرف المزود للصفحة حتى نعرف: هل قدم عرضاً سابقاً؟
            ViewBag.CurrentProviderId = currentUser.Id;

            // 5) نسلم القائمة لصفحة العرض
            return View(availableRequests);
        }

        //==================================================
        // SendOffer : زر فوري (بدون فيو)
        // الفني يقدم عرض سعر على طلب معين ثم يرجع للرادار
        //==================================================
        [HttpPost]                    // يستقبل إرسال الفورم فقط، مو فتح رابط
        [ValidateAntiForgeryToken]    // حماية من التزوير: يقبل الفورم من موقعنا فقط
        public async Task<IActionResult> SendOffer(int eventRequestId, decimal price)
        {
            // 1) المستخدم الحالي + تأكد أنه مزود خدمة
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.AccountType != UserType.Provider)
            {
                TempData["ErrorMessage"] = "تقديم العروض متاح لمزودي الخدمة فقط";
                return RedirectToAction("Index", "Home");
            }

            // 2) نجيب الطلب ونتأكد أنه موجود وما زال متاحاً
            var eventRequest = await _context.EventRequests.FindAsync(eventRequestId);

            if (eventRequest == null || eventRequest.Status != EventStatus.Pending)
            {
                TempData["ErrorMessage"] = "هذا الطلب لم يعد متاحاً للعروض";
                return RedirectToAction(nameof(AvailableRequests));
            }

            // 3) ما يصير المزود يقدم عرض على طلب أنشأه هو بنفسه (كعميل)
            if (eventRequest.UserId == currentUser.Id)
            {
                TempData["ErrorMessage"] = "لا يمكنك تقديم عرض على طلبك الخاص";
                return RedirectToAction(nameof(AvailableRequests));
            }

            // 4) منع تكرار العرض: عرض واحد فقط لكل مزود على نفس الطلب
            bool alreadyOffered = await _context.ProviderOffers
                .AnyAsync(o => o.EventRequestId == eventRequestId
                            && o.ProviderId == currentUser.Id);

            if (alreadyOffered)
            {
                TempData["ErrorMessage"] = "قدمت عرضاً سابقاً على هذا الطلب";
                return RedirectToAction(nameof(AvailableRequests));
            }

            // 5) إنشاء العرض وحفظه
            var providerOffer = new ProviderOffer
            {
                EventRequestId = eventRequestId,
                ProviderId = currentUser.Id,
                Price = price,
                OfferDate = DateTime.Now,
                IsAccepted = false        // يظل false حتى يوافق العميل
            };

            _context.ProviderOffers.Add(providerOffer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم إرسال عرضك للعميل بنجاح";
            return RedirectToAction(nameof(AvailableRequests));
        }

        //==================================================
        // AcceptOffer : زر فوري (بدون فيو)
        // العميل صاحب الطلب يعتمد عرض فني معين
        //==================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOffer(int offerId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // 1) نجيب العرض ومعه الطلب المرتبط فيه
            var providerOffer = await _context.ProviderOffers
                .Include(o => o.EventRequest)
                .FirstOrDefaultAsync(o => o.Id == offerId);

            if (providerOffer == null || providerOffer.EventRequest == null || currentUser == null)
            {
                return NotFound();
            }

            // 2) حماية: فقط صاحب الطلب نفسه يقدر يوافق على العروض
            if (providerOffer.EventRequest.UserId != currentUser.Id)
            {
                return Forbid();
            }

            // 3) حماية: الموافقة ممكنة فقط والطلب ما زال متاحاً
            if (providerOffer.EventRequest.Status != EventStatus.Pending)
            {
                TempData["ErrorMessage"] = "هذا الطلب لم يعد متاحاً للموافقة";
                return RedirectToAction("Details", "EventRequests", new { id = providerOffer.EventRequestId });
            }

            // 4) الاعتماد: نعلّم العرض كمقبول + نسجل الفني الفائز + نغير حالة الطلب
            providerOffer.IsAccepted = true;
            providerOffer.EventRequest.AcceptedProviderId = providerOffer.ProviderId;
            providerOffer.EventRequest.Status = EventStatus.Accepted;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم قبول العرض! معلومات التواصل مع الفني ظاهرة الآن بالأسفل";
            return RedirectToAction("Details", "EventRequests", new { id = providerOffer.EventRequestId });
        }

        //==================================================
        // MyTasks : مهام المزود — الحالية والأرشيف
        // كل الطلبات التي وافق العميل فيها على عرض هذا المزود
        //==================================================
        public async Task<IActionResult> MyTasks()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || currentUser.AccountType != UserType.Provider)
            {
                TempData["ErrorMessage"] = "هذه الصفحة مخصصة لمزودي الخدمة";
                return RedirectToAction("Index", "Home");
            }

            // الفلتر: الطلبات التي رست على هذا المزود (مقبولة أو مكتملة)
            var myTasks = await _context.EventRequests
                .Include(r => r.Category)
                .Include(r => r.Region)
                .Include(r => r.User)              // العميل: نحتاج اسمه وجواله للتواصل
                .Include(r => r.ProviderOffers)    // لعرض السعر المتفق عليه
                .Where(r => r.AcceptedProviderId == currentUser.Id)
                .OrderByDescending(r => r.EventDate)
                .ToListAsync();

            return View(myTasks);
        }

        //==================================================
        // CompleteRequest : زر فوري (بدون فيو)
        // الفني ينهي المهمة بعد إتمامها فتتحول حالتها إلى مكتمل
        //==================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteRequest(int eventRequestId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var eventRequest = await _context.EventRequests.FindAsync(eventRequestId);

            if (eventRequest == null || currentUser == null)
            {
                return NotFound();
            }

            // حماية: فقط الفني المعتمد على هذا الطلب يقدر ينهيه
            if (eventRequest.AcceptedProviderId != currentUser.Id)
            {
                return Forbid();
            }

            // حماية: الإنهاء ممكن فقط للمهام المقبولة (مو الملغية أو المنتهية)
            if (eventRequest.Status != EventStatus.Accepted)
            {
                TempData["ErrorMessage"] = "لا يمكن إنهاء هذه المهمة في حالتها الحالية";
                return RedirectToAction(nameof(MyTasks));
            }

            eventRequest.Status = EventStatus.Completed;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم إنهاء المهمة بنجاح — انتقلت إلى أرشيف أعمالك";
            return RedirectToAction(nameof(MyTasks));
        }
    }
}
