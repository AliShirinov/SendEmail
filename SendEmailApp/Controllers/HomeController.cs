using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendEmailApp.Data;
using SendEmailApp.Models;

namespace SendEmailApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailDbContext db;
        public HomeController(EmailDbContext dbContext, UserManager<AppUser> userManager)
        {
            db = dbContext;
            _userManager = userManager;
        }
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = HttpContext.Session.GetString("userId");
            HttpContext.Session.SetString("userId",userId);
            //var userTasks = await db.Tasks.Where(t => t.AppuserId == userId).ToListAsync();
            var user = await db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var userTasks = db.Shares.Where(x => x.AppUserId == user.Id).Select(x => x.UserTask);
            return View(userTasks);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
