using ApiRest.Modelo;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiRest.DTO
{
    public class CrearDTO
    {
        //public string OrderType { get; set; }

        //public List<arrayCharacteristic> arrayCharacteristic { get; set; }

        public string externalId { get; set; } = "0";

        public string priority { get; set; }

        public string description { get; set; }

        public string startDate { get; set; }

        //public string requestedCompletionDate { get; set; }

        public List<externalReference> externalReference { get; set; }

        public List<note> note { get; set; }

        public List<relatedParty> relatedParty { get; set; }

        public List<serviceOrderItem> serviceOrderItem { get; set; }

    }
}
