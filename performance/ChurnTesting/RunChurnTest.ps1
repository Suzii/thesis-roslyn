[CmdletBinding()]
Param([switch] $Local)

$NuGet = "..\..\.nuget\nuget.exe"
$Git = "$Env:ProgramFiles\Git\bin\git.exe"

#$PathToTestSolution = "..\..\..\thesis-sample-test-project\SampleProject\SampleProject.sln"
$PathToTestSolution = "C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln"

function Clone-SampleProject {
    $PathToTestSolution = ".\bin\repo"
    &$Git clone https://github.com/Suzii/thesis-sample-test-project $PathToTestSolution
	
    Push-Location
    cd .\$PathToTestSolution
	&$Git checkout 1f20d5bb693c51824d3561a5671917853b1dfd46 | Out-String | Write-Verbose
    Pop-Location
}

# not working anyway :(
function Download-NuGetIfNecessary {
    If (-not (Test-Path $NuGet)) {
		    If (-not (Test-Path "..\.nuget")) {
			    mkdir "..\.nuget"
		    }

		    $nugetSource = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
		    Invoke-WebRequest $nugetSource -OutFile $NuGet
		    If (-not $?) {
			    Write-Error "Unable to download NuGet executable, aborting!"
			    exit $LASTEXITCODE
		    }
	    }
}

function Build-BugHunterSolution {
    &$NuGet update -Self -Verbosity quiet
	&$NuGet install Microsoft.CodeAnalysis -Version 1.3.2 -OutputDirectory ..\..\packages -Verbosity quiet
	
	# Make sure the project binaries are up-to-date
	&$NuGet restore ..\..\BugHunter.sln -Verbosity quiet
	Push-Location
	cd ..\..\build
	.\build.ps1 -Incremental -Debug -SkipKeyCheck -Verbosity quiet
	Pop-Location
}

function Copy-TestingToolAndItsDependencies {
    If (Test-Path .\bin\PerformaceTester-Roslyn.1.3.2) {
		Remove-Item .\bin\PerformaceTester-Roslyn.1.3.2 -Force -Recurse
	}

	New-Item .\bin\PerformaceTester-Roslyn.1.3.2 -ItemType Directory
	Copy-Item ..\BugHunter.PerformanceTest\bin\Debug\* .\bin\PerformaceTester-Roslyn.1.3.2
	Copy-Item .\PerformaceTester-Roslyn.1.1.exe.config .\bin\PerformaceTester-Roslyn.1.3.2\PerformaceTester.exe.config

	Copy-Item ..\..\packages\Microsoft.CodeAnalysis.Common.1.3.2\lib\net45\*.dll .\bin\PerformaceTester-Roslyn.1.3.2
	Copy-Item ..\..\packages\Microsoft.CodeAnalysis.Workspaces.Common.1.3.2\lib\net45\*.dll .\bin\PerformaceTester-Roslyn.1.3.2
	Copy-Item ..\..\packages\System.Collections.Immutable.1.1.37\lib\dotnet\*.dll .\bin\PerformaceTester-Roslyn.1.3.2
	Copy-Item ..\..\packages\System.Reflection.Metadata.1.2.0\lib\portable-net45+win8\*.dll .\bin\PerformaceTester-Roslyn.1.3.2
	Copy-Item ..\..\packages\Microsoft.Composition.1.0.27\lib\portable-net45+win8+wp8+wpa81\*.dll .\bin\PerformaceTester-Roslyn.1.3.2
	Copy-Item ..\..\packages\Microsoft.CodeAnalysis.CSharp.1.3.2\lib\net45\*.dll .\bin\PerformaceTester-Roslyn.1.3.2
	Copy-Item ..\..\packages\Microsoft.CodeAnalysis.CSharp.Workspaces.1.3.2\lib\net45\*.dll .\bin\PerformaceTester-Roslyn.1.3.2
}

Try {
	Download-NuGetIfNecessary 

	Write-Output "Building BugHunter.sln..." 
	Build-BugHunterSolution

	Write-Output "Copying all dependencies..." 
	Copy-TestingToolAndItsDependencies
	
    If(-not ($Local)) {
        Write-Output "Cloning thesis-sample-test-project..."
        Clone-SampleProject
    } Else {
        If(-not (Test-Path $PathToTestSolution)) {
            Write-Error "Unable to finnd $PathToTestSolution."
            Write-Error "Run script with -Local switch to download SampleProject from GitHub"
			exit -1
        }
    }
        
    Write-Output "Restoring nuget packages of $PathToTestSolution...";
	&$NuGet restore $PathToTestSolution -Verbosity quiet
			
	# Create directory for output data
	If (-not (Test-Path .\dump)) {
		New-Item .\dump -ItemType Directory
	}
	
    Write-Output "Running build with analyzers on $PathToTestSolution..."
    .\bin\PerformaceTester-Roslyn.1.3.2\BugHunter.PerformanceTest.exe $PathToTestSolution /all /log:dump\SampleProject-1.3.2.txt /stats
}
Catch {
    $Error | Out-String | Write-Error
	$StackTrace | Out-String | Write-Error 
}
Finally {
	# Clean-up
	Write-Host "Deleting temporary data..."
	Remove-Item .\bin -Force -Recurse
}

Read-Host "THE END. Press ENTER to exit..." | Out-Null