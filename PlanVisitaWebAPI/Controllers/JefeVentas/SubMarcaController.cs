using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.SubMarca;
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
    public class SubMarcaController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            var listaSubMarcas = db.SubMarca.Where(x => x.SubMarca_Nombre.Contains(filtro)).ToList().Select(x => new SubMarcaModel() {
                Marca = new Models.Marca.MarcaModel() { Marca_Nombre = x.Marcas.Marca_Nombre, Marca_Id = x.Marca_Id },
                SubMarca_Id = x.SubMarca_Id,
                SubMarca_Nombre = x.SubMarca_Nombre
            }).ToList();

            var paginationModel = new PaginationModel<SubMarcaModel>()
            {
                CantidadTotal = listaSubMarcas.Count,
                Listado = listaSubMarcas.Skip(skip).Take(take)
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            var SubMarca = db.SubMarca.Where(x => x.SubMarca_Id == id).Select(x => new SubMarcaModel()
            {
                Marca = new Models.Marca.MarcaModel() { Marca_Nombre = x.Marcas.Marca_Nombre, Marca_Id = x.Marca_Id },
                SubMarca_Id = x.SubMarca_Id,
                SubMarca_Nombre = x.SubMarca_Nombre
            }).ToList().First();


            var json = JsonConvert.SerializeObject(SubMarca);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] SubMarcaUpsertModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.SubMarca_Nombre))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoSubMarca = new SubMarca();
                nuevoSubMarca.Marca_Id = model.Marca_Id;
                nuevoSubMarca.SubMarca_Nombre = model.SubMarca_Nombre;

                db.SubMarca.Add(nuevoSubMarca);

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
                    validation.Message = "Ocurrió un error al crear SubMarca";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] SubMarcaUpsertModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.SubMarca_Nombre) || string.IsNullOrEmpty(id.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoSubMarca = db.SubMarca.FirstOrDefault(x => x.SubMarca_Id == id);

                nuevoSubMarca.SubMarca_Nombre = model.SubMarca_Nombre;
                nuevoSubMarca.Marca_Id = model.Marca_Id;

                db.SubMarca.Add(nuevoSubMarca);

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
                    validation.Message = "Ocurrió un error al crear SubMarca";
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
            validation.Message = "Por política de la empresa, no pueden eliminar SubMarcas.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}
