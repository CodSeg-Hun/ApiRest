using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using System;
using ApiRest.Modelo;

namespace ApiRest.Clases
{
    public class ConsultaDB
    {

        public static DataSet CnstGuardarData(string respuesta, string recibo, string tipo, int valido, int proceso, string reserva, string evento)
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string cadena = MyConfig.GetValue<string>("ConnectionStrings:SQL");
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(cadena))
                {
                    using (SqlCommand command = new SqlCommand("dbo.sp_negocio_carga_claro_flota", connection))
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@TIPO", SqlDbType.VarChar, 100).Value = tipo;
                        command.Parameters.Add("@RESPUESTA", SqlDbType.VarChar, 8000).Value = respuesta;
                        command.Parameters.Add("@RECIBO", SqlDbType.VarChar, 8000).Value = recibo;
                        command.Parameters.Add("@PROCESO", SqlDbType.Int).Value = proceso;
                        command.Parameters.Add("@VALIDO", SqlDbType.Int).Value = valido;
                        command.Parameters.Add("@IDRESERVA", SqlDbType.VarChar, 100).Value = reserva;
                        command.Parameters.Add("@TIPOEVENTO", SqlDbType.VarChar, 100).Value = evento;
                        var result = command.ExecuteReader();
                        ds.Load(result, LoadOption.OverwriteChanges, "Table");
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return ds;
        }


        public static DataSet ConsultaData(string chasis, int proceso)
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string cadena = MyConfig.GetValue<string>("ConnectionStrings:SQL");
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(cadena))
                {
                    using (SqlCommand command = new SqlCommand("dbo.sp_negocio_carga_claro_flota", connection))
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@CHASIS", SqlDbType.VarChar, 100).Value = chasis;
                        command.Parameters.Add("@PROCESO", SqlDbType.Int).Value = proceso;
                        var result = command.ExecuteReader();
                        ds.Load(result, LoadOption.OverwriteChanges, "Table");
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return ds;
        }


        //public static DataSet CnstData(Claro p)
        //{
        //    var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //    string cadena = MyConfig.GetValue<string>("ConnectionStrings:SQL");
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(cadena))
        //        {
        //            using (SqlCommand command = new SqlCommand("dbo.sp_negocio_carga_claroIngreso", connection))
        //            {
        //                connection.Open();
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.Add("@ORDERTYPE", SqlDbType.VarChar, 100).Value = p.OrderType;
        //                command.Parameters.Add("@TELEFONOSIM", SqlDbType.VarChar, 15).Value = p.TelefonoSIM;
        //                command.Parameters.Add("@NUMEROSIM", SqlDbType.VarChar, 50).Value = p.NumeroSIM;
        //                command.Parameters.Add("@SERIESIM", SqlDbType.VarChar, 50).Value = p.SerieSIM;
        //                command.Parameters.Add("@PLAN", SqlDbType.VarChar, 50).Value = p.Plan;
        //                command.Parameters.Add("@ESTADO", SqlDbType.VarChar, 50).Value = p.Estado;
        //                command.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = p.Fecha;
        //                command.Parameters.Add("@PROCESO", SqlDbType.Int).Value = 2;
        //                var result = command.ExecuteReader();
        //                ds.Load(result, LoadOption.OverwriteChanges, "Table");
        //                connection.Close();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw;
        //    }
        //    return ds;
        //}


        //public static DataSet CnstOrden(Claro p)
        //{
        //    var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //    string cadena = MyConfig.GetValue<string>("ConnectionStrings:SQL");
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(cadena))
        //        {
        //            using (SqlCommand command = new SqlCommand("dbo.sp_negocio_carga_claroOrden", connection))
        //            {
        //                connection.Open();
        //                command.CommandType = CommandType.StoredProcedure;
        //                command.Parameters.Add("@ORDERTYPE", SqlDbType.VarChar).Value = p.OrderType;
        //                command.Parameters.Add("@IDENTIFICACIONTYPE", SqlDbType.VarChar).Value = p.identificationType;
        //                command.Parameters.Add("@IDENTIFICACION", SqlDbType.VarChar).Value = p.identification;
        //                command.Parameters.Add("@NAME", SqlDbType.VarChar).Value = p.name;
        //                command.Parameters.Add("@INTERNTRANSACIONID", SqlDbType.VarChar).Value = p.internTransacionId;
        //                command.Parameters.Add("@providerOrderId", SqlDbType.VarChar).Value = p.providerOrderId;
        //                command.Parameters.Add("@PlanEnsamblaje", SqlDbType.VarChar).Value = p.planEnsamblaje;
        //                command.Parameters.Add("@PlanComercial", SqlDbType.VarChar).Value = p.planComercial;
        //                command.Parameters.Add("@quantity", SqlDbType.Int).Value = p.quantity;
        //                command.Parameters.Add("@PROCESO", SqlDbType.Int).Value = 2;
        //                var result = command.ExecuteReader();
        //                ds.Load(result, LoadOption.OverwriteChanges, "Table");
        //                connection.Close();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw;
        //    }
        //    return ds;
        //}
    }
}
