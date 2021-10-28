using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.Usuario;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlanVisitaWebAPI.Controllers
{
    public class UsuarioController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            var listaUsuarios = db.Usuario.Where(x => x.Usuario_Nombre.Contains(filtro)).ToList();

            var paginationModel = new PaginationModel<Usuario>()
            {
                CantidadTotal = listaUsuarios.Count,
                Listado = listaUsuarios.Skip(skip).Take(take)
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            var Usuario = db.Usuario.FirstOrDefault(x => x.Usuario_Id == id);


            var json = JsonConvert.SerializeObject(Usuario);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] UsuarioUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Usuario_Nombre) || string.IsNullOrEmpty(model.Usuario1) || model.JefeVentas_Id  == 0 || string.IsNullOrWhiteSpace(model.Usuario1))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoUsuario = new Usuario();
                nuevoUsuario.Usuario_Nombre = model.Usuario_Nombre;
                nuevoUsuario.Usuario_Vendedor_Id = model.Usuario_Vendedor_Id;
                nuevoUsuario.Usuario1 = model.Usuario1;
                nuevoUsuario.JefeVentas_Id = model.JefeVentas_Id;
                nuevoUsuario.Usuario_Pass = model.Usuario_Pass;

                db.Usuario.Add(nuevoUsuario);

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
                    validation.Message = "Ocurrió un error al crear usuario";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] UsuarioUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Usuario_Nombre) || string.IsNullOrEmpty(model.Usuario1) || model.JefeVentas_Id == 0 || string.IsNullOrWhiteSpace(model.Usuario1))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoUsuario = db.Usuario.FirstOrDefault(x => x.Usuario_Id == id);

                nuevoUsuario.Usuario_Nombre = model.Usuario_Nombre;
                nuevoUsuario.Usuario_Vendedor_Id = model.Usuario_Vendedor_Id;
                nuevoUsuario.Usuario1 = model.Usuario1;
                nuevoUsuario.JefeVentas_Id = model.JefeVentas_Id;
                if(!string.IsNullOrWhiteSpace(nuevoUsuario.Usuario_Pass))
                {
                    nuevoUsuario.Usuario_Pass = model.Usuario_Pass;
                }

                db.Usuario.Add(nuevoUsuario);

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
            validation.Message = "Por política de la empresa, no puede eliminar usuarioes.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}
