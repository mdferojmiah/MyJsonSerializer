using MyJsonSerializer;



// var person = new Person { Name = "Feroj" };
// person.Friend = person;
// Console.WriteLine(JsonSerializer.Serialize(person));


string userJson = "{\"Id\":1,\"Name\":\"John\",\"IsActive\":true}";
var user = JsonSerializer.Deserialize<User>(userJson);
Console.WriteLine($"User: Id={user!.Id}, Name={user.Name}, Active={user.IsActive}");



string userWithNullJson = "{\"Id\":2,\"Name\":null,\"IsActive\":false}";
var userNull = JsonSerializer.Deserialize<User>(userWithNullJson);
Console.WriteLine($"User with null: Id={userNull!.Id}, Name={userNull.Name ?? "null"}, Active={userNull.IsActive}");



string parentJson = @"{
    ""ParentId"": 100,
    ""ParentName"": ""Parent 1"",
    ""Users"": [
        { ""Id"": 1, ""Name"": ""John"", ""IsActive"": true },
        { ""Id"": 2, ""Name"": ""Jane"", ""IsActive"": false },
        { ""Id"": 3, ""Name"": ""Bob"", ""IsActive"": true }
    ]
}";



var parent = JsonSerializer.Deserialize<ParentUser>(parentJson);
Console.WriteLine($"Parent: Id={parent!.ParentId}, Name={parent.ParentName}");
if (parent.Users != null)
{
    Console.WriteLine($"Number of users: {parent.Users.Count}");
    foreach (var u in parent.Users)
    {
        Console.WriteLine($"  - User: Id={u.Id}, Name={u.Name}, Active={u.IsActive}");
    }
}



string parentEmptyJson = @"{
    ""ParentId"": 101,
    ""ParentName"": ""Parent 2"",
    ""Users"": []
}";



var parentEmpty = JsonSerializer.Deserialize<ParentUser>(parentEmptyJson);
Console.WriteLine($"Parent empty: Id={parentEmpty!.ParentId}, Name={parentEmpty.ParentName}");
Console.WriteLine($"Users count: {parentEmpty.Users?.Count ?? 0}");



string parentNullJson = @"{
    ""ParentId"": 102,
    ""ParentName"": ""Parent 3"",
    ""Users"": null
}";



var parentNull = JsonSerializer.Deserialize<ParentUser>(parentNullJson);
Console.WriteLine($"Parent null: Id={parentNull!.ParentId}, Name={parentNull.ParentName}");
Console.WriteLine($"Users is null: {parentNull.Users == null}");
Console.WriteLine();



Console.WriteLine(JsonSerializer.Serialize(null));
Console.WriteLine(JsonSerializer.Serialize("John"));
Console.WriteLine(JsonSerializer.Serialize(true));
Console.WriteLine(JsonSerializer.Serialize(false));
Console.WriteLine(JsonSerializer.Serialize(10));
Console.WriteLine(JsonSerializer.Serialize(100L));
Console.WriteLine(JsonSerializer.Serialize(10.5));
Console.WriteLine(JsonSerializer.Serialize(99.99m));
Console.WriteLine();



Console.WriteLine(JsonSerializer.Serialize("Hello \"John\""));
Console.WriteLine(JsonSerializer.Serialize("C:\\temp\\file"));
Console.WriteLine(JsonSerializer.Serialize("Line1\nLine2"));
Console.WriteLine(JsonSerializer.Serialize("Tab\tseparated"));
Console.WriteLine();



Console.WriteLine(JsonSerializer.Serialize(new List<int>(){1, 2, 3}));
Console.WriteLine(JsonSerializer.Serialize(new List<string> { "John", "Jane", "Bob" }));

List<List<double>> doubles = new List<List<double>>
{
    new List<double>{1.1, 2.5, 3.33},
    new List<double>{5.0,6.7,7.4}
};
Console.WriteLine(JsonSerializer.Serialize(doubles));

List<User> users = new List<User> 
{ 
    new User { Id = 1, Name = "John", IsActive = true},
    new User { Id = 2, Name = "Jane", IsActive = false}
};
Console.WriteLine(JsonSerializer.Serialize(users)); 
Console.WriteLine();





var user1 = new User
{
    Id = 1,
    Name = "Feroj",
    IsActive = true
};
var user2 = new User
{
    Id = 2,
    Name = "Atik",
    IsActive = true
};
var parentUser = new ParentUser
{
    ParentId = 101,
    ParentName = "Zorna Begum",
    Users = new List<User>{user1, user2}
};
Console.WriteLine(JsonSerializer.Serialize(user1));
Console.WriteLine(JsonSerializer.Serialize(parentUser));
Console.WriteLine();





var dict1 = new Dictionary<string, object>
{
    ["Name"] = "John",
    ["Age"] = 25,
    ["IsActive"] = true
};
Console.WriteLine(JsonSerializer.Serialize(dict1));

var dict2 = new Dictionary<string, object>
{
    ["User"] = new User { Id = 1, Name = "John" },
    ["Active"] = true
};
Console.WriteLine(JsonSerializer.Serialize(dict2));

var dict3 = new Dictionary<int, string>
{
    [1] = "One",
    [2] = "Two"
};
Console.WriteLine(JsonSerializer.Serialize(dict3));







class ParentUser
{
    public int ParentId { get; set; }
    public string? ParentName { get; set; }
    public List<User>? Users { get; set; }
}

class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
}

class Person
{
    public string? Name { get; set; }
    public Person? Friend { get; set; }
}
