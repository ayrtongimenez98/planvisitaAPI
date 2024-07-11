using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.TipoMueble;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models;
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
    public class TipoMuebleController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            var listaTipoMuebles = db.TipoMueble.Where(x => x.TipoMueble_Tipo.Contains(filtro)).ToList().Select(x => new TipoMuebleModel()
            {
                Marca = new Models.Marca.MarcaModel() { Marca_Nombre = x.Marcas.Marca_Nombre, Marca_Id = x.Marca_Id },
                TipoMueble_Id = x.TipoMueble_Id,
                TipoMueble_Tipo = x.TipoMueble_Tipo,
            }).ToList();

            var paginationModel = new PaginationModel<TipoMuebleModel>()
            {
                CantidadTotal = listaTipoMuebles.Count,
                Listado = listaTipoMuebles.Skip(skip).Take(take)
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            var TipoMueble = db.TipoMueble.Where(x => x.TipoMueble_Id == id).ToList().Select(x => new TipoMuebleModel()
            {
                Marca = new Models.Marca.MarcaModel() { Marca_Nombre = x.Marcas.Marca_Nombre, Marca_Id = x.Marca_Id },
                TipoMueble_Id = x.TipoMueble_Id,
                TipoMueble_Tipo = x.TipoMueble_Tipo,
            }).ToList().First();


            var json = JsonConvert.SerializeObject(TipoMueble);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] TipoMuebleUpsertModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.TipoMueble_Tipo))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoTipoMueble = new TipoMueble();
                nuevoTipoMueble.TipoMueble_Tipo = model.TipoMueble_Tipo;
                nuevoTipoMueble.Marca_Id = model.Marca_Id;

                db.TipoMueble.Add(nuevoTipoMueble);

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
                    validation.Message = "Ocurrió un error al crear TipoMueble";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] TipoMuebleUpsertModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.TipoMueble_Tipo) || string.IsNullOrEmpty(id.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoTipoMueble = db.TipoMueble.FirstOrDefault(x => x.TipoMueble_Id == id);

                nuevoTipoMueble.TipoMueble_Tipo = model.TipoMueble_Tipo;
                nuevoTipoMueble.Marca_Id = model.Marca_Id;

                db.TipoMueble.Add(nuevoTipoMueble);

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
                    validation.Message = "Ocurrió un error al crear TipoMueble";
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
            validation.Message = "Por política de la empresa, no pueden eliminar TipoMuebles.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}
