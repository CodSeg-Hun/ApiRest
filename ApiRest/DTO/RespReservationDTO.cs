using ApiRest.Modelo;
using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class RespReservationDTO
    {

        public string id { get; set; }

        public string description { get; set; }

        public relatedParty relatedParty { get; set; }

       // public Dictionary<string, relatedParty> relatedParty { get; set; } = new();

        public string reservationState { get; set; }

        public string valid_for { get; set; }

        public List<reservationItem> reservationItem { get; set; }

    }
}
