using System.Text.Json.Serialization;

namespace FreyaDev.Model;

public class Plant
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    //TODO: optionally latin name and the other fields? might not work like this (or at least with the current jsonoptions in service)
}


[JsonSerializable(typeof(List<Plant>))]
internal sealed partial class PlantContext : JsonSerializerContext
{
}
