using SimCodeDetectionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SimCodeDetectionWeb.Judge
{
    public class JudgeReDoThread
    {
        private Problem pp;

        public JudgeReDoThread(Problem p)
        {
            pp = p;
            Thread thread = new Thread(new ThreadStart(work));
            thread.Start();
        }

        private void work()
        {
            SimCodeDBContext db = new SimCodeDBContext();
            try
            {
                foreach (var submission in pp.submissions)
                {
                    JudgeService.Add(submission.Id, DateTime.Now);
                    Thread.Sleep(1500);
                }
            }
            catch (Exception e)
            {
                Tools.Log.Loger(e.Message);
            }
            db.Dispose();
        }
    }
}