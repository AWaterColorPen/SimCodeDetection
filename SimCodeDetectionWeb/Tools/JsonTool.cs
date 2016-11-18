using SimCodeDetectionWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace SimCodeDetectionWeb.Tools
{
    public class JsonTool
    {
        public static string ListString2Json(List<string> data)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(data);
            System.Diagnostics.Debug.WriteLine(json);
            return json;
        }

        public static string ListInt2Json(List<int> data)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(data);
            System.Diagnostics.Debug.WriteLine(json);
            return json;
        }

        public static List<string> Json2ListString(string json)
        {
            if (json == null)
            {
                return null;
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<string> data = js.Deserialize<List<string>>(json);
            return data;
        }

        public static List<int> Json2ListInt(string json)
        {
            if (json == null)
            {
                return null;
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<int> data = js.Deserialize<List<int>>(json);
            return data;
        }

        public static List<SimResult> Json2ListSimResult(string json)
        {
            if (json == null)
            {
                return null;
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<SimResult> data = js.Deserialize<List<SimResult>>(json);
            return data;
        }

        public static string ListSimResult2Json(List<SimResult> data)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(data);
            System.Diagnostics.Debug.WriteLine(json);
            return json;
        }
    }
}