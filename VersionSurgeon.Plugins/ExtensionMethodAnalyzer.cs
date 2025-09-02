using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class ExtensionMethodAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            bool IsExtension(MethodDeclarationSyntax m) =>
                m.ParameterList.Parameters.Count > 0 &&
                m.ParameterList.Parameters[0].Modifiers.Any(mod => mod.Text == "this");

            var oldExtensions = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(IsExtension)
                .Select(m => m.Identifier.Text);

            var newExtensions = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(IsExtension)
                .Select(m => m.Identifier.Text);

            var added = newExtensions.Except(oldExtensions).ToList();
            var removed = oldExtensions.Except(newExtensions).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"ExtensionMethodAnalyzer: {added.Count} added, {removed.Count} removed extension methods."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "ExtensionMethodAnalyzer: No extension method changes detected."
            };
        }
    }
}
