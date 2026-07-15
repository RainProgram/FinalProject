using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace mini_store.Controllers;

public class LanguageController : Controller
{
    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        // 1. إنشاء الـ Cookie وحفظ اللغة المختارة فيها لمدة عام كامل
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        // 2. إعادة المستخدم إلى الصفحة التي طلب منها تغيير اللغة
        return LocalRedirect(returnUrl);
    }
}