# CMSSolution
$TmpFolder = "D:\tmp\2017-04-29-ms-build-times-with-bh"
$AnalyzedProjectOrSolution = "C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln"
$OutputFile = "$TmpFolder\bh-analyzers.csv"
$MsBuildLogFile = "$TmpFolder\msbuild-output-"
$NumberOfRuns = 100

$msbuild = "${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"
If (-not (Test-Path $msbuild)) {
	$host.UI.WriteErrorLine("Couldn't find MSBuild.exe")
	exit 1
}

$buildTimeMilliseconds = @()
For ($Index = 0; $Index -lt 100; $Index++)
{
    "Running build $Index"
    $buildTimeMilliseconds += @((Measure-Command {
        &$msbuild '/t:Clean,Build' '/p:ReportAnalyzer=True' '/verbosity:diagnostic' $AnalyzedProjectOrSolution > "$MsBuildLogFile$Index.txt"
    }).TotalMilliseconds)
}

$buildTimeMilliseconds -join "`n" | Out-File $OutputFile
