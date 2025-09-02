using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class RecordTypeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldRecords = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<RecordDeclarationSyntax>()
                .Select(r => r.Identifier.Text);

            var newRecords = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<RecordDeclarationSyntax>()
                .Select(r => r.Identifier.Text);

            var added = newRecords.Except(oldRecords).ToList();
            var removed = oldRecords.Except(newRecords).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"RecordTypeAnalyzer: {added.Count} added, {removed.Count} removed record types."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "RecordTypeAnalyzer: No record type changes detected."
            };
        }
    }
}
