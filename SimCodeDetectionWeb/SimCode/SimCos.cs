using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.SimCode
{
    public class SimCos
    {
        private List<string> tokens1;
        private List<string> tokens2;
        private Dictionary<string, int> dict;
        private int[] vector1;
        private int[] vector2;
        private int count;
        public double sim;

        public SimCos(List<string> tokens1, List<string> tokens2)
        {
            this.tokens1 = tokens1;
            this.tokens2 = tokens2;
            dict = new Dictionary<string, int>();
            Run();
        }

        private void Run()
        {
            foreach (var token in this.tokens1)
            {
                if (dict.ContainsKey(token) == false)
                    dict.Add(token, count++);
            }

            foreach (var token in this.tokens2)
            {
                if (dict.ContainsKey(token) == false)
                    dict.Add(token, count++);
            }

            vector1 = new int[count];
            vector2 = new int[count];

            foreach (var token in this.tokens1)
            {
                vector1[dict[token]]++;
            }

            foreach (var token in this.tokens2)
            {
                vector2[dict[token]]++;
            }

            var cosvalue = Cos();
            sim = 1.0 - Math.Acos(cosvalue) * 2 / Math.PI;
        }

        private double Cos()
        {
            double numerator = 0;
            double tmp1 = 0;
            double tmp2 = 0;

            for (var i = 0; i < count; i++)
            {
                numerator += vector1[i] * vector2[i];
                tmp1 += vector1[i] * vector1[i];
                tmp2 += vector2[i] * vector2[i];
            }

            double denominator = Math.Sqrt(tmp1) * Math.Sqrt(tmp2);
            return numerator / denominator;
        }
    }
}