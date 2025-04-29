using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace FreyaDev.Model;

//a converter that deserializes Data into the correct type(LoginData or ValidationErrorData).
//this is needed becaese our apiresponses data can be of both types, we dont know which before deserialising.

internal class LoginDataJsonConverter : JsonConverter<ILoginData>
{
    public override ILoginData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var data = doc.RootElement;
            Debug.WriteLine($"Root: {data}");

            // If "Data" is an empty array or object, return an EmptyLoginData instance
            if ((data.ValueKind == JsonValueKind.Object && !data.EnumerateObject().Any()) 
                || (data.ValueKind == JsonValueKind.Array && data.GetArrayLength() == 0))
            {
                return new EmptyLoginData();
            }

            // If "Errors" key exists, it's ValidationErrorData
            if (data.TryGetProperty("errors", out _))
            {
                return JsonSerializer.Deserialize<LoginValidationErrorData>(data.GetRawText(), options);
            }

            // If "User" key exists, it's LoginSuccessData
            else if (data.TryGetProperty("user", out _))
            {
                return JsonSerializer.Deserialize<LoginSuccessData>(data.GetRawText(), options);
            }
        }

        // Default case
        return new EmptyLoginData(); 
    }

    public override void Write(Utf8JsonWriter writer, ILoginData value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}



