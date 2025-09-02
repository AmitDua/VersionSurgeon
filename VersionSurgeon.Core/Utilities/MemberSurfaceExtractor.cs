using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace VersionSurgeon.Core.Utilities
{
    public class MemberSurfaceExtractor
    {
        private readonly ILogger<MemberSurfaceExtractor> _logger;

        public MemberSurfaceExtractor(ILogger<MemberSurfaceExtractor> logger)
        {
            _logger = logger;
        }

        public List<string> ExtractPublicMembers(string code)
        {
            try
            {
                _logger.LogInformation("Extracting public members...");
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();

                var members = root.DescendantNodes()
                    .Where(node =>
                        (node is MethodDeclarationSyntax m && m.Modifiers.Any(SyntaxKind.PublicKeyword)) ||
                        (node is PropertyDeclarationSyntax p && p.Modifiers.Any(SyntaxKind.PublicKeyword)) ||
                        (node is FieldDeclarationSyntax f && f.Modifiers.Any(SyntaxKind.PublicKeyword)) ||
                        (node is ConstructorDeclarationSyntax c && c.Modifiers.Any(SyntaxKind.PublicKeyword)))
                    .Select(node => node.ToString().Trim())
                    .ToList();

                return members;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting public members.");
                return new List<string>();
            }
        }
    }
}
