using ApiRest.Modelo;
using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class FlotaVentaDTO
    {

        public string orderType { get; set; } 

        public string orderId { get; set; }

        public string providerOrderId { get; set; }

        public customerInformacion customerInfo { get; set; }

        public List<servicesOrderItems> serviceOrderItems { get; set; }



        public class customerInformacion
        {

            public string identificationType { get; set; }

            public string Identification { get; set; }

            public string name { get; set; }

            //public string direccion { get; set; }

            //public string telefono { get; set; }

            //public string email { get; set; }

            //public string ciudad { get; set; }


        }


        public class servicesOrderItems
        {

            public List<items> itemId { get; set; }

            public int quantity { get; set; }

        }


        public class items
        {

            public int sequence { get; set; }

            public string type { get; set; }

            public string value { get; set; }

            public string dataType { get; set; }

        }



    }




}
