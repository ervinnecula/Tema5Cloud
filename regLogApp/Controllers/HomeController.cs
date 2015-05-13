using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using StorageService;
using AzureSQLDB;
using WebGrease.Css.Extensions;

namespace regLogApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private SimpleStorageService _storageService = new SimpleStorageService();

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
            var links = files.Select(x => _storageService.GetDownloadLink(user, x));

            return View(files);
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
     
    }
}