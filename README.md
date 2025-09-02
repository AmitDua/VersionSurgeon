VersionSurgeon  
![NuGet](https://img.shields.io/nuget/v/VersionSurgeon)  
Author: Amit Dua  

VersionSurgeon is a plugin-based framework for semantic versioning and compatibility analysis in .NET 8.  
It helps developers detect breaking changes between versions of their codebase and recommends appropriate version bumps.

✨ Features

- 🔌 Plugin-driven architecture for compatibility analysis  
- 🧠 Roslyn-powered static code inspection  
- 📦 Analyzer registry with over 35 built-in plugins  
- 📁 File-based diffing of C# source files  
- 📊 Compatibility scoring and semantic versioning guidance  
- 🛠️ CLI and SDK support for integration into pipelines  
- 🔧 Extensible via custom analyzers  
- 🚀 NuGet-packable and CI/CD-friendly  
- 🧪 Built-in test harness for plugin validation  

🔍 Built-in Analyzers

VersionSurgeon includes the following analyzers out of the box:

- PublicSurfaceAnalyzer  
- MethodSignatureAnalyzer  
- FieldChangeAnalyzer  
- PropertyChangeAnalyzer  
- EnumChangeAnalyzer  
- ConstructorChangeAnalyzer  
- VisibilityChangeAnalyzer  
- GenericTypeAnalyzer  
- InheritanceChangeAnalyzer  
- AttributeChangeAnalyzer  
- ExceptionContractAnalyzer  
- ReturnTypeAnalyzer  
- ParameterChangeAnalyzer  
- StaticMemberAnalyzer  
- VirtualMemberAnalyzer  
- AbstractMemberAnalyzer  
- SealedClassAnalyzer  
- InternalApiAnalyzer  
- ExtensionMethodAnalyzer  
- EventChangeAnalyzer  
- DelegateChangeAnalyzer  
- NullableReferenceAnalyzer  
- AsyncMethodAnalyzer  
- TupleChangeAnalyzer  
- DynamicUsageAnalyzer  
- RefReadonlyAnalyzer  
- RecordTypeAnalyzer  
- RequiredMemberAnalyzer  
- InterfaceImplementationAnalyzer  
- BaseClassChangeAnalyzer  
- TypeAccessibilityAnalyzer  
- XmlDocChangeAnalyzer  
- AnnotationChangeAnalyzer  
- MetadataTokenAnalyzer  
- ContractAnnotationAnalyzer  
- TestExposureAnalyzer  

Each plugin implements `ICompatibilityAnalyzer` and contributes to the overall compatibility score.



🚀 Getting Started

1. Install the NuGet Package

Use the .NET CLI:

    dotnet add package VersionSurgeon

Or add this to your `.csproj`:

    <PackageReference Include="VersionSurgeon" Version="1.0.0" />

2. Prepare Your Code Snapshots

Create two folders with `.cs` files from different versions of your codebase:

    C:\MyApp\OldVersion  
    C:\MyApp\NewVersion

3. Run the Analysis

In your app:

    using VersionSurgeon.Engine;

    var report = VersionSurgeon.PerformSurgicalAnalysis(
        @"C:\MyApp\OldVersion",
        @"C:\MyApp\NewVersion",
        new Version("1.2.3")
    );

    Console.WriteLine(report);



🤝 Contributing

We welcome contributions of all kinds. Please follow these guidelines:

- Keep classes under 50 lines when possible  
- Use clear naming and consistent formatting  
- Write unit tests for each plugin  
- Submit pull requests with descriptive titles  

Author: Amit Dua

❤️ Sponsor

If you find VersionSurgeon useful, consider sponsoring the project to support ongoing development.

GitHub Sponsors → https://github.com/sponsors/AmitDua
