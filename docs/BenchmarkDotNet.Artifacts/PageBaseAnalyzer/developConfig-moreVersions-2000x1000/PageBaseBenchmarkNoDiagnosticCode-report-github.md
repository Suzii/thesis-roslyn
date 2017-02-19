``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-6200U CPU 2.30GHz, ProcessorCount=4
Frequency=2343751 Hz, Resolution=426.6665 ns, Timer=TSC
  [Host]               : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DevelopmentBenchmark : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
                                              Method |     Mean |   StdErr |   StdDev |      Min |      Max | Op/s | Rank |
---------------------------------------------------- |--------- |--------- |--------- |--------- |--------- |----- |----- |
                      AnalyzerV1_SyntxNodeRegistered | 1.4582 s | 0.0729 s | 0.2061 s | 1.2535 s | 1.6595 s | 0.69 |    1 |
 AnalyzerV4_NamedTypeToStringWithoutCompilationStart | 1.4713 s | 0.0724 s | 0.2047 s | 1.2506 s | 1.7054 s | 0.68 |    1 |
                       AnalyzerV3__NamedTypeToString | 1.4730 s | 0.0802 s | 0.2269 s | 1.2522 s | 1.7529 s | 0.68 |    1 |
                AnalyzerV2_NamedTypeSymbolRegistered | 1.4811 s | 0.0765 s | 0.2164 s | 1.2460 s | 1.7247 s | 0.68 |    1 |
