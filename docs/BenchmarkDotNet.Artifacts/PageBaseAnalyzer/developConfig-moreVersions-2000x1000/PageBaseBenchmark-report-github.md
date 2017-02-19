``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-6200U CPU 2.30GHz, ProcessorCount=4
Frequency=2343751 Hz, Resolution=426.6665 ns, Timer=TSC
  [Host]               : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DevelopmentBenchmark : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
                                              Method |     Mean |   StdErr |   StdDev |      Min |      Max | Op/s | Rank |
---------------------------------------------------- |--------- |--------- |--------- |--------- |--------- |----- |----- |
                      AnalyzerV1_SyntxNodeRegistered | 1.7490 s | 0.0788 s | 0.2230 s | 1.5290 s | 2.0408 s | 0.57 |    1 |
                AnalyzerV2_NamedTypeSymbolRegistered | 1.7549 s | 0.0709 s | 0.2007 s | 1.5377 s | 1.9594 s | 0.57 |    1 |
                       AnalyzerV3__NamedTypeToString | 1.7560 s | 0.0840 s | 0.2377 s | 1.5233 s | 2.0425 s | 0.57 |    1 |
 AnalyzerV4_NamedTypeToStringWithoutCompilationStart | 1.7631 s | 0.0708 s | 0.2002 s | 1.5351 s | 1.9606 s | 0.57 |    1 |
