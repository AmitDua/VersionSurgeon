using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class AbstractMemberAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldAbstracts = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "abstract"))
                .Select(m => m.ToString());

            var newAbstracts = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "abstract"))
                .Select(m => m.ToString());

            var added = newAbstracts.Except(oldAbstracts).ToList();
            var removed = oldAbstracts.Except(newAbstracts).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = $"AbstractMemberAnalyzer: {added.Count} added, {removed.Count} removed abstract members."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "AbstractMemberAnalyzer: No abstract member changes detected."
            };
        }
    }
}
