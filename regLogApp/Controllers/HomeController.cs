using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using StorageService;
using AzureSQLDB;
using regLogApp.Models;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace regLogApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private SimpleStorageService _storageService = new SimpleStorageService();
        private string[] days = { "luni", "marti", "miercuri", "joi", "vineri", "sambata", "duminica" };

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Schedule()
        {
            ViewBag.Message = "Your schedule page.";

            return View();
        }

        public ActionResult StorageTest()
        {
            var user = User.Identity.Name;
            var files = _storageService.GetFilesList(user);
            var links = files.Select(x => new FileModel
            {
                ShowPath = x,
                DownloadPath = _storageService.GetDownloadLink(user, x)
            });

            return View(links);
        }

        [HttpPost]
        public ActionResult StorageTest(HttpPostedFileBase[] files)
        {
            var user = User.Identity.Name;
            if(files != null)
                _storageService.PutFiles(user, "orarSme", files);

            return RedirectToAction("StorageTest");
        }

        public ActionResult Logout()
        {
            ViewBag.Message = "Logout page";

            return View();
        }
        // GET: User
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ervinEntities())
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                    ModelState.Clear();
                    user = null;
                    ViewBag.Message = "Registration Successfully Done";
                }
            }
            return View(user);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Models.LoginUser user)
        {
            if (ModelState.IsValid)
            {
                if (user.IsValid(user.UserName, user.Password))
                {
                    Session["Username"] = user.UserName;
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login data is incorect!");
                }
            }
            return View(user);
        }
        [HttpPost]
        [System.Web.Services.WebMethod]
        public string SendSchedule(string data)
        {
           

            string[] splitResult = data.Split(',');


            //mergem pe randuri
            for (var i = 0; i < splitResult.Length / 9; i++)
            {
                //mergem pe fiecare bucatica
                string hour = null;
                for (int j = 0; j < 9; j++)
                {
                    if (j == 1)
                        hour = splitResult[9 * i + j];
                    if (j > 2)
                    {
                        if (splitResult[9*i + j] != "-")
                        {

                           
                            using (var context = new ervinEntities())
                            {
                                var inregistrare = new Schedule {Hour = hour, Day = days[j - 2]};
                                context.Schedules.Add(inregistrare);
                                context.SaveChanges();
                            }
                            //facem legatura dintre inregistrare si user;
                            using (var context = new ervinEntities())
                            {
                                var user = null;
                                foreach (var u in context.Users)
                                {
                                    if (u.UserName == Session["Username"].ToString())
                                    {
                                        user = u;
                                    }
                                }
                                var user = context.Users.FirstOrDefault(x => x.UserName == Session["Username"].ToString());
                                Schedule inregistrare = null;

                                foreach (var schedule in context.Schedules)
                                {
                                    inregistrare = schedule;
                                    if (schedule.Id > inregistrare.Id)
                                        inregistrare = schedule;
                                }
                                var tabelLegatura = new UsersSchedule {Schedule = inregistrare, User = user};

                                context.UsersSchedules.Add(tabelLegatura);
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }



            return "Data received at " + DateTime.Now.ToString();
        }
     
    }
}