using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilgeShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")] // program.cs'teki area:exists kısmı ile eşleşir.
    [Authorize(Roles = "Admin")] // Claim'lerdeki claims.Add(new Claim(ClaimTypes.Role, userDto.UserType.ToString())); ile bağlantılı (authController).

    // yukarıda yazdığım authorize sayesinde, yetkisi admin olmayan kişiler, bu controller'a istek atamaz.
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
