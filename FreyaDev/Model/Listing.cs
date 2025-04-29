using System.Text.Json.Serialization;

namespace FreyaDev.Model;

//TODO: handle possible null values (plant?)
//TODO: lehet nem kell a flattening

public class Listing
{
    [JsonPropertyName("listing_id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("media")]
    public List<string> Media { get; set; } = new();

    //get the first image of the image list to display as the thumnail (when viewing listings in a list view).
    public string Thumbnail => Media?.FirstOrDefault();

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    public string CreatedAtFormatted => ConverterUtil.GetRelativeTime(CreatedAt);

    [JsonPropertyName("user")]
    public ListingUser User { get; set; }

    [JsonIgnore]
    public string Username => User?.Username;

    [JsonPropertyName("plant")]
    public Plant Plant { get; set; }

    [JsonIgnore]
    public string PlantName => Plant?.Name ?? "";

    [JsonIgnore]
    public string PlantType => Plant?.Type ?? "";

    [JsonPropertyName("stage")]
    public Stage Stage { get; set; }

    [JsonIgnore]
    public string StageName => Stage?.Name ?? "";

    [JsonPropertyName("user_plant")]
    public UserPlant UserPlant { get; set; }
}

public class ListingUser
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }
}

public class UserPlant
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }
}


[JsonSerializable(typeof(List<Listing>))]
internal sealed partial class ListingContext : JsonSerializerContext
{
}
