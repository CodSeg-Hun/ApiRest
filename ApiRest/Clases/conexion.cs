namespace ApiRest.Clases
{
    public class conexion
    {


        private string HMACSHA256SignatureType = "HMAC-SHA256";
        private string OAuthVersion = "1.0";
        private string httpMethodPost = "POST";
        private string httpMethodGet = "GET";
        private string httpMethodPut = "PUT";
        private string httpMethodDelete = "DELETE";

        //produccion
        //private string realm = "7451241";
        //private string realmRuta = "7451241";
        //private string oauthConsumerKey = "ce63cf522b9a8693077aec7daa3f7f3b11cb7bccd42a109e2db4ddd9f2071f94";
        //private string oauthToken = "fd0f8baa2fbea3f7f095d8a560e6838ac8008f74381615a113ab025d750f2d80";
        //private string oauthConsumerSecret = "3875e1166246fda15fb8916ed35f9429bf5e4c65aa884a2d1b2a4d05d0c52228";
        //private string oauthTokenSecret = "4ecdff3d28ccedb08c69cace6f5efa0e53740a0c5a29dc18eb48c20e9daf0598";


        //desarrollo
        private string realm = "7451241_SB1";
        private string realmRuta = "7451241-sb1";
        private string oauthConsumerKey = "86a9e3376fa5b44bb5e1678dc82c588a54849877a8679583491dc493589ba397";
        private string oauthToken = "24c80face0d0a11a2ca81d1be3f89ff6e46579763651537d680d3348fb060f49";
        private string oauthConsumerSecret = "3ae711db8093ea4df105786d9c2da05065507093eeb692cbbc38f40533b01363";
        private string oauthTokenSecret = "4938d66c777b4f6cd31e3ac5209e5cd4b8433be521e0c3d04314286b91f5daf3";


        public string ObtenerDatos(string opcion)
        {
            string cadena = "";
            if (opcion == "1")
                cadena = HMACSHA256SignatureType;
            else if (opcion == "2")
                cadena = OAuthVersion;
            else if (opcion == "3")
                cadena = oauthConsumerKey;
            else if (opcion == "4")
                cadena = oauthToken;
            else if (opcion == "5")
                cadena = oauthConsumerSecret;
            else if (opcion == "6")
                cadena = oauthTokenSecret;
            else if (opcion == "7")
                cadena = httpMethodPost;
            else if (opcion == "8")
                cadena = httpMethodGet;
            else if (opcion == "9")
                cadena = realm;
            else if (opcion == "10")
                cadena = httpMethodPut;
            else if (opcion == "11")
                cadena = httpMethodDelete;
            return cadena;
        }

        public string ObtenerRuta(string script, string deploy)
        {
            string generalruta = "https://" + realmRuta + ".restlets.api.netsuite.com/app/site/hosting/restlet.nl?script=";
            string cadena = generalruta + script + "&deploy=" + deploy;

            return cadena;
        }
    }
}
