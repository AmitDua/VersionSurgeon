using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class ContractAnnotationAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            bool IsContractAttribute(AttributeSyntax attr) =>
                attr.ToString().Contains("Contract") || attr.ToString().Contains("Ensures") || attr.ToString().Contains("Requires");

            var oldContracts = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<AttributeSyntax>()
                .Where(IsContractAttribute)
                .Select(a => a.ToString());

            var newContracts = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<AttributeSyntax>()
                .Where(IsContractAttribute)
                .Select(a => a.ToString());

            var added = newContracts.Except(oldContracts).ToList();
            var removed = oldContracts.Except(newContracts).ToList();

           if (added.Any() || removed.Any())
{
    return new CompatibilityResult
    {
        ChangeType = ChangeType.Patch,
        Summary = $"ContractAnnotationAnalyzer: {added.Count} added, {removed.Count} removed contract annotations."
    };
}

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "ContractAnnotationAnalyzer: No contract annotation changes detected."
            };
        }
    }
}
