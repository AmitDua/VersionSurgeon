using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class NullableReferenceAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            bool IsNullable(TypeSyntax type) => type.ToString().EndsWith("?");

            var oldNullable = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<VariableDeclarationSyntax>()
                .Where(v => IsNullable(v.Type))
                .Select(v => v.ToString());

            var newNullable = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<VariableDeclarationSyntax>()
                .Where(v => IsNullable(v.Type))
                .Select(v => v.ToString());

            var added = newNullable.Except(oldNullable).ToList();
            var removed = oldNullable.Except(newNullable).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"NullableReferenceAnalyzer: {added.Count} added, {removed.Count} removed nullable references."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "NullableReferenceAnalyzer: No nullable reference changes detected."
            };
        }
    }
}
