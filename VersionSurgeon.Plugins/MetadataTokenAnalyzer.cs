using System;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class MetadataTokenAnalyzer : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Analyzer;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            // Placeholder: Metadata tokens require compiled assemblies, not source code.
            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "MetadataTokenAnalyzer: Skipped — requires compiled assemblies for token comparison."
            };
        }
    }
}
