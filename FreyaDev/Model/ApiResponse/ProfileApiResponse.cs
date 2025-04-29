using System.Text.Json.Serialization;

namespace FreyaDev.Model;

public class ProfileApiResponse : IApiResponse
{
    public int Status { get; set; }
    public string Message { get; set; }

    [JsonConverter(typeof(ProfileDataJsonConverter))] // Use custom converter to convert either to ProfileData
    public IProfileData Data { get; set; }

    public ProfileApiResponse(int status, string message, IProfileData data = null)
    {
        Status = status;
        Message = message;
        Data = data ?? new EmptyProfileData();
    }
}

public interface IProfileData : IData
{
}


public class ProfileSuccessData : IProfileData
{
    [JsonInclude]
    public User User { get; set; }
}

public class EmptyProfileData : IProfileData
{
}

public class ProfileValidationErrorData : IProfileData
{
    public Dictionary<string, List<string>> Errors { get; set; }
}