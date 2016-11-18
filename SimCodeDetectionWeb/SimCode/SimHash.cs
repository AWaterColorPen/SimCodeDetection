using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.SimCode
{
    public class SimHash
    {
        private int hashbits = 32;
        private List<string> tokens1;
        private List<string> tokens2;

        public SimHash(List<string> tokens1, List<string> tokens2)
        {
            this.tokens1 = tokens1;
            this.tokens2 = tokens2;
        }

        public int HammingDistance(int simhashvalue1, int simhashvalue2)
        {
            int x = simhashvalue1 ^ simhashvalue2;
            int tot = 0;
            while (x != 0)
            {
                tot++;
                x = (x - 1) & x;
            }
            return tot;
        }

        public int CalcSimHash(List<string> tokens) 
        {
            int[] value = new int[hashbits];
            foreach (var token in tokens)
            {
                int hashvalue = token.GetHashCode();
                for (var i = 0; i < hashbits; i++) 
                {
                    if ((hashvalue & (1 << i)) != 0) value[i]++;
                    else value[i] --;
                }
            }
            
            int fingerprint = 0;
            for (var i = 0; i < hashbits; i++) 
            {
                if (value[i] >= 0) fingerprint |= 1 << i;
            }
            return fingerprint;
        }

        public bool IsSimilar()
        {
            int hash1 = CalcSimHash(tokens1);
            int hash2 = CalcSimHash(tokens2);
            return HammingDistance(hash1, hash2) < 3;
        }
    
    }
}