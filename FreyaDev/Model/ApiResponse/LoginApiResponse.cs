using System.Text.Json.Serialization;

namespace FreyaDev.Model;

public class LoginApiResponse : IApiResponse
{
    public int Status { get; set; }
    public string Message { get; set; }

    [JsonConverter(typeof(LoginDataJsonConverter))] // Use custom converter to convert either to LoginData or ValidationErrorData
    public ILoginData Data { get; set; }

    public LoginApiResponse(int status, string message, ILoginData data = null)
    {
        Status = status;
        Message = message;
        Data = data ?? new EmptyLoginData();
    }
}

public interface ILoginData : IData
{
}


public class LoginSuccessData : ILoginData
{
    public User User { get; set; }
    public string Token { get; set; }
}

public class EmptyLoginData : ILoginData
{
}

public class LoginValidationErrorData : ILoginData
{
    public Dictionary<string, List<string>> Errors { get; set; }
}