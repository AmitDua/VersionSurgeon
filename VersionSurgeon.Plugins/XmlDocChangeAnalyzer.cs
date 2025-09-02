using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class XmlDocChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldDocs = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                .Select(t => t.ToString());

            var newDocs = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                .Select(t => t.ToString());

            var added = newDocs.Except(oldDocs).ToList();
            var removed = oldDocs.Except(newDocs).ToList();

            if (added.Any() || removed.Any())
{
    return new CompatibilityResult
    {
        ChangeType = ChangeType.Patch,
        Summary = $"XmlDocChangeAnalyzer: {added.Count} added, {removed.Count} removed XML doc comments."
    };
}

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "XmlDocChangeAnalyzer: No XML documentation changes detected."
            };
        }
    }
}
