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
    
    $ParseReportAnalyzerResults = "..\statistics\ReportAnalyzerTimes.Parser\bin\Release\ReportAnalyzerTimes.Parser.exe";
    
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
    [int16] $NumberOfRuns,
    [string] $OutputFile)
    
    $AggregateReportAnalyzerResults = "..\statistics\ReportAnalyzerTimes.Aggregator\bin\Release\ReportAnalyzerTimes.Aggregator.exe";
    
    &$AggregateReportAnalyzerResults /inPrefix=$InputFilePrefix /out=$OutputFile /count=$NumberOfRuns
}

<#
  .SYNOPSIS
  Runs the MSBuild on given -ProjectOrSolutionFilePath -NumberOfRuns times, parses each results ro /ReportAnalyzer execution times, then aggregates all the reults into a CSV -OutputFile
#>
function Program
{
    param(
    [string] $ProjectOrSolutionFilePath,
    [int16] $NumberOfRuns,
    [string] $TmpFolder,
    [string] $OutputFile)

    $MsBuildLogFile = "$TmpFolder\msbuild-output-"
    $AggregatedResultsPrefix = "$TmpFolder\aggregated-single-run-"
    [System.Array] $MsbuildTimes = @();

    For ($Index = 0; $Index -lt $NumberOfRuns; $Index++)
    {
        Run-MsBuildWithReportAnalyzer -ProjectOrSolutionFilePath $AnalyzedProjectOrSolution -OutputFile "$MsBuildLogFile$Index.txt"
    
        # $Sec = (Measure-Command { Parse-AnalyzerExecutionTimesFromSingleRun -InputFile $MsBuildLogFile -OutputFile "$AggregatedResultsPrefix$Index.txt"}).TotalSeconds
        # $MsbuildTimes += $Sec
        Parse-AnalyzerExecutionTimesFromSingleRun -InputFile "$MsBuildLogFile$Index.txt" -OutputFile "$AggregatedResultsPrefix$Index.txt"
    }

    # $MsbuildTimes | Export-Csv "$TmpFolder\msbuild-times.csv" -Delimiter ";" -NoTypeInformation
    Aggregate-ResultsFromMultipleRuns -InputFilePrefix $AggregatedResultsPrefix -NumberOfRuns $NumberOfRuns -OutputFile $OutputFile
}

# CMSSolution
$TmpFolder = "D:\tmp\2017-04-18-sample-project"
# $AnalyzedProjectOrSolution = "C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln"
$AnalyzedProjectOrSolution = "C:\Users\zuzanad\code\thesis\thesis-sample-test-project\SampleProject\SampleProject.sln"
$OutputFile = "$TmpFolder\aggregated.csv"
$NumberOfRuns = 100

Program -ProjectOrSolutionFilePath $AnalyzedProjectOrSolution -NumberOfRuns $NumberOfRuns -TmpFolder $TmpFolder -OutputFile $OutputFile
