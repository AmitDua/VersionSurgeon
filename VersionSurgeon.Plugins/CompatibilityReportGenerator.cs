using System.Collections.Generic;
using System.Linq;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Plugins.Analyzers
{
    public class CompatibilityReportGenerator : ICompatibilityAnalyzer
    {
        public PluginType Type => PluginType.Reporter;

        public CompatibilityResult Analyze(string oldCode, string newCode)
        {
            // Not used in this plugin
            return new CompatibilityResult
            {
                ChangeType = ChangeType.None,
                Summary = "CompatibilityReportGenerator: Use GenerateReport instead."
            };
        }

      public string GenerateReport(List<CompatibilityResult> results, Version oldVersion)
{
results = results
    .Where(r => r.ChangeType != ChangeType.None)
    .GroupBy(r => r.Summary)
    .Select(g => g.First())
    .ToList();

    var major = results.Where(r => r.ChangeType == ChangeType.Major).ToList();
var minor = results.Where(r => r.ChangeType == ChangeType.Minor).ToList();
var patch = results.Where(r => r.ChangeType == ChangeType.Patch).ToList();

    // Determine bump recommendation
    string bumpType;
    Version newVersion;

    if (major.Any())
    {
        newVersion = new Version(oldVersion.Major + 1, 0, 0);
        bumpType = "VERSION BUMP RECOMMENDED: MAJOR";
    }
    else if (minor.Any())
    {
        newVersion = new Version(oldVersion.Major, oldVersion.Minor + 1, 0);
        bumpType = "VERSION BUMP RECOMMENDED: MINOR";
    }
    else if (patch.Any())
    {
        newVersion = new Version(oldVersion.Major, oldVersion.Minor, oldVersion.Build + 1);
        bumpType = "VERSION BUMP RECOMMENDED: PATCH";
    }
    else
    {
        newVersion = oldVersion;
        bumpType = "NO VERSION BUMP NEEDED";
    }

    // Build report
    var report = "\n\n\n\n\n========================================================================================================================\n";
    report += "🔍 Compatibility Report\n";

    if (major.Any())
    {
        report += "\n🛑 Major Changes:\n";
        report += string.Join("\n", major.Select(r => "- " + r.Summary));
    }

    if (minor.Any())
    {
        report += "\n\n🟡 Minor Changes:\n";
        report += string.Join("\n", minor.Select(r => "- " + r.Summary));
    }

    if (patch.Any())
    {
        report += "\n\n🩹 Patch Changes:\n";
        report += string.Join("\n", patch.Select(r => "- " + r.Summary));
    }

    report += $"\n\n🔢 Old Version: {oldVersion}";
    report += $"\n🔢 New Version: {newVersion}";
    report += $"\n📣 {bumpType}";
    report += "\n========================================================================================================================";

    return report;
}
    }
}
