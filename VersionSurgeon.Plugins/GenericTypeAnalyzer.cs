using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class GenericTypeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldGenerics = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<TypeDeclarationSyntax>()
                .Where(t => t.TypeParameterList != null)
                .Select(t => t.Identifier.Text);

            var newGenerics = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<TypeDeclarationSyntax>()
                .Where(t => t.TypeParameterList != null)
                .Select(t => t.Identifier.Text);

            var added = newGenerics.Except(oldGenerics).ToList();
            var removed = oldGenerics.Except(newGenerics).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = $"GenericTypeAnalyzer: {added.Count} added, {removed.Count} removed generic types."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "GenericTypeAnalyzer: No generic type changes detected."
            };
        }
    }
}
