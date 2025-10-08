using ApiRest.Modelo;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System;
using ApiRest.Repositorio.IRepositorio;

namespace ApiRest.Repositorio
{
    public class UsuariosSQLServer : IUsuariosSQLServer
    {
        private string CadenaConexion;
        private readonly ILogger<UsuariosSQLServer> log;
        public UsuariosSQLServer(AccesoDatos cadenaConexion, ILogger<UsuariosSQLServer> l)
        {
            CadenaConexion = cadenaConexion.CadenaConexionSQL;
            this.log = l;
        }
        private SqlConnection conexion()
        {
            return new SqlConnection(CadenaConexion);
        }

        public async Task<UsuarioAPI> DameUsuario(LoginAPI login)
        {
            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;
            UsuarioAPI u = null;
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.UsuarioAPI_Obtener";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@UsuarioAPI", SqlDbType.VarChar, 500).Value = login.usuarioAPI;
                Comm.Parameters.Add("@PassApi", SqlDbType.VarChar, 50).Value = login.passAPI;
                SqlDataReader reader = await Comm.ExecuteReaderAsync();

                if (reader.Read())
                {
                    u = new UsuarioAPI
                    {
                        Usuario = reader["UsuarioApi"].ToString(),
                        TipoCuenta = reader["TipoCuenta"].ToString(),
                    };
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                throw new Exception("Se produjo un error al obtener datos del usuario de login" + ex.Message);
            }
            finally
            {
                Comm.Dispose();
                sqlConexion.Close();
                sqlConexion.Dispose();
            }

            return u;

        }
    
    
    
    
    }
}

