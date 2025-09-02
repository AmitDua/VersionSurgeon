using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class VisibilityChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldTree = CSharpSyntaxTree.ParseText(oldCode);
            var newTree = CSharpSyntaxTree.ParseText(newCode);

            var oldRoot = oldTree.GetRoot();
            var newRoot = newTree.GetRoot();

            var oldModifiers = oldRoot.DescendantNodes().OfType<MemberDeclarationSyntax>().SelectMany(m => m.Modifiers);
            var newModifiers = newRoot.DescendantNodes().OfType<MemberDeclarationSyntax>().SelectMany(m => m.Modifiers);

            if (oldModifiers.Any(m => m.Text == "public") && newModifiers.Any(m => m.Text == "private"))
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = "Visibility changed from public to private."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "No visibility change detected."
            };
        }
    }
}
