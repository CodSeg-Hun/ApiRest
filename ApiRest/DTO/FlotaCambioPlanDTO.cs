using ApiRest.Modelo;
using System.Collections.Generic;
using static ApiRest.DTO.FlotaVentaDTO;

namespace ApiRest.DTO
{
    public class FlotaCambioPlanDTO
    {
        public customerInformacionCambioPlan customerInfo { get; set; }

        public bool serviceEnabled { get; set; }

        public List<serviceOrderItemsCambioPlan> serviceOrderItems { get; set; }


        public string serviceOrderType { get; set; }


        public string serviceType { get; set; }


        public class customerInformacionCambioPlan
        {

            public string identificationType { get; set; }

            public string Identification { get; set; }

            public string name { get; set; }


        }



        public class serviceOrderItemsCambioPlan
        {

            public string action { get; set; }

            public string description { get; set; }

            public string name { get; set; }

            public List<itemIdCambioPlan> itemId { get; set; }

            public int quantity { get; set; }


        }



        public class itemIdCambioPlan
        {

            public string dataType { get; set; }

            public string type { get; set; }

            public string value { get; set; }


        }



    }
}
