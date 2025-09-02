using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class DynamicUsageAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldDynamic = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<VariableDeclarationSyntax>()
                .Where(v => v.Type.ToString() == "dynamic")
                .Select(v => v.ToString());

            var newDynamic = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<VariableDeclarationSyntax>()
                .Where(v => v.Type.ToString() == "dynamic")
                .Select(v => v.ToString());

            var added = newDynamic.Except(oldDynamic).ToList();
            var removed = oldDynamic.Except(newDynamic).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"DynamicUsageAnalyzer: {added.Count} added, {removed.Count} removed dynamic usages."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "DynamicUsageAnalyzer: No dynamic usage changes detected."
            };
        }
    }
}
