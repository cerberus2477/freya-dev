using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FreyaDev.Model
{
    internal class ListingsApiResponse : IApiResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }

        public List<Listing> Data { get; set; }

    }
}