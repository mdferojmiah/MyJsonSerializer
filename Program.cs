using MyJsonSerializer;


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

var res = JsonSerializer.Deserialize<Dictionary<string, Object>>(JsonSerializer.Serialize(parentUser));

foreach (var v in res!)
{
    Console.WriteLine($"{v.Key} - {v.Value}");
}
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

