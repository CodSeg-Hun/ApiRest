using System.Collections.Generic;

namespace ApiRest.Modelo
{
    public class serviceOrder
    {
        public string externalId { get; set; }

        public string priority { get; set; }

        public string description { get; set; }

        public string requestedStartDate { get; set; }

        public string requestedCompletionDate { get; set; }

        public List<externalReference> externalReference { get; set; }

        public List<relatedParty> relatedParty { get; set; }

        public List<serviceOrderItem> serviceOrderItem { get; set; }

    }
}
