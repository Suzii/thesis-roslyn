# CMSSolution
$TmpFolder = "D:\tmp\2017-04-28-ms-build-times"
$AnalyzedProjectOrSolution = "C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln"
$OutputFile = "$TmpFolder\no-analyzer.csv"
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
        &$msbuild '/t:Clean,Build' '/p:ReportAnalyzer=True' '/verbosity:diagnostic' $ProjectOrSolutionFilePath > $OutputFile
    }).TotalMilliseconds)
}

$buildTimeMilliseconds -join "`n" | Out-File $OutputFile
