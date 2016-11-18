using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimCodeDetectionWeb.Models;
using System.Text.RegularExpressions;

namespace SimCodeDetectionWeb.Controllers
{
    public class ProblemController : Controller
    {
        private SimCodeDBContext db = new SimCodeDBContext();

        // GET: Problem
        public ActionResult Index()
        {
            return View(db.Problems.ToList());
        }

        // GET: Problem/Show/5
        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Problem problem = db.Problems.Find(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            ViewBag.User = UserFind(User.Identity.Name);
            return View(problem);
        }

        // GET: Problem/List
        public ActionResult List(int page = 1)
        {
            ViewBag.User = UserFind(User.Identity.Name);
            int pages = db.Problems.ToList().Count / 50;
            if (db.Problems.ToList().Count % 50 != 0)
            {
                pages++;
            }
            ViewBag.Pages = pages;
            return View(db.Problems.OrderBy(m => m.Id).Skip((page - 1) * 50).Take(50).ToList());
        }

        // GET: Problem/List
        public ActionResult Summary(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var submissions = db.Problems.Find(id).submissions;
            
            Dictionary<int, Tools.PersonInfo> dp = new Dictionary<int, Tools.PersonInfo>();
            foreach (var subs in submissions)
            {
                if (subs.status == "Finished")
                {
                    if (dp.ContainsKey(subs.OUser.Id) == false)
                    {
                        dp.Add(subs.OUser.Id, new Tools.PersonInfo(subs.OUser.Id, subs.OUser.userName, subs.OUser.studentId));
                    }
                    dp[subs.OUser.Id].Add(subs.results, "--", subs.Id);
                }
            }

            List<Tools.PersonInfo> lp = new List<Tools.PersonInfo>();
            lp = dp.Values.ToList();
            ViewBag.Data = lp.OrderBy(m => m.pbest).ToList();
            return View();
        }

        [HttpPost]
        public string ReJudgeProblem(int? id)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                RedirectToAction("Login", "Account");
                return "unlogin";
            }
            if (id == null)
            {
                return "problem no find";
            }
            Problem problem = db.Problems.Find(id);
            if (problem == null)
            {
                return "problem no find";
            }
            if (User.Identity.Name != problem.OUser.userName && UserFind(User.Identity.Name).userLevel != "Admin")
            {
                return "no3";
            }

            Judge.JudgeService.ReJudgeProblems(problem.Id);
            return "ok";
        }

        [HttpPost]
        public string JudgeTypeChange(int? id, int tp = 0)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                RedirectToAction("Login", "Account");
                return "unlogin";
            }
            if (id == null || tp < 0 || tp > 4)
            {
                return "problem no find";
            }
            Problem problem = db.Problems.Find(id);
            if (problem == null)
            {
                return "problem no find";
            }
            if (User.Identity.Name != problem.OUser.userName && UserFind(User.Identity.Name).userLevel != "Admin")
            {
                return "no3";
            }

            problem.judgeType = tp;
            db.Entry(problem).State = EntityState.Modified;
            db.SaveChanges();
            return "ok";
        }

        public ActionResult Crawl(string url)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                RedirectToAction("Login", "Account");
            }
            Crawls.Crawls.Crawler(url, User.Identity.Name);
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(int pid, string _source)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Login", "Account");
            }

            var _problem = db.Problems.Find(pid);
            var _ouser = UserFind(User.Identity.Name);
            var submission = db.Submissions.Add(new Submission
            {
                problem = _problem,
                source = _source,
                OUser = _ouser,
                subTime = DateTime.Now,
                status = "Submitted"
            });

            _problem.UpdateUsers();
            db.SaveChanges();

            if (_problem.judgeType == 0 || _problem.judgeType == 2 || true)
            {
                Judge.JudgeService.Add(submission.Id, submission.subTime);
            }
            return RedirectToAction("Show", new { id = pid });
        }

        //Test Sumbit API 
        [HttpPost]
        [ValidateInput(false)]
        public string TestSubmitTry(int pid, string _source, string name)
        {
            var _ouser = UserFind(name);
            var _problem = db.Problems.Find(pid);
            Tools.Log.Loger("test submit " + name + " " + pid);
            if (_ouser == null || _problem == null)
            {
                return "-1";
            }

            var submission = db.Submissions.Add(new Submission
            {
                problem = _problem,
                source = _source,
                OUser = _ouser,
                subTime = DateTime.Now,
                status = "Submitted"
            });

            _problem.UpdateUsers();
            db.SaveChanges();
            Judge.JudgeService.Add(submission.Id, submission.subTime);
            return "-2";
        }

        // GET: Problem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Problem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,title,description,input,output,simpleInput,simpleOutput,endTime")] Problem problem)
        {
            problem.OUser = UserFind(User.Identity.Name);
            if (ModelState.IsValid)
            {
                db.Problems.Add(problem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(problem);
        }

        // GET: Problem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Problem problem = db.Problems.Find(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            return View(problem);
        }

        // POST: Problem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,title,description,input,output,simpleInput,simpleOutput,endTime")] Problem problem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(problem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(problem);
        }

        // GET: Problem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Problem problem = db.Problems.Find(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            return View(problem);
        }

        // POST: Problem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Problem problem = db.Problems.Find(id);
            db.Problems.Remove(problem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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
    }
}
