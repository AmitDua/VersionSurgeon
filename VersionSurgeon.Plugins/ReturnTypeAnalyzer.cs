using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class ReturnTypeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldReturns = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Select(m => $"{m.Identifier.Text}:{m.ReturnType}");

            var newReturns = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Select(m => $"{m.Identifier.Text}:{m.ReturnType}");

            var changed = newReturns.Except(oldReturns).ToList();

            if (changed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = $"ReturnTypeAnalyzer: {changed.Count} return type changes detected."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "ReturnTypeAnalyzer: No return type changes detected."
            };
        }
    }
}
