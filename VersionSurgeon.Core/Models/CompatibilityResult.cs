namespace VersionSurgeon.Core.Models
{
    public class CompatibilityResult
    {
        public ChangeType ChangeType { get; set; }
        public string Summary { get; set; } = string.Empty;
    }
}
