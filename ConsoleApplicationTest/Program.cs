using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Parser.Html;
using System.IO;
using System.Web;
using System.Threading;

namespace ConsoleApplicationTest
{
    class Program
    {
        static int testpnum = 10; 
        static void Main(string[] args)
        {
            var list = Tool.CF.HuntForSubs();
            Dictionary<string, List<string>> dc = new Dictionary<string, List<string>>();
            foreach (var ls in list)
            {
                if (dc.ContainsKey(ls.Value) == false)
                {
                    dc.Add(ls.Value, new List<string>());
                }
                dc[ls.Value].Add(ls.Key);
            }
            
            foreach (var dd in dc)
            {
                if (dd.Value.Count >= 10)
                    Console.WriteLine(dd.Value.Count);
            }

            int pid = 0;
            foreach (var dd in dc)
            {
                if (dd.Value.Count >= 10)
                {
                    int cnt = 0;
                    foreach (var ll in dd.Value)
                    {
                        Tool.SendToTestAPI stta = new Tool.SendToTestAPI(pid % testpnum);
                        string source = Tool.CF.HuntForSource(dd.Key, ll);
                        if (source == null) continue;
                        stta.Send(source);
                        Thread.Sleep(1005);
                        if (cnt++ > 50) break;
                    }
                    pid++;
                }
            }
            while (Console.Read() != 0)
            {
            }
        }
    }
}
