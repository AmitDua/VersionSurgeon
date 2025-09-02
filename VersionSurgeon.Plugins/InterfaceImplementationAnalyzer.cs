using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class InterfaceImplementationAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            var oldTree = CSharpSyntaxTree.ParseText(oldCode);
            var newTree = CSharpSyntaxTree.ParseText(newCode);

            var oldRoot = oldTree.GetRoot();
            var newRoot = newTree.GetRoot();

            var oldInterfaces = oldRoot.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .SelectMany(c => c.BaseList?.Types ?? Enumerable.Empty<BaseTypeSyntax>())
                .Select(t => t.Type.ToString());

            var newInterfaces = newRoot.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .SelectMany(c => c.BaseList?.Types ?? Enumerable.Empty<BaseTypeSyntax>())
                .Select(t => t.Type.ToString());

            if (!oldInterfaces.SequenceEqual(newInterfaces))
            {
                return new CompatibilityResult
                {
                    ChangeType = ChangeType.Major,
                    Summary = "Implemented interfaces have changed."
                };
            }

            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "No interface implementation changes detected."
            };
        }
    }
}
