using ApiRest.Clases;
using ApiRest.DTO;
using ApiRest.Modelo;
using ApiRest.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Text.Json;
using System.Threading.Tasks;
using static ApiRest.DTO.FlotaDispositivoDTO;
using static ApiRest.DTO.FlotaVentaDTO;

namespace ApiRest.Controllers
{
    [ApiController]
    [Route("ConecelFlota")]
    [Consumes("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ConecelFlotaController : Controller
    {
        private readonly IClaroEnMemoria repositorio;

        public ConecelFlotaController(IClaroEnMemoria r)
        {
            this.repositorio = r;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "ADMIN,CLAROFLOTA")]
        [Route("/CreateOrder")]
        public async Task<ActionResult<string>> CreateOrder(FlotaVentaDTO p)
        {
            bool bandera = true;
            string jsonString = "";
            string jsonRespuesta = "";
            string mensaje = "";
            string planComercial = "";
            string planEnsamblaje = "";
            if (!(p.orderType.ToUpper() == "CREATEORDER"))
            {
                bandera = false;
                mensaje = "Debe de Enviar el orderType correcto";
            }          
            if (bandera)
            {
                if (p.customerInfo == null)
                {
                    bandera = false;
                    mensaje = "Debe de Enviar el objeto customerInfo";
                }
                else if (string.IsNullOrWhiteSpace(p.customerInfo.identificationType))
                {
                    bandera = false;
                    mensaje = "Debe de Enviar el dato de identificationType";
                }
                else if (string.IsNullOrWhiteSpace(p.customerInfo.Identification))
                {
                    bandera = false;
                    mensaje = "Debe de Enviar el dato de identification";
                }
                else if (string.IsNullOrWhiteSpace(p.customerInfo.name))
                {
                    bandera = false;
                    mensaje = "Debe de Enviar el dato de name";
                }
                //else if (string.IsNullOrWhiteSpace(p.customerInfo.direccion))
                //{
                //    bandera = false;
                //    mensaje = "Debe de Enviar el dato de direcccion";
                //}
                //else if (string.IsNullOrWhiteSpace(p.customerInfo.ciudad))
                //{
                //    bandera = false;
                //    mensaje = "Debe de Enviar el dato de ciudad";
                //}
                //else if (string.IsNullOrWhiteSpace(p.customerInfo.telefono))
                //{
                //    bandera = false;
                //    mensaje = "Debe de Enviar el dato de telefono";
                //}
                //else if (string.IsNullOrWhiteSpace(p.customerInfo.email))
                //{
                //    bandera = false;
                //    mensaje = "Debe de Enviar el dato de email";
                //}             
            }
            if (bandera)
            {
                for (int i = 0; i < p.serviceOrderItems.Count; i++)
                {
                    if ( p.serviceOrderItems[i].quantity == 0 )
                    {
                        bandera = false;
                        mensaje = "Debe de Enviar el valor de quantity";
                        break;
                    }

                    for (int b = 0; b < p.serviceOrderItems[i].itemId.Count; b++)
                    {
                        if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Ensamblaje" && bandera)
                        {
                            if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
                            {
                                bandera = false;
                                mensaje = "Debe de Enviar el valor del Plan Ensamblaje";
                                break;
                            }
                        }
                        if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Comercial" && bandera)
                        {
                            if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
                            {
                                bandera = false;
                                mensaje = "Debe de Enviar el valor del Plan Comercial";
                                break;
                            }
                        }
                        if (bandera)
                        {
                            if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Ensamblaje")
                            {
                                planEnsamblaje = p.serviceOrderItems[i].itemId[b].value.ToString();
                            }
                            else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Comercial")
                            {
                                planComercial = p.serviceOrderItems[i].itemId[b].value.ToString();
                            }
                        }
                    }
                }
            }
            if (bandera)
            {
                Claro producto = null;
                FlotaRespuestaDTO respuesta = null;
                DataSet cnstGenrl = new DataSet();
                jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                producto = new Claro
                {
                    OrderType = p.orderType.ToUpper(),
                    identificationType = p.customerInfo.identificationType,
                    identification = p.customerInfo.Identification,
                    name = p.customerInfo.name,
                    internTransacionId = p.orderId,
                    providerOrderId = p.providerOrderId,
                    planComercial = planComercial,
                    planEnsamblaje = planEnsamblaje,
                    quantity = p.serviceOrderItems[0].quantity.ToString(),
                    cadena = jsonString,
                };
                cnstGenrl = ConsultaDB.CnstOrden(producto);
                if (cnstGenrl.Tables.Count > 0)
                {
                    p.providerOrderId = cnstGenrl.Tables[0].Rows[0]["codigo"].ToString();
                    if (cnstGenrl.Tables[0].Rows[0]["codigo"].ToString() =="0")
                    {
                        respuesta = new FlotaRespuestaDTO
                        {
                            code = 400,
                            status = "ERROR",
                            message = "Ya existe esa Orden",
                            orderId = p.orderId,
                            providerOrderId = "0",
                        };
                        return BadRequest(respuesta);
                    }
                    else
                    {
                        respuesta = new FlotaRespuestaDTO
                        {
                            code = 200,
                            status = "CREATED/ACCEPTED",
                            message = "Order created successfully",
                            orderId = p.orderId,
                            providerOrderId = p.providerOrderId
                        };
                        jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                        producto.respuesta = jsonRespuesta;
                        producto.providerOrderId = cnstGenrl.Tables[0].Rows[0]["codigo"].ToString();
                        await repositorio.CrearOrdenAsincrono(producto);
                        return Ok(respuesta);
                    }                   
                }
                else
                {
                    respuesta = new FlotaRespuestaDTO
                    {
                        code = 400,
                        status = "ERROR",
                        message = "No se Guardo de Forma correcta",
                        orderId = p.orderId,
                        providerOrderId = "0",
                    };
                    return BadRequest(respuesta);
                }             
            }
            else
            {
                FlotaRespuestaDTO error = null;             
                error = new FlotaRespuestaDTO
                {
                    code = 400,
                    status = p.orderType,
                    message = mensaje,
                    orderId = p.orderId,
                    providerOrderId = "0"
                };
                return BadRequest(error);
            }
        }


        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "ADMIN,CLAROFLOTA")]
        [Route("/UpdateOrder")]
        public async Task<ActionResult<string>> UpdateOrder(FlotaVentaDTO p)
        {
            bool bandera = true;
            string jsonString = "";
            string jsonRespuesta = "";
            string mensaje = "";
            string planComercial = "";
            string planEnsamblaje = "";
            Claro producto = null;
            if (!(p.orderType.ToUpper() == "UPDATEORDER"))
            {
                bandera = false;
                mensaje = "Debe de Enviar el orderType correcto";
            }
            if (bandera)
            {
                for (int i = 0; i < p.serviceOrderItems.Count; i++)
                {
                    if (p.serviceOrderItems[i].quantity == 0)
                    {
                        bandera = false;
                        mensaje = "Debe de Enviar el valor de quantity";
                        break;
                    }
                    for (int b = 0; b < p.serviceOrderItems[i].itemId.Count; b++)
                    {
                        if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Ensamblaje" && bandera)
                        {
                            if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
                            {
                                bandera = false;
                                mensaje = "Debe de Enviar el valor del Plan Ensamblaje";
                                break;
                            }
                        }
                        if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Comercial" && bandera)
                        {
                            if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
                            {
                                bandera = false;
                                mensaje = "Debe de Enviar el valor del Plan Comercial";
                                break;
                            }
                        }
                        if (bandera)
                        {
                            if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Ensamblaje")
                            {
                                planEnsamblaje = p.serviceOrderItems[i].itemId[b].value.ToString();
                            }
                            else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "Plan-Comercial")
                            {
                                planComercial = p.serviceOrderItems[i].itemId[b].value.ToString();
                            }
                        }
                    }
                }
            }
            if (bandera)
            {
                FlotaRespuestaDTO respuesta = null;
                DataSet cnstGenrl = new DataSet();
                producto = new Claro
                {
                    OrderType = p.orderType,
                    providerOrderId = p.providerOrderId,
                    planComercial = planComercial,
                    planEnsamblaje = planEnsamblaje,
                    internTransacionId = p.orderId,
                    quantity = p.serviceOrderItems[0].quantity.ToString(),
                };              
                cnstGenrl = ConsultaDB.CnstOrden(producto);
                if (cnstGenrl.Tables.Count > 0)
                {
                    respuesta = new FlotaRespuestaDTO
                    {
                        code = 200,
                        status = "UPDATED/ACCEPTED",
                        message = "Order updated successfully",
                        orderId = p.orderId,
                        providerOrderId = p.providerOrderId // numero de orden de hunter

                    };
                    jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                    jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                    producto.respuesta= jsonRespuesta;
                    producto.cadena=jsonString;
                    await repositorio.CrearOrdenAsincrono(producto);
                    return Ok(respuesta);
                }
                else
                {
                    respuesta = new FlotaRespuestaDTO
                    {
                        code = 400,
                        status = "ERROR",
                        message = "No se Guardo de Forma correcta",
                        orderId = p.orderId,
                        providerOrderId = p.providerOrderId // numero de orden de hunter
                    };
                    return BadRequest(respuesta);
                }
            }
            else
            {
                FlotaRespuestaDTO error = null;
                error = new FlotaRespuestaDTO
                {
                    code = 400,
                    status = p.orderType,
                    message = mensaje,
                    orderId = p.orderId,
                    providerOrderId = "0"

                };
                return BadRequest(error);
            }
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "ADMIN")]
        [Route("/EnviarDispositivos")]
        public async Task<ActionResult<string>> EnviarDispositivos( string orderID, string providerOrderID)
        {
            bool bandera = true;
            FlotaDispositivoDTO datos = null;
            string jsonString = "";
            string jsonRespuesta = "";
            string imei = "";
            string simcard = "";
            int quantity = 0;
            string codReserva = "";
            string cadenanetsuite = "";
            //hacer la reserva para tener los datos del dispositovo y sim
            DataSet cnstGenrl = new DataSet();
            cnstGenrl = ConsultaDB.CnstQuantity(orderID, providerOrderID,3);
            if (cnstGenrl != null && cnstGenrl.Tables.Count > 0 && cnstGenrl.Tables[0].Rows.Count > 0)
            {
                quantity = (int)cnstGenrl.Tables[0].Rows[0]["quantity_impulso"];
                string opcion = "5";
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
                API_URL = API_URL + "&opcion=" + opcion + "&cantidad=" + quantity;
                var client = new RestClient(API_URL);
                var request = new RestRequest("", Method.Get);
                Uri url = new Uri(API_URL);
                var signature = Clases.OAuthBase.GenerateSignature(url, oauthConsumerKey, oauthConsumerSecret, oauthToken, oauthTokenSecret, httpMethod, timestamp, nonce);
                request.AddHeader("Authorization", "OAuth realm=\"" + realm + "\", oauth_token=\"" + oauthToken + "\", oauth_consumer_key=\"" + oauthConsumerKey + "\"," + " oauth_nonce=\"" + nonce + "\", oauth_timestamp=\"" + timestamp + "\", oauth_signature_method=\"" + HMACSHA256SignatureType + "\", oauth_version=\"" + OAuthVersion + "\", oauth_signature=\"" + signature + "\"");
                var response = client.Execute(request);
                Console.WriteLine(response.Content);
                FlotaDispositivoAltaDTO myObj = JsonConvert.DeserializeObject<FlotaDispositivoAltaDTO>(response.Content);
                 //FlotaDispositivoAltaDTO myObj = JsonConvert.DeserializeObject<FlotaDispositivoAltaDTO>("{\r\n  \"results\": [\r\n    {\r\n      \"ItemsDetail\": [\r\n        {\r\n          \"items\": [\r\n            {\r\n              \"id\": \"633928\",\r\n              \"name\": \"SIMCARD\",\r\n              \"value\": \"895930100113089806\"\r\n            },\r\n            {\r\n              \"id\": \"574868\",\r\n              \"name\": \"IMEI\",\r\n              \"value\": \"864002070072583\"\r\n            },\r\n            {\r\n              \"id\": \"78206\",\r\n              \"name\": \"COD_RESERVA\",\r\n              \"value\": \"78206\"\r\n            }\r\n          ]\r\n        },\r\n        {\r\n          \"items\": [\r\n            {\r\n              \"id\": \"633928\",\r\n              \"name\": \"SIMCARD\",\r\n              \"value\": \"895930100113089805\"\r\n            },\r\n            {\r\n              \"id\": \"574868\",\r\n              \"name\": \"IMEI\",\r\n              \"value\": \"864002070072575\"\r\n            },\r\n            {\r\n              \"id\": \"78205\",\r\n              \"name\": \"COD_RESERVA\",\r\n              \"value\": \"78205\"\r\n            }\r\n          ]\r\n        }\r\n      ]\r\n    }\r\n  ],\r\n  \"id\": \"9999\",\r\n  \"fecha\": \"2026-09-18T17:42:15.462Z\",\r\n  \"status\": \"200\",\r\n  \"message\": \"Proceso de Reserva Finalizado\"\r\n}");
                if (myObj.status == "200")
                {
                    cadenanetsuite = JsonConvert.SerializeObject(myObj, Formatting.Indented);
                    datos = new FlotaDispositivoDTO
                    {
                        orderId = orderID,
                        providerOrderId = providerOrderID,
                        serviceType = "HUNTER-ALTA",
                        serviceOrderType = "UPDATE",
                    };
                    // List<servicesOrderItemsAlta> serviceOrderItems = new List<servicesOrderItemsAlta>();
                    //List<itemsAlta> itemsAlta = new List<itemsAlta>();
                    if (datos.serviceOrderItems == null)
                    {
                        datos.serviceOrderItems = new List<servicesOrderItemsAlta>();
                    }
                    for (int i = 0; i < myObj.results[0].ItemsDetail.Count; i++)
                    {
                        List<itemsAlta> itemsAlta = new List<itemsAlta>();
                        for (int b = 0; b < myObj.results[0].ItemsDetail[i].items.Count; b++)
                        {
                            itemsAlta.Add(new itemsAlta()
                            {
                                type = myObj.results[0].ItemsDetail[i].items[b].name,
                                value = myObj.results[0].ItemsDetail[i].items[b].value,
                                dataType = "String",
                            });
                            //if (datos.serviceOrderItems == null)
                            //{
                            //    datos.serviceOrderItems = new List<servicesOrderItemsAlta>();
                            //}

                            //if (datos.serviceOrderItems.Count == 0)
                            //{
                            //    datos.serviceOrderItems.Add(new servicesOrderItemsAlta());
                            //}
                        }
                        //datos.serviceOrderItems[0].itemId = itemsAlta;
                        // Crear un serviceOrderItems para cada pareja
                        datos.serviceOrderItems.Add(new servicesOrderItemsAlta()
                        {
                            itemId = itemsAlta
                        });
                    }
                }
                // SE TIENE QUE ENVIAR A UN METODO DE CLARO LAS SIM/DISPOSITIVOS
                if (bandera)
                {
                    FlotaRespuestaDTO respuesta = null;
                    respuesta = new FlotaRespuestaDTO
                    {
                        code = 200,
                        status = "ACCEPTED",
                        message = "SIMCARD and IMEI pairs received successfully",
                        orderId = datos.orderId,
                        providerOrderId = datos.providerOrderId, // numero de orden de hunter
                        platform = "Hunter-Claro"
                    };
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                    for (int i = 0; i < datos.serviceOrderItems.Count; i++)
                    {
                        for (int b = 0; b < datos.serviceOrderItems[i].itemId.Count; b++)
                        {
                            if ((datos.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "SIMCARD")
                            {
                                simcard = datos.serviceOrderItems[i].itemId[b].value.ToString();
                            }
                            else if ((datos.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "IMEI")
                            {
                                imei = datos.serviceOrderItems[i].itemId[b].value.ToString();
                            }
                            else if ((datos.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "COD_RESERVA")
                            {
                                codReserva = datos.serviceOrderItems[i].itemId[b].value.ToString();
                            }
                        }
                        jsonString = System.Text.Json.JsonSerializer.Serialize(datos);
                        jsonString = JsonConvert.SerializeObject(datos, Formatting.Indented);
                        Claro producto = null;
                        producto = new Claro
                        {
                            OrderType = datos.serviceType,
                            providerOrderId = datos.providerOrderId,
                            OrderId = datos.orderId,
                            codReserva = codReserva,
                            SerieSIM = simcard,
                            IMEI = imei,
                            APN = "",
                            IP = "",
                            TelefonoSIM = "",
                            Estado = "",
                            Fecha = string.Format("{0:dd/MM/yyyy}", DateTime.Now),
                            Error = "",
                            FechaActivacion = "",
                            cadena = jsonString,
                            cadenanetsuite = cadenanetsuite,
                            respuesta = jsonRespuesta,
                        };
                        await repositorio.DispositivoAsincrono(producto);
                    }
                    ConsultaDB.CnstReserva(datos.orderId, datos.providerOrderId, 3, "", "");
                    return Ok(respuesta);
                }
                else
                {
                    FlotaRespuestaDTO error = null;
                    error = new FlotaRespuestaDTO
                    {
                        code = 400,
                        status = "Bad Request",
                        message = "Error en el impulso",
                        orderId = orderID,
                        providerOrderId = providerOrderID

                    };
                    return BadRequest(error);
                }
            } else
            {
                FlotaRespuestaDTO error = null;
                error = new FlotaRespuestaDTO
                {
                    code = 400,
                    status = "Bad Request",
                    message = "NO existe el registro para el Impulso",
                    orderId = orderID,
                    providerOrderId = providerOrderID

                };
                return BadRequest(error);
            }           
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "ADMIN,CLAROFLOTA")]
        [Route("/NotifyFlota")]
        public async Task<ActionResult<string>> NotifyFlota(FlotaNotifyDTO p)
        {
            Claro producto = null;
            ClaroDTO actualizar = null;
            FlotaRespuestaDTO respuesta = null;
            string cadenanetsuite = "";
            string jsonString = "";
            string jsonRespuesta = "";
            string simcard = "";
            string imei = "";
            string apn = "";
            string ip = "";
            string telefonoSim = "";
            string estado = "";
            string fechaactivacion="";
            string mensajerror = "";
            bool bandera = true;
            string mensaje = "";
            string codReserva = "";
            if (!(p.OrderType.ToUpper() == "NOTIFYPAREJAS" || p.OrderType.ToUpper() == "NOTIFYDELETE" || p.OrderType.ToUpper() == "NOTIFACTSOL" || p.OrderType.ToUpper() == "NOTIFRESCAMBPLAN"))
            {
                bandera = false;
                mensaje = "Debe de Enviar el OrderType correcto";
            }
            if (p.providerOrderId == "" || p.providerOrderId == "0" || p.providerOrderId == null || string.IsNullOrEmpty(p.providerOrderId))
            {
                bandera = false;
                mensaje = "Debe de Enviar el providerOrderId correcto";
            }
            if (p.orderId == "" || p.orderId == "0" || p.orderId == null || string.IsNullOrEmpty(p.orderId))
            {
                bandera = false;
                mensaje = "Debe de Enviar el orderId correcto";
            }
            if (bandera)
            {
                respuesta = new FlotaRespuestaDTO
                {
                    code = 200,
                    status = "ACCEPTED",
                    message = "Procesado...",
                    orderId = p.orderId,
                    providerOrderId = p.providerOrderId
                };
                if (p.OrderType.ToUpper() != "NOTIFRESCAMBPLAN")
                {
                    for (int i = 0; i < p.OrderDetail.Count; i++)
                    {
                        for (int b = 0; b < p.OrderDetail[i].Characteristics.Count; b++)
                        {
                            telefonoSim = p.OrderDetail[i].TelefonoSIM.ToString();
                            if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "SIMCARD" && bandera)
                            {
                                if (p.OrderDetail[i].Characteristics[b].value == null || string.IsNullOrEmpty(p.OrderDetail[i].Characteristics[b].value.ToString()))
                                {
                                    bandera = false;
                                    mensaje = "Debe de Enviar el valor de SIMCARD";
                                    break;
                                }
                            }
                            if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "IMEI" && bandera)
                            {
                                if (p.OrderDetail[i].Characteristics[b].value == null || string.IsNullOrEmpty(p.OrderDetail[i].Characteristics[b].value.ToString()))
                                {
                                    bandera = false;
                                    mensaje = "Debe de Enviar el valor de IMEI";
                                    break;
                                }
                            }
                            if (p.OrderType.ToUpper() == "NOTIFY" || p.OrderType.ToUpper() == "NOTIFACTSOL")
                            {
                                if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "APN" && bandera)
                                {
                                    if (p.OrderDetail[i].Characteristics[b].value == null || string.IsNullOrEmpty(p.OrderDetail[i].Characteristics[b].value.ToString()))
                                    {
                                        bandera = false;
                                        mensaje = "Debe de Enviar el valor de APN";
                                        break;
                                    }
                                }
                                if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "IP" && bandera)
                                {
                                    if (p.OrderDetail[i].Characteristics[b].value == null || string.IsNullOrEmpty(p.OrderDetail[i].Characteristics[b].value.ToString()))
                                    {
                                        bandera = false;
                                        mensaje = "Debe de Enviar el valor de IP";
                                        break;
                                    }
                                }
                                //if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "TelefonoSIM" && bandera)
                                //{
                                //    if (p.OrderDetail[i].Characteristics[b].value == null || string.IsNullOrEmpty(p.OrderDetail[i].Characteristics[b].value.ToString()))
                                //    {
                                //        bandera = false;
                                //        mensaje = "Debe de Enviar el valor de TelefonoSIM";
                                //        break;
                                //    }
                                //}
                                if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "Estado" && bandera)
                                {
                                    if (p.OrderDetail[i].Characteristics[b].value == null || string.IsNullOrEmpty(p.OrderDetail[i].Characteristics[b].value.ToString()))
                                    {
                                        bandera = false;
                                        mensaje = "Debe de Enviar el valor de Estado";
                                        break;
                                    }
                                }
                            }
                            if (p.OrderType.ToUpper() == "NOTIFYPAREJAS")
                            {
                                if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "Estado" && bandera)
                                {
                                    if (p.OrderDetail[i].Characteristics[b].value == null || string.IsNullOrEmpty(p.OrderDetail[i].Characteristics[b].value.ToString()))
                                    {
                                        bandera = false;
                                        mensaje = "Debe de Enviar el valor de Estado";
                                        break;
                                    }
                                }
                                if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "Error" && bandera)
                                {
                                    if (p.OrderDetail[i].Characteristics[b].value == null || string.IsNullOrEmpty(p.OrderDetail[i].Characteristics[b].value.ToString()))
                                    {
                                        bandera = false;
                                        mensaje = "Debe de Enviar el valor de Error";
                                        break;
                                    }
                                }
                            }
                            if (bandera)
                            {
                                if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "SIMCARD")
                                {
                                    simcard = p.OrderDetail[i].Characteristics[b].value.ToString();
                                }
                                else if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "IMEI")
                                {
                                    imei = p.OrderDetail[i].Characteristics[b].value.ToString();
                                }
                                else if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "APN")
                                {
                                    apn = p.OrderDetail[i].Characteristics[b].value.ToString();
                                }
                                else if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "IP")
                                {
                                    ip = p.OrderDetail[i].Characteristics[b].value.ToString();
                                }
                                //else if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "TelefonoSIM")
                                //{
                                //    telefonoSim = p.OrderDetail[i].Characteristics[b].value.ToString();
                                //}
                                else if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "FECHAACTIVACION")
                                {
                                    fechaactivacion = p.OrderDetail[i].Characteristics[b].value.ToString();
                                }
                                else if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "ESTADO")
                                {
                                    estado = p.OrderDetail[i].Characteristics[b].value.ToString();
                                }
                                else if ((p.OrderDetail[i].Characteristics[b].id).TrimEnd().TrimStart() == "ERROR")
                                {
                                    mensajerror = p.OrderDetail[i].Characteristics[b].value.ToString();
                                }
                            }
                        }
                        //aqui se guarda
                        if ( p.OrderType.ToUpper() == "NOTIFACTSOL" )
                        {
                            if (bandera)
                            {
                                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                                producto = new Claro
                                {
                                    OrderType = p.OrderType,
                                    providerOrderId = p.providerOrderId,
                                    OrderId = p.orderId,
                                    //executionDate = p.executionDate,
                                    SerieSIM = simcard,
                                    IMEI = imei,
                                    APN = apn,
                                    IP = ip,
                                    TelefonoSIM = telefonoSim,
                                    Estado = estado,
                                    Fecha = string.Format("{0:dd/MM/yyyy}", DateTime.Now),
                                    Error = mensajerror,
                                    FechaActivacion = fechaactivacion,
                                    cadena = jsonString,
                                    respuesta = jsonRespuesta,
                                    cadenanetsuite = cadenanetsuite
                                };
                                await repositorio.DispositivoAsincrono(producto);
                                DataSet cnstGenrl = new DataSet();
                                cnstGenrl = ConsultaDB.CnstReserva(p.orderId, p.providerOrderId, 2, simcard, imei);
                                if (cnstGenrl != null && cnstGenrl.Tables.Count > 0 && cnstGenrl.Tables[0].Rows.Count > 0)
                                {
                                    codReserva = (string)cnstGenrl.Tables[0].Rows[0]["CODRESERVA"];
                                    actualizar = new ClaroDTO
                                    {
                                        idreserva = codReserva,
                                        accion = "SIMCARD",
                                        celular = telefonoSim,
                                        apn = apn,
                                        ip = ip
                                    };
                                    conexion ruta = new conexion();
                                    string API_URL = ruta.ObtenerRuta("5276", "1");
                                    string HMACSHA256SignatureType = ruta.ObtenerDatos("1");
                                    string OAuthVersion = ruta.ObtenerDatos("2");
                                    var oauthConsumerKey = ruta.ObtenerDatos("3");
                                    var oauthToken = ruta.ObtenerDatos("4");
                                    var oauthConsumerSecret = ruta.ObtenerDatos("5");
                                    var oauthTokenSecret = ruta.ObtenerDatos("6");
                                    var realm = ruta.ObtenerDatos("9");
                                    var httpMethod = ruta.ObtenerDatos("7");
                                    OAuthBase auth = new OAuthBase();
                                    var timestamp = Clases.OAuthBase.GenerateTimeStamp();
                                    var nonce = Clases.OAuthBase.GenerateNonce();
                                    var client = new RestClient(API_URL);
                                    var request = new RestRequest("", Method.Post);
                                    Uri url = new Uri(API_URL);
                                    var signature = Clases.OAuthBase.GenerateSignature(url, oauthConsumerKey, oauthConsumerSecret, oauthToken, oauthTokenSecret, httpMethod, timestamp, nonce);
                                    request.AddHeader("Authorization", "OAuth realm=\"" + realm + "\", oauth_token=\"" + oauthToken + "\", oauth_consumer_key=\"" + oauthConsumerKey + "\"," + " oauth_nonce=\"" + nonce + "\", oauth_timestamp=\"" + timestamp + "\", oauth_signature_method=\"" + HMACSHA256SignatureType + "\", oauth_version=\"" + OAuthVersion + "\", oauth_signature=\"" + signature + "\"");
                                    request.AddHeader("Content-Type", "application/json");
                                    request.AddParameter("application/json", actualizar, ParameterType.RequestBody);
                                    Console.WriteLine(request);
                                    var response = client.Execute(request);
                                    Console.WriteLine(response.Content);
                                    cadenanetsuite = JsonConvert.SerializeObject(response.Content, Formatting.Indented);
                                    producto.cadenanetsuite = cadenanetsuite;
                                    await repositorio.DispositivoAsincrono(producto);
                                }
                            }
                        }
                        if ( p.OrderType.ToUpper() == "NOTIFYPAREJAS")
                        {
                            if (bandera)
                            {
                                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                                producto = new Claro
                                {
                                    OrderType = p.OrderType,
                                    providerOrderId = p.providerOrderId,
                                    OrderId = p.orderId,
                                    //executionDate = p.executionDate,
                                    SerieSIM = simcard,
                                    IMEI = imei,
                                    APN = apn,
                                    IP = ip,
                                    TelefonoSIM = telefonoSim,
                                    Estado = estado,
                                    Fecha = string.Format("{0:dd/MM/yyyy}", DateTime.Now),
                                    Error = mensajerror,
                                    FechaActivacion = fechaactivacion,
                                    cadena = jsonString,
                                    respuesta = jsonRespuesta,
                                    cadenanetsuite = cadenanetsuite 
                                };
                                await repositorio.DispositivoAsincrono(producto);
                            }
                        }
                    }
                }
                if (p.OrderType.ToUpper() == "NOTIFRESCAMBPLAN")
                {
                    for (int i = 0; i < p.arrayCharacteristic.Count; i++)
                    {             
                        if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "TelefonoSIM" && bandera)
                        {
                            if (p.arrayCharacteristic[i].value == null || string.IsNullOrEmpty(p.arrayCharacteristic[i].value.ToString()))
                            {
                                bandera = false;
                                mensaje = "Debe de Enviar el valor de TelefonoSIM";
                                break;
                            }
                        }
                        if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "FechaEjecucion" && bandera)
                        {
                            if (p.arrayCharacteristic[i].value == null || string.IsNullOrEmpty(p.arrayCharacteristic[i].value.ToString()))
                            {
                                bandera = false;
                                mensaje = "Debe de Enviar el valor de FechaEjecucion";
                                break;
                            }
                        }
                        if (bandera)
                        {
                            if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "TelefonoSIM")
                            {
                                telefonoSim = p.arrayCharacteristic[i].value.ToString();
                            }
                            else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "FechaEjecucion")
                            {
                                fechaactivacion = p.arrayCharacteristic[i].value.ToString();
                            }
                        }
                    }                    
                }              
                if (p.OrderType.ToUpper() == "NOTIFRESCAMBPLAN"  )
                {
                    jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                    producto = new Claro
                    {
                        OrderType = p.OrderType,
                        providerOrderId = p.providerOrderId,
                        OrderId = p.orderId,
                        SerieSIM = simcard,
                        IMEI = imei,
                        APN = apn,
                        IP = ip,
                        TelefonoSIM = telefonoSim,
                        Estado = estado,
                        Fecha = string.Format("{0:dd/MM/yyyy}", DateTime.Now),
                        Error = mensajerror,
                        FechaActivacion = fechaactivacion,
                        cadena = jsonString,
                        respuesta = jsonRespuesta,
                        cadenanetsuite = cadenanetsuite
                    };
                    await repositorio.DispositivoAsincrono(producto);
                }
                /*cancela la reserva*/
                if ( p.OrderType.ToUpper() == "NOTIFYDELETE")
                {
                    DataSet cnstGenrl = new DataSet();
                    cnstGenrl = ConsultaDB.CnstReserva(p.orderId, p.providerOrderId, 2, simcard, imei);
                    if (cnstGenrl != null && cnstGenrl.Tables.Count > 0 && cnstGenrl.Tables[0].Rows.Count > 0)
                    {
                        codReserva = (string)cnstGenrl.Tables[0].Rows[0]["CODRESERVA"];
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
                        API_URL = API_URL + "&opcion=" + opcion + "&id=" + codReserva;
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
                            jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                            jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                            cadenanetsuite = JsonConvert.SerializeObject(myObj, Formatting.Indented);
                            producto = new Claro
                            {
                                OrderType = p.OrderType,
                                providerOrderId = p.providerOrderId,
                                OrderId = p.orderId,
                                SerieSIM = simcard,
                                IMEI = imei,
                                APN = apn,
                                IP = ip,
                                TelefonoSIM = telefonoSim,
                                Estado = estado,
                                Fecha = string.Format("{0:dd/MM/yyyy}", DateTime.Now),
                                Error = mensajerror,
                                FechaActivacion = fechaactivacion,
                                cadena = jsonString,
                                respuesta = jsonRespuesta,
                                cadenanetsuite = cadenanetsuite
                            };
                            await repositorio.DispositivoAsincrono(producto);
                        }
                        else
                        {
                            respuesta = new FlotaRespuestaDTO
                            {
                                code = 400,
                                status = "No se pudo hacer la Anulación de la Reserva",
                                message = mensaje,
                                orderId = p.orderId,
                                providerOrderId = p.providerOrderId
                            };
                            return BadRequest(respuesta);
                        }                      
                    }
                    else
                    {
                        respuesta = new FlotaRespuestaDTO
                        {
                            code = 400,
                            status = "No encontro el Registro para hacer la Anulación",
                            message = mensaje,
                            orderId = p.orderId,
                            providerOrderId = p.providerOrderId
                        };
                        return BadRequest(respuesta);
                    }                 
                }
                if (bandera)
                {
                    return Ok(respuesta);
                }
                else
                {
                    respuesta = new FlotaRespuestaDTO
                    {
                        code = 400,
                        status = "ERROR",
                        message = mensaje,
                        orderId = p.orderId,
                        providerOrderId = p.providerOrderId
                    };
                    return BadRequest(respuesta);
                }               
            }
            else
            {
                respuesta = new FlotaRespuestaDTO
                {
                    code = 400,
                    status = "ERROR",
                    message = mensaje,
                    orderId = p.orderId,
                    providerOrderId = p.providerOrderId

                };
                return BadRequest(respuesta);
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "ADMIN,CLAROFLOTA")]
        [Route("/PostVenta")]
        public async Task<ActionResult<string>> PostVenta(FlotaClaroDTO p)
        {
            Claro producto = null;
            FlotaRespuestaDTO respuesta = null;
            string jsonString = "";
            string jsonRespuesta = "";
            string TelefonoSIM = "";
            string NumeroSIM = "";
            string Plan = "";
            string Estado = "";
            string fecha = "";
            string SerieSIM = "";
            Boolean bandera = true;
            string mensaje = "";
            if (!(p.OrderType.ToUpper() == "CAMB" || p.OrderType.ToUpper() == "INAC" || p.OrderType.ToUpper() == "RECO" || p.OrderType.ToUpper() == "SUSP"))
            {
                bandera = false;
                mensaje = "Debe de Enviar el OrderType correcto";
            }
            if (bandera)
            {
                for (int i = 0; i < p.arrayCharacteristic.Count; i++)
                {
                    if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "TelefonoSIM" && bandera)
                    {
                        if (p.arrayCharacteristic[i].value == "")
                        {
                            bandera = false;
                            mensaje = "Debe de Enviar el valor del TelefonoSIM";
                            break;
                        }
                    }
                    if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Plan" && bandera)
                    {
                        if (p.arrayCharacteristic[i].value == "")
                        {
                            bandera = false;
                            mensaje = "Debe de Enviar el valor del Plan";
                            break;
                        }
                    }
                    if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "SerieSIM" && bandera)
                    {
                        if (p.arrayCharacteristic[i].value == "")
                        {
                            bandera = false;
                            mensaje = "Debe de Enviar el valor del SerieSIM";
                            break;
                        }
                    }
                    if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Estado" && bandera)
                    {
                        if (p.arrayCharacteristic[i].value == "")
                        {
                            bandera = false;
                            mensaje = "Debe de Enviar el valor del Estado";
                        }
                    }
                    if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Fecha" && bandera)
                    {
                        if (p.arrayCharacteristic[i].value == "")
                        {
                            bandera = false;
                            mensaje = "Debe de Enviar el valor del Fecha";
                            break;
                        }
                    }
                    //if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "SerieSIM" && bandera)
                    //{
                    //    if (p.arrayCharacteristic[i].value == "")
                    //    {
                    //        bandera = false;
                    //        mensaje = "Debe de Enviar el valor de SerieSIM";
                    //        break;
                    //    }
                    //}
                    if (bandera)
                    {
                        if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "TelefonoSIM")
                        {
                            TelefonoSIM = p.arrayCharacteristic[i].value;
                        }
                        else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Plan")
                        {
                            Plan = p.arrayCharacteristic[i].value;
                        }
                        //else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "NumeroSIM")
                        //{
                        //    NumeroSIM = p.arrayCharacteristic[i].value;
                        //}
                        else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Estado")
                        {
                            Estado = p.arrayCharacteristic[i].value;
                        }
                        else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "Fecha")
                        {
                            fecha = p.arrayCharacteristic[i].value;
                        }
                        else if ((p.arrayCharacteristic[i].name).TrimEnd().TrimStart() == "SerieSIM")
                        {
                            SerieSIM = p.arrayCharacteristic[i].value;
                        }
                        mensaje = "";
                    }
                }
            }
            if (bandera)
            {
                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                producto = new Claro
                {
                    OrderType = p.OrderType,
                    Fecha = fecha,
                    Estado = Estado,
                    Plan = Plan,
                    NumeroSIM = NumeroSIM,
                    TelefonoSIM = TelefonoSIM,
                    SerieSIM = SerieSIM,
                    cadena= jsonString,
                };
                DataSet cnstGenrl = new DataSet();
                cnstGenrl = ConsultaDB.CnstData(producto);
                if (cnstGenrl != null && cnstGenrl.Tables.Count > 0 && cnstGenrl.Tables[0].Rows.Count > 0)
                {
                    respuesta = new FlotaRespuestaDTO
                    {
                        code = 200,
                        status = "ACCEPTED",
                        message = mensaje,
                        orderId = (string)cnstGenrl.Tables[0].Rows[0]["orderid"],
                        providerOrderId = (string)cnstGenrl.Tables[0].Rows[0]["providerOrderId"]
                    };
                    jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                    producto.respuesta = jsonRespuesta;
                    await repositorio.CrearProductoAsincrono(producto);
                    return Ok(respuesta);
                }
                else
                {
                    respuesta = new FlotaRespuestaDTO
                    {
                        code = 400,
                        status = "ERROR",
                        message = mensaje,
                        orderId = "0",
                        providerOrderId ="0"

                    };
                    return BadRequest(respuesta);
                }
            }
            else
            {
                respuesta = new FlotaRespuestaDTO
                {
                    code = 400,
                    status = "ERROR",
                    message = mensaje,
                    orderId = "0",
                    providerOrderId = "0"

                };
                return BadRequest(respuesta);
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "ADMIN")]
        [Route("/CambioPlan")]
        public async Task<ActionResult<string>> CambioPlan(FlotaCambioPlanDTO p)
        {
            Claro producto = null;
            bool bandera = true;
            string cadenanetsuite = "";
            string jsonString = "";
            string jsonRespuesta = "";
            string mensaje = "";
            string orderId = "";
            string telefonoSim = "";
            string providerOrderId = "";
            for (int i = 0; i < p.serviceOrderItems.Count; i++)
            {
                for (int b = 0; b < p.serviceOrderItems[i].itemId.Count; b++)
                {
                    //telefonoSim = p.OrderDetail[i].TelefonoSIM.ToString();
                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "orderId" && bandera)
                    {
                        if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
                        {
                            bandera = false;
                            mensaje = "Debe de Enviar el valor de orderId";
                            break;
                        }
                    }
                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "newPlan" && bandera)
                    {
                        if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
                        {
                            bandera = false;
                            mensaje = "Debe de Enviar el valor de newPlan";
                            break;
                        }
                    }
                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "serviceId" && bandera)
                    {
                        if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
                        {
                            bandera = false;
                            mensaje = "Debe de Enviar el valor de serviceId";
                            break;
                        }
                    }
                    if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "providerOrderId" && bandera)
                    {
                        if (p.serviceOrderItems[i].itemId[b].value == null || string.IsNullOrEmpty(p.serviceOrderItems[i].itemId[b].value.ToString()))
                        {
                            bandera = false;
                            mensaje = "Debe de Enviar el valor de providerOrderId";
                            break;
                        }
                    }
                    if (bandera)
                    {
                        if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "orderId")
                        {
                            orderId = p.serviceOrderItems[i].itemId[b].value.ToString();
                        }
                        else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "serviceId")
                        {
                            telefonoSim = p.serviceOrderItems[i].itemId[b].value.ToString();
                        }
                        else if ((p.serviceOrderItems[i].itemId[b].type).TrimEnd().TrimStart() == "providerOrderId")
                        {
                            providerOrderId = p.serviceOrderItems[i].itemId[b].value.ToString();
                        }
                    }
                }
            }
            if (bandera)
            {
                FlotaRespuestaDTO respuesta = null;
                DataSet cnstGenrl = new DataSet();
                //valido = 1;
                //cnstGenrl = ConsultaDB.CnstGuardarData("", jsonString, "HunterAlta", valido, 1, p.orderId, p.serviceType, "");
                // p.externalId = (string)cnstGenrl.Tables[0].Rows[0]["codigo"];
                respuesta = new FlotaRespuestaDTO
                {
                    code = 200,
                    status = "OK",
                    message = "La creación de la orden en el sistema externo fue exitosa",
                    orderId = orderId,
                    providerOrderId = providerOrderId, // numero de orden de hunter
                    platform= "Hunter-Claro"
                };
                jsonRespuesta = System.Text.Json.JsonSerializer.Serialize(respuesta);
                jsonString = System.Text.Json.JsonSerializer.Serialize(p);
                jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                producto = new Claro
                {
                    OrderType = p.serviceOrderType,
                    providerOrderId = providerOrderId,
                    OrderId = orderId,
                    SerieSIM = "",
                    IMEI = "",
                    APN = "",
                    IP = "",
                    TelefonoSIM = telefonoSim,
                    Estado = "",
                    Fecha = string.Format("{0:dd/MM/yyyy}", DateTime.Now),
                    Error = "",
                    FechaActivacion = "",
                    cadena = jsonString,
                    respuesta = jsonRespuesta,
                    cadenanetsuite = cadenanetsuite
                };
                await repositorio.DispositivoAsincrono(producto);
                return Ok(respuesta);
            }
            else
            {
                FlotaRespuestaDTO error = null;
                error = new FlotaRespuestaDTO
                {
                    code = 400,
                    status = "Bad Request",
                    message = mensaje,
                    orderId = orderId,
                    providerOrderId = providerOrderId

                };
                return BadRequest(error);
            }
        }



    }
}
