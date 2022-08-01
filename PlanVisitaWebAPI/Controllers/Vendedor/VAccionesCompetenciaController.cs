using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
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
    public class VAccionesCompetenciaController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/VAccionesCompetencia
        public HttpResponseMessage Get(string filtro = "", int skip = 0, int take = 10)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            if (filtro == null)
                filtro = "";
            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                string token = headers.GetValues("userToken").First();
                var vendedor = Convert.ToInt32(token);
                var nombreV = db.Vendedor.FirstOrDefault(x => x.Vendedor_Id == vendedor);

                var listaDivisions = db.AccionesCompetencia.Where(x => x.AccionesCompetencia_Colaborador == nombreV.Vendedor_Nombre && x.AccionesCompetencia_Descripcion.Contains(filtro)).ToList();

                var paginationModel = new PaginationModel<AccionesCompetencia>()
                {
                    CantidadTotal = listaDivisions.Count,
                    Listado = listaDivisions.Skip(skip).Take(take)
                };

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

        // GET: api/VAccionesCompetencia/5
        public HttpResponseMessage Get(int id)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;


            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                string token = headers.GetValues("userToken").First();
                var vendedor = Convert.ToInt32(token);
                var nombreV = db.Vendedor.FirstOrDefault(x => x.Vendedor_Id == vendedor);
                var marcacion = db.AccionesCompetencia.FirstOrDefault(x => x.AccionesCompetencia_Id == id);

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
                        Message = "No existe el registro buscado."
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

        // POST: api/VAccionesCompetencia
        public HttpResponseMessage Post([FromBody] AccionesCompetencia model)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;

            var validation = new SystemValidationModel();
            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                string token = headers.GetValues("userToken").First();
                var vendedor = Convert.ToInt32(token);
                var nombreV = db.Vendedor.FirstOrDefault(x => x.Vendedor_Id == vendedor);
                var newVisita = new AccionesCompetencia()
                {
                    AccionesCompetencia_Division = model.AccionesCompetencia_Division,
                    AccionesCompetencia_Canal = model.AccionesCompetencia_Canal,
                    AccionesCompetencia_Descripcion = model.AccionesCompetencia_Descripcion,
                    AccionesCompetencia_Observacion = model.AccionesCompetencia_Observacion,
                    AccionesCompetencia_Precio = model.AccionesCompetencia_Precio,
                    AccionesCompetencia_PrecioOferta = model.AccionesCompetencia_PrecioOferta,
                    AccionesCompetencia_Colaborador = nombreV.Vendedor_Nombre,
                    AccionesCompetencia_PuntoVentaDireccion = model.AccionesCompetencia_PuntoVentaDireccion,
                    AccionesCompetencia_FechaCreacion = DateTime.Now
                };
                db.AccionesCompetencia.Add(newVisita);

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

        // PUT: api/VAccionesCompetencia/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/VAccionesCompetencia/5
        public void Delete(int id)
        {
        }
    }
}
