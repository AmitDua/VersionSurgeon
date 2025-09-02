using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class SealedClassAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldSealed = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(c => c.Modifiers.Any(m => m.Text == "sealed"))
                .Select(c => c.Identifier.Text);

            var newSealed = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(c => c.Modifiers.Any(m => m.Text == "sealed"))
                .Select(c => c.Identifier.Text);

            var added = newSealed.Except(oldSealed).ToList();
            var removed = oldSealed.Except(newSealed).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = $"SealedClassAnalyzer: {added.Count} classes newly sealed, {removed.Count} unsealed."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "SealedClassAnalyzer: No sealed class changes detected."
            };
        }
    }
}
