using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.API
{
    public class APIException : Exception
    {
        public APIException(HttpResponseMessage message)
        {
            APIMessage = message;
        }

        public HttpResponseMessage APIMessage { get; set; }

        public Exception Inner { get; set; }
    }
}
