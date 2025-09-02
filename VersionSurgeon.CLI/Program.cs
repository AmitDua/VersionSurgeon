using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VersionSurgeon.Core.Interfaces;
using VersionSurgeon.Core.Models;
using VersionSurgeon.Plugins.Analyzers;

namespace VersionSurgeon.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || !Version.TryParse(args[0], out var oldVersion))
            {
                Console.WriteLine("Please provide the old version number as the first argument (e.g., 1.2.3)");
                return;
            }

            var services = new ServiceCollection();
            services.AddLogging(config => config.AddConsole());
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.PublicSurfaceAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.MethodSignatureAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.FieldChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.PropertyChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.EnumChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.ConstructorChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.VisibilityChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.GenericTypeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.InheritanceChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.AttributeChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.ExceptionContractAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.ReturnTypeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.ParameterChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.StaticMemberAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.VirtualMemberAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.AbstractMemberAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.SealedClassAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.InternalApiAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.ExtensionMethodAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.EventChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.DelegateChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.NullableReferenceAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.AsyncMethodAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.TupleChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.DynamicUsageAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.RefReadonlyAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.RecordTypeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.RequiredMemberAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.InterfaceImplementationAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.BaseClassChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.TypeAccessibilityAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.CompatibilityReportGenerator>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.XmlDocChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.AnnotationChangeAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.MetadataTokenAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.ContractAnnotationAnalyzer>();
            services.AddSingleton<ICompatibilityAnalyzer, VersionSurgeon.Plugins.Analyzers.TestExposureAnalyzer>();

            var provider = services.BuildServiceProvider();
            var analyzers = provider.GetServices<ICompatibilityAnalyzer>()
                                    .Where(p => p.Type == PluginType.Analyzer)
                                    .ToList();

            Console.WriteLine($"🔍 Running compatibility analysis for version {oldVersion}...");

    var sourcePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
var oldPath = Path.Combine(sourcePath, "OldVersion");
var newPath = Path.Combine(sourcePath,  "NewVersion");

Console.WriteLine($"Resolved OldVersion path: {oldPath}");
Console.WriteLine($"Resolved NewVersion path: {newPath}");

if (!Directory.Exists(oldPath))
{
    Console.WriteLine("ERROR: OldVersion folder not found at resolved path.");
    return;
}

if (!Directory.Exists(newPath))
{
    Console.WriteLine("ERROR: NewVersion folder not found at resolved path.");
    return;
}

var oldFiles = Directory.GetFiles(oldPath, "*.cs", SearchOption.AllDirectories);
var newFiles = Directory.GetFiles(newPath, "*.cs", SearchOption.AllDirectories);

            var results = new List<(string file, string plugin, CompatibilityResult result)>();

            foreach (var oldFile in oldFiles)
            {
                var relativePath = Path.GetRelativePath(oldPath, oldFile);
var newFile = Path.Combine(newPath, relativePath);

                if (File.Exists(newFile))
                {
                    var oldCode = File.ReadAllText(oldFile);
                    var newCode = File.ReadAllText(newFile);

                    foreach (var analyzer in analyzers)
                    {
                        var result = analyzer.Analyze(oldCode, newCode);
                        results.Add((relativePath, analyzer.GetType().Name, result));

                        var emoji = result.ChangeType switch {
                            ChangeType.Major => "🛑",
                            ChangeType.Minor => "🟡",
                            ChangeType.Patch => "🩹",
                            ChangeType.None => "🟢",
                            _ => "❓"
                        };
                        Console.WriteLine($"[{relativePath}] [{analyzer.GetType().Name}] {emoji} {result.ChangeType}: {result.Summary}");
                    }
                }
            }

            var majorResults = results.Where(r => r.result.ChangeType == ChangeType.Major).ToList();
            var minorResults = results.Where(r => r.result.ChangeType == ChangeType.Minor).ToList();
            var patchResults = results.Where(r => r.result.ChangeType == ChangeType.Patch).ToList();
            var noneResults  = results.Where(r => r.result.ChangeType == ChangeType.None).ToList();


            var distinctMajorPlugins = majorResults.Select(r => r.plugin).Distinct().Count();
            var distinctMinorPlugins = minorResults.Select(r => r.plugin).Distinct().Count();
            var distinctPatchPlugins = patchResults.Select(r => r.plugin).Distinct().Count();
            var distinctNonePlugins  = noneResults.Select(r => r.plugin).Distinct().Count();

            var majorFiles = majorResults.Select(r => r.file).Distinct().Count();
            var minorFiles = minorResults.Select(r => r.file).Distinct().Count();
            var patchFiles = patchResults.Select(r => r.file).Distinct().Count();
            var noneFiles  = noneResults.Select(r => r.file).Distinct().Count();


            Console.WriteLine("\n📊 Summary(raw plugin results):");
            Console.WriteLine($"🛑 Major changes: {majorResults.Count} results from {distinctMajorPlugins} plugins across {majorFiles} files");
            Console.WriteLine($"🟡 Minor changes: {minorResults.Count} results from {distinctMinorPlugins} plugins across {minorFiles} files");
            Console.WriteLine($"🩹 Patch changes: {patchResults.Count} results from {distinctPatchPlugins} plugins across {patchFiles} files");
            Console.WriteLine($"🟢 No changes:    {noneResults.Count} results from {distinctNonePlugins} plugins across {noneFiles} files");

            Version newVersion;
            string bumpType;
            string emojiFinal;

            if (majorResults.Any())
            {
                newVersion = new Version(oldVersion.Major + 1, 0, 0);
                bumpType = "VERSION BUMP RECOMMENDED: MAJOR";
                emojiFinal = "🛑";
            }
            else if (minorResults.Any())
            {
                newVersion = new Version(oldVersion.Major, oldVersion.Minor + 1, 0);
                bumpType = "VERSION BUMP RECOMMENDED: MINOR";
                emojiFinal = "🟡";
            }
            else if (patchResults.Any())
            {
                newVersion = new Version(oldVersion.Major, oldVersion.Minor, oldVersion.Build + 1);
                bumpType = "VERSION BUMP RECOMMENDED: PATCH";
                emojiFinal = "🩹";
            }
            else
            {
                newVersion = oldVersion;
                bumpType = "NO VERSION BUMP NEEDED";
                emojiFinal = "🟢";
            }

            Console.WriteLine($"\n{emojiFinal} ** {bumpType} **");
            Console.WriteLine($"🔢 New version: {newVersion}");

            var reporter = provider.GetServices<ICompatibilityAnalyzer>()
                                   .FirstOrDefault(p => p.GetType().Name == "CompatibilityReportGenerator");

            if (reporter is CompatibilityReportGenerator reportGen)
            {
                var summary = reportGen.GenerateReport(results.Select(r => r.result).ToList(), oldVersion);
Console.WriteLine(summary);
            }

           
        }
    }
}
