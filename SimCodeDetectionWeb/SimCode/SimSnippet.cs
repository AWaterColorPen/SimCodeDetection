using SimCodeDetectionWeb.CodeParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.SimCode
{
    public class SimSnippet
    {
        public Snippets snippet1;
        public Snippets snippet2;
        public double similar { get; set; }

        public SimSnippet(Snippets snippet1, Snippets snippet2)
        {
            this.snippet1 = snippet1;
            this.snippet2 = snippet2;
            Judging();
        }

        private void Judging()
        {
            SimHash simhash = new SimHash(snippet1.keytokens, snippet2.keytokens);
            SimCos simcos = new SimCos(snippet1.keytokens, snippet2.keytokens);
            Levenshtein levenshtein = new Levenshtein(snippet1.alltokens, snippet2.alltokens);
            LCS lcs = new LCS(snippet1.alltokens, snippet2.alltokens);

            similar = IsSimrlar(simhash.IsSimilar(), simcos.sim, levenshtein.sim, lcs.sim);
        }

        private double IsSimrlar(bool p1, double p2, double p3, double p4)
        {
            System.Diagnostics.Debug.WriteLine("{0} {1} {2} {3}", p1, p2, p3, p4);
            if (p1 == false)
            {
                if (p2 < 0.5 || p3 < 0.5 || p4 < 0.5)
                    return 0;
            }

            if (p2 < 0.8 && p3 < 0.8 && p4 < 0.8)
                return 0;

            return (p2 + p3 + p4) / 3;
        }
    }
}