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

            User u = (User)Session["user"];
            if (u == null) return RedirectToAction("Index", "Login");
            else
            {
                ViewBag.user = u;
                return View();
            }
        }
        public void addAssets()
        { List<Asset> assets = new List<Asset>();
            assets = new JavaScriptSerializer().Deserialize<List<Asset>>(Request.Params["arr"]);
            DAL dal = new DAL();
                //Loop and insert records.
            foreach (Asset asset in assets)
            {
                dal.addAsset(asset);
            }
            
        }

        public String GetAssets()
        {

            int id = int.Parse(Request.Params["id"]);
            DAL dal = new DAL();
            List<Asset> assets = dal.getUserAssets(id);
            String result = "<table id=\"assets\"><th>Name</th><th>Description</th><th>NValue</th>";
            foreach (Asset asset in assets) {
                if(asset.Value>10)
                result += "<tr style= \"color: red \" ><td>" + asset.Name + "</td><td>" + asset.Description + "</td><td>" + asset.Value + "</td></tr>";
                else result += "<tr><td>" + asset.Name + "</td><td>" + asset.Description + "</td><td>" + asset.Value + "</td></tr>";

            }
            result += "</table>";

            return result;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

 
}