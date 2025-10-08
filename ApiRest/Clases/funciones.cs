
using System;
using System.Collections.Generic;


namespace ApiRest.Clases
{
    public class funciones
    {
        internal static string descomponer(string etiqueta, string trama)
        {
            string resultado = "0";
            if (trama.IndexOf(etiqueta) > 0)
            {
                resultado = trama.Substring(trama.IndexOf(etiqueta));
                resultado = resultado.Substring(resultado.IndexOf(":") + 1, i_if(resultado.IndexOf(",") < 0, resultado.Length - 1, resultado.IndexOf(",")) - (resultado.IndexOf(":") + 1));
                resultado = resultado.Replace("\"", "").Replace("{", "").Replace("}", "").Replace("\\", "");
                resultado = resultado.Trim();
            }
            return resultado;
        }

        internal static int i_if(Boolean expresion, int c1, int c2)
        {
            int resultado = -1;
            if (expresion)
            {
                resultado = c1;
            }
            else
            {
                resultado = c2;
            }
            return resultado;
        }

        //internal static object Desencriptar(string v, string assetAMI)
        //{
        //    throw new NotImplementedException();
        //}



    }

}
