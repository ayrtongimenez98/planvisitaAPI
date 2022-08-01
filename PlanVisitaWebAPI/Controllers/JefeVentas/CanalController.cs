using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Canal;
using PlanVisitaWebAPI.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PlanVisitaWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CanalController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            HttpRequestMessage re = Request;
            var headers = re.Headers;


            var response = new HttpResponseMessage();
            string token = headers.GetValues("jefeToken").First();
            var usuarioid = Convert.ToInt32(token);
            var jefeVentasId = db.Usuario.First(x => x.Usuario_Id == usuarioid).JefeVentas_Id;
            var usuario = db.Usuario.First(x => x.Usuario_Id == usuarioid);
            var canales = new List<CanalResponseModel>();
            if (usuario.Usuario_Rol == "A")
            {
                canales = db.Canal.ToList().Select(x => new CanalResponseModel() { Canal_Id = x.Canal_Id, Descripcion = x.Descripcion, Tipo = ""}).ToList();
            }
            else {
                var listaCanalesId = db.JefeVentasCanal.ToList().Select(x => x.Canal_Id);
                canales = db.Canal.ToList().Where(x => listaCanalesId.Contains(x.Canal_Id)).Select(x => new CanalResponseModel() { Canal_Id = x.Canal_Id, Descripcion = x.Descripcion, Tipo = "" }).ToList();
            }
            

            var paginationModel = new PaginationModel<CanalResponseModel>()
            {
                CantidadTotal = canales.Count,
                Listado = canales.Skip(skip).Take(take)
            };
            var json = JsonConvert.SerializeObject(paginationModel, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            var canal = db.Canal.FirstOrDefault(x => x.Canal_Id == id);
            

            var json = JsonConvert.SerializeObject(canal);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] CanalUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Descripcion))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoCanal = new Canal();
                nuevoCanal.Descripcion = model.Descripcion;

                db.Canal.Add(nuevoCanal);

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
                    validation.Message = "Ocurrió un error al crear Canal";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] CanalUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Descripcion) || string.IsNullOrEmpty(model.Canal_Id.ToString()) || string.IsNullOrEmpty(id.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoCanal = db.Canal.FirstOrDefault(x => x.Canal_Id == id);

                nuevoCanal.Descripcion = model.Descripcion;

                db.Canal.Add(nuevoCanal);

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
                    validation.Message = "Ocurrió un error al crear Canal";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string id)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            validation.Success = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            validation.Message = "Por política de la empresa, no pueden eliminar canales.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}
