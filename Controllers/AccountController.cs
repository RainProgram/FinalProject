using EventRescue.Constants;
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
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            LoadLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (register.AccountType == UserType.Provider && register.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "يرجى اختيار التخصص لمزود الخدمة");
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = register.Email,
                    Email = register.Email,
                    FullName = register.FullName,
                    RegionId = register.RegionId,
                    AccountType = register.AccountType,
                    CategoryId = register.AccountType == UserType.Provider ? register.CategoryId : null,
                    IsAvailableNow = register.AccountType == UserType.Provider,
                    IsBlocked = false
                };

                var result = await _userManager.CreateAsync(user, register.Password);

                if (result.Succeeded)
                {
                    string roleName = register.AccountType == UserType.Provider
                        ? UserRoles.Provider
                        : UserRoles.Client;

                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                    }

                    await _userManager.AddToRoleAsync(user, roleName);

                    TempData["SuccessMessage"] = "تم إنشاء الحساب بنجاح، يمكنك تسجيل الدخول الآن";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoadLists();
            return View(register);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

               if (result.Succeeded)
                    {
                        var user = await _userManager.FindByEmailAsync(model.Email);

                        if (user != null)
                        {
                            var roles = await _userManager.GetRolesAsync(user);

                            if (roles.Contains(UserRoles.Admin))
                            {
                                return RedirectToAction("Dashboard", "Admin");
                            }
                        }

                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return LocalRedirect(model.ReturnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }

                ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            return View(model);
        }

       [Authorize]
public async Task<IActionResult> Profile()
{
    var user = await _context.Users
        .Include(u => u.Region)
        .Include(u => u.Specialty)
        .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

    if (user == null)
    {
        return NotFound();
    }

    if (user.AccountType == UserType.Client)
    {
        var myRequests = _context.EventRequests
            .Where(r => r.UserId == user.Id);

        ViewBag.TotalRequests = await myRequests.CountAsync();
        ViewBag.PendingRequests = await myRequests.CountAsync(r => r.Status == EventStatus.Pending);
        ViewBag.AcceptedRequests = await myRequests.CountAsync(r => r.Status == EventStatus.Accepted);
        ViewBag.CanceledRequests = await myRequests.CountAsync(r => r.Status == EventStatus.Canceled);

        var latestRequest = await myRequests
            .Include(r => r.Category)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();

        ViewBag.LatestRequest = latestRequest;
    }

    return View(user);
}



     [Authorize]
[HttpGet]
public async Task<IActionResult> EditProfile()
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
    {
        return RedirectToAction("Login");
    }

    var model = new EditProfileViewModel
    {
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber,
        RegionId = user.RegionId,
        CategoryId = user.CategoryId,
        IsAvailableNow = user.IsAvailableNow
    };

    LoadLists();
    return View(model);
}

[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditProfile(EditProfileViewModel model)
{
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
    {
        return RedirectToAction("Login");
    }

    if (user.AccountType == UserType.Provider && model.CategoryId == null)
    {
        ModelState.AddModelError("CategoryId", "يرجى اختيار التخصص لمزود الخدمة");
    }

    if (ModelState.IsValid)
    {
        user.FullName = model.FullName;
        user.PhoneNumber = model.PhoneNumber;
        user.RegionId = model.RegionId;

        if (user.AccountType == UserType.Provider)
        {
            user.CategoryId = model.CategoryId;
            user.IsAvailableNow = model.IsAvailableNow;
        }
        else
        {
            user.CategoryId = null;
            user.IsAvailableNow = false;
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "تم تحديث الملف الشخصي بنجاح";
            return RedirectToAction("Profile");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    LoadLists();
    return View(model);
}





        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        private void LoadLists()
        {
            ViewBag.Regions = _context.Regions.ToList();
            ViewBag.Categories = _context.Categories.ToList();
        }
    }
}