using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class FieldChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldFields = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<FieldDeclarationSyntax>()
                .Select(f => f.ToString());

            var newFields = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<FieldDeclarationSyntax>()
                .Select(f => f.ToString());

            var added = newFields.Except(oldFields).ToList();
            var removed = oldFields.Except(newFields).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = removed.Any() ? ChangeType.Major : ChangeType.Minor,
                    Summary = $"FieldChangeAnalyzer: {added.Count} added, {removed.Count} removed fields."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "FieldChangeAnalyzer: No field changes detected."
            };
        }
    }
}
