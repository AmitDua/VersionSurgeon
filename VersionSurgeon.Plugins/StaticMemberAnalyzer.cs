using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class StaticMemberAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldStatics = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "static"))
                .Select(m => m.ToString());

            var newStatics = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "static"))
                .Select(m => m.ToString());

            var added = newStatics.Except(oldStatics).ToList();
            var removed = oldStatics.Except(newStatics).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"StaticMemberAnalyzer: {added.Count} added, {removed.Count} removed static members."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "StaticMemberAnalyzer: No static member changes detected."
            };
        }
    }
}
