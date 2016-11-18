using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.SimCode
{
    public class LCS
    {
        private List<string> tokens1;
        private List<string> tokens2;
        public double sim;

        public LCS(List<string> tokens1, List<string> tokens2)
        {
            this.tokens1 = tokens1;
            this.tokens2 = tokens2;
            Run();
        }

        private void Run()
        {
            var count1 = tokens1.Count;
            var count2 = tokens2.Count;
            var lcs = Common();
            sim = 2.0 * lcs / (count1 + count2);
        }

        private int Common()
        {
            int n1 = tokens1.Count;
            int n2 = tokens2.Count;
            int[,] f = new int[n1 + 1, n2 + 1];

            f.Initialize();
            for (var i = 0; i <= n1; i++) f[i, 0] = 0;
            for (var j = 0; j <= n2; j++) f[0, j] = 0;

            for (var i = 0; i < n1; i++)
                for (var j = 0; j < n2; j++)
                {
                    if (tokens1[i] == tokens2[j]) f[i + 1, j + 1] = f[i, j] + 1;
                    else f[i + 1, j + 1] = f[i, j];

                    f[i + 1, j + 1] = Math.Max(f[i + 1, j + 1], f[i + 1, j]);
                    f[i + 1, j + 1] = Math.Max(f[i + 1, j + 1], f[i, j + 1]);
                }
            return f[n1, n2];
        }
    }
}