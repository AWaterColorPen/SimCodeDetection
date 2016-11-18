using SimCodeDetectionWeb.CodeParse;
using SimCodeDetectionWeb.Tools;
using SimCodeDetectionWeb.Models;
using SimCodeDetectionWeb.SimCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.Judge
{
    public class JudgeCodes
    {
        public static List<string> CodeSlicing(string source)
        {
            var codes = Slicer.Slicing(source);
            List<string> ret = new List<string>();
            foreach (var code in codes)
            {
                ret.Add(code.text);
            }
            return ret;
        }

        public static SimResult GetSnippetPair(Submission subs, Submission sub)
        {
            var snippet1 = subs.Snippets;
            var snippet2 = sub.Snippets;

            List<int> index1 = new List<int>();
            List<int> index2 = new List<int>();
            List<double> sim = new List<double>();
            for (var i = 0; i < snippet1.Count; i++)
            {
                KeyValuePair<int, double> tmppair = new KeyValuePair<int, double>(0, 0);
                for (var j = 0; j < snippet2.Count; j++)
                {
                    SimSnippetPair snippetpair = new SimSnippetPair(snippet1[i], snippet2[j]);
                    if (snippetpair.similar > tmppair.Value)
                    {
                        tmppair = new KeyValuePair<int, double>(j, snippetpair.similar);
                    }
                }

                if (tmppair.Value > 0.1)
                {
                    index1.Add(i);
                    index2.Add(tmppair.Key);
                    sim.Add(tmppair.Value);
                }
            }

            if (index1.Count == 0)
            {
                return null;
            }

            var count = index1.Count;
            var simvalue = CalcSimValue(sim, snippet1.Count);
            return new SimResult
            {
                index1 = index1,
                index2 = index2,
                sim = sim,
                simvalue = simvalue,
                count = count,
                otherid = sub.Id
            };
        }

        private static double CalcSimValue(List<double> sim, int count)
        {
            double sum = 0;
            double tmpvalue = 0;
            foreach (var v in sim)
            {
                tmpvalue += v * (v + 1);
                sum += v;
            }
            return tmpvalue / (sum + count);
        }
    }
}