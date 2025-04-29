using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreyaDev.Model
{
    public interface IApiResponse
    {
        int Status { get; set; }
        string Message { get; set; }
        //IData Data { get; set; }
    }

    public class ApiResponse : IApiResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        //public IData Data { get; set; }
    }

    public interface IData
    {
    }

}
