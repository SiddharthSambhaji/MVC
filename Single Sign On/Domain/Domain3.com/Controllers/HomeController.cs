using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SSOLib;
using Models.User;
using Microsoft.AspNet.Identity;
using ARS.ITCS.Core;

namespace ARS.ITCS.ControlPanel
{
    public class HomeController : BaseController
    {
       /* public HomeController()
        {

        }
        #region Constructor
        /// <summary>
        /// HomeController
        /// </summary>
        /// <param name="unitOfWork"></param>
        public HomeController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }
        #endregion Constructor
        */
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public ActionResult About(UserViewModel user)
        {
            ViewBag.Message = "Your application description page.";

            if (ModelState.IsValid)
            {
                if (user.UserName == null)
                {

                    //  FormsAuthentication.SetAuthCookie(user.UserName);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    try
                    {
                        var identityUser = UserManager.Find(user.UserName, user.Password);
                        if (identityUser != null)
                        {
                            var boolValue = Login(user.UserName, identityUser.PasswordHash);
                            ViewBag.Message = "UserName :" + user.UserName + "Hashed : " + identityUser.PasswordHash;
                        }
                    }
                    catch (Exception ex)
                    {
                        var excep = ex.Message;
                    }
                  

                    
                    //ModelState.AddModelError("", "Login data is incorrect!");
                }
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult LogoutPage()
        {
            Logout();

            ViewBag.Message = "You Logged Out";

           return View("About");
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}