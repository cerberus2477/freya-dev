using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FreyaDev.Model
{
    public class UserApiResponse : IApiResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }

        [JsonConverter(typeof(UserDataJsonConverter))] // Use custom converter to convert either to UserData
        public IUserData Data { get; set; }

        public UserApiResponse(int status, string message, IUserData data = null)
        {
            Status = status;
            Message = message;
            Data = data ?? new EmptyUserData();
        }
    }
    public interface IUserData : IData
    {
    }


    public class UserSuccessData : IUserData
    {
        [JsonInclude]
        public User User { get; set; }
    }

    public class EmptyUserData : IUserData
    {
    }

    public class UserValidationErrorData : IUserData
    {
        public Dictionary<string, List<string>> Errors { get; set; }
    }

}
