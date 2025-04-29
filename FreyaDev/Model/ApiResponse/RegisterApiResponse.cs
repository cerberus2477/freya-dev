using System.Text.Json.Serialization;

namespace FreyaDev.Model;

public class RegisterApiResponse : IApiResponse
{
    public int Status { get; set; }
    public string Message { get; set; }

    [JsonConverter(typeof(RegisterDataJsonConverter))] // Use custom converter to convert either to RegisterSuccessData or ValidationErrorData
    public IRegisterData Data { get; set; }

    public RegisterApiResponse(int status, string message, IRegisterData data = null)
    {
        Status = status;
        Message = message;
        Data = data;
        //Data = data ?? new EmptyLoginData();
        //TODO: kell ide az emtpy login data? elvileg ez nem adhat vissza ilyet
    }
}


public interface IRegisterData : IData
{
}


public class RegisterSuccessData : IRegisterData
{
    public User User { get; set; }
    public string Token { get; set; }
}

public class RegisterValidationErrorData : IRegisterData
{
    public Dictionary<string, List<string>> Errors { get; set; }
}