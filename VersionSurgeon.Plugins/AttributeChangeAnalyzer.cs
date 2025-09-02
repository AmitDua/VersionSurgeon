using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class AttributeChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldAttrs = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<AttributeSyntax>()
                .Select(a => a.ToString());

            var newAttrs = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<AttributeSyntax>()
                .Select(a => a.ToString());

            var added = newAttrs.Except(oldAttrs).ToList();
            var removed = oldAttrs.Except(newAttrs).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"AttributeChangeAnalyzer: {added.Count} added, {removed.Count} removed attributes."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "AttributeChangeAnalyzer: No attribute changes detected."
            };
        }
    }
}
