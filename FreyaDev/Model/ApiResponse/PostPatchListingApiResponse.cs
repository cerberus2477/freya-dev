
using System.Text.Json.Serialization;

namespace FreyaDev.Model;

public class PostPatchListingApiResponse : IApiResponse
{
    public int Status { get; set; }
    public string Message { get; set; }

    [JsonConverter(typeof(PostPatchListingDataJsonConverter))] // Use custom converter to convert either to ProfileData
    public IPostPatchListingData Data { get; set; }

    public PostPatchListingApiResponse(int status, string message, IPostPatchListingData data = null)
    {
        Status = status;
        Message = message;
        Data = data ?? new EmptyPostPatchListingData();
    }
}

public interface IPostPatchListingData : IData
{
}


public class PostPatchListingSuccessData : IPostPatchListingData
{
    [JsonInclude]
    public Listing Listing { get; set; }
}

public class EmptyPostPatchListingData : IPostPatchListingData
{
}

public class PostPatchListingValidationErrorData : IPostPatchListingData
{
    public Dictionary<string, List<string>> Errors { get; set; }
}
