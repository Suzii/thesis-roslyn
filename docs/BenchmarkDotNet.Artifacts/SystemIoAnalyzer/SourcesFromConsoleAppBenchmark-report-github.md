``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Core(TM) i5-4690 CPU 3.50GHz, ProcessorCount=4
Frequency=3410166 Hz, Resolution=293.2409 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0


```
                                                                                            Method |       Mean |    StdErr |    StdDev |        Min |        Max |   Op/s | Rank |
-------------------------------------------------------------------------------------------------- |----------- |---------- |---------- |----------- |----------- |------- |----- |
                                                                                  FilesCompilation |  1.1598 ms | 0.0024 ms | 0.0093 ms |  1.1473 ms |  1.1778 ms | 862.21 |    1 |
                     V07_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolAnallysis_WithBag | 32.1370 ms | 0.1619 ms | 0.6269 ms | 31.2465 ms | 33.3771 ms |  31.12 |    2 |
                                                                          AnalyzerV0_EmptyCallback | 35.9494 ms | 0.1270 ms | 0.4919 ms | 35.1232 ms | 36.7789 ms |  27.82 |    3 |
 V09_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelExecutionAndAnallysis_WithBag | 37.3831 ms | 0.3662 ms | 2.4011 ms | 34.3653 ms | 42.6425 ms |  26.75 |    4 |
             V08_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelAnallysis_WithBag | 38.1874 ms | 0.3816 ms | 2.0191 ms | 34.6595 ms | 42.1606 ms |  26.19 |    4 |
                       V11_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysisWithCachedResults | 40.0134 ms | 0.1360 ms | 0.5269 ms | 39.2280 ms | 40.8455 ms |  24.99 |    5 |
                                        V10_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysis | 40.8882 ms | 0.1306 ms | 0.4885 ms | 40.1164 ms | 41.7339 ms |  24.46 |    6 |
                                                                 V02_IdentifierName_SymbolAnalysis | 51.4762 ms | 0.1959 ms | 0.7329 ms | 50.4069 ms | 52.7714 ms |  19.43 |    7 |
                                   V05_CompilationStartIdentifierNameAndEnd_SymbolAnalysis_WithBag | 53.0034 ms | 0.2994 ms | 1.1595 ms | 51.5216 ms | 55.0182 ms |  18.87 |    8 |
