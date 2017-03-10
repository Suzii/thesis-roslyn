``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Core(TM) i5-4690 CPU 3.50GHz, ProcessorCount=4
Frequency=3410166 Hz, Resolution=293.2409 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0


```
                                                                        Method |        Mean |    StdErr |     StdDev |         Min |         Max | Op/s | Rank |
------------------------------------------------------------------------------ |------------ |---------- |----------- |------------ |------------ |----- |----- |
 V07_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolAnallysis_WithBag | 546.2620 ms | 1.9539 ms |  7.3107 ms | 536.8553 ms | 563.2618 ms | 1.83 |    1 |
                                                      AnalyzerV0_EmptyCallback | 585.1679 ms | 3.2983 ms | 12.7744 ms | 565.2064 ms | 605.8173 ms | 1.71 |    2 |
   V11_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysisWithCachedResults | 588.7483 ms | 2.0219 ms |  7.8306 ms | 578.6600 ms | 605.6318 ms |  1.7 |    2 |
                                             V02_IdentifierName_SymbolAnalysis | 623.3980 ms | 3.7437 ms | 14.4994 ms | 603.6560 ms | 650.6266 ms |  1.6 |    3 |
               V05_CompilationStartIdentifierNameAndEnd_SymbolAnalysis_WithBag | 633.6398 ms | 4.7364 ms | 18.3442 ms | 612.3931 ms | 668.6385 ms | 1.58 |    3 |
