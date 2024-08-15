using System.Security.Claims;

using _2302b1TempEmbedding.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _2302b1TempEmbedding.Controllers
{
    public class AuthController : Controller
    {
        private readonly _2302b1dotnetContext db;
        private string hashPassword;

        public AuthController(_2302b1dotnetContext _db)
        {
            db = _db;
        }
        public IActionResult signup()
        {
            return View();
        }
        [HttpPost]
        public IActionResult signup(User user)
        
        {
            var checkUser = db.Users.FirstOrDefault(u  => u.Email == user.Email);
            if (checkUser == null)
            {
                var hasher = new PasswordHasher<string>();
                var hashPassword = hasher.HashPassword(user.Email, user.Password);
                user.Password = hashPassword;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Login");

            }
            else { 

                ViewBag.msg = "User already exist.please login";
            return View();

            }

            
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            bool IsAuthenticated = false;
            string controller = "";
            ClaimsIdentity identity = null;

            var checkUser = db.Users.FirstOrDefault(u1 => u1.Email == user.Email);
            if (checkUser != null)
            {
                var hasher = new PasswordHasher<string>();
                var verifyPass = hasher.VerifyHashedPassword(user.Email, checkUser.Password, user.Password);

                if (verifyPass == PasswordVerificationResult.Success && checkUser.Roleid == 1)
                {
                    identity = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name ,checkUser.Username),
                    new Claim(ClaimTypes.Role ,"Admin"),
                }
                   , CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpContext.Session.SetString("email", checkUser.Email);
                    HttpContext.Session.SetString("username", checkUser.Username);

                    IsAuthenticated = true;
                    controller = "Admin";
                    HttpContext.Session.SetInt32("UserID", checkUser.Id);
                    HttpContext.Session.SetString("UserEmail", checkUser.Email);
                }
                else if (verifyPass == PasswordVerificationResult.Success && checkUser.Roleid == 2)
                {
                    IsAuthenticated = true;
                    identity = new ClaimsIdentity(new[]
                   {
                    new Claim(ClaimTypes.Name ,checkUser.Username),
                    new Claim(ClaimTypes.Role ,"User"),
                }
                   , CookieAuthenticationDefaults.AuthenticationScheme);
                   
                    controller = "Home";
                    HttpContext.Session.SetInt32("UserID", checkUser.Id);
                    HttpContext.Session.SetString("UserEmail", checkUser.Email);
                }
                else
                {
                    IsAuthenticated = false;

                }
                if (IsAuthenticated)
                {
                    var principal = new ClaimsPrincipal(identity);

                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", controller);
                }

                else
                {
                    ViewBag.msg = "Invalid Credentials";
                    return View();
                }
            }
            else
            {
                ViewBag.msg = "User not found";
                return View();
            }

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove ("UserID");
            HttpContext.Session.Remove ("UserEmail");
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}