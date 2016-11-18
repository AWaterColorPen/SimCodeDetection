using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimCodeDetectionWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Syntax()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Syntax(string source)
        {
            ViewBag.snippets = CodeParse.Slicer.RoslynTest(source);
            return View();
        }
        
        public ActionResult Debug()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Debug(string source)
        {
            ViewBag.api = new CodeParse.DebugAPI(source);
            return View();
        }
    }
}