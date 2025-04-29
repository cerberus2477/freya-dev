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

internal class RegisterDataJsonConverter : JsonConverter<IRegisterData>
{
    public override IRegisterData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var data = doc.RootElement;
            Debug.WriteLine($"Converting Data: {data}");


            // If "Errors" key exists, it's ValidationErrorData
            if (data.TryGetProperty("errors", out _))
            {
                return JsonSerializer.Deserialize<RegisterValidationErrorData>(data.GetRawText(), options);
            }

            // If "User" key exists, it's LoginSuccessData
            else if (data.TryGetProperty("user", out _))
            {
                return JsonSerializer.Deserialize<RegisterSuccessData>(data.GetRawText(), options);
            }
        }

        //TODO: Default case ?
        return new RegisterValidationErrorData();
    }

    public override void Write(Utf8JsonWriter writer, IRegisterData value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}



