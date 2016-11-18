using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.Tools
{
    public class Log
    {
        public static void Loger(string message)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Log");

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, DateTime.Now.ToString("yyyy-MM-dd") + ".log");

            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(DateTime.Now);
            sw.WriteLine(message);
            sw.WriteLine();
            sw.Dispose();
        }
    }
}