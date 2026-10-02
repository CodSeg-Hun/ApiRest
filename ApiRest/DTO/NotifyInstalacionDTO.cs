using ApiRest.Modelo;
using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class NotifyInstalacionDTO
    {
        public string eventTime { get; set; }

        public string eventType { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string priority { get; set; }

        public Events evento { get; set; }

        // public String<eventonotificacion> evento { get; set; }

    }
}
