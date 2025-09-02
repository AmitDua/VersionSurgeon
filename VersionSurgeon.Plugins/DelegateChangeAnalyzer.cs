using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class DelegateChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldDelegates = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<DelegateDeclarationSyntax>()
                .Select(d => d.ToString());

            var newDelegates = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<DelegateDeclarationSyntax>()
                .Select(d => d.ToString());

            var added = newDelegates.Except(oldDelegates).ToList();
            var removed = oldDelegates.Except(newDelegates).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = removed.Any() ? ChangeType.Major : ChangeType.Minor,
                    Summary = $"DelegateChangeAnalyzer: {added.Count} added, {removed.Count} removed delegates."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "DelegateChangeAnalyzer: No delegate changes detected."
            };
        }
    }
}
