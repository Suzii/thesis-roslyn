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

Run-MsBuildWithReportAnalyzer -ProjectOrSolutionFilePath "C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln" -OutputFile "C:\tmp\msbuild-output-systemio.txt"