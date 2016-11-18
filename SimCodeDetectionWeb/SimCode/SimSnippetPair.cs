using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.SimCode
{
    public class SimSnippetPair
    {
        private List<string> alltokens1 { get; set; }
        private List<string> keytokens1 { get; set; }
        private List<string> alltokens2 { get; set; }
        private List<string> keytokens2 { get; set; }
        public double similar { get; set; }

        public SimSnippetPair(string source1, string source2)
        {
            alltokens1 = CodeParse.Tokenize.GetAllTokens(source1);
            keytokens1 = CodeParse.Tokenize.GetKeyTokens(source1);
            alltokens2 = CodeParse.Tokenize.GetAllTokens(source2);
            keytokens2 = CodeParse.Tokenize.GetKeyTokens(source2);
            Judging();
        }

        private void Judging()
        {
            SimHash simhash = new SimHash(keytokens1, keytokens2);
            SimCos simcos = new SimCos(keytokens1, keytokens2);
            Levenshtein levenshtein = new Levenshtein(alltokens1, alltokens2);
            LCS lcs = new LCS(alltokens1, alltokens2);

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