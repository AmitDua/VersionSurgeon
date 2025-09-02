using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class MethodSignatureAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldMethods = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Select(m => $"{m.Identifier.Text}({string.Join(",", m.ParameterList.Parameters.Select(p => p.Type?.ToString()))})");

            var newMethods = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Select(m => $"{m.Identifier.Text}({string.Join(",", m.ParameterList.Parameters.Select(p => p.Type?.ToString()))})");

            var added = newMethods.Except(oldMethods).ToList();
            var removed = oldMethods.Except(newMethods).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = removed.Any() ? ChangeType.Major : ChangeType.Minor,
                    Summary = $"MethodSignatureAnalyzer: {added.Count} added, {removed.Count} removed or changed method signatures."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "MethodSignatureAnalyzer: No method signature changes detected."
            };
        }
    }
}
