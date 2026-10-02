using System.Collections.Generic;

namespace ApiRest.Modelo
{
    public class serviceOrderItem
    {
        //public List<itemsId> itemId { get; set; }

        public string id { get; set; }

        public string action { get; set; }

        public service service { get; set; }

    }
}
