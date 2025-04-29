using System.Text.Json.Serialization;
namespace FreyaDev.Model;

internal class PostPatchListingDataJsonConverter : JsonConverter<IPostPatchListingData>
{
    public override IPostPatchListingData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var data = doc.RootElement;
            Debug.WriteLine($"Root: {data}");

            // If "Data" is an empty array or object, return an EmptyPostPatchListingData instance
            if ((data.ValueKind == JsonValueKind.Object && !data.EnumerateObject().Any())
                || (data.ValueKind == JsonValueKind.Array && data.GetArrayLength() == 0))
            {
                return new EmptyPostPatchListingData();
            }

            // If "Errors" key exists, it's ValidationErrorData
            if (data.TryGetProperty("errors", out _))
            {
                return JsonSerializer.Deserialize<PostPatchListingValidationErrorData>(data.GetRawText(), options);
            }

            // If "listing_id" key exists, it's PostPatchListingSuccessData
            else if (data.TryGetProperty("listing_id", out _))
            {
                try
                {
                    // Deserialize the listing data directly
                    var listing = JsonSerializer.Deserialize<Listing>(data.GetRawText(), options);
                    return new PostPatchListingSuccessData { Listing = listing };
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Error deserializing listing data: {ex}");
                    return new EmptyPostPatchListingData();
                }
            }
        }

        // Default case
        return new EmptyPostPatchListingData();
    }

    public override void Write(Utf8JsonWriter writer, IPostPatchListingData value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
