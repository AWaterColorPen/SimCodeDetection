using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.Tools
{
    public class PersonInfo
    {
        public int userid { get; set; }
        public string username { get; set; }
        public string studentid { get; set; }
        public List<KeyValuePair<KeyValuePair<int, string>, double>> subs = new List<KeyValuePair<KeyValuePair<int, string>, double>>();
        public string personalbest { get; set; }
        public string eva { get; set; }
        public string tabletype { get; set; }
        public double pbest;

        public PersonInfo(int id, string name, string sid)
        {
            userid = id;
            username = name;
            studentid = sid;
            pbest = 1;
        }

        public void Add(string results, string OJstatus, int subid)
        {
            var sresults = JsonTool.Json2ListSimResult(results);
            double best = 0;
            foreach (var s in sresults)
            {
                best = Math.Max(best, s.simvalue);
            }
            pbest = Math.Min(best, pbest);
            personalbest = pbest.ToString("P");

            if (pbest < 0.4) { eva = "GOOD"; tabletype = "success"; }
            else if (pbest < 0.9) { eva = "JUST SO SO"; tabletype = "warning"; }
            else { eva = "SO BAD"; tabletype = "danger"; }

            subs.Add(new KeyValuePair<KeyValuePair<int, string>, double>(new KeyValuePair<int, string>(subid, OJstatus), best));
        }
    }
}