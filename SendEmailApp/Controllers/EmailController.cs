using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendEmailApp.Data;
using SendEmailApp.Infrastructure;
using SendEmailApp.Models;

namespace SendEmailApp.Controllers
{
    public class EmailController : Controller
    {
        private readonly EmailDbContext _db;
        public EmailController(EmailDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public IActionResult Send(int id)
        {
            HttpContext.Session.SetString("taskId", id.ToString());
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(Email email, [FromServices] EmailService service, int id)
        {
            var sharedUser = _db.Users.Where(u => u.Email == email.To).FirstOrDefault();
            if (sharedUser != null)
            {
                var user = _db.Users.Where(u => u.Id == HttpContext.Session.GetString("userId")).FirstOrDefault();
                var message = $"{user.Email} share task with you";

                await service.SendMailAsync(email.To, "SHARE", message);
                ViewBag.Message = "Mail Has Been Sent Successfully";

                UserTask task = _db.Tasks.Where(x => x.Id == id).FirstOrDefault();

                if (task != null)
                {
                    _db.Shares.Add(new UserTaskRelation()
                    {
                        AppUserId = sharedUser.Id,
                        UserTaskId = task.Id
                    });
                    _db.SaveChanges();
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "This email adress is not exists");
            }
            return View();
        }
    }
}