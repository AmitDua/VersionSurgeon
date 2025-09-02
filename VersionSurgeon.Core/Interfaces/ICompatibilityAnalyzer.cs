using VersionSurgeon.Core.Models;

namespace VersionSurgeon.Core.Interfaces
{
    public interface ICompatibilityAnalyzer
    {
        PluginType Type { get; }
        CompatibilityResult Analyze(string oldCode, string newCode);
    }
}
