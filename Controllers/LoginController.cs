using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movie_Project.Data;
using Movie_Project.Models;

namespace Movie_Project.Controllers
{
    public class LoginController : Controller
    {

        private readonly MovieDBContext _context;

        public LoginController(MovieDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogIn(Login u)
        {

           // if (ModelState.IsValid)
            {
                User admin = AuthenticateAdmin(u.Email, u.Password);
                if (admin != null)
                {

                    HttpContext.Session.SetInt32("IDAdmin", admin.UserId);
                    //HttpContext.Session.SetString("NameAdmin", admin.FirstName);
                    //ViewBag.NameAdmin = GetFirstName(admin);
                    //TempData["NameAdmin"] = GetFirstName(admin);
                    TempData["NameAdmin"] = GetFirstName(admin);

                    return RedirectToAction("IndexAdmin", "Movie");

                }

                User user = AuthenticateUser(u.Email, u.Password);
                if (user != null)
                {
                    HttpContext.Session.SetInt32("IDUser", user.UserId);
                    //HttpContext.Session.SetString("NameUser", user.FirstName);
                    TempData["NameUser"] = GetFirstName(user);
                    return RedirectToAction("IndexUser", "Movie");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect password or Email");
                }


            }
            return View(u);
        }

        private User AuthenticateUser(string email, string password)
        {

            // Retrieve user from your data store
            // Compare provided email and password with stored values
            // Return User object if authenticated, null otherwise
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && user.Password == password)
            {
                return user;
            }
            return null;
        }
        private User AuthenticateAdmin(string email, string password)
        {

            // Retrieve user from your data store
            // Compare provided email and password with stored values
            // Return User object if authenticated, null otherwise
            var admin = _context.Users.FirstOrDefault(u => u.Email == email);

            if (admin != null && admin.Password == password && admin.status == "admin")
            {
                return admin;
            }
            return null;
        }

        private string GetFirstName(User user)
        {

            return (user.FirstName);
        }
    }
}
