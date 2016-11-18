using SimCodeDetectionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SimCodeDetectionWeb.Crawls
{
    public class Crawls
    {
        private static SimCodeDBContext db = new SimCodeDBContext();

        public static void Crawler(string url, string username)
        {
            CrawlLink(url, username);
        }

        private static void CrawlLink(string url, string username)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    string html = client.GetStringAsync(url).Result;
                    Tools.Log.Loger(html);

                    Problem problem = new Problem
                    {
                        OUser = UserFind(username),
                        title = Regex.Match(html, "<title>([\\s\\S]*?)</title>", RegexOptions.IgnoreCase).Groups[1].Value.Trim(),
                        description = Regex.Match(html, "<dd id=\"problem-desc\">([\\s\\S]*?)</dd>", RegexOptions.IgnoreCase).Groups[1].Value.Trim(),
                        input = Regex.Match(html, "<dt>Input</dt>\\s*<dd>([\\s\\S]*?)</dd>", RegexOptions.IgnoreCase).Groups[1].Value.Trim(),
                        output = Regex.Match(html, "<dt>Output</dt>\\s*<dd>([\\s\\S]*?)</dd>", RegexOptions.IgnoreCase).Groups[1].Value.Trim(),
                        simpleInput = Regex.Match(html, "<dt>Sample Input</dt>\\s*<dd>\\s*<pre>([\\s\\S]*?)</pre>\\s*</dd>", RegexOptions.IgnoreCase).Groups[1].Value.Trim(),
                        simpleOutput = Regex.Match(html, "<dt>Sample Output</dt>\\s*<dd>\\s*<pre>([\\s\\S]*?)</pre>\\s*</dd>", RegexOptions.IgnoreCase).Groups[1].Value.Trim(),
                        endTime = DateTime.Now
                    };

                    if (problem.title.Length > 0 && problem.description.Length > 0)
                    {
                        db.Problems.Add(problem);
                        db.SaveChanges();
                    }
                }
                
            }
            catch (Exception e)
            {
                Tools.Log.Loger(e.Message);
            }
        }

        private static OJUser UserFind(string userName)
        {
            var user = from m in db.OJUsers where m.userName == userName select m;
            if (user == null || user.ToList().Count == 0)
            {
                return null;
            }
            System.Diagnostics.Debug.WriteLine("fuck");
            return user.ToList()[0];
        }

        protected void Dispose()
        {
            db.Dispose();
        }
    }
}