using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class PublicSurfaceAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldPublic = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "public"))
                .Select(m => m.ToString());

            var newPublic = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "public"))
                .Select(m => m.ToString());

            var added = newPublic.Except(oldPublic).ToList();
            var removed = oldPublic.Except(newPublic).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = removed.Any() ? ChangeType.Major : ChangeType.Minor,
                    Summary = $"PublicSurfaceAnalyzer: {added.Count} added, {removed.Count} removed public members."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "PublicSurfaceAnalyzer: No public surface changes detected."
            };
        }
    }
}
