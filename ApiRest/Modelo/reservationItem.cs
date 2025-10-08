using System.Collections.Generic;

namespace ApiRest.Modelo
{
    public class reservationItem
    {

        public int quantity { get; set; }

        public List<itemCharacteristic> itemCharacteristic { get; set; }

    }
}
