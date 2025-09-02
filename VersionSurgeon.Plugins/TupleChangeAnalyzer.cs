using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class TupleChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldTuples = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<TupleExpressionSyntax>()
                .Select(t => t.ToString());

            var newTuples = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<TupleExpressionSyntax>()
                .Select(t => t.ToString());

            var added = newTuples.Except(oldTuples).ToList();
            var removed = oldTuples.Except(newTuples).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Minor,
                    Summary = $"TupleChangeAnalyzer: {added.Count} added, {removed.Count} removed tuple expressions."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "TupleChangeAnalyzer: No tuple changes detected."
            };
        }
    }
}
