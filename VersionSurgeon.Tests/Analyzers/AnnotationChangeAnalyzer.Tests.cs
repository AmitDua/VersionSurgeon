using Xunit;
using VersionSurgeon.Core.Models;
using VersionSurgeon.Plugins.Analyzers;

namespace VersionSurgeon.Tests.Analyzers
{
    public class AnnotationChangeAnalyzerTests
    {
        [Fact]
        public void Analyze_ShouldDetectChange()
        {
            // Arrange
            var analyzer = new AnnotationChangeAnalyzer();
            var oldCode = "public class Sample { }";
            var newCode = "public class Sample { /* simulated change */ }";

            // Act
            var result = analyzer.Analyze(oldCode, newCode);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CompatibilityResult>(result);
            Assert.Equal(ChangeType.Patch, result.ChangeType);
            Assert.False(string.IsNullOrWhiteSpace(result.Summary));
        }

        [Fact]
        public void Analyze_ShouldReturnNoneForIdenticalCode()
        {
            // Arrange
            var analyzer = new AnnotationChangeAnalyzer();
            var code = "public class Sample { }";

            // Act
            var result = analyzer.Analyze(code, code);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CompatibilityResult>(result);
            Assert.Equal(ChangeType.None, result.ChangeType);
            Assert.Contains("No", result.Summary);
        }
    }
}
