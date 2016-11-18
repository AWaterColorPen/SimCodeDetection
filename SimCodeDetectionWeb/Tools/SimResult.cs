using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.Tools
{
    public class SimResult
    {
        public int count { get; set; }
        public double simvalue { get; set; }
        public List<double> sim { get; set; }
        public List<int> index1 { get; set; }
        public List<int> index2 { get; set; }
        public int otherid { get; set; }
    }
}