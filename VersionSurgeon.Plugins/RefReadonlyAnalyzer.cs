using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class RefReadonlyAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            bool IsRefReadonly(ParameterSyntax p) =>
                p.Modifiers.Any(m => m.Text == "ref") && p.Modifiers.Any(m => m.Text == "readonly");

            var oldParams = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<ParameterSyntax>()
                .Where(IsRefReadonly)
                .Select(p => p.ToString());

            var newParams = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<ParameterSyntax>()
                .Where(IsRefReadonly)
                .Select(p => p.ToString());

            var added = newParams.Except(oldParams).ToList();
            var removed = oldParams.Except(newParams).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"RefReadonlyAnalyzer: {added.Count} added, {removed.Count} removed ref readonly parameters."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "RefReadonlyAnalyzer: No ref readonly parameter changes detected."
            };
        }
    }
}
