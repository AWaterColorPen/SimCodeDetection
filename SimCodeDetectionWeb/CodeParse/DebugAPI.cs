using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimCodeDetectionWeb.CodeParse
{
    public class DebugAPI
    {
        public string source { get; set; }
        public string results { get; set; }
        public List<string> alltokens { get; set; }
        public List<string> keytokens { get; set; }

        public DebugAPI(string source)
        {
            this.source = source;
            this.results = "";
            Maker();

        }

        public void Maker()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            Walker(root, 0);
            alltokens = Tokenize.GetAllTokens(source);
            keytokens = Tokenize.GetKeyTokens(alltokens);
        }

        private void Walker(SyntaxNode root, int depth)
        {
            var result = String.Format("{0} member Kind = {1}\n", new string(' ', depth * 4), root.CSharpKind());
            result += String.Format("{0} member SourceSpan.Start = {1}\n", new string(' ', depth * 4), root.Span.Start);
            result += String.Format("{0} member SourceSpan.End = {1}\n", new string(' ', depth * 4), root.Span.End);
            result += String.Format("{0} member FirstToken.ValueText = {1}\n", new string(' ', depth * 4), root.GetFirstToken().ValueText);
            result += String.Format("{0} member LastToken.ValueText = {1}\n", new string(' ', depth * 4), root.GetLastToken().ValueText);
            results += result + "\n";
            foreach (var member in root.ChildNodes())
            {
                Walker(member, depth + 1);
            }
        }
    }
}