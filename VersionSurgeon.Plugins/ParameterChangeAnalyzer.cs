using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class ParameterChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldParams = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Select(m => $"{m.Identifier.Text}:{string.Join(",", m.ParameterList.Parameters.Select(p => p.Type?.ToString()))}");

            var newParams = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Select(m => $"{m.Identifier.Text}:{string.Join(",", m.ParameterList.Parameters.Select(p => p.Type?.ToString()))}");

            var changed = newParams.Except(oldParams).ToList();

            if (changed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = $"ParameterChangeAnalyzer: {changed.Count} parameter list changes detected."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "ParameterChangeAnalyzer: No parameter changes detected."
            };
        }
    }
}
