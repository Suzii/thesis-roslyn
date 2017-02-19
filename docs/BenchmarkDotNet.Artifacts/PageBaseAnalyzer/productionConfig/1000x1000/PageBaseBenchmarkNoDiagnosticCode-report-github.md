``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-6200U CPU 2.30GHz, ProcessorCount=4
Frequency=2343751 Hz, Resolution=426.6665 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
                               Method |        Mean |    StdErr |     StdDev |         Min |         Max | Op/s | Rank |
------------------------------------- |------------ |---------- |----------- |------------ |------------ |----- |----- |
       AnalyzerV1_SyntxNodeRegistered | 774.7907 ms | 2.6283 ms |  9.8343 ms | 765.0493 ms | 797.7153 ms | 1.29 |    1 |
 AnalyzerV2_NamedTypeSymbolRegistered | 786.9164 ms | 6.0067 ms | 23.2637 ms | 761.3424 ms | 833.8461 ms | 1.27 |    1 |
