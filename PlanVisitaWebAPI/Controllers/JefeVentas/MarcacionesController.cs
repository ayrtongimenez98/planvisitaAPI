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

namespace PlanVisitaWebAPI.Controllers
{
    public class MarcacionesController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get([FromBody] MarcacionesFiltroModel model)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            if (model.Filtro == null)
                model.Filtro = "";

            var response = new HttpResponseMessage();
            if (headers.Contains("jefeToken") && headers.GetValues("jefeToken") != null && !string.IsNullOrEmpty(headers.GetValues("jefeToken").First()))
            {
                string token = headers.GetValues("jefeToken").First();
                var jefeVentasId = Convert.ToInt32(token);
                var canales = db.JefeVentas.First(x => x.JefeVentas_Id == jefeVentasId).Canal;
                var canalesId = canales.Select(x => x.Canal_Id);
                var sucursalesId = db.CanalSucursal.Where(x => canalesId.Any(y => y == x.Canal_Id)).ToList().Select(x => x.Sucursal_Id);
                var vendedoresId = db.VendedorCliente.Where(x => sucursalesId.Any(y => y == x.Sucursal_Id)).ToList().Select(x => x.Vendedor_Id);

                var marcacionesList = new List<V_Visitas_Detalle>();
                var paginationModel = new PaginationModel<V_Visitas_Detalle>();
                var marcacionesQuery = db.V_Visitas_Detalle.Where(x => x.Cliente.Contains(model.Filtro) || x.Dirección.Contains(model.Filtro));

                if(model.Vendedor_Id != 0)
                {
                    marcacionesQuery = marcacionesQuery.Where(x => x.Vendedor_Id == model.Vendedor_Id);
                } else
                {
                    marcacionesQuery = marcacionesQuery.Where(x => vendedoresId.Any(y => y == x.Vendedor_Id));
                }

                if(model.Cliente_Cod != null)
                {
                    marcacionesQuery = marcacionesQuery.Where(x => x.CodCliente == model.Cliente_Cod);
                }

                if(model.Fecha_Desde.Date != Convert.ToDateTime("01/01/0001").Date && model.Fecha_Hasta.Date != Convert.ToDateTime("01/01/0001").Date)
                {
                    marcacionesQuery = marcacionesQuery.Where(x => x.Visita_fecha >= model.Fecha_Desde && x.Visita_fecha <= model.Fecha_Hasta);
                }
                marcacionesList = marcacionesQuery.ToList();

                paginationModel.CantidadTotal = marcacionesList.Count;
                paginationModel.Listado = marcacionesList.Skip(model.Skip).Take(model.Take);

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
            if (headers.Contains("jefeToken") && headers.GetValues("jefeToken") != null && !string.IsNullOrEmpty(headers.GetValues("jefeToken").First()))
            {
                string token = headers.GetValues("jefeToken").First();
                var jefeVentasId = Convert.ToInt32(token);
                var marcacion = db.V_Visitas_Detalle.FirstOrDefault(x => x.Visita_Id == id);

                if (marcacion != null)
                {
                    var json = JsonConvert.SerializeObject(marcacion);
                    response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                } else
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
            if (headers.Contains("jefeToken") && headers.GetValues("jefeToken") != null && !string.IsNullOrEmpty(headers.GetValues("jefeToken").First()))
            {
                string token = headers.GetValues("jefeToken").First();
                var jefeVentasId = Convert.ToInt32(token);

                var newVisita = new Visita() { 
                    Visita_Ubicacion = value.Visita_Ubicacion,
                    Visita_Observacion = value.Visita_Observacion,
                    Estado_Id = value.Estado_Id,
                    Motivo_Id = value.Motivo_Id,
                    Sucursal_Id = value.Sucursal_Id,
                    Vendedor_Id = value.Vendedor_Id,
                    Visita_fecha = value.Visita_fecha,
                    Visita_Hora = value.Visita_Hora
                };

                db.Visita.Add(newVisita);

                var resultado = db.SaveChanges();

                if (resultado > 0)
                {
                    validation.Success = false;
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
        public HttpResponseMessage Put(int id)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            validation.Success = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            validation.Message = "Por política de la empresa, no puede editar marcaciones.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
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