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
  Parse-AnalyzerExecutionTimesFromSingleRun -InputFile "C:\tmp\msbuild-output.txt" -OutputFile "C:\tmp\analyzers-execution-times-aggregated.txt"
#>
function Parse-AnalyzerExecutionTimesFromSingleRun
{
    param(
    [string] $InputFile,
    [string] $OutputFile)
    
    $ParseReportAnalyzerResults = "..\statistics\ReportAnalyzerTimesParser\bin\Release\ReportAnalyzerTimes.Parser.exe";
    
    &$ParseReportAnalyzerResults /in=$InputFile /out=$OutputFile
}

<#
  .SYNOPSIS
  Taks the $inputFile path to the MSBuild log and runs a console app that aggregates execution times of analyzers and writes them to $OutputFile
  .EXAMPLE
  Aggregate-AnalyzerExecutionTimes -InputFilePrefix "C:\tmp\analyzer-execution-times-" -NumberOfRuns 100 -OutputFile "C:\tmp\aggregated.csv"
#>
function Aggregate-ResultsFromMultipleRuns 
{
    param(
    [string] $InputFilePrefix,
    [number] $NumberOfRuns,
    [string] $OutputFile)
    
    $AggregateReportAnalyzerResults = "..\statistics\ReportAnalyzerTimesParser\bin\Release\ReportAnalyzerTimesParser.exe";
    
    &$AggregateReportAnalyzerResults /in=$InputFile /out=$OutputFile
}

<#
  .SYNOPSIS
  Runs the MSBuild on given -ProjectOrSolutionFilePath -NumberOfRuns times, parses each results ro /ReportAnalyzer execution times, then aggregates all the reults into a CSV -OutputFile
#>
function Program
{
    param(
    [string] $ProjectOrSolutionFilePath,
    [number] $NumberOfRuns,
    [string] $TmpFolder,
    [string] $OutputFile)

    $MsBuildLogFile = "$TmpFolder\msbuild-output.txt"
    $AggregatedResultsPrefix = "$TmpFolder\aggregated-single-run-"
    Run-MsBuildWithReportAnalyzer -ProjectOrSolutionFilePath $AnalyzedProjectOrSolution -OutputFile $MsBuildLogFile
    
    Parse-AnalyzerExecutionTimesFromSingleRun -InputFile $MsBuildLogFile -OutputFile $AggregatedResultsPrefix

    # Aggregate-ResultsFromMultipleRuns 
}

# CMSSolution
$TmpFolder = "C:\tmp"
$AnalyzedProjectOrSolution = "C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln"
$OutputFile = "$TmpFolder\aggregated.csv"

Program -ProjectOrSolutionFilePath $AnalyzedProjectOrSolution -NumberOfRuns 1 -TmpFolder $TmpFolder -OutputFile $OutputFile

#Run-MsBuildWithReportAnalyzer -ProjectOrSolutionFilePath $AnalyzedProjectOrSolution -OutputFile $MsBuildLogFile

#Parse-AnalyzerExecutionTimesFromSingleRun -InputFile $MsBuildLogFile -OutputFile $AggregateedResults

# measure-command 1000x build + azure data analysis

#Aggregate-ResultsFromMultipleRuns 