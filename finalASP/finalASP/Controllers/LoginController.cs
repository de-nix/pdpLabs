using finalASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace finalASP.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View("Login");
        }        
        public ActionResult RegisterView()
        {
            return View("Register");
        }

        public ActionResult register()
        {
            string username = Request.Params["username"];
            string password = Request.Params["password"];
            DAL dal = new DAL();
            User user = new User();
            user.Name = username;
            user.Password = password;
            if (dal.isUnique(username)) {
                User newUser = dal.insertUser(user);
                if (newUser != null)
                {
                    Session.Add("user", newUser);
                   return RedirectToAction("Index", "Home");
                }
            };
            return View("Register");
        }   public ActionResult login()
        {
            string username = Request.Params["username"];
            string password = Request.Params["password"];
            DAL dal = new DAL();
            User newUser = dal.authenticate(username, password);
            if (newUser != null)
            {
                Session.Add("user", newUser);
                return RedirectToAction("Index","Home");
            }
            return View("Login");
        }

    }
}