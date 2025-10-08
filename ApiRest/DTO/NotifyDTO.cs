using ApiRest.Modelo;
using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class NotifyDTO
    {
        //public string serviceOrderType { get; set; }

        //public string providerOrderId { get; set; }

        //public string executionDate { get; set; }

        //public List<serviceOrderItem> serviceOrderItems { get; set; }

        public eventos evento { get; set; }

        public string eventTime { get; set; }

        public string eventType { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string priority { get; set; }
    }



    public class eventos
    {
        public serviceOrderes serviceOrder { get; set; }

    }

    public class serviceOrderes
    {
        public string externalId { get; set; }

        public string description { get; set; }

        public string requestedCompletionDate { get; set; }

        public string notificationContact { get; set; }

    }
}
