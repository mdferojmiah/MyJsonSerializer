# MyJsonSerializer

A custom JSON serializer/deserializer built from scratch in C#.

## Usage

No setup needed, just call `Serialize` / `Deserialize<T>`. Properties are discovered via reflection, and it works the same way for objects, collections, and dictionaries.

```csharp
using MyJsonSerializer;

var user = new User { Id = 1, Name = "John Doe", IsActive = true };

string json = JsonSerializer.Serialize(user);
// {"Id":1,"Name":"John Doe","IsActive":true}

User? result = JsonSerializer.Deserialize<User>(json);
```

Malformed JSON throws an exception with a descriptive message and character position (see [Error Handling](#error-handling)).

## Testing

`Test.cs`, run from `Program.cs`, provides manual verification output:
- `Test.RunFunctionalityTest()`: exercises every supported type, collections, dictionaries, nesting, edge cases, and circular references, printing input/output for inspection.
- `Test.RunPerformanceTest()`: runs the benchmarks behind `Performance.md`.

To run either, uncomment the corresponding call in `Program.cs` and run the project.

## Supported Types
- Primitive types: string, int, long, float, double, decimal, bool, null
- Objects (any class with public properties)
- Collections: arrays, List<T>, IEnumerable<T>
- Dictionaries: Dictionary<string, object>
- Special types: DateTime, Guid, enum, nullable types

## Design Decisions
- Recursive serialization/deserialization
- Reflection-based property discovery
- Invariant culture for numeric formatting
- Thread-safe with [ThreadStatic] for circular reference detection
- Metadata caching for performance

## Limitations
- Only public instance properties are serialized
- Fields are not serialized
- Dictionary keys are converted to strings using ToString()

## Circular Reference Behavior
Throws `InvalidOperationException` with clear message when a circular reference is detected.

## Error Handling
Provides meaningful error messages with position information for malformed JSON.