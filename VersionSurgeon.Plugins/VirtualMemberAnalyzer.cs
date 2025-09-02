using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class VirtualMemberAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldVirtuals = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "virtual"))
                .Select(m => m.ToString());

            var newVirtuals = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<MemberDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "virtual"))
                .Select(m => m.ToString());

            var added = newVirtuals.Except(oldVirtuals).ToList();
            var removed = oldVirtuals.Except(newVirtuals).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"VirtualMemberAnalyzer: {added.Count} added, {removed.Count} removed virtual members."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "VirtualMemberAnalyzer: No virtual member changes detected."
            };
        }
    }
}
