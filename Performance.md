# Performance Report

Measured with `Test.RunPerformanceTest()` using `Stopwatch`, Release build, .NET 9. Results reflect repeated runs of the same workloads.

## Before caching
- Serialize single object ×10,000: 33 ms
- Serialize 100 different objects ×1,000: 3 ms
- Deserialize single object ×10,000: 43 ms
- Deserialize 100 different objects ×1,000: 3 ms
- Serialize 1,000 users: 3 ms, JSON length: 97,061 chars
- Deserialize 1,000 users: 4 ms
- Verification: 1,000 users deserialized correctly

## After caching
- Serialize single object ×10,000: 21 ms
- Serialize 100 different objects ×1,000: 3 ms
- Deserialize single object ×10,000: 35 ms
- Deserialize 100 different objects ×1,000: 3 ms
- Serialize 1,000 users: 2 ms, JSON length: 97,061 chars
- Deserialize 1,000 users: 6 ms
- Verification: 1,000 users deserialized correctly

## Summary
Caching reduced repeated reflection overhead, with the clearest gain in single-object serialization (~36% faster). Deserialization still spends most time in parsing and object creation, but repeated type metadata lookup is now avoided.