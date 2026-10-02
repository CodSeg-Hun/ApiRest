using ApiRest.Modelo;
using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class EventoDTO
    {
       
        public evento evento { get; set; }

        public string eventId { get; set; }

        public string eventTime { get; set; }

        public string eventType { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string priority { get; set; }

    }
}
