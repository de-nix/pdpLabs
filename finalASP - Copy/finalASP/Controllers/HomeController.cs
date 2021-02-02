using Antlr.Runtime;
using finalASP.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace finalASP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            Person u = (Person)Session["user"];
            if (u == null) return RedirectToAction("Index", "Login");
            else
            {
                ViewBag.user = u;
                return View();
            }
        }
        public void addAssets()
        { List<Course> assets = new List<Course>();
            assets = new JavaScriptSerializer().Deserialize<List<Course>>(Request.Params["arr"]);
            DAL dal = new DAL();
                //Loop and insert records.
            foreach (Course asset in assets)
            {
                dal.addAsset(asset);
            }
            
        }    
        public void addGrade()
        {

            Person u = (Person)Session["user"];
            int grade = int.Parse(Request.Params["grade"]);
            String student =Request.Params["student"];
            String course = Request.Params["course"];

            
            DAL dal = new DAL();
            String sgrade = "";
            //Loop and insert records.
            if (grade < 10)
            {
                sgrade = " " + grade;
            }
            else
            {
                sgrade = grade + "";
            }

            dal.insertGrade(student, course, sgrade , u.Id);
            
        }

        public String GetAssets()
        {

            int id = int.Parse(Request.Params["id"]);
            DAL dal = new DAL();
            List<Course> assets = dal.getUserAssets(id);
            String result = "<div id=\"assets\"><th>Name</th><th>Description</th><th>NValue</th>";
            foreach (Course asset in assets) {
                if(asset.Value>10)
                result += "<tr style= \"color: red \" ><td>" + asset.Name + "</td><td>" + asset.Description + "</td><td>" + asset.Value + "</td></tr>";
                else result += "<tr><td>" + asset.Name + "</td><td>" + asset.Description + "</td><td>" + asset.Value + "</td></tr>";

            }
            result += "</table>";

            return result;
        }        
        
        public String getParticipants()
        {

            string course = Request.Params["name"];
            DAL dal = new DAL();
            String result = dal.getParticipants(course);
           
            return result;
        }
                
        public String getCourses()
        {

            string course = Request.Params["name"];
            DAL dal = new DAL();
            String result = dal.getCourses(course);
           
            return result;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

 
}