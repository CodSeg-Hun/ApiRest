using ApiRest.Modelo;
using System;
using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class RespuestaDTO
    {
        //public int code { get; set; }

        //public string status { get; set; }

        //public string message { get; set; }

        //public long providerOrderId { get; set; }

        public string id { get; set; }

        public string externalId { get; set; }

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
