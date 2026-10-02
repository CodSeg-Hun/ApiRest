using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class FlotaDispositivoDTO
    {

        public string serviceType { get; set; }

        public string serviceOrderType { get; set; }

        public string orderId { get; set; }

        public string providerOrderId { get; set; }

        public List<servicesOrderItemsAlta> serviceOrderItems { get; set; }


        public class servicesOrderItemsAlta
        {

            public List<itemsAlta> itemId { get; set; }

        }


        public class itemsAlta  
        {


            public string type { get; set; }

            public string value { get; set; }

            public string dataType { get; set; }

        }


    }
}
