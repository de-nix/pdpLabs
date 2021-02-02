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
            Person user = new Person();
            user.Name = username;
            user.Role = password;
            if (dal.isUnique(username)) {
                Person newUser = dal.insertUser(user);
                if (newUser != null)
                {
                    Session.Add("user", newUser);
                   return RedirectToAction("Index", "Home");
                }
            };
            return View("Register");
        }   
        public ActionResult login()
        {
            string username = Request.Params["username"];
            DAL dal = new DAL();
            Person newUser = dal.authenticate(username);
            if (newUser != null)
            {
                Session.Add("user", newUser);
                return RedirectToAction("Index","Home");
            }
            return View("Login");
        }

    }
}