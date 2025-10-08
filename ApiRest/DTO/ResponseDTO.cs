using ApiRest.Modelo;
using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class ResponseDTO
    {
        //public int code { get; set; }

        //public string status { get; set; }

        //public string message { get; set; }

        //public long internTransacionId { get; set; }

        //public long providerOrderId { get; set; }

        public string id { get; set; }

        public string externalId { get; set; }

        public string priority { get; set; }

        public string description { get; set; }

        public string requestedStartDate { get; set; }

        public string requestedCompletionDate { get; set; }

        public List<externalReference> externalReference { get; set; }

        public List<note> note { get; set; }

        public List<relatedParty> relatedParty { get; set; }

        public List<serviceOrderItem> serviceOrderItem { get; set; }


    }
}
