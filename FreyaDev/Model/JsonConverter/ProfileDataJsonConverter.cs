using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FreyaDev.Model;

internal class ProfileDataJsonConverter : JsonConverter<IProfileData>
{
    public override IProfileData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var data = doc.RootElement;
            Debug.WriteLine($"Root: {data}");

            // If "Data" is an empty array or object, return an EmptyProfileData instance
            if ((data.ValueKind == JsonValueKind.Object && !data.EnumerateObject().Any())
                || (data.ValueKind == JsonValueKind.Array && data.GetArrayLength() == 0))
            {
                return new EmptyProfileData();
            }

            // If "Errors" key exists, it's ValidationErrorData
            if (data.TryGetProperty("errors", out _))
            {
                return JsonSerializer.Deserialize<ProfileValidationErrorData>(data.GetRawText(), options);
            }

            // If "User" key exists, it's ProfileSuccessData
            else if (data.TryGetProperty("username", out _))
            {
                try
                {
                    // Deserialize the user data directly
                    var user = JsonSerializer.Deserialize<User>(data.GetRawText(), options);
                    return new ProfileSuccessData { User = user };
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Error deserializing user data: {ex}");
                    return new EmptyProfileData();
                }
            }
        }

        // Default case
        return new EmptyProfileData();
    }

    public override void Write(Utf8JsonWriter writer, IProfileData value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
