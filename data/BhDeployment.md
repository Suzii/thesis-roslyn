# BugHunter Deployment Notes
[16.3.2017]

674 warnings after initial deployment of both packages. 
* about 130 false positive errors that should have been on excluded path (ValidationHelperGetAnalyzer), amendment to implementation, errors are gone
* SeekOrigin - was additionally white listed from SystemIOAnalyzer as there was extensive usage with Streams (+/- 30 warnings down)

## SystemIOAnalyer configuration
Motivation behind SystemIO [here](https://docs.kentico.com/k10/custom-development/working-with-physical-files-using-the-api).


Most of the warnings are from SystemIO analyzer since the old BH condiguration needs to be applied.
Setting [_Action_](https://github.com/dotnet/roslyn/blob/master/docs/compilers/Rule%20Set%20Format.md) to `None` (analyzer should not be even instantiated) in `.ruleset` file of projects:
- IO, 
- AzureStorage, 
- AmazonStorage projects - setting SystemIO analyzer severity to None ()
- Search.Lucene3
- WinService
- HealthMonitoringService
- etc.

After that, 448 warnings remaining.

Then, getting rid of hackery applied to get prevent false positive results on Stream & Exception from old BH check (named import + use method from System.IO): 

Fixing bunch of previously undetected SystemIO usages (mostly by replacing with CMS.IO, occasional spppressions by pragme warnings). 


## String and culture rules
Many false negatives from old BH were fixed:
* IndexOf and whole string comparisons and manipulations - tooooo many discovered usages without StringComparison, which could not have been found by old BH
* Equals no way to find out whether it is called on string so no way to by found by old BH, also many usages detedcted by new BH

## Pragmas in helpers
In helper files that wrap the actually forbidden methods, pragmas on whole file explicitely in the code were used.
