using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;
using VersionSurgeon.Plugins.Analyzers;

namespace VersionSurgeon.Engine
{
    public static class VersionSurgeon
    {
        public static string PerformSurgicalAnalysis(string oldVersionPath, string newVersionPath, Version oldVersion)
        {
            if (!Directory.Exists(oldVersionPath))
                throw new DirectoryNotFoundException($"Old version path not found: {oldVersionPath}");

            if (!Directory.Exists(newVersionPath))
                throw new DirectoryNotFoundException($"New version path not found: {newVersionPath}");

            var oldFiles = Directory.GetFiles(oldVersionPath, "*.cs", SearchOption.AllDirectories);
            var newFiles = Directory.GetFiles(newVersionPath, "*.cs", SearchOption.AllDirectories);

            var analyzers = new List<ICompatibilityAnalyzer>
            {
                new PublicSurfaceAnalyzer(),
                new MethodSignatureAnalyzer(),
                new FieldChangeAnalyzer(),
                new PropertyChangeAnalyzer(),
                new EnumChangeAnalyzer(),
                new ConstructorChangeAnalyzer(),
                new VisibilityChangeAnalyzer(),
                new GenericTypeAnalyzer(),
                new InheritanceChangeAnalyzer(),
                new AttributeChangeAnalyzer(),
                new ExceptionContractAnalyzer(),
                new ReturnTypeAnalyzer(),
                new ParameterChangeAnalyzer(),
                new StaticMemberAnalyzer(),
                new VirtualMemberAnalyzer(),
                new AbstractMemberAnalyzer(),
                new SealedClassAnalyzer(),
                new InternalApiAnalyzer(),
                new ExtensionMethodAnalyzer(),
                new EventChangeAnalyzer(),
                new DelegateChangeAnalyzer(),
                new NullableReferenceAnalyzer(),
                new AsyncMethodAnalyzer(),
                new TupleChangeAnalyzer(),
                new DynamicUsageAnalyzer(),
                new RefReadonlyAnalyzer(),
                new RecordTypeAnalyzer(),
                new RequiredMemberAnalyzer(),
                new InterfaceImplementationAnalyzer(),
                new BaseClassChangeAnalyzer(),
                new TypeAccessibilityAnalyzer(),
                new XmlDocChangeAnalyzer(),
                new AnnotationChangeAnalyzer(),
                new MetadataTokenAnalyzer(),
                new ContractAnnotationAnalyzer(),
                new TestExposureAnalyzer()
            };

            var results = new List<CompatibilityResult>();

            foreach (var oldFile in oldFiles)
            {
                var relativePath = Path.GetRelativePath(oldVersionPath, oldFile);
                var newFile = Path.Combine(newVersionPath, relativePath);

                if (!File.Exists(newFile)) continue;

                var oldCode = File.ReadAllText(oldFile);
                var newCode = File.ReadAllText(newFile);

                foreach (var analyzer in analyzers)
                {
                    var result = analyzer.Analyze(oldCode, newCode);
                    results.Add(result);
                }
            }

            var reporter = new CompatibilityReportGenerator();
            return reporter.GenerateReport(results, oldVersion);
        }
    }
}
