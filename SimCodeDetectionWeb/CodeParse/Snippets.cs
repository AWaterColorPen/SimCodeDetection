using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.CodeParse
{
    public class Snippets
    {
        public string text { get; set; }
        public List<string> alltokens { get; set; }
        public List<string> keytokens { get; set; }
        public int spanstart { get; set; }
        public int spanend { get; set; }
        public int lines { get; set; }
        public SnippetStatus status { get; set; }

        public Snippets(string text, int start, int end, SnippetStatus status)
        {
            this.text = text;
            this.spanstart = start;
            this.spanend = end;
            this.status = status;
            this.lines = GetSourceTextLine();
        }

        public void CsharpSnippetsMaker()
        {

        }

        public void CplusplusSnippetsMaker()
        {
            alltokens = Tokenize.GetAllTokens(text);
            keytokens = Tokenize.GetKeyTokens(alltokens);
            if (CheckBraceMatch(alltokens) == false  || lines < 2)
            {
                status = SnippetStatus.CE;
                return;
            }
            if (lines < 5)
            {
                status = SnippetStatus.SHORT;
            }
        }

        public void SnippetsMaker()
        {
            alltokens = Tokenize.GetAllTokens(text);
            keytokens = Tokenize.GetKeyTokens(alltokens);
            if (CheckBraceMatch(alltokens) == false || lines < 2)
            {
                status = SnippetStatus.CE;
                return;
            }
            if (lines < 5)
            {
                status = SnippetStatus.SHORT;
            }
        }

        private int GetSourceTextLine()
        {
            var lines = text.Split(new char[] { '\n' });
            var cnt = 0;
            foreach (var line in lines)
            {
                if (line != null && line.Length != 0)
                    cnt++;
            }
            return cnt;
        }

        private bool CheckBraceMatch(List<string> alltokens)
        {
            bool hasbrace = false;
            var cnt = 0;
            foreach (var token in alltokens)
            {
                if (token.Contains("{")) cnt++;
                if (token.Contains("}")) cnt--;
                if (cnt < 0) return false;
                if (token.Contains("{") || token.Contains("}")) hasbrace = true;
            }
            return cnt == 0 && hasbrace;
        }
    }

    public enum SnippetStatus
    {
        CE,
        UE,
        CLASS,
        STRUCT,
        METHOD,
        SHORT,
        OTHER
    }
}