using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FreyaDev.Model;

internal class UserDataJsonConverter : JsonConverter<IUserData>
{
    public override IUserData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var data = doc.RootElement;
            Debug.WriteLine($"Root: {data}");

            // If "Data" is an empty array or object, return an EmptyUserData instance
            if ((data.ValueKind == JsonValueKind.Object && !data.EnumerateObject().Any())
                || (data.ValueKind == JsonValueKind.Array && data.GetArrayLength() == 0))
            {
                return new EmptyUserData();
            }

            // If "Errors" key exists, it's ValidationErrorData
            if (data.TryGetProperty("errors", out _))
            {
                return JsonSerializer.Deserialize<UserValidationErrorData>(data.GetRawText(), options);
            }

            // If "User" key exists, it's UserSuccessData
            else if (data.TryGetProperty("username", out _))
            {
                try
                {
                    // Deserialize the user data directly
                    var user = JsonSerializer.Deserialize<User>(data.GetRawText(), options);
                    return new UserSuccessData { User = user };
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Error deserializing user data: {ex}");
                    return new EmptyUserData();
                }
            }
        }

        // Default case
        return new EmptyUserData();
    }

    public override void Write(Utf8JsonWriter writer, IUserData value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
