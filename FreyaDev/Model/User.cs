using System.Text.Json.Serialization;

namespace FreyaDev.Model;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string City { get; set; }
    public string Birthdate { get; set; }

    [JsonPropertyName("role_id")]
    public int RoleId { get; set; }
    public string Picture { get; set; }
    public string Description { get; set; }
}

