using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.SimCode
{
    public class Levenshtein
    {
        private List<string> tokens1;
        private List<string> tokens2;
        public double sim;

        public Levenshtein(List<string> tokens1, List<string> tokens2)
        {
            this.tokens1 = tokens1;
            this.tokens2 = tokens2;
            Run();
        }

        private void Run()
        {
            var count1 = tokens1.Count;
            var count2 = tokens2.Count;
            var difference = Distance();
            sim = 1.0 - 2.0 * difference / (count1 + count2);
        }

        private int Distance()
        {
            int n1 = tokens1.Count;
            int n2 = tokens2.Count;
            int[,] f = new int[n1 + 1, n2 + 1];

            f.Initialize();
            for (var i = 0; i <= n1; i++) f[i, 0] = i;
            for (var j = 0; j <= n2; j++) f[0, j] = j;

            for (var i = 0; i < n1; i++)
                for (var j = 0; j < n2; j++)
                {
                    if (tokens1[i] == tokens2[j]) f[i + 1, j + 1] = f[i, j];
                    else f[i + 1, j + 1] = f[i, j] + 1;

                    f[i + 1, j + 1] = Math.Min(f[i + 1, j + 1], f[i + 1, j] + 1);
                    f[i + 1, j + 1] = Math.Min(f[i + 1, j + 1], f[i, j + 1] + 1);
                }
            return f[n1, n2];
        }
    }
}