using EventRescue.Data;
using EventRescue.Models;
using EventRescue.Models.Enums;
using EventRescue.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventRescue.Controllers
{
    [Authorize]
    public class EventRequestsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventRequestsController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // حسب مواصفات المدرب: المزود يستطيع استخدام النظام كعميل
            // إذا كان لديه مناسبة خاصة، لذلك لا نمنعه من إنشاء الطلبات

            LoadCategories();

            var model = new CreateEventRequestViewModel
            {
                EventDate = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventRequestViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.EventDate < DateTime.Now)
            {
                ModelState.AddModelError("EventDate", "تاريخ ووقت المناسبة لا يمكن أن يكون في الماضي");
            }

            if (ModelState.IsValid)
            {
                string? imagePath = null;

                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "uploads",
                        "requests"
                    );

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string fileName = Guid.NewGuid().ToString()
                        + Path.GetExtension(model.ImageFile.FileName);

                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    imagePath = "/uploads/requests/" + fileName;
                }

                var eventRequest = new EventRequest
                {
                    UserId = user.Id,
                    CategoryId = model.CategoryId,
                    RegionId = user.RegionId,
                    Title = model.Title,
                    Description = model.Description,
                    EventDate = model.EventDate,
                    Address = model.Address,
                    ImagePath = imagePath,
                    Status = EventStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                _context.EventRequests.Add(eventRequest);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم إنشاء الطلب بنجاح";
                return RedirectToAction("MyRequests");
            }

            LoadCategories();
            return View(model);
        }

        public async Task<IActionResult> MyRequests()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var requests = await _context.EventRequests
                .Include(r => r.Category)
                .Include(r => r.Region)
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(requests);
        }
          



     public async Task<IActionResult> Details(int id)
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
    {
        return RedirectToAction("Login", "Account");
    }

    var request = await _context.EventRequests
        .Include(r => r.Category)
        .Include(r => r.Region)
        .Include(r => r.ProviderOffers)          // العروض المقدمة على الطلب
            .ThenInclude(o => o.Provider)        // ومع كل عرض: بيانات الفني مقدم العرض
        .Include(r => r.AcceptedProvider)        // الفني المعتمد (إن وجد) لعرض معلومات التواصل
        .FirstOrDefaultAsync(r => r.Id == id && r.UserId == user.Id);

    if (request == null)
    {
        return NotFound();
    }

    return View(request);
}

    

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Cancel(int id)
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
    {
        return RedirectToAction("Login", "Account");
    }

    var request = await _context.EventRequests
        .FirstOrDefaultAsync(r => r.Id == id && r.UserId == user.Id);

    if (request == null)
    {
        return NotFound();
    }

    if (request.Status != EventStatus.Pending)
    {
        TempData["ErrorMessage"] = "لا يمكن إلغاء هذا الطلب لأنه لم يعد متاحًا للعروض";
        return RedirectToAction("Details", new { id = request.Id });
    }

    request.Status = EventStatus.Canceled;

    await _context.SaveChangesAsync();

    TempData["SuccessMessage"] = "تم إلغاء الطلب بنجاح";

    return RedirectToAction("MyRequests");
}

    
        private void LoadCategories()
        {
            ViewBag.Categories = _context.Categories.ToList();
        }
    }
}