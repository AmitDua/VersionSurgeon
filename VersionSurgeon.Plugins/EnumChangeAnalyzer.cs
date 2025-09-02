using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class EnumChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldEnums = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<EnumDeclarationSyntax>();

            var newEnums = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<EnumDeclarationSyntax>();

            var changes = newEnums.SelectMany(newEnum =>
            {
                var oldEnum = oldEnums.FirstOrDefault(e => e.Identifier.Text == newEnum.Identifier.Text);
                if (oldEnum == null) return new[] { $"Enum {newEnum.Identifier.Text} added." };

                var oldMembers = oldEnum.Members.Select(m => m.Identifier.Text).ToHashSet();
                var newMembers = newEnum.Members.Select(m => m.Identifier.Text).ToHashSet();

                var added = newMembers.Except(oldMembers).Select(m => $"Enum {newEnum.Identifier.Text} added member: {m}");
                var removed = oldMembers.Except(newMembers).Select(m => $"Enum {newEnum.Identifier.Text} removed member: {m}");

                return added.Concat(removed);
            }).ToList();

            if (changes.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = $"EnumChangeAnalyzer: {changes.Count} enum member changes detected."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "EnumChangeAnalyzer: No enum changes detected."
            };
        }
    }
}
