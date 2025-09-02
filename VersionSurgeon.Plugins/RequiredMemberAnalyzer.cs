using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class RequiredMemberAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            bool IsRequired(PropertyDeclarationSyntax p) =>
                p.Modifiers.Any(m => m.Text == "required");

            var oldRequired = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<PropertyDeclarationSyntax>()
                .Where(IsRequired)
                .Select(p => p.Identifier.Text);

            var newRequired = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<PropertyDeclarationSyntax>()
                .Where(IsRequired)
                .Select(p => p.Identifier.Text);

            var added = newRequired.Except(oldRequired).ToList();
            var removed = oldRequired.Except(newRequired).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"RequiredMemberAnalyzer: {added.Count} added, {removed.Count} removed required members."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "RequiredMemberAnalyzer: No required member changes detected."
            };
        }
    }
}
