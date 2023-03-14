using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BilgeShop.WebUI.Controllers
{
    // Authentication ve Authorization 
    // (Kimlik Doğrulama - Yetkilendirme)
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("kayit-ol")]
        public IActionResult Register()
        {
            
            return View();
        }

        [HttpPost]
        [Route("kayit-ol")]
        public  IActionResult Register(RegisterViewModel formData)
        {

            if (!ModelState.IsValid)
            { 
                return View(formData); // formData'yı geri göndermezsem, açılan view boş gelecek, yeni kullanıcının girdiği bütün veriler silinecek.
            }

            var addUserDto = new AddUserDto()
            {
                FirstName = formData.FirstName.Trim(),
                LastName = formData.LastName.Trim(),
                Password = formData.Password.Trim(),
                Email = formData.Email.Trim(),
            };

            var response = _userService.AddUser(addUserDto);

            if(response.IsSucceed)
            {
                return RedirectToAction("Index" , "Home");
            }
            else
            {
                ViewBag.ErrorMessage = response.Message;
                return View(formData);
            }

        

        }


        public async Task<IActionResult> Login(LoginViewModel formData)
        {

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            var loginDto = new LoginDto()
            {
                Email = formData.Email,
                Password = formData.Password
            };

            var userDto = _userService.Login(loginDto);

            if(userDto is null)
            {
               // ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı."; // RedirectToAction kullanıyorsanız, ViewBag tanınmaz.

                TempData["ErrorMessage"] = "Kullanıcı adı veya şifre hatalı."; // RedirectToAction için, TempData kullanılır.
                return RedirectToAction("Index", "Home");
            }


            // Oturumda tutacağım her bir veri -> Claim
            // Bütün verilerin listesi -> List<Claim>
            var claims = new List<Claim>();

            claims.Add(new Claim("id", userDto.Id.ToString()));
            claims.Add(new Claim("email" , userDto.Email));
            claims.Add(new Claim("firstName" , userDto.FirstName));
            claims.Add(new Claim("lastName", userDto.LastName));
            claims.Add(new Claim("userType", userDto.UserType.ToString()));

            // yetkilendirme için , özel olarak bir claim açmam gerekiyor. 

            claims.Add(new Claim(ClaimTypes.Role, userDto.UserType.ToString()));


            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Listedeki verilerle , bir oturum açılacağı için , yukarıda idendity nesnesini tanımladım.

            var autProperties = new AuthenticationProperties
            {
                AllowRefresh = true, // sayfa yenilendiğinde oturum düşmesin.
                ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(48)) // oturum açıldıktan sonra 48 saat geçerli olsun.
            };

            // bir metot içerisinde await kullanılacaksa , metot tanımına async ve task eklenir.

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), autProperties);


            // Önemli not : Kesinlikle password, claim'de tutulmaz. UI katmanı güvenlik düzenin en düşük olduğu katmandır. Password ile zaten burada bir işimiz yok!

            TempData["LoginMessage"] = "Giriş başarıyla yapıldı.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); // oturumu kapat.

            return RedirectToAction("Index", "Home");
        }
    }
}
