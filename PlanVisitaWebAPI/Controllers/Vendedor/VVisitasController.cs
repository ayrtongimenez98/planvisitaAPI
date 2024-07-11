using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Marcaciones;
using PlanVisitaWebAPI.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PlanVisitaWebAPI.Controllers.Vendedor
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VVisitasController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(DateTime Fecha_Desde, DateTime Fecha_Hasta, string Cliente_Cod = "", string Filtro = "", int Skip = 0, int Take = 10)
        {

            HttpRequestMessage re = Request;
            var headers = re.Headers;
            if (Filtro == null)
                Filtro = "";

            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                string token = headers.GetValues("userToken").First();
                var vendedor = Convert.ToInt32(token);

                var marcacionesList = new List<MarcacionesResponseModel>();
                var paginationModel = new PaginationModel<MarcacionesResponseModel>();
                var marcacionesQuery = db.Database.SqlQuery<MarcacionesResponseModel>(@"
                   SELECT vs.Visita_Id,  
                    	   FORMAT (vs.Visita_fecha, 'yyyy_MM') as Periodo, 
                    	   vs.Visita_fecha, 
                           CAST(FORMAT(vs.Visita_Hora_Entrada, 'hh:mm') AS varchar) AS Visita_Hora_Entrada, 
                    	   CAST(FORMAT(vs.Visita_Hora_Salida, 'hh:mm')AS varchar) AS Visita_Hora_Salida, 
                    	   vs.Vendedor_Id, 
                    	   v.Vendedor_Nombre as Vendedor, 
                    	   vs.Cliente_Cod as CodCliente, 
                    	   c.Cliente_RazonSocial as Cliente,
                    	   s.Sucursal_Ciudad as Ciudad, 
                            s.Sucursal_Direccion as Direccion, 
                            cast(vs.Sucursal_Id as varchar) as Sucursal_Id, 
                    	   vs.Visita_Observacion as Observacion, 
                    	   vs.Visita_Ubicacion_Entrada, 
                    	   vs.Visita_Ubicacion_Salida,
                            DATENAME(MONTH, vs.Visita_fecha) AS Mes,
                            DATENAME(YEAR, vs.Visita_fecha) AS Año,
                            DATENAME(WEEKDAY, vs.Visita_fecha) AS Dia
                    FROM VisitaSAP vs 
                    inner join Vendedor v on vs.Vendedor_Id = v.Vendedor_Id
                    inner join Sucursal s on vs.Sucursal_Id  = s.Sucursal_Id and vs.Cliente_Cod = s.Cliente_Cod
					inner join Cliente c on vs.Cliente_Cod = s.Cliente_Cod
                    inner join Motivo m on m.Motivo_Id = vs.Motivo_Id
                    inner join Estado e on vs.Estado_Id = e.Estado_Id").ToList<MarcacionesResponseModel>();

                if (vendedor != 0)
                {
                    marcacionesQuery = marcacionesQuery.Where(x => x.Vendedor_Id == vendedor).ToList();
                }

                if (!string.IsNullOrEmpty(Cliente_Cod) && Cliente_Cod != "null")
                {
                    marcacionesQuery = marcacionesQuery.Where(x => x.CodCliente == Cliente_Cod).ToList();
                }

                if (Fecha_Desde.Date != Convert.ToDateTime("01/01/0001").Date && Fecha_Hasta.Date != Convert.ToDateTime("01/01/0001").Date)
                {
                    marcacionesQuery = marcacionesQuery.Where(x => x.Visita_fecha.Date >= Fecha_Desde.Date && x.Visita_fecha.Date <= Fecha_Hasta.Date).ToList();
                }
                marcacionesList = marcacionesQuery.ToList();

                paginationModel.CantidadTotal = marcacionesList.Count;
                paginationModel.Listado = marcacionesList.Skip(Skip).Take(Take);

                var json = JsonConvert.SerializeObject(paginationModel);
                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            }
            else
            {
                var validation = new SystemValidationModel()
                {
                    Success = false,
                    Message = "Credenciales incorrectas."
                };
                var json = JsonConvert.SerializeObject(validation);
                response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;


            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                string token = headers.GetValues("userToken").First();
                var vendedor = Convert.ToInt32(token);

                var marcacionesQuery = db.Database.SqlQuery<MarcacionesResponseModel>(@"
                   SELECT vs.Visita_Id,  
                    	   FORMAT (vs.Visita_fecha, 'yyyy_MM') as Periodo, 
                    	   vs.Visita_fecha, 
                           CAST(FORMAT(vs.Visita_Hora_Entrada, 'hh:mm') AS varchar) AS Visita_Hora_Entrada, 
                    	   CAST(FORMAT(vs.Visita_Hora_Salida, 'hh:mm')AS varchar) AS Visita_Hora_Salida, 
                    	   vs.Vendedor_Id, 
                    	   v.Vendedor_Nombre as Vendedor, 
                    	   vs.Cliente_Cod as CodCliente, 
                    	   c.Cliente_RazonSocial as Cliente,
                    	   s.Sucursal_Ciudad as Ciudad, 
                            s.Sucursal_Direccion as Direccion, 
                            cast(vs.Sucursal_Id as varchar) as Sucursal_Id, 
                    	   vs.Visita_Observacion as Observacion, 
                    	   vs.Visita_Ubicacion_Entrada, 
                    	   vs.Visita_Ubicacion_Salida,
                            DATENAME(MONTH, vs.Visita_fecha) AS Mes,
                            DATENAME(YEAR, vs.Visita_fecha) AS Año,
                            DATENAME(WEEKDAY, vs.Visita_fecha) AS Dia
                    FROM VisitaSAP vs 
                    inner join Vendedor v on vs.Vendedor_Id = v.Vendedor_Id
                    inner join Sucursal s on vs.Sucursal_Id  = s.Sucursal_Id and vs.Cliente_Cod = s.Cliente_Cod
					inner join Cliente c on vs.Cliente_Cod = s.Cliente_Cod
                    inner join Motivo m on m.Motivo_Id = vs.Motivo_Id
                    inner join Estado e on vs.Estado_Id = e.Estado_Id").ToList<MarcacionesResponseModel>();

                var marcacion = marcacionesQuery.FirstOrDefault(x => x.Visita_Id == id);

                if (marcacion != null)
                {
                    var json = JsonConvert.SerializeObject(marcacion);
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    var validation = new SystemValidationModel()
                    {
                        Success = false,
                        Message = "No existe la visita buscada."
                    };
                    var json = JsonConvert.SerializeObject(validation);
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }
            }
            else
            {
                var validation = new SystemValidationModel()
                {
                    Success = false,
                    Message = "Credenciales incorrectas."
                };
                var json = JsonConvert.SerializeObject(validation);
                response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] MarcacionesUpdateModel value)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;

            var validation = new SystemValidationModel();
            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                string token = headers.GetValues("userToken").First();
                var vendedor = Convert.ToInt32(token);
                var visitas = db.VisitaSAP.Where(x => string.IsNullOrEmpty(x.Visita_Ubicacion_Salida) && x.Visita_Hora_Salida == null);
                if(visitas.Any())
                {
                    validation.Success = false;
                    validation.Message = "Debe cerrar la visita actual para abrir la siguiente.";
                    var json = JsonConvert.SerializeObject(validation);
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    return response;
                }
                var newVisita = new VisitaSAP()
                {
                    Cliente_Cod = value.Cliente_Cod,
                    Visita_Observacion = value.Visita_Observacion,
                    Estado_Id = value.Estado_Id,
                    Motivo_Id = value.Motivo_Id,
                    Sucursal_Id = value.Sucursal_Id,
                    Vendedor_Id = vendedor,
                    Visita_fecha = value.Visita_fecha,
                    Visita_Hora_Entrada = value.Visita_Hora_Entrada,
                    Visita_Ubicacion_Entrada = value.Visita_Ubicacion_Entrada,
                    Visita_Ubicacion_Salida = "",
                    Visita_Hora_Salida = null
                };
                db.VisitaSAP.Add(newVisita);
                var plan = db.PlanSemanalSAP.ToList().Where(x => x.Vendedor_Id == vendedor && x.Sucursal_Id == value.Sucursal_Id && x.Cliente_Cod == value.Cliente_Cod && x.PlanSemanal_Horario.Date == DateTime.Now.Date && x.PlanSemanal_Estado == "N");
                foreach (var item in plan)
                {
                    if(value.Visita_Ubicacion_Salida != null && value.Visita_Ubicacion_Salida != "") {
                        item.PlanSemanal_Estado = item.PlanSemanal_Horario.Date <= DateTime.Now.Date ? "S" : "A";
                    } else
                    {
                        item.PlanSemanal_Estado = item.PlanSemanal_Horario.Date <= DateTime.Now.Date ? "E" : "A";
                    }
                        
                }

                

                var resultado = db.SaveChanges();

                if (resultado > 0)
                {
                    validation.Success = true;
                    validation.Message = "Creado con éxito";
                    var json = JsonConvert.SerializeObject(validation);
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }
                else
                {
                    validation.Success = false;
                    validation.Message = "Ocurrió un error al añadir.";
                    var json = JsonConvert.SerializeObject(validation);
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }
            }
            else
            {
                validation.Success = false;
                validation.Message = "Credenciales incorrectas.";
                var json = JsonConvert.SerializeObject(validation);
                response = Request.CreateResponse(HttpStatusCode.BadRequest);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            return response;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody] MarcacionesUpdateModel value)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;

            var validation = new SystemValidationModel();
            var response = new HttpResponseMessage();
            try
            {
                if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
                {
                    string token = headers.GetValues("userToken").First();
                    var vendedor = Convert.ToInt32(token);

                    var visita = db.VisitaSAP.FirstOrDefault(x => x.Visita_Id == value.Visita_Id);



                    visita.Visita_Hora_Salida = value.Visita_Hora_Salida;
                    visita.Visita_Ubicacion_Salida = value.Visita_Ubicacion_Salida;
                    visita.Visita_Observacion = value.Visita_Observacion;
                    visita.Estado_Id = value.Estado_Id;
                    visita.Motivo_Id = value.Motivo_Id;

                    var plan = db.PlanSemanalSAP.ToList().Where(x => x.Vendedor_Id == vendedor && x.Sucursal_Id == value.Sucursal_Id && x.Cliente_Cod == value.Cliente_Cod && x.PlanSemanal_Horario.Date == DateTime.Now.Date && x.PlanSemanal_Estado == "E");


                    foreach (var item in plan)
                    {
                        item.PlanSemanal_Estado = item.PlanSemanal_Horario.Date <= DateTime.Now.Date ? "S" : "A";
                    }
                    var resultado = db.SaveChanges();

                    if (resultado > 0)
                    {
                        validation.Success = true;
                        validation.Message = "Actualizado con éxito";
                        var json = JsonConvert.SerializeObject(validation);
                        response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        validation.Success = false;
                        validation.Message = "Ocurrió un error al actualizar.";
                        var json = JsonConvert.SerializeObject(validation);
                        response = Request.CreateResponse(HttpStatusCode.NotFound);
                        response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    }
                }
                else
                {
                    validation.Success = false;
                    validation.Message = "Credenciales incorrectas.";
                    var json = JsonConvert.SerializeObject(validation);
                    response = Request.CreateResponse(HttpStatusCode.BadRequest);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }
            }
            catch (Exception ex)
            {
                validation.Success = false;
                validation.Message = ex.Message;
                var json = JsonConvert.SerializeObject(validation);
                response = Request.CreateResponse(HttpStatusCode.NotFound);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            return response;
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(int id)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            validation.Success = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            validation.Message = "Por política de la empresa, no puede eliminar marcaciones.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}