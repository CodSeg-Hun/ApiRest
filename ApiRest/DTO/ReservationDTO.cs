using ApiRest.Modelo;
using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class ReservationDTO
    {
        public string description { get; set; }

        public relatedParty relatedParty { get; set; }

        public string reservationState { get; set; }

        public string valid_for { get; set; }

        public List<reservationItem> reservationItem { get; set; }

        public string id { get; set; }

    }
}
