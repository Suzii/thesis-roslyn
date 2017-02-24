$ErrorActionPreference = "Stop";

<#
  .SYNOPSIS
  Tells whether project on $ProjectFilePath is on of third-party library project
  .EXAMPLE
  Is-ThirdPartyProject -ProjectFilePath "C:\TFS\CMS\MAIN\CMSSolution\FiftyOne\FiftyOne.csproj"
#>
function Is-ThirdPartyProject
{
    param([string]$ProjectFilePath)

    $ThirdPartyProjects = @(
    "Contrib.WordNet.SynExpand", 
    "FiftyOne",
    "ITHitWebDAVServer",
    "Lucene.Net.v3",
    "PDFClown",
    "QRCodeLib"
    );

    return @($ThirdPartyProjects | % { $ProjectFilePath -match $_ }) -contains $true
}

<#
  .SYNOPSIS
  Returns all CMS projects from $SolutionFolder that are suppoded to have the analyzer installed
  .DESCRIPTION
  Does not return projects from Test/ folder nor third-party library projects
  .EXAMPLE
  Get-Projects -SolutionFolder C:\TFS\CMS\MAIN\CMSSolution
#>
function Get-Projects
{
    param([string]$SolutionFolder)
    
    return Get-ChildItem -Path $SolutionFolder -Directory |
       % { Get-ChildItem -Path $_.Fullname -Filter "*.csproj" } |
       ? { -not (Is-ThirdPartyProject -ProjectFilePath $_) }
}

<#
  .SYNOPSIS
  Adds reference of type $ReferenceType to csproj $ProjectFilePath and includes all $IncludePaths
  .EXAMPLE
  Add-Reference -ReferenceType "None" -ProjectFilePath "C:\TFS\CMS\MAIN\CMSSolution\Activities\Activities.csproj" -IncludePaths @("packages.config")
#>
function Add-Reference
{
    param(
        [string] $ReferenceType,
        [string] $ProjectFilePath,
        [string[]] $IncludePaths)

    [xml] $xml = Get-Content $ProjectFilePath;
    $project = $xml.Project;

    $group = $xml.CreateElement("ItemGroup", $project.NamespaceURI);

    $IncludePaths | % {
        $analyzer = $xml.CreateElement($ReferenceType, $project.NamespaceURI);
        $analyzer.SetAttribute("Include", $_);
        
        $group.AppendChild($analyzer);
    } | Out-Null;

    $project.InsertBefore($group, $project.ItemGroup[0]) | Out-Null;

    $xml.Save($ProjectFilePath);
}

<#
  .SYNOPSIS
  Adds reference to packages.json of project $ProjectFile for package $PackageName
  .DESCRIPTION
  Creates new package.json file if neccessary and ensures .csproj references it
  .EXAMPLE
  Add-PackageReference -ProjectFile "C:\TFS\CMS\MAIN\CMSSolution\Activities\Activities.csproj" -PackageName "BugHunter.Analyzers" -Version 1.0.6263.25017
#>
function Add-PackageReference
{
    param(
        [System.IO.FileInfo] $ProjectFile,
        [string] $PackageName,
        [version] $Version,
        [string] $NetFramework = "net452")

    $packagesConfig = Join-Path $ProjectFile.Directory.FullName "packages.config";

    if (!(Test-Path $packagesConfig))
    {
        @"
<?xml version="1.0" encoding="utf-8"?>
<packages>
</packages>
"@ | Out-File -FilePath $packagesConfig;

        Add-Reference -ReferenceType "None" -ProjectFilePath $ProjectFile.FullName -IncludePaths @("packages.config")
    }

    [xml]$config = Get-Content $packagesConfig;

    $package = $config.CreateElement("package");
    $package.SetAttribute("id", $PackageName);
    $package.SetAttribute("version", $Version);
    $package.SetAttribute("targetFramework", $NetFramework);

    $config.DocumentElement.AppendChild($package) | Out-Null;
    $config.Save($packagesConfig);
}

<#
  .SYNOPSIS
  Installs $Package with analyzers of version $PackageVersion into CMS solution on path $SolutionPath
  .EXAMPLE
  Install-MassAnalyzer -SolutionPath "C:\TFS\CMS\MAIN\CMSSolution" -PackageName "BugHunter.Analyzers" -PackageVersion 1.0.6263.25017 -PackageFileNames @("BugHunter.Analyzers.dll", "BugHunter.Core.dll")
#>
function Install-MassAnalyzer
{
    param(
        [string] $SolutionPath,
        [string] $PackageName,
        [version] $PackageVersion,
        [string[]] $PackageFileNames)

    $paths = $PackageFileNames | % { "..\packages\$PackageName.$PackageVersion\analyzers\dotnet\cs\$_" }

    Get-Projects -SolutionFolder $SolutionPath | % {
        Write-Output "Processing $($_.FullName)"

        Add-Reference -ReferenceType "Analyzer" -ProjectFilePath $_.FullName -IncludePaths $paths
        Add-PackageReference -ProjectFile $_ -PackageName $PackageName -Version $PackageVersion
    } 
}

Install-MassAnalyzer -SolutionPath "C:\TFS\CMS\MAIN\CMSSolution" -PackageName "BugHunter.SystemIO.Analyzers" -PackageVersion 1.0.6264.23780 -PackageFileNames @("BugHunter.SystemIO.Analyzers.dll", "BugHunter.Core.dll")

# Then run if necessary
# cd C:\TFS\CMS\MAIN\CMSSolution\.nuget
#.\NuGet.exe restore C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln -Source nuget.org -Source \\kentico\dev\Build\NugetFeed -Source C:\LocalNugetSource
