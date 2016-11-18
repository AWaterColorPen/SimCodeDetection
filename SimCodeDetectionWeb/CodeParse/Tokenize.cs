using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.CodeParse
{
    public class Tokenize
    {
        public static List<string> GetAllTokens(string source)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            var root = tree.GetRoot();
            var tokens = root.DescendantTokens();

            List<string> tokenlist = new List<string>();
            foreach (var token in tokens)
            {
                tokenlist.Add(token.ValueText);
            }
            return tokenlist;
        }

        public static List<string> GetKeyTokens(List<string> alltokens)
        {
            List<string> tokenlist = new List<string>();
            foreach (var token in alltokens)
            {
                if (IsOnlyContainsLetterOrDight(token) == false) continue;
                tokenlist.Add(token);
            }
            return tokenlist;
        }

        public static List<string> GetKeyTokens(string source)
        {
            List<string> alltokens = GetAllTokens(source);
            return GetKeyTokens(alltokens);
        }

        private static bool IsOnlyContainsLetterOrDight(string word)
        {
            bool HasLetter = false;
            foreach (var achar in word)
            {
                if (Char.IsLetterOrDigit(achar) == false) return false;
                if (Char.IsLetter(achar)) HasLetter = true;
            }
            return HasLetter;
        }
    }
}