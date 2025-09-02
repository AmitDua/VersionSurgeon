using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class ConstructorChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldCtors = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<ConstructorDeclarationSyntax>()
                .Select(c => $"{c.Identifier.Text}({string.Join(",", c.ParameterList.Parameters.Select(p => p.Type?.ToString()))})");

            var newCtors = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<ConstructorDeclarationSyntax>()
                .Select(c => $"{c.Identifier.Text}({string.Join(",", c.ParameterList.Parameters.Select(p => p.Type?.ToString()))})");

            var added = newCtors.Except(oldCtors).ToList();
            var removed = oldCtors.Except(newCtors).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = removed.Any() ? ChangeType.Major : ChangeType.Minor,
                    Summary = $"ConstructorChangeAnalyzer: {added.Count} added, {removed.Count} removed constructors."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "ConstructorChangeAnalyzer: No constructor changes detected."
            };
        }
    }
}
