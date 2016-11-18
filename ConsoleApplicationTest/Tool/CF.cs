using AngleSharp.Parser.Html;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace ConsoleApplicationTest.Tool
{
    class CF
    {
        public static List<KeyValuePair<string, string>> HuntForSubs()
        {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();
            using (var client = new System.Net.Http.HttpClient())
            {
                Uri url = new Uri("http://codeforces.com/api/problemset.recentStatus?count=1000");
                var response = client.GetAsync(url).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(result);
                if (json.Property("status").Value.ToString() == "OK")
                {
                    var datas = json.Property("result").Value.ToArray();
                    foreach (var data in datas)
                    {
                        JObject js = JObject.Parse(data.ToString());
                        string x = js.Property("id").Value.ToString();
                        string y = js.Property("contestId").Value.ToString();
                        Console.WriteLine(x + " " + y);
                        ret.Add(new KeyValuePair<string, string>(x, y));
                    }
                }
            }
            return ret;
        }

        public static string HuntForSource(string cid, string sid)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                Uri url = new Uri("http://codeforces.com/contest/" + cid +"/submission/" + sid);
                var response = client.GetAsync(url).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                Console.WriteLine(response.StatusCode);
                if (response.StatusCode.ToString() != "OK")
                {
                    return null;
                }

                var doc = new HtmlParser(result).Parse();
                var pre = doc.QuerySelector("pre");
                if (pre == null) return null;
                return HttpUtility.HtmlDecode(pre.InnerHtml);
            }
        }
    }
}
