using ApiRest.Clases;
using ApiRest.DTO;
using ApiRest.Modelo;
using ApiRest.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace ApiRest.Controllers
{

    [ApiController]
    [Route("Conecel")]
    [Consumes("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ConecelController : ControllerBase
    {
        private readonly IClaroEnMemoria repositorio;
        public ConecelController(IClaroEnMemoria r)
        {
            this.repositorio = r;
        }

        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [Route("/CrearOrden")]
        //[Produces("application/json")]
        public ActionResult<String> CrearOrden(CrearDTO p)
        {
            //RespuestaDTO respuesta = null;
            bool bandera = true;
            int valido = 0;
            string jsonString = "";
            string jsonRespuesta = "";
            string idreserva = "";
            string chasis = "";
            if (bandera)
            {
                RespuestaDTO respuesta = null;
                List<externalReference> externalReferenceList = new List<externalReference>();
                List<note> noteList = new List<note>();
                List<relatedParty> relatedPartyList = new List<relatedParty>();
                for (int i = 0; i < p.externalReference.Count; i++)
                {
                    if (p.externalReference[i].name == "IdReserva")
                    {
                        idreserva = p.externalReference[i].externalReferenceValue;
                    }
                    externalReferenceList.Add(new externalReference()
                    {
                        name = p.externalReference[i].name,
                        externalReferenceValue = p.externalReference[i].externalReferenceValue,

                       
                    });
                }
                for (int i = 0; i < p.note.Count; i++)
                {
                    if (p.note[i].id == "CHASIS")
                    {
                        chasis = p.note[i].text;
                    }

                    noteList.Add(new note()
                    {
                        id = p.note[i].id,
                        text = p.note[i].text,
                    });
                }
                for (int i = 0; i < p.relatedParty.Count; i++)
                {
                    relatedPartyList.Add(new relatedParty()
                    {
                        id = p.relatedParty[i].id,
                        name = p.relatedParty[i].name,
                        role = p.relatedParty[i].role,
                    });
                }
                List<serviceOrderItem> serviceOrderItemList = new List<serviceOrderItem>();
                service service = null;
                for (int i = 0; i < p.serviceOrderItem.Count; i++)
                {
                    List<serviceCharacteristic> serviceCharacteristicList = new List<serviceCharacteristic>();
                    for (int b = 0; b < p.serviceOrderItem[i].service.serviceCharacteristic.Count; b++)
                    {
                        serviceCharacteristicList = new List<serviceCharacteristic>() {
                        new serviceCharacteristic()
                            {
                                name = p.serviceOrderItem[i].service.serviceCharacteristic[b].name,
                                valueType =  p.serviceOrderItem[i].service.serviceCharacteristic[b].valueType,
                                value =  p.serviceOrderItem[i].service.serviceCharacteristic[b].value,
                            }
                        };
                        for (int c = 1; c < p.serviceOrderItem[i].service.serviceCharacteristic.Count; c++)
                        {
                            b++;
                            serviceCharacteristicList.Add(new serviceCharacteristic()
                            {
                                name = p.serviceOrderItem[i].service.serviceCharacteristic[b].name,
                                valueType = p.serviceOrderItem[i].service.serviceCharacteristic[b].valueType,
                                value = p.serviceOrderItem[i].service.serviceCharacteristic[b].value,
                            });
                        }
                        service = new service
                        {
                            serviceState = p.serviceOrderItem[i].service.serviceState,
                            type = p.serviceOrderItem[i].service.type,
                            serviceCharacteristic = serviceCharacteristicList,
                        };
                        serviceOrderItemList.Add(new serviceOrderItem()
                        {
                            id = p.serviceOrderItem[i].id,
                            action = p.serviceOrderItem[i].action,
                            service = service,
                            //itemCharacteristic = itemCharacteristicList
                        }); 
                    }                   
                }
                DataSet cnstGeneral = new DataSet();
                cnstGeneral = ConsultaDB.ConsultaData(chasis,  3);
                string verificacionchasis = (string)cnstGeneral.Tables[0].Rows[0]["CODIGO_ID"];
                if (verificacionchasis== "0")
                {
                    jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                    jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                    valido = 1;
                    DataSet cnstGenrl = new DataSet();
                    cnstGenrl = ConsultaDB.CnstGuardarData("", jsonString, "CrearOrden", valido, 1, idreserva,"");
                    p.externalId = (string)cnstGenrl.Tables[0].Rows[0]["codigo"];
                    respuesta = new RespuestaDTO
                    {
                        id = "0",
                        externalId = p.externalId,
                        priority = p.priority,
                        description = p.description,
                        startDate = p.startDate,
                        //requestedCompletionDate = p.requestedCompletionDate,
                        externalReference = externalReferenceList,
                        note = noteList,
                        relatedParty = relatedPartyList,
                        serviceOrderItem = serviceOrderItemList,
                    };
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                    cnstGenrl = ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "CrearOrden", valido, 2, idreserva, "");


                    //envio el dato de la reserva al  momento de crear la OS
                    string opcion = "3";
                    conexion ruta = new conexion();
                    //string API_URL = ruta.ObtenerRuta("4067", "1"); //desarrollo
                    string API_URL = ruta.ObtenerRuta("5276", "1"); //produccion
                    string HMACSHA256SignatureType = ruta.ObtenerDatos("1");
                    string OAuthVersion = ruta.ObtenerDatos("2");
                    var oauthConsumerKey = ruta.ObtenerDatos("3");
                    var oauthToken = ruta.ObtenerDatos("4");
                    var oauthConsumerSecret = ruta.ObtenerDatos("5");
                    var oauthTokenSecret = ruta.ObtenerDatos("6");
                    var realm = ruta.ObtenerDatos("9");
                    var httpMethod = ruta.ObtenerDatos("8");
                    OAuthBase auth = new OAuthBase();
                    var timestamp = Clases.OAuthBase.GenerateTimeStamp();
                    var nonce = Clases.OAuthBase.GenerateNonce();
                    //API_URL = API_URL + "&opcion=" + opcion + "&idcliente=" + p.relatedParty.id;
                    API_URL = API_URL + "&opcion=" + opcion + "&id=" + idreserva;
                    var client = new RestClient(API_URL);
                    var request = new RestRequest("", Method.Get);
                    Uri url = new Uri(API_URL);
                    var signature = Clases.OAuthBase.GenerateSignature(url, oauthConsumerKey, oauthConsumerSecret, oauthToken, oauthTokenSecret, httpMethod, timestamp, nonce);
                    request.AddHeader("Authorization", "OAuth realm=\"" + realm + "\", oauth_token=\"" + oauthToken + "\", oauth_consumer_key=\"" + oauthConsumerKey + "\"," + " oauth_nonce=\"" + nonce + "\", oauth_timestamp=\"" + timestamp + "\", oauth_signature_method=\"" + HMACSHA256SignatureType + "\", oauth_version=\"" + OAuthVersion + "\", oauth_signature=\"" + signature + "\"");
                    var response = client.Execute(request);
                    Console.WriteLine(response.Content);
                    DispositivoDTO myObj = JsonConvert.DeserializeObject<DispositivoDTO>(response.Content);
                    if (myObj.status == "200")
                    {
                        //jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                        //cnstGenrl = ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "CrearOrden", valido, 2, idreserva);
                    }

                    return Ok(respuesta);
                } else
                {
                    ResponseErrorDTO error = null;
                    error = new ResponseErrorDTO
                    {
                        code = -1,
                        reason = "MISSING_REQUIRED_TAG",
                        message = "Error, Ya existe una Orden Generada con el Chasis que esta impulsando",
                        status = "400 Bad Request",
                        //StatusCode = HttpStatusCode.,
                    };
                    //jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                    jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(error);
                    ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "CrearOrden", 0, 1, "0", "");
                    return BadRequest(error);
                }
                              
            }
            else
            {
                ResponseErrorDTO error = null;
                HttpStatusCode code = HttpStatusCode.BadRequest;
                int cod = 0;
                string mensaje = "";
                string reason = "";
                string status = "";
                if (code == (HttpStatusCode)400)
                {
                    cod = -2;
                    mensaje = "Parece que falta información necesaria para continuar. Por favor, verifica y vuelve a intentarlo";
                    reason = "MISSING_REQUIRED_TAG";
                    status = "400 Bad Request";
                }
                if (code == (HttpStatusCode)401)
                {
                    cod = -4;
                    mensaje = "Lo siento, tu solicitud no pudo ser autenticada. Por favor, verifica tus credenciales e intenta nuevamente";
                    reason = "AUTHENTICATION_FAILED";
                    status = "401 Unauthorized";
                }
                if (code == (HttpStatusCode)403)
                {
                    cod = -5;
                    mensaje = "Parece que no tienes permiso para acceder a este recurso. Por favor, verifica e intenta de nuevo.";
                    reason = "FORBIDDEN";
                    status = "403 Forbidden";
                }
                if (code == (HttpStatusCode)404)
                {
                    cod = -6;
                    mensaje = "No pudimos encontrar lo que estás buscando. Por favor, verifica y vuelve a intentarlo.";
                    reason = "NOT_FOUND";
                    status = "404 Not Found";
                }
                if (code == (HttpStatusCode)500)
                {
                    cod = -8;
                    mensaje = "Estamos experimentando problemas técnicos. Por favor, intenta de nuevo más tarde.";
                    reason = "INTERNAL_SERVER_ERROR";
                    status = "500 Internal Server Error";
                }
                if (code == (HttpStatusCode)503)
                {
                    cod = -10;
                    mensaje = "Actualmente estamos experimentando una interrupción en el servicio de terceros. Por favor, intenta de nuevo más tarde.";
                    reason = "THIRD_SYSTEM_UNAVAILABLE";
                    status = "503 Service Unavailable";
                }
                error = new ResponseErrorDTO
                {
                    code = cod,
                    reason = reason,
                    message = mensaje,
                    status = status,
                    //StatusCode = HttpStatusCode.,
                };
                //jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(error);
                ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "CrearOrden", valido, 1, "0", "");
                return BadRequest(error);
            }
            //return respuesta;
        }


        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize]
        //[Route("/NotificacionAgendamiento")]
        ////[Produces("application/json")]
        //public ActionResult<String> NotificacionAgendamiento(EventoDTO p)
        //{

        //    //RespuestaDTO respuesta = null;
        //    bool bandera = true;
        //    string jsonString = "";
        //    string jsonRespuesta = "";
        //    if (bandera)
        //    {

        //        //string jsonString = System.Text.Json.JsonSerializer.Serialize(p);
        //        //ConsultaDB.CnstGuardarData("response.Content", jsonString, "NotificacionAgendamiento");


        //        RespNotifyDTO respuesta = null;

        //        respuesta = new RespNotifyDTO
        //        {
        //            id = "0",
        //            callback = "",
        //            query = "",
        //        };

        //        jsonString = System.Text.Json.JsonSerializer.Serialize(p);
        //        jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
        //        ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "NotificacionAgendamiento", 1);

        //        return Ok(respuesta);

        //    }
        //    else
        //    {
        //        ResponseErrorDTO error = null;

        //        HttpStatusCode code = HttpStatusCode.BadRequest;

        //        int cod = 0;
        //        string mensaje = "";
        //        string reason = "";
        //        string status = "";
        //        if (code == (HttpStatusCode)400)
        //        {
        //            cod = -2;
        //            mensaje = "Parece que falta información necesaria para continuar. Por favor, verifica y vuelve a intentarlo";
        //            reason = "MISSING_REQUIRED_TAG";
        //            status = "400 Bad Request";
        //        }
        //        if (code == (HttpStatusCode)401)
        //        {
        //            cod = -4;
        //            mensaje = "Lo siento, tu solicitud no pudo ser autenticada. Por favor, verifica tus credenciales e intenta nuevamente";
        //            reason = "AUTHENTICATION_FAILED";
        //            status = "401 Unauthorized";
        //        }
        //        if (code == (HttpStatusCode)403)
        //        {
        //            cod = -5;
        //            mensaje = "Parece que no tienes permiso para acceder a este recurso. Por favor, verifica e intenta de nuevo.";
        //            reason = "FORBIDDEN";
        //            status = "403 Forbidden";
        //        }
        //        if (code == (HttpStatusCode)404)
        //        {
        //            cod = -6;
        //            mensaje = "No pudimos encontrar lo que estás buscando. Por favor, verifica y vuelve a intentarlo.";
        //            reason = "NOT_FOUND";
        //            status = "404 Not Found";
        //        }

        //        if (code == (HttpStatusCode)500)
        //        {
        //            cod = -8;
        //            mensaje = "Estamos experimentando problemas técnicos. Por favor, intenta de nuevo más tarde.";
        //            reason = "INTERNAL_SERVER_ERROR";
        //            status = "500 Internal Server Error";
        //        }

        //        if (code == (HttpStatusCode)503)
        //        {
        //            cod = -10;
        //            mensaje = "Actualmente estamos experimentando una interrupción en el servicio de terceros. Por favor, intenta de nuevo más tarde.";
        //            reason = "THIRD_SYSTEM_UNAVAILABLE";
        //            status = "503 Service Unavailable";
        //        }

        //        error = new ResponseErrorDTO
        //        {
        //            code = cod,
        //            reason = reason,
        //            message = mensaje,
        //            status = status,
        //            //StatusCode = HttpStatusCode.,
        //        };

        //        jsonString = System.Text.Json.JsonSerializer.Serialize(p);
        //        jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(error);
        //        ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "NotificacionAgendamiento", 0);

        //        return BadRequest(error);
        //    }

        //    //return respuesta;
        //}


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [Route("/Notificacion")]
        //[Produces("application/json")]
        public ActionResult<String> Notificacion(NotifyInstalacionDTO p)
        {
            //DISCONTINUATION_SERVICE - SUSPENSION_SERVICE - REACTIVATION_SERVICE
            //RespuestaDTO respuesta = null;
            bool bandera = true;
            int valido = 0;
            string jsonString = "";
            string jsonRespuesta = "";
            string idrespuesta = "";
            if (bandera)
            {
                RespNotifyDTO respuesta = null;
                //jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                //ConsultaDB.CnstGuardarData("response.Content", jsonString, "NotificacionInstalacion");
                idrespuesta = p.evento.serviceOrder.ExternalId;
                respuesta = new RespNotifyDTO
                {
                    id = idrespuesta,
                    callback = "",
                    query = "",
                };
                valido = 1;
               
                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "NotificacionClaro", valido, 1, "0","");
                return Ok(respuesta);
            }
            else
            {
                ResponseErrorDTO error = null;
                HttpStatusCode code = HttpStatusCode.BadRequest;
                int cod = 0;
                string mensaje = "";
                string reason = "";
                string status = "";
                if (code == (HttpStatusCode)400)
                {
                    cod = -2;
                    mensaje = "Parece que falta información necesaria para continuar. Por favor, verifica y vuelve a intentarlo";
                    reason = "MISSING_REQUIRED_TAG";
                    status = "400 Bad Request";
                }
                if (code == (HttpStatusCode)401)
                {
                    cod = -4;
                    mensaje = "Lo siento, tu solicitud no pudo ser autenticada. Por favor, verifica tus credenciales e intenta nuevamente";
                    reason = "AUTHENTICATION_FAILED";
                    status = "401 Unauthorized";
                }
                if (code == (HttpStatusCode)403)
                {
                    cod = -5;
                    mensaje = "Parece que no tienes permiso para acceder a este recurso. Por favor, verifica e intenta de nuevo.";
                    reason = "FORBIDDEN";
                    status = "403 Forbidden";
                }
                if (code == (HttpStatusCode)404)
                {
                    cod = -6;
                    mensaje = "No pudimos encontrar lo que estás buscando. Por favor, verifica y vuelve a intentarlo.";
                    reason = "NOT_FOUND";
                    status = "404 Not Found";
                }

                if (code == (HttpStatusCode)500)
                {
                    cod = -8;
                    mensaje = "Estamos experimentando problemas técnicos. Por favor, intenta de nuevo más tarde.";
                    reason = "INTERNAL_SERVER_ERROR";
                    status = "500 Internal Server Error";
                }

                if (code == (HttpStatusCode)503)
                {
                    cod = -10;
                    mensaje = "Actualmente estamos experimentando una interrupción en el servicio de terceros. Por favor, intenta de nuevo más tarde.";
                    reason = "THIRD_SYSTEM_UNAVAILABLE";
                    status = "503 Service Unavailable";
                }
                error = new ResponseErrorDTO
                {
                    code = cod,
                    reason = reason,
                    message = mensaje,
                    status = status,
                    //StatusCode = HttpStatusCode.,
                };
                //jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(error);
                ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "NotificacionClaro", valido, 1, "0", "");
                return BadRequest(error);
            }
            //return respuesta;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [Route("/NotificacionHunter")]
        //[Produces("application/json")]
        public ActionResult<String> NotificacionHunter(NotifyDTO p)
        {
            //SCHEDULE  -  IN_PROCESS  - INSTALLED -  NOT_INSTALLED 
            //NotifyInstalacionDTO p
            //String valor, String tipo
            //variables
            bool bandera = true;
            string jsonString = "";
            string jsonRespuesta = "";
            int codeapi = 0;
            string idrespuesta = "";
            string query = "";
            string callback = "";
            string mensajeerror = "";
            string codetexto = "";
            string reasontexto = "";
            string access_token = "";
            string valor = "";
            string tipo = "";
            RespNotifyDTO respuesta = null;
            Conexionconecel ruta = new Conexionconecel();
             //rutatoken = "";
            // rutanotificacion = "";
            int valido = 0;
            //conexion
            string rutatoken = ruta.ObtenerDatos("1");
            string rutanotificacion = ruta.ObtenerDatos("2");
            if (bandera)
            {
                valor = JsonConvert.SerializeObject(p, Formatting.Indented).Replace("evento", "event");
                tipo = p.title.ToUpper();
               // valor.Replace("\\", "").Replace("evento", "event");
                //obtener el token
                RestResponse responsetoken;
                var token = new RestClient(rutatoken);
                var requesttoken = new RestRequest("", Method.Post);
                requesttoken.AddHeader("content-type", "application/x-www-form-urlencoded");
                //desarrollo
                //requesttoken.AddHeader("Authorization", "Basic  " + "TWJrb0EzaFdSZHZLR21wejBIbVZCdk5LREU5MDc5a3A6Z2tuTWJXV1g0b0tqakNsbA==");
                //produccion
                requesttoken.AddHeader("Authorization", "Basic  " + "MENlRUtSMVprckdIQlB2MDNldUxud1dtb3dMQVlCcXM6VXpVS25lanhZWlJzYjhEYQ==");
                requesttoken.AddParameter("grant_type", "client_credentials");
                responsetoken = token.Execute(requesttoken);
                if (Convert.ToInt32(responsetoken.StatusCode) == 200)
                {
                    access_token = funciones.descomponer("access_token", responsetoken.Content);
                    bandera = true;
                }
                else
                {
                    bandera = false;
                    codeapi = 404;
                    codetexto = "404";
                    mensajeerror = "Error obtener el token";
                    reasontexto = responsetoken.Content;
                }
                //access_token = "pqlxyo7dif9HGl4wgGixxgxQYIdA";
                //bandera = true;
                //enviar el notificacion de agendamiento
                if (bandera)
                {
                    RestResponse responsenotificacion;
                    var notificacion = new RestClient(rutanotificacion);
                    var requestnotificacion = new RestRequest("", Method.Post);
                    requestnotificacion.AddHeader("content-type", "application/json");
                    requestnotificacion.AddHeader("Authorization", "Bearer " + access_token);
                    requestnotificacion.AddHeader("x-transactionId", "3000456030001001");
                    requestnotificacion.AddHeader("X-OriginChannel", "601/CRM");
                    requestnotificacion.AddParameter("application/json", valor, ParameterType.RequestBody);
                    responsenotificacion = notificacion.Execute(requestnotificacion);
                    //ConsultaDB.CnstGuardarData(responsenotificacion.Content, responsenotificacion.Content, "NotificacionHunter", 0, 1, "0");
                    if (Convert.ToInt32(responsenotificacion.StatusCode) == 200)
                    {
                        idrespuesta = funciones.descomponer("id", responsenotificacion.Content);
                        callback = funciones.descomponer("callback", responsenotificacion.Content);
                        query = funciones.descomponer("query", responsenotificacion.Content);
                        bandera = true;
                    }
                    else
                    {
                        codeapi = 400;
                        codetexto = funciones.descomponer("status", responsenotificacion.Content);
                        mensajeerror = funciones.descomponer("message", responsenotificacion.Content);
                        reasontexto = funciones.descomponer("reason", responsenotificacion.Content);
                        bandera = false;
                    }
                }
            }
            else
            {
                bandera = false;
                codeapi = 0;
                codetexto = "0";
                mensajeerror = "No conexion";
            }
            //respuesta
            if (bandera)
            {
                respuesta = new RespNotifyDTO
                {
                    id = idrespuesta,
                    callback = callback,
                    query = query,
                };
                valido = 1;
                //jsonString = System.Text.Json.JsonSerializer.Serialize(valor);
                //jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                //jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString.Replace("\\", ""), "NotificacionHunter", valido, 1, idrespuesta, tipo);
                return Ok(respuesta);
            }
            else
            {
                ResponseErrorDTO error = null;
                error = new ResponseErrorDTO
                {
                    code = codeapi,
                    reason = reasontexto,
                    message = mensajeerror,
                    status = codetexto,
                };
                //jsonString = System.Text.Json.JsonSerializer.Serialize(valor);
                //jsonString = JsonConvert.SerializeObject(valor, Formatting.Indented);
                jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                //jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(error);
                ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString.Replace("\\", ""), "NotificacionHunter", valido, 1, "0", tipo);
                return BadRequest(error);
            }
            //return respuesta;
        }


        //metodo de reserva para claro
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [Route("/Reservation")]
       /// [Produces("application/json")]
        public ActionResult Reservation(ReservationDTO p)
        {
            int cod = 0;
            int valido = 0;
            string mensaje = "";
            string reason = "";
            string status = "";
            string jsonString = "";
            string jsonRespuesta = "";
            ResponseErrorDTO error = null;
            bool bandera = true;
            if (bandera)
            {
                string opcion = "1";
                conexion ruta = new conexion();
                //string API_URL = ruta.ObtenerRuta("5276", "1"); //desarrollo
                string API_URL = ruta.ObtenerRuta("5276", "1"); //produccion
                string HMACSHA256SignatureType = ruta.ObtenerDatos("1");
                string OAuthVersion = ruta.ObtenerDatos("2");
                var oauthConsumerKey = ruta.ObtenerDatos("3");
                var oauthToken = ruta.ObtenerDatos("4");
                var oauthConsumerSecret = ruta.ObtenerDatos("5");
                var oauthTokenSecret = ruta.ObtenerDatos("6");
                var realm = ruta.ObtenerDatos("9");
                var httpMethod = ruta.ObtenerDatos("8");
                OAuthBase auth = new OAuthBase();
                var timestamp = Clases.OAuthBase.GenerateTimeStamp();
                var nonce = Clases.OAuthBase.GenerateNonce();
                API_URL = API_URL + "&opcion=" + opcion + "&idcliente=" + p.relatedParty.id;
                var client = new RestClient(API_URL);
                var request = new RestRequest("", Method.Get);
                Uri url = new Uri(API_URL);
                var signature = Clases.OAuthBase.GenerateSignature(url, oauthConsumerKey, oauthConsumerSecret, oauthToken, oauthTokenSecret, httpMethod, timestamp, nonce);
                request.AddHeader("Authorization", "OAuth realm=\"" + realm + "\", oauth_token=\"" + oauthToken + "\", oauth_consumer_key=\"" + oauthConsumerKey + "\"," + " oauth_nonce=\"" + nonce + "\", oauth_timestamp=\"" + timestamp + "\", oauth_signature_method=\"" + HMACSHA256SignatureType + "\", oauth_version=\"" + OAuthVersion + "\", oauth_signature=\"" + signature + "\"");
                //request.AddHeader("Content-Type", "application/json");
                //request.AddParameter("application/json", p, ParameterType.RequestBody);
                // Console.WriteLine(request);
                var response = client.Execute(request);
                Console.WriteLine(response.Content);
                DispositivoDTO myObj = JsonConvert.DeserializeObject<DispositivoDTO>(response.Content); 
                if (myObj.status == "200")
                {
                    RespReservationDTO respuesta = null;
                    relatedParty relatedParty = null;
                    List<itemCharacteristic> itemCharacteristicList = null;
                    relatedParty = new relatedParty
                    {
                        id = p.relatedParty.id,
                        name = p.relatedParty.name,
                        role = p.relatedParty.role,
                    };
                    List<reservationItem> reservationList = new List<reservationItem>();
                    //for (int i = 0; i < p.reservationItem.Count; i++)
                    for (int i = 0; i < 2; i++)
                    {
                        //for (int b = 0; b < p.reservationItem[i].itemCharacteristic.Count; b++)
                        for (int b = 0; b < 1; b++)
                        {
                            itemCharacteristicList = new List<itemCharacteristic>() {
                                new itemCharacteristic()
                                 {
                                     //id = p.reservationItem[i].itemCharacteristic[b].id,
                                     //name = p.reservationItem[i].itemCharacteristic[b].name,
                                     //valueType = p.reservationItem[i].itemCharacteristic[b].valueType,
                                     //value = p.reservationItem[i].itemCharacteristic[b].value,
                                     id = myObj.results[i].id,
                                     name = "Recurso " + myObj.results[i].name,
                                     valueType =  myObj.results[i].name,
                                     value =myObj.results[i].value,
                                 },
                            };
                            reservationList.Add(new reservationItem()
                            {
                                //quantity = p.reservationItem[i].quantity,
                                quantity = 1,
                                itemCharacteristic = itemCharacteristicList
                            });
                        }
                    }
                    respuesta = new RespReservationDTO
                    {
                        //id = "0",
                        id = myObj.id,
                        description = p.description,
                        reservationState = p.reservationState,
                        //valid_for = p.valid_for,
                        valid_for = myObj.fecha,
                        relatedParty = relatedParty,
                        //reservationItem = p.reservationItem,
                        reservationItem = reservationList,
                    };
                    valido = 1;
                    //jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                    jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                    ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "ReservationPost", valido, 1, myObj.id, "");
                    return Ok(respuesta);
                }
                else
                {
                    cod = -2;
                    // mensaje = "Parece que falta información necesaria para continuar. Por favor, verifica y vuelve a intentarlo";
                    mensaje = myObj.message;
                    reason = "MISSING_REQUIRED_TAG";
                    status = "400 Bad Request";                 
                    error = new ResponseErrorDTO
                    {
                        code = cod,
                        reason = reason,
                        message = mensaje,
                        status = status,
                        //StatusCode = HttpStatusCode.,
                    };
                   // jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                    jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(error);
                    ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "ReservationPost", valido, 1, "0", "");
                    return BadRequest(error);
                }
            }
            else
            {
                HttpStatusCode code = HttpStatusCode.BadRequest;
                if (code == (HttpStatusCode)400)
                {
                    cod = -2;
                    mensaje = "Parece que falta información necesaria para continuar. Por favor, verifica y vuelve a intentarlo";
                    reason = "MISSING_REQUIRED_TAG";
                    status = "400 Bad Request";
                }
                if (code == (HttpStatusCode)401)
                {
                    cod = -4;
                    mensaje = "Lo siento, tu solicitud no pudo ser autenticada. Por favor, verifica tus credenciales e intenta nuevamente";
                    reason = "AUTHENTICATION_FAILED";
                    status = "401 Unauthorized";
                }
                if (code == (HttpStatusCode)403)
                {
                    cod = -5;
                    mensaje = "Parece que no tienes permiso para acceder a este recurso. Por favor, verifica e intenta de nuevo.";
                    reason = "FORBIDDEN";
                    status = "403 Forbidden";
                }
                if (code == (HttpStatusCode)404)
                {
                    cod = -6;
                    mensaje = "No pudimos encontrar lo que estás buscando. Por favor, verifica y vuelve a intentarlo.";
                    reason = "NOT_FOUND";
                    status = "404 Not Found";
                }

                if (code == (HttpStatusCode)500)
                {
                    cod = -8;
                    mensaje = "Estamos experimentando problemas técnicos. Por favor, intenta de nuevo más tarde.";
                    reason = "INTERNAL_SERVER_ERROR";
                    status = "500 Internal Server Error";
                }

                if (code == (HttpStatusCode)503)
                {
                    cod = -10;
                    mensaje = "Actualmente estamos experimentando una interrupción en el servicio de terceros. Por favor, intenta de nuevo más tarde.";
                    reason = "THIRD_SYSTEM_UNAVAILABLE";
                    status = "503 Service Unavailable";
                }
                error = new ResponseErrorDTO
                {
                    code = cod,
                    reason = reason,
                    message = mensaje,
                    status = status,
                    //StatusCode = HttpStatusCode.,
                };
               // jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(error);
                ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "ReservationPost", valido, 1, "0", "");
                return BadRequest(error);
            }

            //return respuesta;
        }


        //metodo utilizado para la cancelacion de la reversa
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize]
        [Route("/Reservation")]
        //[Produces("application/json")]
        public ActionResult<String> Reservacion(ReservationDTO p)
        {
            int cod = 0;
            int valido = 0; 
            string mensaje = "";
            string reason = "";
            string status = "";
            string jsonString = "";
            string jsonRespuesta = "";
            ResponseErrorDTO error = null;
            bool bandera = true;
            if (bandera)
            {
                string opcion = "2";//anulacion
                conexion ruta = new conexion();
                //string API_URL = ruta.ObtenerRuta("5276", "1"); //desarrollo
                string API_URL = ruta.ObtenerRuta("5276", "1"); //produccion
                string HMACSHA256SignatureType = ruta.ObtenerDatos("1");
                string OAuthVersion = ruta.ObtenerDatos("2");
                var oauthConsumerKey = ruta.ObtenerDatos("3");
                var oauthToken = ruta.ObtenerDatos("4");
                var oauthConsumerSecret = ruta.ObtenerDatos("5");
                var oauthTokenSecret = ruta.ObtenerDatos("6");
                var realm = ruta.ObtenerDatos("9");
                var httpMethod = ruta.ObtenerDatos("8");
                OAuthBase auth = new OAuthBase();
                var timestamp = Clases.OAuthBase.GenerateTimeStamp();
                var nonce = Clases.OAuthBase.GenerateNonce();
                //API_URL = API_URL + "&opcion=" + opcion + "&idcliente=" + p.relatedParty.id;
                API_URL = API_URL + "&opcion=" + opcion + "&id=" + p.id;
                var client = new RestClient(API_URL);
                var request = new RestRequest("", Method.Get);
                Uri url = new Uri(API_URL);
                var signature = Clases.OAuthBase.GenerateSignature(url, oauthConsumerKey, oauthConsumerSecret, oauthToken, oauthTokenSecret, httpMethod, timestamp, nonce);
                request.AddHeader("Authorization", "OAuth realm=\"" + realm + "\", oauth_token=\"" + oauthToken + "\", oauth_consumer_key=\"" + oauthConsumerKey + "\"," + " oauth_nonce=\"" + nonce + "\", oauth_timestamp=\"" + timestamp + "\", oauth_signature_method=\"" + HMACSHA256SignatureType + "\", oauth_version=\"" + OAuthVersion + "\", oauth_signature=\"" + signature + "\"");
                //request.AddHeader("Content-Type", "application/json");
                //request.AddParameter("application/json", p, ParameterType.RequestBody);
                // Console.WriteLine(request);
                var response = client.Execute(request);
                Console.WriteLine(response.Content);
                DispositivoDTO myObj = JsonConvert.DeserializeObject<DispositivoDTO>(response.Content);
                if (myObj.status == "200")
                {
                    RespReservationDTO respuesta = null;
                    relatedParty relatedParty = null;
                    List<itemCharacteristic> itemCharacteristicList = null;
                    relatedParty = new relatedParty
                    {
                        id = p.relatedParty.id,
                        name = p.relatedParty.name,
                        role = p.relatedParty.role,
                    };
                    List<reservationItem> reservationList = new List<reservationItem>();
                    //for (int i = 0; i < p.reservationItem.Count; i++)
                    for (int i = 0; i < 2; i++)
                    {
                        //List<itemCharacteristic> itemCharacteristicList = new List<itemCharacteristic>();
                        //for (int b = 0; b < p.reservationItem[i].itemCharacteristic.Count; b++)
                        for (int b = 0; b < 1; b++)
                        {
                            itemCharacteristicList = new List<itemCharacteristic>() {
                            new itemCharacteristic()
                            {
                                //id = p.reservationItem[i].itemCharacteristic[b].id,
                                //name = p.reservationItem[i].itemCharacteristic[b].name,
                                //valueType = p.reservationItem[i].itemCharacteristic[b].valueType,
                                //value = p.reservationItem[i].itemCharacteristic[b].value,
                                 id = myObj.results[i].id,
                                 name = "Recurso " + myObj.results[i].name,
                                 valueType =  myObj.results[i].name,
                                 value =myObj.results[i].value,
                            },
                        };

                            //reservationList = new List<reservationItem>() {
                            //    new reservationItem()
                            //    {
                            //        quantity = p.reservationItem[i].quantity,
                            //        itemCharacteristic = itemCharacteristicList
                            //    };
                            //};
                            reservationList.Add(new reservationItem()
                            {
                               // quantity = p.reservationItem[i].quantity,
                                quantity = 1,
                                itemCharacteristic = itemCharacteristicList
                            });
                            //itemCharacteristicList.Add
                            //    (
                            //    p.reservationItem[i].itemCharacteristic[b].id,
                            //    p.reservationItem[i].itemCharacteristic[b].name, 
                            //    p.reservationItem[i].itemCharacteristic[b].valueType,
                            //    p.reservationItem[i].itemCharacteristic[b].value
                            //    );
                        }

                        //reservationList = new List<reservationItem>() {
                        //new reservationItem()
                        //{
                        //    quantity = p.reservationItem[i].quantity,
                        //    itemCharacteristic = itemCharacteristicList
                        //};
                        //        //new Student(){ StudentID=2, StudentName="Steve"},
                        //       // new Student(){ StudentID=3, StudentName="Ram"},
                        //       // new Student(){ StudentID=1, StudentName="Moin"}
                        //    };
                    }

                    respuesta = new RespReservationDTO
                    {
                        //id = "0",
                        // id = myObj.id,
                        id = p.id,
                        description = p.description,
                        reservationState = p.reservationState,
                        //valid_for = p.valid_for,
                        valid_for = myObj.fecha,
                        relatedParty = relatedParty,
                        //reservationItem = p.reservationItem,
                        reservationItem = reservationList,
                    };
                    valido = 1;
                    //jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                    jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                    ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "ReservationPatch", valido, 1, p.id, "");
                    return Ok(respuesta);
                }
                else
                {                 
                    cod = -2;
                    //mensaje = "Parece que falta información necesaria para continuar. Por favor, verifica y vuelve a intentarlo";
                    mensaje = myObj.message;
                    reason = "MISSING_REQUIRED_TAG";
                    status = "400 Bad Request";
                    error = new ResponseErrorDTO
                    {
                        code = cod,
                        reason = reason,
                        message = mensaje,
                        status = status,
                        //StatusCode = HttpStatusCode.,
                    };
                    //jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                    jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(error);
                    ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "ReservationPatch", valido, 1, "0", "");
                    return BadRequest(error);
                }
                // IList<reservationItem> roDinosaurs = p.reservationItem.AsReadOnly();             
            }
            else
            {
                HttpStatusCode code = HttpStatusCode.BadRequest;
                if (code == (HttpStatusCode)400)
                {
                    cod = -2;
                    mensaje = "Parece que falta información necesaria para continuar. Por favor, verifica y vuelve a intentarlo";
                    reason = "MISSING_REQUIRED_TAG";
                    status = "400 Bad Request";
                }
                if (code == (HttpStatusCode)401)
                {
                    cod = -4;
                    mensaje = "Lo siento, tu solicitud no pudo ser autenticada. Por favor, verifica tus credenciales e intenta nuevamente";
                    reason = "AUTHENTICATION_FAILED";
                    status = "401 Unauthorized";
                }
                if (code == (HttpStatusCode)403)
                {
                    cod = -5;
                    mensaje = "Parece que no tienes permiso para acceder a este recurso. Por favor, verifica e intenta de nuevo.";
                    reason = "FORBIDDEN";
                    status = "403 Forbidden";
                }
                if (code == (HttpStatusCode)404)
                {
                    cod = -6;
                    mensaje = "No pudimos encontrar lo que estás buscando. Por favor, verifica y vuelve a intentarlo.";
                    reason = "NOT_FOUND";
                    status = "404 Not Found";
                }
                if (code == (HttpStatusCode)500)
                {
                    cod = -8;
                    mensaje = "Estamos experimentando problemas técnicos. Por favor, intenta de nuevo más tarde.";
                    reason = "INTERNAL_SERVER_ERROR";
                    status = "500 Internal Server Error";
                }
                if (code == (HttpStatusCode)503)
                {
                    cod = -10;
                    mensaje = "Actualmente estamos experimentando una interrupción en el servicio de terceros. Por favor, intenta de nuevo más tarde.";
                    reason = "THIRD_SYSTEM_UNAVAILABLE";
                    status = "503 Service Unavailable";
                }
                error = new ResponseErrorDTO
                {
                    code = cod,
                    reason = reason,
                    message = mensaje,
                    status = status,
                    //StatusCode = HttpStatusCode.,
                };
                //jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(error);
                ConsultaDB.CnstGuardarData(jsonRespuesta, jsonString, "ReservationPatch", valido, 1, "0", "");
                return BadRequest(error);
            }
            //return respuesta;
        }


        //[HttpPost]
        //[Authorize]
        //[Route("/Conecel")]
        //public async Task<ActionResult<RespuestaDTO>> ConsultarClaro(ClaroDTO p)
        //{
        //    Claro producto = null;
        //    RespuestaDTO respuesta = null;
        //    string TelefonoSIM = "";
        //    string NumeroSIM = "";
        //    string Plan = "";
        //    string Estado = "";
        //    string fecha = "";
        //    string SerieSIM = "";
        //    Boolean bandera = true;
        //    string mensaje = "";
        //    if (!(p.OrderType.ToUpper() == "CAMB" || p.OrderType.ToUpper() == "INAC" || p.OrderType.ToUpper() == "RECO" || p.OrderType.ToUpper() == "SUSP"))
        //    {
        //        bandera = false;
        //        mensaje = "Debe de Enviar el OrderType correcto";               
        //    }
        //    if (bandera)
        //    {
        //        for (int i = 0; i < p.arrayCharacteristic.Count; i++)
        //        {
        //            if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "TelefonoSIM" && bandera)
        //            {
        //                if (p.arrayCharacteristic[i].value == "")
        //                {
        //                    bandera = false;
        //                    mensaje = "Debe de Enviar el valor del TelefonoSIM";
        //                    break;
        //                }
        //            }
        //            if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Plan" && bandera)
        //            {
        //                if (p.arrayCharacteristic[i].value == "")
        //                {
        //                    bandera = false;
        //                    mensaje = "Debe de Enviar el valor del Plan";
        //                    break;
        //                }
        //            }
        //            if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "NumeroSIM" && bandera)
        //            {
        //                if (p.arrayCharacteristic[i].value == "")
        //                {
        //                    bandera = false;
        //                    mensaje = "Debe de Enviar el valor del NumeroSIM";
        //                    break;
        //                }
        //            }
        //            if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Estado" && bandera)
        //            {
        //                if (p.arrayCharacteristic[i].value == "")
        //                {
        //                    bandera = false;
        //                    mensaje = "Debe de Enviar el valor del Estado";
        //                }
        //            }
        //            if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "fecha" && bandera)
        //            {
        //                if (p.arrayCharacteristic[i].value == "")
        //                {
        //                    bandera = false;
        //                    mensaje = "Debe de Enviar el valor del fecha";
        //                    break;
        //                }
        //            }
        //            if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "SerieSIM" && bandera)
        //            {
        //                if (p.arrayCharacteristic[i].value == "")
        //                {
        //                    bandera = false;
        //                    mensaje = "Debe de Enviar el valor de SerieSIM";
        //                    break;
        //                }
        //            }
        //            if (bandera)
        //            {
        //                if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "TelefonoSIM")
        //                {
        //                    TelefonoSIM = p.arrayCharacteristic[i].value;
        //                }
        //                else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Plan")
        //                {
        //                    Plan = p.arrayCharacteristic[i].value;
        //                }
        //                else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "NumeroSIM")
        //                {
        //                    NumeroSIM = p.arrayCharacteristic[i].value;
        //                }
        //                else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Estado")
        //                {
        //                    Estado = p.arrayCharacteristic[i].value;
        //                }
        //                else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "fecha")
        //                {
        //                    fecha = p.arrayCharacteristic[i].value;
        //                }
        //                else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "SerieSIM")
        //                {
        //                    SerieSIM = p.arrayCharacteristic[i].value;
        //                }
        //                mensaje = "";
        //            }
        //        }
        //    }           
        //    if (bandera)
        //    {
        //        producto = new Claro
        //        {
        //            OrderType = p.OrderType,
        //            Fecha = fecha,
        //            Estado = Estado,
        //            Plan = Plan,
        //            NumeroSIM = NumeroSIM,
        //            TelefonoSIM = TelefonoSIM,
        //            SerieSIM = SerieSIM,

        //        };
        //        await repositorio.CrearProductoAsincrono(producto);
        //        DataSet cnstGenrl = new DataSet();
        //        cnstGenrl = ConsultaDB.CnstData(producto);
        //        if (cnstGenrl.Tables.Count > 0)
        //        {
        //            respuesta = new RespuestaDTO
        //            {
        //                code = 200,
        //                status = "OK",
        //                message = mensaje,
        //                providerOrderId = (long)cnstGenrl.Tables[0].Rows[0]["CODIGO_ID"],
        //            };
        //        }
        //        else
        //        {
        //            respuesta = new RespuestaDTO
        //            {
        //                code = 400,
        //                status = "ERROR",
        //                message = "No se Guardo de Forma correcta",
        //                providerOrderId = 0,
        //            };
        //        }
        //    }
        //    else
        //    {
        //        respuesta = new RespuestaDTO
        //        {
        //            code = 400,
        //            status = "ERROR",
        //            message = mensaje,
        //            providerOrderId = 0,
        //        };
        //    }
        //    return respuesta;
        //}


        //[HttpPost]
        //[Authorize]
        //[Route("/CrearOrden")]
        //public async Task<ActionResult<ResponseDTO>> CrearOrden(OrdenDTO p)
        //{
        //    Claro producto = null;
        //    ResponseDTO respuesta = null;
        //    string planComercial = "";
        //    string planEnsamblaje = "";

        //    Boolean bandera = true;
        //    string mensaje = "";
        //    if (!(p.serviceOrderType.ToUpper() == "CREATION"  ))
        //    {
        //        bandera = false;
        //        mensaje = "Debe de Enviar el serviceOrderType correcto";
        //    }
        //    if (bandera)
        //    {
        //        for (int i = 0; i < p.customerInfo.Count; i++)
        //        {

        //            if (p.customerInfo[i].identificationType == "")
        //            {
        //                bandera = false;
        //                mensaje = "Debe de Enviar el dato de identificationType";
        //                break;
        //            }
        //            if (p.customerInfo[i].identification == "")
        //            {
        //                bandera = false;
        //                mensaje = "Debe de Enviar el dato de identification";
        //                break;
        //            }
        //            if (p.customerInfo[i].name == "")
        //            {
        //                bandera = false;
        //                mensaje = "Debe de Enviar el dato de name";
        //                break;
        //            }
        //        }
        //    }
        //    if (bandera)
        //    {
        //        for (int i = 0; i < p.serviceOrderItems.Count; i++)
        //        {
        //            if (p.serviceOrderItems[i].quantity == "" || p.serviceOrderItems[i].quantity == "0" || p.serviceOrderItems[i].quantity == null || string.IsNullOrEmpty(p.serviceOrderItems[i].quantity))
        //            {
        //                bandera = false;
        //                mensaje = "Debe de Enviar el dato de name";
        //                break;
        //            }

        //            for (int b = 0; b < p.serviceOrderItems[i].itemId.Count; b++)
        //            {
        //                if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Ensamblaje" && bandera)
        //                {
        //                    if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                    {
        //                        bandera = false;
        //                        mensaje = "Debe de Enviar el valor del Plan Ensamblaje";
        //                        break;
        //                    }
        //                }

        //                if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Comercial" && bandera)
        //                {
        //                    if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                    {
        //                        bandera = false;
        //                        mensaje = "Debe de Enviar el valor del Plan Comercial";
        //                        break;
        //                    }
        //                }

        //                if (bandera)
        //                {
        //                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Ensamblaje")
        //                    {
        //                        planEnsamblaje = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                    }
        //                    else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Comercial")
        //                    {
        //                        planComercial = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (bandera)
        //    {
        //        producto = new Claro
        //        {
        //            OrderType = p.serviceOrderType,
        //            identificationType= p.customerInfo[0].identificationType,
        //            identification = p.customerInfo[0].identification,
        //            name = p.customerInfo[0].name,
        //            internTransacionId = p.internTransacionId,
        //            planComercial = planComercial,
        //            planEnsamblaje = planEnsamblaje,
        //            quantity = p.serviceOrderItems[0].quantity,
        //        };
        //        await repositorio.CrearOrdenAsincrono(producto);
        //        DataSet cnstGenrl = new DataSet();
        //        cnstGenrl = ConsultaDB.CnstOrden(producto);
        //        if (cnstGenrl.Tables.Count > 0)
        //        {
        //            respuesta = new ResponseDTO
        //            {
        //                code = 0,
        //                status = "ACCEPTED",
        //                message = "Procesado...",
        //                internTransacionId = long.Parse((string)p.internTransacionId),
        //                providerOrderId = (long)cnstGenrl.Tables[0].Rows[0]["CODIGO_ID"],
        //            };
        //        }
        //        else
        //        {
        //            respuesta = new ResponseDTO
        //            {
        //                code = 400,
        //                status = "ERROR",
        //                message = "No se Guardo de Forma correcta",
        //                internTransacionId = 0,
        //                providerOrderId = 0,
        //            };
        //        }
        //    }
        //    else
        //    {
        //        respuesta = new ResponseDTO
        //        {
        //            code = 400,
        //            status = "ERROR",
        //            message = mensaje,
        //            internTransacionId=0,
        //            providerOrderId = 0,
        //        };
        //    }
        //    return respuesta;
        //}


        //[HttpPatch]
        //[Authorize]
        //[Route("/ActualizarOrden")]
        //public async Task<ActionResult<ResponseDTO>> ActualizarOrden(OrdenDTO p)
        //{
        //    Claro producto = null;
        //    ResponseDTO respuesta = null;
        //    string planComercial = "";
        //    string planEnsamblaje = "";

        //    Boolean bandera = true;
        //    string mensaje = "";
        //    if (!(p.serviceOrderType.ToUpper() == "UPDATE"))
        //    {
        //        bandera = false;
        //        mensaje = "Debe de Enviar el serviceOrderType correcto";
        //    }

        //    if (bandera)
        //    {
        //        for (int i = 0; i < p.serviceOrderItems.Count; i++)
        //        {
        //            if (p.serviceOrderItems[i].quantity == "" || p.serviceOrderItems[i].quantity == "0" || p.serviceOrderItems[i].quantity == null || string.IsNullOrEmpty(p.serviceOrderItems[i].quantity))
        //            {
        //                bandera = false;
        //                mensaje = "Debe de Enviar el dato de name";
        //                break;
        //            }

        //            for (int b = 0; b < p.serviceOrderItems[i].itemId.Count; b++)
        //            {
        //                if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Ensamblaje" && bandera)
        //                {
        //                    if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                    {
        //                        bandera = false;
        //                        mensaje = "Debe de Enviar el valor del Plan Ensamblaje";
        //                        break;
        //                    }
        //                }

        //                if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Comercial" && bandera)
        //                {
        //                    if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                    {
        //                        bandera = false;
        //                        mensaje = "Debe de Enviar el valor del Plan Comercial";
        //                        break;
        //                    }
        //                }

        //                if (bandera)
        //                {
        //                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Ensamblaje")
        //                    {
        //                        planEnsamblaje = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                    }
        //                    else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Comercial")
        //                    {
        //                        planComercial = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (bandera)
        //    {
        //       // string providerOrderId = p.providerOrderId;
        //        producto = new Claro
        //        {
        //            OrderType = p.serviceOrderType,
        //            //internTransacionId = p.internTransacionId,
        //            providerOrderId = p.providerOrderId,
        //            planComercial = planComercial,
        //            planEnsamblaje = planEnsamblaje,
        //            quantity = p.serviceOrderItems[0].quantity,
        //        };
        //        await repositorio.CrearOrdenAsincrono(producto);
        //        DataSet cnstGenrl = new DataSet();
        //        cnstGenrl = ConsultaDB.CnstOrden(producto);
        //        if (cnstGenrl.Tables.Count > 0)
        //        {
        //            respuesta = new ResponseDTO
        //            {
        //                code = 0,
        //                status = "ACCEPTED",
        //                message = "Procesado...",
        //                internTransacionId = long.Parse((string)cnstGenrl.Tables[0].Rows[0]["INTERNTRANSACIONID"]),
        //                providerOrderId = long.Parse(p.providerOrderId),
        //            };
        //        }
        //        else
        //        {
        //            respuesta = new ResponseDTO
        //            {
        //                code = 400,
        //                status = "ERROR",
        //                message = "No se Guardo de Forma correcta",
        //                internTransacionId = 0,
        //                providerOrderId = 0,
        //            };
        //        }
        //    }
        //    else
        //        {
        //            respuesta = new ResponseDTO
        //            {
        //                code = 400,
        //                status = "ERROR",
        //                message = mensaje,
        //                internTransacionId = 0,
        //                providerOrderId = 0,
        //            };
        //        }
        //        return respuesta;
        //}


        //[HttpPost]
        //[Authorize]
        //[Route("/Dispositivo")]
        //public async Task<ActionResult<RespNotifyDTO>> Dispositivo(NotifyDTO p)
        //{
        //    Claro producto = null;
        //    RespNotifyDTO respuesta = null;
        //    string serieSim = "";
        //    string imei = "";
        //    string apn = "";
        //    string ip = "";
        //    string telefonoSim = "";
        //    string estado = "";
        //    string mensajerror = "";
        //    Boolean bandera = true;
        //    string mensaje = "";
        //    if (!(p.serviceOrderType.ToUpper() == "NOTIFY" || p.serviceOrderType.ToUpper() == "NOTIFYPAREJAS" || p.serviceOrderType.ToUpper() == "NOTIFYDELETE"))
        //    {
        //        bandera = false;
        //        mensaje = "Debe de Enviar el serviceOrderType correcto";
        //    }
        //    if (p.providerOrderId == "" || p.providerOrderId == "0" || p.providerOrderId == null || string.IsNullOrEmpty(p.providerOrderId))
        //    {
        //        bandera = false;
        //        mensaje = "Debe de Enviar el providerOrderId correcto";

        //    }
        //    if (bandera)
        //    {
        //        for (int i = 0; i < p.serviceOrderItems.Count; i++)
        //        {
        //            for (int b = 0; b < p.serviceOrderItems[i].itemId.Count; b++)
        //            {
        //                if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "SerieSIM" && bandera)
        //                {
        //                    if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                    {
        //                        bandera = false;
        //                        mensaje = "Debe de Enviar el valor de SerieSIM";
        //                        break;
        //                    }
        //                }

        //                if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "IMEI" && bandera)
        //                {
        //                    if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                    {
        //                        bandera = false;
        //                        mensaje = "Debe de Enviar el valor de IMEI";
        //                        break;
        //                    }
        //                }

        //                if (p.serviceOrderType.ToUpper() == "NOTIFY")
        //                {
        //                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "APN" && bandera)
        //                    {
        //                        if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                        {
        //                            bandera = false;
        //                            mensaje = "Debe de Enviar el valor de APN";
        //                            break;
        //                        }
        //                    }

        //                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "IP" && bandera)
        //                    {
        //                        if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                        {
        //                            bandera = false;
        //                            mensaje = "Debe de Enviar el valor de IP";
        //                            break;
        //                        }
        //                    }

        //                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "TelefonoSIM" && bandera)
        //                    {
        //                        if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                        {
        //                            bandera = false;
        //                            mensaje = "Debe de Enviar el valor de TelefonoSIM";
        //                            break;
        //                        }
        //                    }

        //                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Estado" && bandera)
        //                    {
        //                        if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                        {
        //                            bandera = false;
        //                            mensaje = "Debe de Enviar el valor de Estado";
        //                            break;
        //                        }
        //                    }

        //                }

        //                if (p.serviceOrderType.ToUpper() == "NOTIFYPAREJAS")
        //                    {
        //                        if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Estado" && bandera)
        //                        {
        //                            if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                            {
        //                                bandera = false;
        //                                mensaje = "Debe de Enviar el valor de Estado";
        //                                break;
        //                            }
        //                        }
        //                        if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Error" && bandera)
        //                        {
        //                            if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
        //                            {
        //                                bandera = false;
        //                                mensaje = "Debe de Enviar el valor de Error";
        //                                break;
        //                            }
        //                        }
        //                    }

        //                if (bandera)
        //                    {
        //                        if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "SerieSIM")
        //                        {
        //                            serieSim = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                        }
        //                        else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "IMEI")
        //                        {
        //                            imei = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                        }
        //                        else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "APN")
        //                        {
        //                            apn = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                        }
        //                        else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "IP")
        //                        {
        //                            ip = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                        }
        //                        else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "TelefonoSIM")
        //                        {
        //                            telefonoSim = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                        }
        //                        else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Estado")
        //                        {
        //                            estado = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                        }
        //                        else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Error")
        //                        {
        //                            mensajerror = p.serviceOrderItems[i].itemId[b].value.ToString();
        //                        }
        //                    }
        //            }
        //        }
        //    }

        //    if (bandera)
        //    {
        //        producto = new Claro
        //        {
        //            OrderType = p.serviceOrderType,
        //            providerOrderId = p.providerOrderId,
        //            executionDate = p.executionDate,
        //            SerieSIM = serieSim,
        //            IMEI = imei,
        //            APN = apn,
        //            IP = ip,
        //            TelefonoSIM = telefonoSim,
        //            Estado = estado,
        //            Fecha = p.executionDate,
        //            Error= mensajerror,
        //        };
        //        await repositorio.DispositivoAsincrono(producto);
        //        respuesta = new RespNotifyDTO
        //        {
        //            code = 0,
        //            status = "ACCEPTED",
        //            message = "Procesado...",
        //        };

        //    }
        //    else
        //    {
        //        respuesta = new RespNotifyDTO
        //        {
        //            code = 400,
        //            status = "ERROR",
        //            message = mensaje,
        //        };
        //    }
        //    return respuesta;
        //}


    }
}
