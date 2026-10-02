using ApiRest.Modelo;
using System.Collections.Generic;
using static ApiRest.DTO.FlotaDispositivoDTO;

namespace ApiRest.DTO
{
    public class FlotaDispositivoAltaDTO
    {
        public List<resultsAltas> results { get; set; }

        public string id { get; set; }

        public string status { get; set; }

        public string message { get; set; }

        public string fecha { get; set; }




        public class resultsAltas
        {
            public List<ItemsDetailAltas> ItemsDetail { get; set; }


        }

        public class ItemsDetailAltas
        {
            public List<ItemsAltas> items { get; set; }


        }


        public class ItemsAltas
        {

            public string id { get; set; }

            public string name { get; set; }

            public string value { get; set; }


        }
    }
}
