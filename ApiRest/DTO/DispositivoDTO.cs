using ApiRest.Modelo;
using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class DispositivoDTO
    {
        public List<results> results { get; set; }

        public string id { get; set; }

        public string status { get; set; }

        public string message { get; set; }

        public string fecha { get; set; }

    }
}
