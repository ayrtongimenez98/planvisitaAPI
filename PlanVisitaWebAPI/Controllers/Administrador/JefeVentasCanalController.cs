using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Canal;
using PlanVisitaWebAPI.Models.JefeVentasCanal;
using PlanVisitaWebAPI.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PlanVisitaWebAPI.Controllers.Administrador
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class JefeVentasCanalController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/JefeVentasCanal
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/JefeVentasCanal/5
        public HttpResponseMessage Get(int id)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            var response = new HttpResponseMessage();
            var lista = db.JefeVentasCanal.Where(x => x.JefeVentas_Id == id).ToList().Select(x => x.Canal_Id).ToList();
            var query = "SELECT GroupCode as Canal_Id, GroupName as Descripcion, GroupType as Tipo FROM V_Canales_HBF c";
            var canales = db.Database.SqlQuery<CanalResponseModel>(query).ToList<CanalResponseModel>();
            var listaReal = canales.Where(x => lista.Contains(x.Canal_Id)).ToList();
            var paginationModel = new PaginationModel<CanalResponseModel>()
            {
                CantidadTotal = listaReal.Count,
                Listado = listaReal
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST: api/JefeVentasCanal
        public HttpResponseMessage Post([FromBody] JefeVentasCanalUpsertModel value)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            var validation = new SystemValidationModel();
            var response = new HttpResponseMessage();
            if (headers.Contains("jefeToken") && headers.GetValues("jefeToken") != null && !string.IsNullOrEmpty(headers.GetValues("jefeToken").First()))
            {
                var nuevoJefeCanal = new JefeVentasCanal();
                nuevoJefeCanal.Canal_Id = value.Canal_Id;
                nuevoJefeCanal.JefeVentas_Id = value.JefeVentas_Id;

                db.JefeVentasCanal.Add(nuevoJefeCanal);

                var resultado = db.SaveChanges();


                validation.Success = resultado > 0;
                if (resultado > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    validation.Message = "Creado con éxito";

                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    validation.Message = "Ocurrió un error al crear Usuario";
                }
                var json = JsonConvert.SerializeObject(validation);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                validation = new SystemValidationModel()
                {
                    Success = false,
                    Message = "Credenciales incorrectas."
                };
                var json = JsonConvert.SerializeObject(validation);
                response = Request.CreateResponse(HttpStatusCode.NonAuthoritativeInformation);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }
            return response;
         }

        // PUT: api/JefeVentasCanal/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/JefeVentasCanal/5
        public HttpResponseMessage Delete([FromBody] JefeVentasCanalUpsertModel value)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            var validation = new SystemValidationModel();
            var response = new HttpResponseMessage();
            if (headers.Contains("jefeToken") && headers.GetValues("jefeToken") != null && !string.IsNullOrEmpty(headers.GetValues("jefeToken").First()))
            {
                var nuevoJefeCanal = db.JefeVentasCanal.First(x => x.JefeVentas_Id == value.JefeVentas_Id && x.Canal_Id == value.Canal_Id);

                db.JefeVentasCanal.Remove(nuevoJefeCanal);

                var resultado = db.SaveChanges();


                validation.Success = resultado > 0;
                if (resultado > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    validation.Message = "Eliminado con éxito";

                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    validation.Message = "Ocurrió un error al eliminar";
                }
                var json = JsonConvert.SerializeObject(validation);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }
            else
            {
                validation = new SystemValidationModel()
                {
                    Success = false,
                    Message = "Credenciales incorrectas."
                };
                var json = JsonConvert.SerializeObject(validation);
                response = Request.CreateResponse(HttpStatusCode.NonAuthoritativeInformation);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }
            return response;
        }
    }
}
