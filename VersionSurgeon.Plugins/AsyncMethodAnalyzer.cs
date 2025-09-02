using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class AsyncMethodAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldAsync = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "async"))
                .Select(m => m.Identifier.Text);

            var newAsync = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "async"))
                .Select(m => m.Identifier.Text);

            var added = newAsync.Except(oldAsync).ToList();
            var removed = oldAsync.Except(newAsync).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"AsyncMethodAnalyzer: {added.Count} methods became async, {removed.Count} lost async."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "AsyncMethodAnalyzer: No async method changes detected."
            };
        }
    }
}
