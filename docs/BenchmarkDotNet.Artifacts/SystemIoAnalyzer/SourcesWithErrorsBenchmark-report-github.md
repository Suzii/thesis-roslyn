``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Core(TM) i5-4690 CPU 3.50GHz, ProcessorCount=4
Frequency=3410166 Hz, Resolution=293.2409 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0


```
                                                                                            Method |           Mean |      StdErr |        StdDev |            Min |            Max |    Op/s | Rank |
-------------------------------------------------------------------------------------------------- |--------------- |------------ |-------------- |--------------- |--------------- |-------- |----- |
                                                                                  FilesCompilation |    367.2888 us |   1.1633 us |     4.5055 us |    362.9958 us |    376.1639 us | 2722.65 |    1 |
                                                                          AnalyzerV0_EmptyCallback |  3,537.7367 us |  34.5371 us |   133.7617 us |  3,382.6673 us |  3,741.0168 us |  282.67 |    2 |
                                   V05_CompilationStartIdentifierNameAndEnd_SymbolAnalysis_WithBag |  5,713.4902 us |  40.1501 us |   150.2280 us |  5,454.0322 us |  5,977.5680 us |  175.02 |    3 |
                       V11_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysisWithCachedResults |  5,802.0550 us |  20.3658 us |    78.8763 us |  5,634.0925 us |  5,928.0848 us |  172.35 |    3 |
                                        V10_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysis |  6,080.1534 us |  37.1365 us |   143.8291 us |  5,900.5769 us |  6,387.0956 us |  164.47 |    4 |
                                                                 V02_IdentifierName_SymbolAnalysis |  6,174.0453 us |  50.0214 us |   193.7321 us |  5,950.5750 us |  6,602.8984 us |  161.97 |    4 |
                     V07_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolAnallysis_WithBag |  8,672.4819 us |  33.7186 us |   130.5918 us |  8,506.1276 us |  8,960.5867 us |  115.31 |    5 |
             V08_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelAnallysis_WithBag | 23,742.8410 us | 234.0959 us | 1,404.5754 us | 21,061.6314 us | 26,350.1753 us |   42.12 |    6 |
 V09_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelExecutionAndAnallysis_WithBag | 25,672.2864 us | 225.2434 us |   872.3638 us | 23,731.2103 us | 27,010.8161 us |   38.95 |    7 |
