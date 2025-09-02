using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class AnnotationChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldAnnotations = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<AttributeSyntax>()
                .Select(a => a.ToString());

            var newAnnotations = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<AttributeSyntax>()
                .Select(a => a.ToString());

            var added = newAnnotations.Except(oldAnnotations).ToList();
            var removed = oldAnnotations.Except(newAnnotations).ToList();

            if (added.Any() || removed.Any())
{
    return new CompatibilityResult
    {
        ChangeType = ChangeType.Patch,
        Summary = $"AnnotationChangeAnalyzer: {added.Count} added, {removed.Count} removed annotations."
    };
}

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "AnnotationChangeAnalyzer: No annotation changes detected."
            };
        }
    }
}
