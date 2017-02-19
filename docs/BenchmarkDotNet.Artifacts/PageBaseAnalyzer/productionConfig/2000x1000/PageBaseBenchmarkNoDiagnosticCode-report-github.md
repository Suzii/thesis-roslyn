``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-6200U CPU 2.30GHz, ProcessorCount=4
Frequency=2343751 Hz, Resolution=426.6665 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
                               Method |     Mean |   StdErr |   StdDev |      Min |      Max | Op/s | Rank |
------------------------------------- |--------- |--------- |--------- |--------- |--------- |----- |----- |
 AnalyzerV2_NamedTypeSymbolRegistered | 1.2202 s | 0.0033 s | 0.0130 s | 1.1999 s | 1.2478 s | 0.82 |    1 |
       AnalyzerV1_SyntxNodeRegistered | 1.2574 s | 0.0034 s | 0.0133 s | 1.2389 s | 1.2845 s |  0.8 |    2 |
