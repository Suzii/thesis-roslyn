Running MSBuild with ReportAnalyzers
This guide explains how to run MSBuild with so that compiler provides detailed statistics about analyzers execution times.

As per https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Report%20Analyzer%20Format.md it should be possible to run the build with `/ReportAnalyzer` switch and all the stats should be there. Not so easy though... This is not swith for the MSBuild.exe but rather for CSharp compiler csc.exe. That one is of course run internally by MSBuild. Therefore, it should be (somehow) possible to switch it on. As it turns out, it is not so straightforward and not everything is so well documented in Microsoft. (https://msdn.microsoft.com/en-us/library/ms164311.aspx)

To put it shortly, run:
```MSBuild.exe /t:Clean,Build /p:ReportAnalyzer=True /verbosity:diagnostic .\SampleProject.sln > msbuild-output.txt```
 
Be carefull to redirect the output and prepare yourself for the output file to be rather large (~1-100MB). Parameter `/p:ReportAnalyzer=True` turns `/ReportAnalyzer` for `csc.exe`, but only `/verbosity:diagnostic` makes it appear in the output.

NOTE: If the diagnostics are only warnings, build seems to ignore them after first run, therefore the Clean target.