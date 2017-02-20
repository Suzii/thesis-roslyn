``` ini

BenchmarkDotNet=v0.10.1, OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Core(TM) i5-4690 CPU 3.50GHz, ProcessorCount=4
Frequency=3410136 Hz, Resolution=293.2434 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1087.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1087.0


```
                    Method |        Mean |    StdErr |    StdDev |         Min |         Max |  Op/s | Rank |
-------------------------- |------------ |---------- |---------- |------------ |------------ |------ |----- |
  AnalyzerV0_EmptyCallback |  65.5341 ms | 0.0634 ms | 0.2454 ms |  65.1309 ms |  65.9624 ms | 15.26 |    1 |
 AnalyzerV1_SingleCallback | 367.6363 ms | 0.8051 ms | 3.1183 ms | 362.1799 ms | 372.6239 ms |  2.72 |    2 |
   AnalyzerV2_TwoCallbacks | 370.0269 ms | 0.5696 ms | 2.2061 ms | 366.9141 ms | 373.5797 ms |   2.7 |    3 |
