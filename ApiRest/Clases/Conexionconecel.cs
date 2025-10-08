namespace ApiRest.Clases
{
    public class Conexionconecel
    {

        public string ObtenerDatos(string opcion)
        {
            string cadena = "";
            //desarrollo
            //if (opcion == "1")
            //    cadena = "http://api-proxy.developer.gate.dev.conecel.com:9001/oauth/v2/token";
            //else if (opcion == "2")
            //    cadena = "http://api-proxy.developer.gate.dev.conecel.com:9001/api-smartcar/sale/contactInfoCreate";
            //produccion
            if (opcion == "1")
                cadena = "https://iot.conecel.com:443/oauth/v2/token";
            else if (opcion == "2")
                cadena = "https://iot.conecel.com:443/api-smartcar/sale/contactInfoCreate";
            return cadena;
        }
    }
}
