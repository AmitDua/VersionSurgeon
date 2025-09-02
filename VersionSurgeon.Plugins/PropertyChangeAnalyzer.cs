using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class PropertyChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldProps = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<PropertyDeclarationSyntax>()
                .Select(p => p.ToString());

            var newProps = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<PropertyDeclarationSyntax>()
                .Select(p => p.ToString());

            var added = newProps.Except(oldProps).ToList();
            var removed = oldProps.Except(newProps).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = removed.Any() ? ChangeType.Major : ChangeType.Minor,
                    Summary = $"PropertyChangeAnalyzer: {added.Count} added, {removed.Count} removed properties."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "PropertyChangeAnalyzer: No property changes detected."
            };
        }
    }
}
