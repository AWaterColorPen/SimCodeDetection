using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using System.Net.Http;

namespace ConsoleApplicationTest.Tool
{
    class SendToTestAPI
    {
        string url;
        string id;
        string name;
        public SendToTestAPI()
        {
            url = "http://211.69.197.56:8080/Problem/TestSubmitTry";
        }

        public SendToTestAPI(int pid)
        {
            url = "http://211.69.197.56:8080/Problem/TestSubmitTry";
            id = (pid + 1).ToString();
            name = RandomName();
        }

        string RandomName()
        {
            string ss = "Student ";
            Random random = new Random();
            int num = random.Next(1, 30);
            return ss + num; 
        }

        public int Send(string source)
        {
            //var newurl = String.Format("{0}?id={1}&_source={2}&name={3}", url, id, source, name);
            var newurl = url;
            using (var client = new System.Net.Http.HttpClient())
            {
                Uri uri = new Uri(newurl);
                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
                paramList.Add(new KeyValuePair<string, string>("pid", id));
                paramList.Add(new KeyValuePair<string, string>("_source", source));
                paramList.Add(new KeyValuePair<string, string>("name", name));
                var response = client.PostAsync(uri, new FormUrlEncodedContent(paramList)).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(newurl);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(result);
            }
            return 0;
        }
    }
}
