# thesis-roslyn
This repository contains BugHunter analyzers for Kentico CMS solution. They were implemented as part of the master thesis at Faculty of Informatics, Masaryk University.

## Repository structure:
- `docs/` - contains the HTML pages with online documentation available also at TODO
- `src/` - contains projects with BugHunter analyzers
- `test/` - contains tests for BugHunter analyzers and util functions from BH.Core project

## Licence
All the source codes are published under MIT licence.

## Versions
The current version of analyzers is v1.0.0, compatible with Kentico.Livraries v10.0.13 and using Microsoft.CodeAnalysis v1.3.2.
Due to Roslyn version analyzers are only compatible with Visual Studio 2015 Update 3 and higher. Update to Microsoft.CodeAnalysis v2.0, which only works in Visual Studio 2017, is currently impossible due to company restrictions at Kentico (.NET framework and Visual Studio backward compatibility).

## NuGet Availability
The NuGet packages with BugHunter analyzers are currently only available via inernal Kentico NuGet feed. Publishing to the official NuGet Gallery is considered for the future.