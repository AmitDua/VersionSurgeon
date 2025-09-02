using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class InternalApiAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldInternal = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "internal"))
                .Select(m => m.ToString());

            var newInternal = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "internal"))
                .Select(m => m.ToString());

            var added = newInternal.Except(oldInternal).ToList();
            var removed = oldInternal.Except(newInternal).ToList();

            if (added.Any() || removed.Any())
{
    return new CompatibilityResult
    {
        ChangeType = ChangeType.Patch,
        Summary = $"InternalApiAnalyzer: {added.Count} added, {removed.Count} removed internal APIs."
    };
}

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "InternalApiAnalyzer: No internal API changes detected."
            };
        }
    }
}
