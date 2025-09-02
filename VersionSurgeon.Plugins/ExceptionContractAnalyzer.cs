using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class ExceptionContractAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldThrows = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<ThrowStatementSyntax>()
                .Select(t => t.ToString());

            var newThrows = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<ThrowStatementSyntax>()
                .Select(t => t.ToString());

            var added = newThrows.Except(oldThrows).ToList();
            var removed = oldThrows.Except(newThrows).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = $"ExceptionContractAnalyzer: {added.Count} added, {removed.Count} removed throw statements."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "ExceptionContractAnalyzer: No exception contract changes detected."
            };
        }
    }
}
