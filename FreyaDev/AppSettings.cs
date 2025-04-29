using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreyaDev
{
    public static class AppSettings
    {
        //később ezt át kell írni dev tunnelinggel hogy elérhető legyen telefonról is
        public static string ApiBaseUrl { get; } = "http://127.0.0.1:8069/api/";
    }
}
