<#
  .SYNOPSIS
  Runs MSBuild on $ProjectOrSolutionFile and reports analyzer times that shall be installed as nuget package dependencies. Outputs the log ot $OutputFile
  .EXAMPLE
  Run-MsBuildWithReportAnalyzer -ProjectOrSolutionFilePath "C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln" -OutputFile "C:\tmp\msbuild-output.txt"
#>
function Run-MsBuildWithReportAnalyzer
{
    param(
    [string] $ProjectOrSolutionFilePath,
    [string] $OutputFile)

    $msbuild = "${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"
    If (-not (Test-Path $msbuild)) {
	    $host.UI.WriteErrorLine("Couldn't find MSBuild.exe")
	    exit 1
    }

    echo "Running build of $ProjectOrSolutionFilePath"
    &$msbuild '/t:Clean,Build' '/p:ReportAnalyzer=True' '/verbosity:diagnostic' $ProjectOrSolutionFilePath > $OutputFile
    echo "Build finished. Results can be found in $OutputFile"
}

<#
  .SYNOPSIS
  Taks the $inputFile path to the MSBuild log and runs a console app that aggregates execution times of analyzers and writes them to $OutputFile
  .EXAMPLE
  Aggregate-AnalyzerExecutionTimes -inputFile "C:\tmp\msbuild-output.txt" -OutputFile "C:\tmp\analyzers-execution-times-aggregated.txt"
#>
function Aggregate-AnalyzerExecutionTimes
{
    param(
    [string] $InputFile,
    [string] $OutputFile)
    
    $AggregateReportAnalyzerResults = "..\statistics\ReportAnalyzerTimesParser\bin\Release\ReportAnalyzerTimesParser.exe";
    
    &$AggregateReportAnalyzerResults /in=$InputFile /out=$OutputFile
}

# CMSSolution
$AnalyzedProjectOrSolution = "C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln"
# Other project
# $AnalyzedProjectOrSolution = "C:\TFS\CMS\MAIN\CMSSolution\Blogs\Blogs.csproj"

$TmpFolder = "C:\tmp" 
$MsBuildLogFile = "$TmpFolder\msbuild-output-2017-03-14.txt"
$AggregateedResults = "$TmpFolder\analyzers-execution-times-aggregated-2017-03-14-isDerivedFrom-optimization-2.txt"

Run-MsBuildWithReportAnalyzer -ProjectOrSolutionFilePath $AnalyzedProjectOrSolution -OutputFile $MsBuildLogFile

Aggregate-AnalyzerExecutionTimes -InputFile $MsBuildLogFile -OutputFile $AggregateedResults

# measure-command 1000x build + azure data analysis