using ApiRest.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRest.Repositorio.IRepositorio
{
    public interface IUsuariosSQLServer
    {
        Task<UsuarioAPI> DameUsuario(LoginAPI login);

    }
}
