using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class InheritanceChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldTree = CSharpSyntaxTree.ParseText(oldCode);
            var newTree = CSharpSyntaxTree.ParseText(newCode);

            var oldRoot = oldTree.GetRoot();
            var newRoot = newTree.GetRoot();

            var oldBases = oldRoot.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Select(c => c.BaseList?.Types.FirstOrDefault()?.ToString());

            var newBases = newRoot.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Select(c => c.BaseList?.Types.FirstOrDefault()?.ToString());

            if (!oldBases.SequenceEqual(newBases))
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = "Base class or interface changed."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "No inheritance changes detected."
            };
        }
    }
}
