using System.Net;

namespace ApiRest.DTO
{
    public class ResponseErrorDTO
    {
        public int code { get; set; }

        public string reason { get; set; }

        public string message { get; set; }

        public string status { get; set; }

       // public HttpStatusCode StatusCode { get; set; }

    }
}
