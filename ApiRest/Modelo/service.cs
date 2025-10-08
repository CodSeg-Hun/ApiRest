using System.Collections.Generic;

namespace ApiRest.Modelo
{
    public class service
    {
        public string serviceState { get; set; }

        public string type { get; set; }

        public List<serviceCharacteristic> serviceCharacteristic { get; set; }

    }
}
