# Resseting VS2015 Experimental Hive
In case the analyzers/codefixes misbehave in the experimental instance reset might be required. Use:

1. Go to the VS tools folder: `cd C:\Program Files (x86)\Microsoft Visual Studio 14.0\VSSDK\VisualStudioIntegration\Tools\Bin`
2. Reset the experimental hive, depends on configuration but one of these must work:
 - `CreateExpInstance.exe /Reset /VSInstance=14.0 /RootSuffix=Exp`
 - `CreateExpInstance.exe /Reset /VSInstance=14.0 /RootSuffix=Roslyn`
 - `CreateExpInstance.exe /Reset /VSInstance=14.0 /RootSuffix=RoslynDev`
