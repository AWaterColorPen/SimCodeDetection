using SimCodeDetectionWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;

namespace SimCodeDetectionWeb.Judge
{
    public class JudgeService
    {
        public static void Add(int subid, DateTime time)
        {
            var judge = JudgeSingleton.JudgeSingletonCreate();
            while (judge.PushBack(subid, time) == -1)
            {
                SimCodeDBContext db = new SimCodeDBContext();
                try
                {
                    var sub = db.Submissions.Find(subid);
                    sub.status = "Submission Error";
                    db.Entry(sub).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Tools.Log.Loger(e.Message);
                }
                db.Dispose();
                Thread.Sleep(10);
            }
        }

        public static void ReJudgeProblems(int pid)
        {
            SimCodeDBContext db = new SimCodeDBContext();
            try
            {
                var problem = db.Problems.Find(pid);
                JudgeReDoThread jrt = new JudgeReDoThread(problem);
            }
            catch (Exception e)
            {
                Tools.Log.Loger(e.Message);
            }
            db.Dispose();
        }

        public static void ReJudgeSubmission(int subid, DateTime time)
        {
            SimCodeDBContext db = new SimCodeDBContext();
            try
            {
                var sub = db.Submissions.Find(subid);
                Add(subid, DateTime.Now);
            }
            catch (Exception e)
            {
                Tools.Log.Loger(e.Message);
            }
        }

        public static List<KeyValuePair<int, DateTime>> JudgeInfo()
        {
            var judge = JudgeSingleton.JudgeSingletonCreate();
            return judge.Info();
        }
    }
}