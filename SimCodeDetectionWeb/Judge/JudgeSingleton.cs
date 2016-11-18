using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimCodeDetectionWeb.Models;
using System.Threading;
using System.Data.Entity;
using SimCodeDetectionWeb.Tools;

namespace SimCodeDetectionWeb.Judge
{
    public class JudgeSingleton
    {
        static readonly JudgeSingleton instance = new JudgeSingleton();

        public static JudgeSingleton JudgeSingletonCreate()
        {
            return instance;
        }

        private Queue<KeyValuePair<int, DateTime>> queue = new Queue<KeyValuePair<int, DateTime>>();
        private Dictionary<int, DateTime> verson = new Dictionary<int, DateTime>();
        private int queueLengthLimit = 32;

        private SimCodeDBContext db = new SimCodeDBContext();

        private JudgeSingleton()
        {
            Thread thread = new Thread(new ThreadStart(MainWork));
            thread.Start();
        }

        private void MainWork()
        {
            int sleeptime = 500;
            DateTime lastsubtime = DateTime.Now;
            while (true)
            {
                Thread.Sleep(sleeptime);

                if (QLength() > 0)
                {
                    var ele = TopAndPop();
                    if (CheckRightVerson(ele))
                    {
                        var sub = db.Submissions.Find(ele.Key);

                        {
                            sub.status = "Running";
                            db.Entry(sub).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        var otherslists = GetSimCodeObject(sub);
                        SubmissionCodeSlicer(otherslists, sub);
                        ReflashResult(sub);
                        JudgeWork(otherslists, sub);

                        {
                            sub.status = "Finished";
                            db.Entry(sub).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    sleeptime = Math.Max(sleeptime / 2, 50);
                }
                else
                {
                    sleeptime = Math.Min(sleeptime * 2, 500);
                }
            }
        }

        private bool CheckRightVerson(KeyValuePair<int, DateTime> ele)
        {
            bool ret = false;
            Monitor.Enter(this);
            if (verson.ContainsKey(ele.Key))
            {
                if (verson[ele.Key] == ele.Value)
                {
                    ret = true;
                    verson.Remove(ele.Key);
                }
            }
            Monitor.Exit(this);
            return ret;
        }

        private int QLength()
        {
            int length = 0;
            Monitor.Enter(this);
            length = queue.Count;
            Monitor.Exit(this);
            return length;
        }

        private KeyValuePair<int, DateTime> TopAndPop()
        {
            KeyValuePair<int, DateTime> ret = new KeyValuePair<int, DateTime>();
            Monitor.Enter(this);
            ret = queue.Dequeue();
            Monitor.Exit(this);
            return ret;

        }

        public int PushBack(int id, DateTime time)
        {
            int res = 0;
            Monitor.Enter(this);
            if (queue.Count < queueLengthLimit)
            {
                queue.Enqueue(new KeyValuePair<int, DateTime>(id, time));
                if (verson.ContainsKey(id))
                {
                    verson[id] = time;
                }
                else
                {
                    verson.Add(id, time);
                }
                var sub = db.Submissions.Find(id);
                sub.status = "Pending";
                db.Entry(sub).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                res = -1;
            }
            Monitor.Exit(this);
            return res;
        }

        private List<Submission> GetSimCodeObject(Submission sub)
        {
            List<Submission> list = new List<Submission>();
            var userid = sub.OUser.Id;
            DateTime time = sub.subTime;
            var judgetype = sub.problem.judgeType;
            foreach (var subs in sub.problem.submissions)
            {
                if (judgetype == 0)
                {
                    if (subs.subTime < time && subs.OUser.Id != userid)
                    {
                        list.Add(subs);
                    }
                }
                if (judgetype == 1)
                {
                    if (subs.OUser.Id != userid)
                    {
                        list.Add(subs);
                    }
                }
                if (judgetype == 2)
                {
                    if (subs.subTime < time)
                    {
                        list.Add(subs);
                    }
                }
                if (judgetype == 3)
                {
                    if (subs.Id != sub.Id)
                    {
                        list.Add(subs);
                    }
                }
            }
            return list;
        }

        private void SubmissionCodeSlicer(List<Submission> otherslists, Submission sub)
        {
            foreach (var subs in otherslists)
            {
                if (subs.Snippets == null || subs.Snippets.Count == 0)
                {
                    subs.Snippets = JudgeCodes.CodeSlicing(subs.source);
                    db.Entry(subs).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            if (sub.Snippets == null || sub.Snippets.Count == 0)
            {
                sub.Snippets = JudgeCodes.CodeSlicing(sub.source);
                db.Entry(sub).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private void ReflashResult(Submission sub)
        {
            sub.results = null;
        }

        private void JudgeWork(List<Submission> otherslists, Submission sub)
        {
            List<SimResult> results = new List<SimResult>();
            foreach (var subs in otherslists)
            {
                var result = JudgeCodes.GetSnippetPair(sub, subs);
                if (result != null)
                {
                    results.Add(result);
                }
            }
            sub.results = JsonTool.ListSimResult2Json(results);
            db.Entry(sub).State = EntityState.Modified;
            db.SaveChanges();
        }

        public List<KeyValuePair<int, DateTime>> Info()
        {
            List<KeyValuePair<int, DateTime>> ret = new List<KeyValuePair<int, DateTime>>();
            ret.Add(new KeyValuePair<int, DateTime>(-1, DateTime.Now));
            Monitor.Enter(this);
            ret.Clear();
            ret = queue.ToArray().ToList();
            Monitor.Exit(this);
            return ret;
        }

        protected void Dispose()
        {
            db.Dispose();
        }
    }
}