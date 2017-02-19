``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-6200U CPU 2.30GHz, ProcessorCount=4
Frequency=2343751 Hz, Resolution=426.6665 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
                               Method |        Mean |    StdErr |     StdDev |         Min |         Max | Op/s | Rank |
------------------------------------- |------------ |---------- |----------- |------------ |------------ |----- |----- |
       AnalyzerV1_SyntxNodeRegistered | 924.3919 ms | 2.7223 ms | 10.5435 ms | 912.1120 ms | 947.4075 ms | 1.08 |    1 |
 AnalyzerV2_NamedTypeSymbolRegistered | 926.2446 ms | 4.0504 ms | 15.6870 ms | 904.9083 ms | 953.9061 ms | 1.08 |    1 |
