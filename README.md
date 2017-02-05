# thesis-roslyn
Master thesis - Roslyn - Automated source code review (Masaryk University &amp; Kentico)

### Resseting VS2015 Experimental Hive
In case the analyzers/codefixes misbehave in the experimental instance reset it using these commands:

1. `cd C:\Program Files (x86)\Microsoft Visual Studio 14.0\VSSDK\VisualStudioIntegration\Tools\Bin`
2. `CreateExpInstance.exe /Reset /VSInstance=14.0 /RootSuffix=Exp`
or alternatively 
2. `CreateExpInstance.exe /Reset /VSInstance=14.0 /RootSuffix=Roslyn`
2. `CreateExpInstance.exe /Reset /VSInstance=14.0 /RootSuffix=RoslynDev`
