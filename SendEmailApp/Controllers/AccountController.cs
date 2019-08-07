using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendEmailApp.Data;
using SendEmailApp.Infrastructure;
using SendEmailApp.Models;
using SendEmailApp.Models.ViewModel;

namespace SendEmailApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        //private readonly PasswordValidator<AppUser> _passwordValidator;
        private readonly EmailDbContext _db;

        public AccountController(UserManager<AppUser> userManager
                                , SignInManager<AppUser> signInManager
                                , IPasswordHasher<AppUser> passwordHasher
                                , EmailDbContext db
                                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordHasher = passwordHasher;
            _db = db;

            //_passwordValidator = passwordValidator;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                AppUser currentUser = await _userManager.FindByEmailAsync(registerModel.Email);
                if (currentUser != null)
                {
                    ModelState.AddModelError("", "This user already exists");
                    return View();
                }

                AppUser user = new AppUser
                {
                    Name = registerModel.Name,
                    Surname = registerModel.Surname,
                    Email = registerModel.Email,
                    UserName = registerModel.Name + registerModel.Surname
                };
                user.PasswordHash = _passwordHasher.HashPassword(user, registerModel.Password);

                IdentityResult userCreateResult = await _userManager.CreateAsync(user);
                if (userCreateResult.Succeeded)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (IdentityError error in userCreateResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel, [FromServices] EmailService service)
        {
            if (ModelState.IsValid)
            {
                AppUser currentUser = await _userManager.FindByEmailAsync(loginModel.Email);
                if (currentUser == null)
                {
                    ModelState.AddModelError("", "Given user not exists");
                    return View();
                }
                if (currentUser != null)
                {
                    PasswordVerificationResult verificationResult = _passwordHasher.VerifyHashedPassword(currentUser, currentUser.PasswordHash, loginModel.Password);
                    if (verificationResult == PasswordVerificationResult.Success)
                    {
                        var signInResult = await _signInManager.PasswordSignInAsync(currentUser, loginModel.Password, true, true);
                        if (signInResult.Succeeded)
                        {

                            //await _signInManager.SignInAsync(currentUser, true);
                            await SendExpireDateEmail(currentUser, service);
                            HttpContext.Session.SetString("userId", currentUser.Id);
                            return RedirectToAction(actionName: "Index", controllerName: "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Password is not valid ");
                        return View();
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult CreateTask()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(UserTask task)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.Users.Where(x => x.UserName == HttpContext.User.Identity.Name).First();
                UserTask newTask = new UserTask
                {
                    Title = task.Title,
                    Description = task.Description,
                    CreatedDate = DateTime.Now,
                    ExpireDate = task.ExpireDate,
                    AppUserId = user.Id
                };

                _db.Tasks.Add(newTask);

                _db.Shares.Add(new UserTaskRelation()
                {
                    AppUserId = user.Id,
                    UserTaskId = newTask.Id
                });

                await _db.SaveChangesAsync();
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
            else
            {
                ModelState.AddModelError("", "This task don't created");
                return View(task);

            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //if (id == null)
            //{
            //    return RedirectToAction(actionName: "Index", controllerName: "Home");
            //}
            //string userId = HttpContext.Session.GetString("userId");
            //var del = _db.Tasks.Where(x => x.Id == id).FirstOrDefault();
            ////if (del.AppUserId == userId)
            ////{
            //    _db.Tasks.Remove(del);
            //    await _db.SaveChangesAsync();
            //return RedirectToAction(actionName: "Index", controllerName: "Home");

            var del = _db.Tasks.Where(x => x.Id == id).FirstOrDefault();
            _db.Tasks.Remove(del);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index","Home");
            //return View();

        }


        [HttpGet]
        public IActionResult EditTask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTask(UserTask userTask)
        {
            if (ModelState.IsValid)
            {
                var task = _db.Tasks.Where(x => x.Id == userTask.Id).FirstOrDefault();
                if (task != null)
                {
                    task.Title = userTask.Title;
                    task.Description = userTask.Description;
                    task.CreatedDate = userTask.CreatedDate;
                    task.ExpireDate = userTask.ExpireDate;

                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else return View();
            }
            return View();
        }




        public async Task SendExpireDateEmail(AppUser user, EmailService service)
        {
            //var userTasks = await _db.Tasks.Where(t => t.AppuserId == user.Id).ToListAsync();
            var appUser = await _db.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();

            var userTasks = _db.Shares.Where(x => x.AppUserId == user.Id).Select(x => x.UserTask);

            foreach (var task in userTasks)
            {
                var date = DateTime.Now - task.ExpireDate;
                if (date.Minutes <= 10)
                {
                    await service.SendMailAsync(user.Email, "Exprie Date", "In 10 minutes your task's will end");
                }
            }
        }

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

    }
}