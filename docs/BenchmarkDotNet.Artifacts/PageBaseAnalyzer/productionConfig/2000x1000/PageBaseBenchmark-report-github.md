``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i5-6200U CPU 2.30GHz, ProcessorCount=4
Frequency=2343751 Hz, Resolution=426.6665 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
                               Method |     Mean |   StdErr |   StdDev |      Min |      Max | Op/s | Rank |
------------------------------------- |--------- |--------- |--------- |--------- |--------- |----- |----- |
 AnalyzerV2_NamedTypeSymbolRegistered | 1.4889 s | 0.0033 s | 0.0129 s | 1.4694 s | 1.5107 s | 0.67 |    1 |
       AnalyzerV1_SyntxNodeRegistered | 1.4934 s | 0.0039 s | 0.0151 s | 1.4654 s | 1.5176 s | 0.67 |    1 |
