``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Core(TM) i5-4690 CPU 3.50GHz, ProcessorCount=4
Frequency=3410166 Hz, Resolution=293.2409 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0


```
                                                                                            Method |          Mean |     StdErr |      StdDev |           Min |           Max |    Op/s | Rank |
-------------------------------------------------------------------------------------------------- |-------------- |----------- |------------ |-------------- |-------------- |-------- |----- |
                                                                                  FilesCompilation |   368.9114 us |  0.7437 us |   2.8802 us |   364.2130 us |   373.4309 us | 2710.68 |    1 |
                     V07_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolAnallysis_WithBag | 3,702.3613 us | 24.0411 us |  89.9535 us | 3,613.2671 us | 3,868.2217 us |   270.1 |    2 |
 V09_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelExecutionAndAnallysis_WithBag | 3,731.6080 us | 14.1977 us |  53.1230 us | 3,656.7472 us | 3,822.8819 us |  267.98 |    2 |
             V08_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolParallelAnallysis_WithBag | 3,803.6272 us | 26.1697 us | 101.3546 us | 3,673.0880 us | 4,072.6103 us |  262.91 |    3 |
                                                                          AnalyzerV0_EmptyCallback | 4,429.7883 us | 37.7973 us | 146.3884 us | 4,273.1562 us | 4,700.7013 us |  225.74 |    4 |
                                        V10_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysis | 4,610.1861 us | 25.0152 us |  93.5984 us | 4,469.3379 us | 4,835.4033 us |  216.91 |    5 |
                       V11_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysisWithCachedResults | 4,630.7035 us | 45.8759 us | 199.9683 us | 4,437.5880 us | 5,127.7533 us |  215.95 |    5 |
                                   V05_CompilationStartIdentifierNameAndEnd_SymbolAnalysis_WithBag | 5,680.7970 us | 37.8530 us | 146.6041 us | 5,531.8362 us | 6,013.4110 us |  176.03 |    6 |
                                                                 V02_IdentifierName_SymbolAnalysis | 5,748.0483 us | 30.7120 us | 114.9137 us | 5,579.7294 us | 6,010.2757 us |  173.97 |    6 |
