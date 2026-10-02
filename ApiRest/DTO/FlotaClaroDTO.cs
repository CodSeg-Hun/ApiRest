using ApiRest.Modelo;
using System.Collections.Generic;
using static ApiRest.DTO.FlotaNotifyDTO;

namespace ApiRest.DTO
{
    public class FlotaClaroDTO
    {
        public string OrderType { get; set; }


        public List<arrayCharacteristic> arrayCharacteristic { get; set; }
    }


    public class arrayCharacteristic
    {


        public string id { get; set; }

        public string value { get; set; }

        public string valueType { get; set; }

        public string name { get; set; }

    }
}
