using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.JefeVentas;
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
    public class JefeVentasController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            var listaJefeVentass = db.JefeVentas.Where(x => x.JefeVentas_Nombre.Contains(filtro) || x.JefeVentas_Id.ToString().Contains(filtro) || x.JefeVentas_Mail.Contains(filtro)).ToList();

            var paginationModel = new PaginationModel<JefeVentasResponseModel>()
            {
                CantidadTotal = listaJefeVentass.Count,
                Listado = listaJefeVentass.Skip(skip).Take(take).Select(x => new JefeVentasResponseModel() {Division_Id = x.Division_Id, JefeVentas_Id = x.JefeVentas_Id, JefeVentas_Mail = x.JefeVentas_Mail, JefeVentas_Nombre = x.JefeVentas_Nombre})
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            var JefeVentas = db.JefeVentas.ToList().Select(x => new JefeVentasResponseModel() { Division_Id = x.Division_Id, JefeVentas_Id = x.JefeVentas_Id, JefeVentas_Mail = x.JefeVentas_Mail, JefeVentas_Nombre = x.JefeVentas_Nombre }).FirstOrDefault(x => x.JefeVentas_Id == id);


            var json = JsonConvert.SerializeObject(JefeVentas);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] JefeVentasUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.JefeVentas_Nombre) || string.IsNullOrEmpty(model.JefeVentas_Id.ToString()) || string.IsNullOrEmpty(model.JefeVentas_Mail) || string.IsNullOrEmpty(model.Division_Id.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoJefeVentas = new DB.JefeVentas();
                nuevoJefeVentas.JefeVentas_Nombre = model.JefeVentas_Nombre;
                nuevoJefeVentas.JefeVentas_Id = model.JefeVentas_Id;
                nuevoJefeVentas.JefeVentas_FechaCreacion = DateTime.Now;
                nuevoJefeVentas.Division_Id = model.Division_Id;
                nuevoJefeVentas.JefeVentas_Mail = model.JefeVentas_Mail;
                nuevoJefeVentas.JefeVentas_FechaLastUpdate = DateTime.Now;

                db.JefeVentas.Add(nuevoJefeVentas);

                var resultado = db.SaveChanges();

                if (resultado > 0)
                {
                    var ultimoId = db.JefeVentas.ToList().OrderByDescending(x => x.JefeVentas_Id).First();
                    foreach (var item in model.CanalesId)
                    {
                        var jefeCanal = new JefeVentasCanal();
                        jefeCanal.JefeVentas_Id = ultimoId.JefeVentas_Id;
                        jefeCanal.Canal_Id = item;

                        db.JefeVentasCanal.Add(jefeCanal);
                    }
                }

                


                validation.Success = resultado > 0;
                if (resultado > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    validation.Message = "Creado con éxito";

                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    validation.Message = "Ocurrió un error al crear JefeVentas";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] JefeVentasUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.JefeVentas_Nombre) || string.IsNullOrEmpty(model.JefeVentas_Id.ToString()) || string.IsNullOrEmpty(id.ToString()) || string.IsNullOrEmpty(model.JefeVentas_Mail) || string.IsNullOrEmpty(model.Division_Id.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoJefeVentas = db.JefeVentas.FirstOrDefault(x => x.JefeVentas_Id == id);

                nuevoJefeVentas.JefeVentas_Nombre = model.JefeVentas_Nombre;
                nuevoJefeVentas.JefeVentas_Id = model.JefeVentas_Id;
                nuevoJefeVentas.JefeVentas_FechaCreacion = DateTime.Now;
                nuevoJefeVentas.Division_Id = model.Division_Id;
                nuevoJefeVentas.JefeVentas_Mail = model.JefeVentas_Mail;
                nuevoJefeVentas.JefeVentas_FechaLastUpdate = DateTime.Now;

                if (model.CanalesId.Any())
                {
                    var jefesCanales = db.JefeVentasCanal.Where(x => x.JefeVentas_Id == id).ToList();
                    db.JefeVentasCanal.RemoveRange(jefesCanales);
                    foreach (var item in model.CanalesId)
                    {
                        var jefeCanal = new JefeVentasCanal();
                        jefeCanal.JefeVentas_Id = id;
                        jefeCanal.Canal_Id = item;

                        db.JefeVentasCanal.Add(jefeCanal);
                    }
                }

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
                    validation.Message = "Ocurrió un error al crear JefeVentas";
                }
            }
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
            validation.Message = "Por política de la empresa, no pueden eliminar Jefes de Ventas.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}
