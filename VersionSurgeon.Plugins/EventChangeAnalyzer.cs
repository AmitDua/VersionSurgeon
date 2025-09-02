using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class EventChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldEvents = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<EventDeclarationSyntax>()
                .Select(e => e.ToString());

            var newEvents = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<EventDeclarationSyntax>()
                .Select(e => e.ToString());

            var added = newEvents.Except(oldEvents).ToList();
            var removed = oldEvents.Except(newEvents).ToList();

            if (added.Any() || removed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = removed.Any() ? ChangeType.Major : ChangeType.Minor,
                    Summary = $"EventChangeAnalyzer: {added.Count} added, {removed.Count} removed events."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "EventChangeAnalyzer: No event changes detected."
            };
        }
    }
}
