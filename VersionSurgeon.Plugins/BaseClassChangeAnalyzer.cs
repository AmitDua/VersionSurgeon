using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class BaseClassChangeAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            string GetBaseClass(ClassDeclarationSyntax c) =>
                c.BaseList?.Types.FirstOrDefault()?.Type.ToString() ?? string.Empty;

            var oldBases = CSharpSyntaxTree.ParseText(oldCode).GetRoot()
                .DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Select(c => $"{c.Identifier.Text}:{GetBaseClass(c)}");

            var newBases = CSharpSyntaxTree.ParseText(newCode).GetRoot()
                .DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Select(c => $"{c.Identifier.Text}:{GetBaseClass(c)}");

            var changed = newBases.Except(oldBases).ToList();

            if (changed.Any())
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = $"BaseClassChangeAnalyzer: {changed.Count} base class changes detected."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "BaseClassChangeAnalyzer: No base class changes detected."
            };
        }
    }
}
