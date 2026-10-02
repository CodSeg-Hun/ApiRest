using System.Collections.Generic;

namespace ApiRest.DTO
{
    public class FlotaNotifyDTO
    {

        public string OrderType { get; set; }

        public string orderId { get; set; }

        public string providerOrderId { get; set; }

        public List<OrderDetailNotify> OrderDetail { get; set; }

        public List<CharacteristicsNotify> arrayCharacteristic { get; set; }


        public class OrderDetailNotify
        {

            public string TelefonoSIM { get; set; } = "";


            public List<CharacteristicsNotify> Characteristics { get; set; }

        }


        public class CharacteristicsNotify
        {


            public string id { get; set; }

            public string value { get; set; }

            public string valueType { get; set; }

            public string name { get; set; }

        }
    }
}
