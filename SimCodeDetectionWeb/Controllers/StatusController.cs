using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimCodeDetectionWeb.Models;

namespace SimCodeDetectionWeb.Controllers
{
    public class StatusController : Controller
    {
        private SimCodeDBContext db = new SimCodeDBContext();

        // GET: Status
        public ActionResult Index()
        {
          // ApplicationUser ss = new ApplicationUser();
          // var ss = ApplicationDbContext.Create();
          // Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext xx = new  Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext();
            return View(db.Submissions.OrderByDescending(m => m.Id).ToList());
           // return View(db.Submissions.OrderByDescending(m => m.Id).Take(50).ToList());
        }

        public ActionResult Solution(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var submission = db.Submissions.Find(id);
            if (submission.status != "Finished")
            {
                ViewBag.Results = new List<int>();
                return View(submission);
            }
            var results = Tools.JsonTool.Json2ListSimResult(submission.results);

            List<List<KeyValuePair<string, string>>> data = new List<List<KeyValuePair<string, string>>>();
            List<KeyValuePair<string, DateTime>> MPW = new List<KeyValuePair<string, DateTime>>();
            List<int> OtherUser = new List<int>();
            foreach (var result in results)
            {
                var index1 = result.index1;
                var index2 = result.index2;
                var othersub = db.Submissions.Find(result.otherid);
                MPW.Add(new KeyValuePair<string, DateTime>(othersub.OUser.userName, othersub.subTime));
                OtherUser.Add(othersub.OUser.Id);
                List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
                for (var i = 0; i < result.count; i++)
                {
                    string A = submission.Snippets[index1[i]];
                    string B = othersub.Snippets[index2[i]];
                    pairs.Add(new KeyValuePair<string, string>(A, B));
                }
                data.Add(new List<KeyValuePair<string, string>>(pairs));
            }
            ViewBag.MPW = MPW;
            ViewBag.OtherUser = OtherUser;
            ViewBag.Results = results;
            ViewBag.Data = data;
            return View(submission);
        }

        public ActionResult U(int? id)
        {
            if (id == null)
            {
                if (User.Identity.IsAuthenticated == false)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                id = UserFind(User.Identity.Name).Id;
            }
            var user = db.OJUsers.Find(id);
            var problems = (from m in user.submissions select m.problem).Distinct().OrderBy(m => m.Id);
            ViewBag.user = user;
            ViewBag.problems = problems.ToList();
            ViewBag.p = problems.ToList().Count;
            ViewBag.List = user.submissions.OrderByDescending(m => m.Id).ToList();
            return View();
        }

        public ActionResult OJuser()
        {
            return View(db.OJUsers.ToList());
        }

        public ActionResult Appuser()
        {
            var ss = ApplicationDbContext.Create();
            Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext xx = new  Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext();
            return View(ss.Users.ToList());
        }

        private OJUser UserFind(string userName)
        {
            var user = from m in db.OJUsers where m.userName == userName select m;
            if (user == null || user.ToList().Count == 0)
            {
                return null;
            }
            System.Diagnostics.Debug.WriteLine("fuck");
            return user.ToList()[0];
        }

        public ActionResult JudgeInfo()
        {
            ViewBag.Info = Judge.JudgeService.JudgeInfo();
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
