using System.Diagnostics;

namespace MyJsonSerializer;

public static class Test
{
    #region PerformanceTest
    public static void RunPerformanceTest()
    {
        Console.WriteLine("========== PERFORMANCE TESTS ==========\n");
        
        RunSerializationPerformanceTest();
    
        RunDeserializationPerformanceTest();
        
        RunLargeCollectionPerformanceTest();
    }
    
    #region PrivateMethods
    private static void RunSerializationPerformanceTest()
    {
        Console.WriteLine("--- Serialization Performance ---");
        
        var user = new User 
        { 
            Id = 1, 
            Name = "John Doe", 
            IsActive = true,
            Address = new Address { Street = "123 Main St", City = "Boston" }
        };
        
        // Warmup
        for (int i = 0; i < 100; i++)
        {
            JsonSerializer.Serialize(user);
        }
        
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            JsonSerializer.Serialize(user);
        }
        stopwatch.Stop();
        Console.WriteLine($"  Serialized single object 10,000 times: {stopwatch.ElapsedMilliseconds}ms");
        
        var users = new List<User>();
        for (int i = 0; i < 100; i++)
        {
            users.Add(new User 
            { 
                Id = i, 
                Name = $"User {i}", 
                IsActive = i % 2 == 0,
                Address = new Address { Street = $"Street {i}", City = $"City {i}" }
            });
        }
        
        stopwatch.Restart();
        for (int i = 0; i < 1000; i++)
        {
            JsonSerializer.Serialize(users[i % users.Count]);
        }
        stopwatch.Stop();
        Console.WriteLine($"  Serialized 100 different objects 1,000 times: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine();
    }
    
    private static void RunDeserializationPerformanceTest()
    {
        Console.WriteLine("--- Deserialization Performance ---");
        
        var user = new User 
        { 
            Id = 1, 
            Name = "John Doe", 
            IsActive = true,
            Address = new Address { Street = "123 Main St", City = "Boston" }
        };
        string json = JsonSerializer.Serialize(user);
        
        // Warmup
        for (int i = 0; i < 100; i++)
        {
            JsonSerializer.Deserialize<User>(json);
        }
        
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            JsonSerializer.Deserialize<User>(json);
        }
        stopwatch.Stop();
        Console.WriteLine($"  Deserialized single object 10,000 times: {stopwatch.ElapsedMilliseconds}ms");
        
        var jsonList = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            var u = new User 
            { 
                Id = i, 
                Name = $"User {i}", 
                IsActive = i % 2 == 0,
                Address = new Address { Street = $"Street {i}", City = $"City {i}" }
            };
            jsonList.Add(JsonSerializer.Serialize(u));
        }
        
        stopwatch.Restart();
        for (int i = 0; i < 1000; i++)
        {
            JsonSerializer.Deserialize<User>(jsonList[i % jsonList.Count]);
        }
        stopwatch.Stop();
        Console.WriteLine($"  Deserialized 100 different objects 1,000 times: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine();
    }
    
    private static void RunLargeCollectionPerformanceTest()
    {
        Console.WriteLine("--- Large Collection Performance ---");
        
        var users = new List<User>();
        for (int i = 0; i < 1000; i++)
        {
            users.Add(new User 
            { 
                Id = i, 
                Name = $"User {i}", 
                IsActive = i % 2 == 0,
                Address = new Address { Street = $"Street {i}", City = $"City {i}" }
            });
        }
        
        var stopwatch = Stopwatch.StartNew();
        string json = JsonSerializer.Serialize(users);
        stopwatch.Stop();
        Console.WriteLine($"  Serialized 1,000 users: {stopwatch.ElapsedMilliseconds}ms, JSON length: {json.Length} chars");
        
        stopwatch.Restart();
        var deserializedUsers = JsonSerializer.Deserialize<List<User>>(json);
        stopwatch.Stop();
        Console.WriteLine($"  Deserialized 1,000 users: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Verification: {deserializedUsers?.Count} users deserialized correctly");
        Console.WriteLine();
    }
    #endregion
    
    #endregion


    #region FunctionalityTest
    public static void RunFunctionalityTest()
    {
        Console.WriteLine("========== FUNCTIONALITY TESTS ==========\n");
        
        Console.WriteLine("--- Primitive Types ---");
        TestPrimitiveTypes();
        Console.WriteLine();
        
        Console.WriteLine("--- Objects ---");
        TestObjects();
        Console.WriteLine();
        
        Console.WriteLine("--- Collections ---");
        TestCollections();
        Console.WriteLine();
        
        Console.WriteLine("--- Dictionaries ---");
        TestDictionaries();
        Console.WriteLine();
        
        Console.WriteLine("--- Special Types ---");
        TestSpecialTypes();
        Console.WriteLine();
        
        Console.WriteLine("--- Nullable Types ---");
        TestNullableTypes();
        Console.WriteLine();
        
        Console.WriteLine("--- Nested Objects ---");
        TestNestedObjects();
        Console.WriteLine();
        
        Console.WriteLine("--- Edge Cases ---");
        TestEdgeCases();
        Console.WriteLine();

        Console.WriteLine("--- Circular Reference Detection ---");
        TestCircularReference();
        Console.WriteLine();
    }


    #region PrivateMethods
    private static void TestPrimitiveTypes()
    {
        Console.WriteLine("  Serializing primitives:");
        Console.WriteLine($"    null: {JsonSerializer.Serialize(null)}");
        Console.WriteLine($"    string \"John\": {JsonSerializer.Serialize("John")}");
        Console.WriteLine($"    bool true: {JsonSerializer.Serialize(true)}");
        Console.WriteLine($"    bool false: {JsonSerializer.Serialize(false)}");
        Console.WriteLine($"    int 42: {JsonSerializer.Serialize(42)}");
        Console.WriteLine($"    long 100L: {JsonSerializer.Serialize(100L)}");
        Console.WriteLine($"    float 10.5f: {JsonSerializer.Serialize(10.5f)}");
        Console.WriteLine($"    double 10.5: {JsonSerializer.Serialize(10.5)}");
        Console.WriteLine($"    decimal 99.99m: {JsonSerializer.Serialize(99.99m)}");
        
        Console.WriteLine("  Deserializing primitives:");
        var json = "\"Hello World\"";
        var result = JsonSerializer.Deserialize<string>(json);
        Console.WriteLine($"    Deserialize(\"Hello World\") -> {result}");
        
        json = "true";
        var boolResult = JsonSerializer.Deserialize<bool>(json);
        Console.WriteLine($"    Deserialize(true) -> {boolResult}");
        
        json = "42";
        var intResult = JsonSerializer.Deserialize<int>(json);
        Console.WriteLine($"    Deserialize(42) -> {intResult}");
        
        json = "10.5";
        var doubleResult = JsonSerializer.Deserialize<double>(json);
        Console.WriteLine($"    Deserialize(10.5) -> {doubleResult}");
    }
    
    private static void TestObjects()
    {
        var user = new User 
        { 
            Id = 1, 
            Name = "John Doe", 
            IsActive = true,
            Address = new Address { Street = "123 Main St", City = "Boston" }
        };
        string json = JsonSerializer.Serialize(user);
        Console.WriteLine($"  Serialized User: {json}");
        
        var deserialized = JsonSerializer.Deserialize<User>(json);
        Console.WriteLine($"  Deserialized User: Id={deserialized?.Id}, Name={deserialized?.Name}, " +
                         $"IsActive={deserialized?.IsActive}, Address={deserialized?.Address?.Street}, " +
                         $"{deserialized?.Address?.City}");
    }
    
    private static void TestCollections()
    {
        Console.WriteLine("  Arrays:");
        int[] numbers = { 1, 2, 3, 4, 5 };
        string json = JsonSerializer.Serialize(numbers);
        Console.WriteLine($"    Serialized int[]: {json}");
        var deserialized = JsonSerializer.Deserialize<int[]>(json);
        Console.WriteLine($"    Deserialized int[]: [{string.Join(", ", deserialized ?? Array.Empty<int>())}]");
        
        Console.WriteLine("  Lists:");
        List<string> names = new List<string> { "John", "Jane", "Bob" };
        json = JsonSerializer.Serialize(names);
        Console.WriteLine($"    Serialized List<string>: {json}");
        var deserializedList = JsonSerializer.Deserialize<List<string>>(json);
        Console.WriteLine($"    Deserialized List<string>: [{string.Join(", ", deserializedList ?? new List<string>())}]");
        
        Console.WriteLine("  List of objects:");
        var users = new List<User>
        {
            new User { Id = 1, Name = "John", IsActive = true },
            new User { Id = 2, Name = "Jane", IsActive = false }
        };
        json = JsonSerializer.Serialize(users);
        Console.WriteLine($"    Serialized List<User>: {json}");
        var deserializedUsers = JsonSerializer.Deserialize<List<User>>(json);
        Console.WriteLine($"    Deserialized List<User>: Count={deserializedUsers?.Count}");
        if (deserializedUsers != null)
        {
            foreach (var u in deserializedUsers)
            {
                Console.WriteLine($"      - Id={u.Id}, Name={u.Name}, Active={u.IsActive}");
            }
        }
        
        Console.WriteLine("  Empty collections:");
        var emptyList = new List<int>();
        json = JsonSerializer.Serialize(emptyList);
        Console.WriteLine($"    Serialized empty List: {json}");
        var deserializedEmpty = JsonSerializer.Deserialize<List<int>>(json);
        Console.WriteLine($"    Deserialized empty List: Count={deserializedEmpty?.Count ?? 0}");
    }
    
    private static void TestDictionaries()
    {
        Console.WriteLine("  Dictionary<string, object>:");
        var dict = new Dictionary<string, object>
        {
            ["Name"] = "John",
            ["Age"] = 25,
            ["IsActive"] = true,
            ["Address"] = new Dictionary<string, object>
            {
                ["Street"] = "123 Main St",
                ["City"] = "Boston"
            }
        };
        string json = JsonSerializer.Serialize(dict);
        Console.WriteLine($"    Serialized: {json}");
        
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        Console.WriteLine($"    Deserialized: Name={deserialized?["Name"]}, Age={deserialized?["Age"]}, " +
                         $"IsActive={deserialized?["IsActive"]}");
        if (deserialized?["Address"] is Dictionary<string, object> address)
        {
            Console.WriteLine($"    Address: {address["Street"]}, {address["City"]}");
        }
        
        Console.WriteLine("  Dictionary<int, string>:");
        var intDict = new Dictionary<int, string>
        {
            [1] = "One",
            [2] = "Two",
            [3] = "Three"
        };
        json = JsonSerializer.Serialize(intDict);
        Console.WriteLine($"    Serialized: {json}");
        var deserializedIntDict = JsonSerializer.Deserialize<Dictionary<int, string>>(json);
        Console.WriteLine($"    Deserialized: Count={deserializedIntDict?.Count}");
        if (deserializedIntDict != null)
        {
            foreach (var kvp in deserializedIntDict)
            {
                Console.WriteLine($"      {kvp.Key}: {kvp.Value}");
            }
        }
    }
    
    private static void TestSpecialTypes()
    {
        Console.WriteLine("  DateTime:");
        var date = DateTime.Now;
        string json = JsonSerializer.Serialize(date);
        Console.WriteLine($"    Serialized DateTime: {json}");
        var deserializedDate = JsonSerializer.Deserialize<DateTime>(json);
        Console.WriteLine($"    Deserialized DateTime: {deserializedDate}");
        
        Console.WriteLine("  Guid:");
        var guid = Guid.NewGuid();
        json = JsonSerializer.Serialize(guid);
        Console.WriteLine($"    Serialized Guid: {json}");
        var deserializedGuid = JsonSerializer.Deserialize<Guid>(json);
        Console.WriteLine($"    Deserialized Guid: {deserializedGuid}");
        
        Console.WriteLine("  Enum:");
        var status = Status.Active;
        json = JsonSerializer.Serialize(status);
        Console.WriteLine($"    Serialized Enum: {json}");
        var deserializedStatus = JsonSerializer.Deserialize<Status>(json);
        Console.WriteLine($"    Deserialized Enum: {deserializedStatus}");
        
        Console.WriteLine("  Object with special types:");
        var specialObj = new SpecialTypesObject
        {
            CreatedAt = DateTime.Now,
            Id = Guid.NewGuid(),
            Status = Status.Pending
        };
        json = JsonSerializer.Serialize(specialObj);
        Console.WriteLine($"    Serialized: {json}");
        var deserializedSpecial = JsonSerializer.Deserialize<SpecialTypesObject>(json);
        Console.WriteLine($"    Deserialized: CreatedAt={deserializedSpecial?.CreatedAt}, " +
                         $"Id={deserializedSpecial?.Id}, Status={deserializedSpecial?.Status}");
    }
    
    private static void TestNullableTypes()
    {
        Console.WriteLine("  Nullable int:");
        int? nullableInt = null;
        string json = JsonSerializer.Serialize(nullableInt);
        Console.WriteLine($"    Serialized null int? -> {json}");
        var deserializedNull = JsonSerializer.Deserialize<int?>(json);
        Console.WriteLine($"    Deserialized null int? -> {deserializedNull?.ToString() ?? "null"}");
        
        nullableInt = 42;
        json = JsonSerializer.Serialize(nullableInt);
        Console.WriteLine($"    Serialized 42 int? -> {json}");
        var deserializedValue = JsonSerializer.Deserialize<int?>(json);
        Console.WriteLine($"    Deserialized 42 int? -> {deserializedValue}");
        
        Console.WriteLine("  Object with nullable properties:");
        var nullableObj = new NullablePropertiesObject
        {
            Id = 1,
            Name = null,
            IsActive = true,
            Age = null
        };
        json = JsonSerializer.Serialize(nullableObj);
        Console.WriteLine($"    Serialized: {json}");
        var deserializedNullable = JsonSerializer.Deserialize<NullablePropertiesObject>(json);
        Console.WriteLine($"    Deserialized: Id={deserializedNullable?.Id}, Name={deserializedNullable?.Name ?? "null"}, " +
                         $"IsActive={deserializedNullable?.IsActive}, Age={deserializedNullable?.Age?.ToString() ?? "null"}");
    }
    
    private static void TestNestedObjects()
    {
        Console.WriteLine("  Deep nesting:");
        var company = new Company
        {
            Name = "Tech Corp",
            Address = new Address { Street = "123 Tech St", City = "Silicon Valley" },
            Departments = new List<Department>
            {
                new Department
                {
                    Name = "Engineering",
                    Manager = new User { Id = 1, Name = "John Manager", IsActive = true },
                    Employees = new List<User>
                    {
                        new User { Id = 2, Name = "Alice", IsActive = true },
                        new User { Id = 3, Name = "Bob", IsActive = false }
                    }
                },
                new Department
                {
                    Name = "Sales",
                    Manager = new User { Id = 4, Name = "Jane Manager", IsActive = true },
                    Employees = new List<User>
                    {
                        new User { Id = 5, Name = "Charlie", IsActive = true }
                    }
                }
            }
        };
        
        string json = JsonSerializer.Serialize(company);
        Console.WriteLine($"    Serialized Company (length: {json.Length} chars)");
        var deserializedCompany = JsonSerializer.Deserialize<Company>(json);
        Console.WriteLine($"    Deserialized: {deserializedCompany?.Name}, " +
                         $"Departments: {deserializedCompany?.Departments.Count}");
        if (deserializedCompany?.Departments != null)
        {
            foreach (var dept in deserializedCompany.Departments)
            {
                Console.WriteLine($"      Dept: {dept.Name}, Manager: {dept.Manager.Name}, " +
                                 $"Employees: {dept.Employees.Count}");
            }
        }
    }
    
    private static void TestEdgeCases()
    {
        Console.WriteLine("  Empty object:");
        var empty = new EmptyObject();
        string json = JsonSerializer.Serialize(empty);
        Console.WriteLine($"    Serialized empty object: {json}");
        var deserializedEmpty = JsonSerializer.Deserialize<EmptyObject>(json);
        Console.WriteLine($"    Deserialized empty object: {(deserializedEmpty != null ? "Success" : "Failed")}");
        
        Console.WriteLine("  Object with null properties:");
        var nullProps = new NullPropertiesObject
        {
            Id = 1,
            Name = null,
            IsActive = true,
            Age = null
        };
        json = JsonSerializer.Serialize(nullProps);
        Console.WriteLine($"    Serialized: {json}");
        
        Console.WriteLine("  String with special characters:");
        string specialStr = "Hello \"World\" with \\ backslash and \n newline";
        json = JsonSerializer.Serialize(specialStr);
        Console.WriteLine($"    Serialized: {json}");
        var deserializedStr = JsonSerializer.Deserialize<string>(json);
        Console.WriteLine($"    Deserialized: {deserializedStr}");
        
        Console.WriteLine("  Large number:");
        long largeNumber = 9223372036854775807;
        json = JsonSerializer.Serialize(largeNumber);
        Console.WriteLine($"    Serialized: {json}");
        var deserializedLarge = JsonSerializer.Deserialize<long>(json);
        Console.WriteLine($"    Deserialized: {deserializedLarge}");
    }
    
    private static void TestCircularReference()
{
    var node = new CircularNode { Id = 1, Name = "Root" };
    node.Child = node; // self-referencing cycle

    Console.WriteLine("  Attempting to serialize object with circular reference...");
    try
    {
        string json = JsonSerializer.Serialize(node);
        Console.WriteLine($"  Result (no detection): {json}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"  Circular reference detected: {ex.Message}");
    }

    // Two-node cycle: A -> B -> A
    var a = new CircularNode { Id = 2, Name = "A" };
    var b = new CircularNode { Id = 3, Name = "B" };
    a.Child = b;
    b.Child = a;

    Console.WriteLine("  Attempting to serialize two-node cycle...");
    try
    {
        string json = JsonSerializer.Serialize(a);
        Console.WriteLine($"  Result (no detection): {json}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"  Circular reference detected: {ex.Message}");
    }
}
    #endregion
    
    #endregion
}

#region classes
public class CircularNode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CircularNode? Child { get; set; }
}

public class NullPropertiesObject
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public int? Age { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Address? Address { get; set; }
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public enum Status
{
    Inactive,
    Pending,
    Active,
    Archived
}

public class SpecialTypesObject
{
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
    public Status Status { get; set; }
}

public class NullablePropertiesObject
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public int? Age { get; set; }
}

public class EmptyObject
{
    // No properties
}

public class Department
{
    public string Name { get; set; } = string.Empty;
    public User Manager { get; set; } = new User();
    public List<User> Employees { get; set; } = new List<User>();
}

public class Company
{
    public string Name { get; set; } = string.Empty;
    public Address Address { get; set; } = new Address();
    public List<Department> Departments { get; set; } = new List<Department>();
}

#endregion