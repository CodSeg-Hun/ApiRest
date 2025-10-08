using ApiRest.Controllers;
using ApiRest.DTO;
using ApiRest.Modelo;
using ApiRest.Repositorio.IRepositorio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Numerics;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApiRest.Repositorio
{
    public class ClaroSQLServer : IClaroEnMemoria
    {
        private string CadenaConexion;
        private readonly ILogger<ClaroSQLServer> log;
        //private readonly IClaroEnMemoria repositorio;
        public ClaroSQLServer(AccesoDatos cadenaConexion, ILogger<ClaroSQLServer> l)
        {
            CadenaConexion = cadenaConexion.CadenaConexionSQL;
            this.log = l;
            //this.repositorio = r;
        }
        private SqlConnection conexion()
        {
            return new SqlConnection(CadenaConexion);
        }


        //public async Task CrearProductoAsincrono(Claro p)
        //{
        //    SqlConnection sqlConexion = conexion();
        //    SqlCommand Comm = null;
        //    try
        //    {
        //        sqlConexion.Open();
        //        Comm = sqlConexion.CreateCommand();
        //        Comm.CommandText = "dbo.sp_negocio_carga_claroIngreso";
        //        Comm.CommandType = CommandType.StoredProcedure;
        //        Comm.Parameters.Add("@ORDERTYPE", SqlDbType.VarChar, 100).Value = p.OrderType;
        //        Comm.Parameters.Add("@TELEFONOSIM", SqlDbType.VarChar, 15).Value = p.TelefonoSIM;
        //        Comm.Parameters.Add("@NUMEROSIM", SqlDbType.VarChar, 50).Value = p.NumeroSIM;
        //        Comm.Parameters.Add("@SERIESIM", SqlDbType.VarChar, 50).Value = p.SerieSIM;
        //        Comm.Parameters.Add("@PLAN", SqlDbType.VarChar, 50).Value = p.Plan;
        //        Comm.Parameters.Add("@ESTADO", SqlDbType.VarChar, 50).Value = p.Estado;
        //        Comm.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = p.Fecha;
        //        Comm.Parameters.Add("@PROCESO", SqlDbType.Int).Value = 1;
        //        SqlDataReader reader = await Comm.ExecuteReaderAsync();     
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogError(ex.ToString());
        //        throw new Exception("Se produjo un error al obtener datos" + ex.Message);
        //    }
        //    finally
        //    {
        //        Comm.Dispose();
        //        sqlConexion.Close();
        //        sqlConexion.Dispose();
        //    }
        //    await Task.CompletedTask;
        //}

        //public async Task CrearOrdenAsincrono(Claro p)
        //{
        //    SqlConnection sqlConexion = conexion();
        //    SqlCommand Comm = null;
        //    try
        //    {
        //        sqlConexion.Open();
        //        Comm = sqlConexion.CreateCommand();
        //        Comm.CommandText = "dbo.sp_negocio_carga_claroOrden";
        //        Comm.CommandType = CommandType.StoredProcedure;
        //        Comm.Parameters.Add("@ORDERTYPE", SqlDbType.VarChar).Value = p.OrderType;
        //        Comm.Parameters.Add("@IDENTIFICACIONTYPE", SqlDbType.VarChar).Value = p.identificationType;
        //        Comm.Parameters.Add("@IDENTIFICACION", SqlDbType.VarChar).Value = p.identification;
        //        Comm.Parameters.Add("@NAME", SqlDbType.VarChar).Value = p.name;
        //        Comm.Parameters.Add("@INTERNTRANSACIONID", SqlDbType.VarChar).Value = p.internTransacionId;
        //        Comm.Parameters.Add("@providerOrderId", SqlDbType.VarChar).Value = p.providerOrderId;
        //        Comm.Parameters.Add("@PlanEnsamblaje", SqlDbType.VarChar).Value = p.planEnsamblaje;
        //        Comm.Parameters.Add("@PlanComercial", SqlDbType.VarChar).Value = p.planComercial;
        //        Comm.Parameters.Add("@quantity", SqlDbType.Int).Value = p.quantity;
        //        Comm.Parameters.Add("@PROCESO", SqlDbType.Int).Value = 1;
        //        SqlDataReader reader = await Comm.ExecuteReaderAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogError(ex.ToString());
        //        throw new Exception("Se produjo un error al obtener datos" + ex.Message);
        //    }
        //    finally
        //    {
        //        Comm.Dispose();
        //        sqlConexion.Close();
        //        sqlConexion.Dispose();
        //    }
        //    await Task.CompletedTask;
        //}

        //public async Task DispositivoAsincrono(Claro p)
        //{
        //    SqlConnection sqlConexion = conexion();
        //    SqlCommand Comm = null;
        //    try
        //    {
        //        sqlConexion.Open();
        //        Comm = sqlConexion.CreateCommand();
        //        Comm.CommandText = "dbo.sp_negocio_carga_claroDispositivo";
        //        Comm.CommandType = CommandType.StoredProcedure;
        //        Comm.Parameters.Add("@ORDERTYPE", SqlDbType.VarChar).Value = p.OrderType;
        //        Comm.Parameters.Add("@providerOrderId", SqlDbType.VarChar).Value = p.providerOrderId;
        //        Comm.Parameters.Add("@SerieSIM", SqlDbType.VarChar).Value = p.SerieSIM;
        //        Comm.Parameters.Add("@IMEI", SqlDbType.VarChar).Value = p.IMEI;
        //        Comm.Parameters.Add("@APN", SqlDbType.VarChar).Value = p.APN;
        //        Comm.Parameters.Add("@IP", SqlDbType.VarChar).Value = p.IP;
        //        Comm.Parameters.Add("@TelefonoSIM", SqlDbType.VarChar).Value = p.TelefonoSIM;
        //        Comm.Parameters.Add("@Estado", SqlDbType.VarChar).Value = p.Estado;
        //        Comm.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = p.Fecha;
        //        Comm.Parameters.Add("@ERROR", SqlDbType.VarChar).Value = p.Error;
        //        Comm.Parameters.Add("@PROCESO", SqlDbType.Int).Value = 1;
        //        SqlDataReader reader = await Comm.ExecuteReaderAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogError(ex.ToString());
        //        throw new Exception("Se produjo un error al obtener datos" + ex.Message);
        //    }
        //    finally
        //    {
        //        Comm.Dispose();
        //        sqlConexion.Close();
        //        sqlConexion.Dispose();
        //    }
        //    await Task.CompletedTask;
        //}
    
    }
}
