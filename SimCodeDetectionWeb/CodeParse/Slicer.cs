using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Runtime;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace SimCodeDetectionWeb.CodeParse
{
    public class Slicer
    {
        public static List<Snippets> Slicing(string source)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            List<Snippets> snippets = new List<Snippets>();
            snippets.AddRange(GetMethodSnippets(root));

            foreach (var snippet in snippets)
            {
                snippet.SnippetsMaker();
            }

            for (var i = 0; i < snippets.Count; i++)
            {
                if (snippets[i].status == SnippetStatus.CE || snippets[i].status == SnippetStatus.SHORT)
                {
                    snippets.Remove(snippets[i]);
                    i--;
                }
            }

            return snippets;
        }

        public static List<Snippets> RoslynTest(string source)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(source);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            Debug.WriteLine("root.Span.Start = {0}", root.Span.Start);
            Debug.WriteLine("root.Span.End = {0}", root.Span.End);
            
            var methodsnippet = GetMethodSnippets(root);
            var classsnippet = GetClassSnippets(root);
            var structsnippet = GetStructSnippets(root);

            List<Snippets> snippets = new List<Snippets>();
            snippets.AddRange(methodsnippet);
            snippets.AddRange(classsnippet);
            snippets.AddRange(structsnippet);

            foreach (var snippet in snippets)
            {
                snippet.SnippetsMaker();
            }

            for (var i = 0; i < snippets.Count; i++)
            {
                if (snippets[i].status == SnippetStatus.CE)
                {
                    snippets.Remove(snippets[i]);
                    i--;
                }
            }
            snippets.Sort(SnippetsCompare);

            var othersnippet = GetOtherSnippets(source, snippets);
            snippets.AddRange(othersnippet);

            snippets.Sort(SnippetsCompare);
            return snippets;
        }

        private static int SnippetsCompare(Snippets A, Snippets B)
        {
            return A.spanstart - B.spanstart;
        }

        private static IEnumerable<Snippets> GetOtherSnippets(string source, List<Snippets> snippets)
        {
            var last = 0;
            List<Snippets> othersnippet = new List<Snippets>();
            foreach (var snippet in snippets)
            {
                if (last < snippet.spanstart)
                {
                    var text = source.Substring(last, snippet.spanstart - last);
                    othersnippet.Add(new Snippets(text, last, snippet.spanstart, SnippetStatus.OTHER));
                }
                last = snippet.spanend;
            }

            if (last < source.Length)
            {
                var text = source.Substring(last, source.Length - last);
                othersnippet.Add(new Snippets(text, last, source.Length, SnippetStatus.OTHER));
            }
            return othersnippet;
        }

        private static IEnumerable<Snippets> GetStructSnippets(CompilationUnitSyntax root)
        {
            List<Snippets> structsnippet = new List<Snippets>();
            foreach (var member in root.DescendantNodes().OfType<StructDeclarationSyntax>())
            {
                var snippet = member.Members.OfType<MethodDeclarationSyntax>();
                if (snippet.Count() != 0) continue;
                structsnippet.Add(new Snippets(member.GetText().ToString(), member.Span.Start, member.Span.End, SnippetStatus.STRUCT));
            }
            return structsnippet;
        }

        private static IEnumerable<Snippets> GetClassSnippets(CompilationUnitSyntax root)
        {
            List<Snippets> classsnippet = new List<Snippets>();
            foreach (var member in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var snippet = member.Members.OfType<MethodDeclarationSyntax>();
                if (snippet.Count() != 0) continue;
                classsnippet.Add(new Snippets(member.GetText().ToString(), member.Span.Start, member.Span.End, SnippetStatus.CLASS));
            }
            return classsnippet;
        }

        private static IEnumerable<Snippets> GetMethodSnippets(CompilationUnitSyntax root)
        {
            List<Snippets> methodsnippet = new List<Snippets>();
            foreach (var member in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                Debug.WriteLine("member GetText = {0}", member.GetText());
                Debug.WriteLine("member SourceSpan.Start = {0}", member.Span.Start);
                Debug.WriteLine("member SourceSpan.End = {0}", member.Span.End);
                methodsnippet.Add(new Snippets(member.GetText().ToString(), member.Span.Start, member.Span.End, SnippetStatus.METHOD));
            }
            return methodsnippet;
        }
    }
}