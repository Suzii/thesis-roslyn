# Performance considerations

This file contains bunch interesting links with information on testing the performance of Roslyn analyzers or on the performance considerations considerations while developing.

https://github.com/dotnet/roslyn/wiki/Performance-considerations-for-large-solution

https://github.com/code-cracker/code-cracker/issues/766
The way roslyn does it is that error and warnings analyzers run on all the project files, and info and hidden only when you open a document (I am not sure about the details). Code fixes run before you ask for them, when you have your cursor on top of a node that has a diagnostic.


http://roslyn.codeplex.com/discussions/541953

https://github.com/dotnet/roslyn-analyzers-contrib/issues/16
https://github.com/code-cracker/code-cracker/issues/766
https://github.com/dotnet/roslyn/issues/2065
https://github.com/dotnet/roslyn/issues/10464

https://github.com/DotNetAnalyzers/StyleCopAnalyzers/pull/1970

https://github.com/dotnet/roslyn/issues/670

https://github.com/dotnet/roslyn/issues/621
(CSharpPerfBuildWithThirdPartyAnalyzersTemplate) 