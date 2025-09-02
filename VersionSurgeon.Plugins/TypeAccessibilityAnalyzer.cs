using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class TypeAccessibilityAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldTree = CSharpSyntaxTree.ParseText(oldCode);
            var newTree = CSharpSyntaxTree.ParseText(newCode);

            var oldRoot = oldTree.GetRoot();
            var newRoot = newTree.GetRoot();

            var oldTypes = oldRoot.DescendantNodes().OfType<TypeDeclarationSyntax>();
            var newTypes = newRoot.DescendantNodes().OfType<TypeDeclarationSyntax>();

            foreach (var oldType in oldTypes)
            {
                var newType = newTypes.FirstOrDefault(t => t.Identifier.Text == oldType.Identifier.Text);
                if (newType != null)
                {
                    var oldAccess = oldType.Modifiers.FirstOrDefault().Text;
                    var newAccess = newType.Modifiers.FirstOrDefault().Text;

                    if (oldAccess == "public" && newAccess == "internal")
                    {
                        return new CompatibilityResult
                        {
                            ChangeType = ChangeType.Major,
                            Summary = $"Type '{oldType.Identifier.Text}' changed from public to internal."
                        };
                    }
                }
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "No accessibility changes detected."
            };
        }
    }
}
