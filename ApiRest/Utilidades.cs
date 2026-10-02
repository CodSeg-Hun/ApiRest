using ApiRest.DTO;
using ApiRest.Modelo;
using System.Numerics;

namespace ApiRest
{
    public static class Utilidades
    {

        public static UsuarioDTO convertirDTO(this UsuarioAPI u)
        {
            if (u != null)
            {
                return new UsuarioDTO
                {
                    Token = u.Token,
                    Usuario = u.Usuario
                };
            }
            return null;
        }


    }
}
