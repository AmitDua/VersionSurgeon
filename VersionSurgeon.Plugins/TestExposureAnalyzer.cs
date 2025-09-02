using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class TestExposureAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Utility;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            bool IsTestAttribute(AttributeSyntax attr) =>
                attr.ToString().Contains("Test") || attr.ToString().Contains("Fact") || attr.ToString().Contains("Theory");

            var oldTests = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<AttributeSyntax>()
                .Where(IsTestAttribute)
                .Select(a => a.ToString());

            var newTests = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<AttributeSyntax>()
                .Where(IsTestAttribute)
                .Select(a => a.ToString());

            var added = newTests.Except(oldTests).ToList();
            var removed = oldTests.Except(newTests).ToList();

            if (added.Any() || removed.Any())
{
    return new CompatibilityResult
    {
        ChangeType = ChangeType.Patch,
        Summary = $"TestExposureAnalyzer: {added.Count} added, {removed.Count} removed test annotations."
    };
}

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "TestExposureAnalyzer: No test exposure changes detected."
            };
        }
    }
}
