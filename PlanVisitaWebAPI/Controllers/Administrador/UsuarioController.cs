using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models.Usuario;
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
    public class UsuarioController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            if (filtro == null)
                filtro = "";

            var response = new HttpResponseMessage();
            if (headers.Contains("jefeToken") && headers.GetValues("jefeToken") != null && !string.IsNullOrEmpty(headers.GetValues("jefeToken").First()))
            {


                var listaUsuarios = db.Usuario.Where(x => (x.Usuario_Nombre.Contains(filtro))).ToList();

                var paginationModel = new PaginationModel<UsuarioResponseModel>()
                {
                    CantidadTotal = listaUsuarios.Count,
                    Listado = listaUsuarios.Skip(skip).Take(take).Select(x => new UsuarioResponseModel()
                    {
                        Usuario_Id = x.Usuario_Id,
                        Es_Jefe = x.Usuario_Rol == "J",
                        Usuario_Nombre = x.Usuario_Nombre,
                        Id = x.Usuario_Id,
                        JefeVentas_Id = x.JefeVentas_Id,
                        Rol = x.Usuario_Rol,
                        Usuario = x.Usuario1,
                        Usuario_Pass = x.Usuario_Pass,
                        Vendedor_Id = x.Usuario_Vendedor_Id
                    })
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

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            var Usuario = db.Usuario.FirstOrDefault(x => x.Usuario_Id == id);

            var model = new UsuarioResponseModel()
            {
                Usuario_Id = Usuario.Usuario_Id,
                Es_Jefe = Usuario.Usuario_Rol == "J",
                Usuario_Nombre = Usuario.Usuario_Nombre,
                Id = Usuario.Usuario_Id,
                JefeVentas_Id = Usuario.JefeVentas_Id,
                Rol = Usuario.Usuario_Rol,
                Usuario = Usuario.Usuario1,
                Usuario_Pass = Usuario.Usuario_Pass,
                Vendedor_Id = Usuario.Usuario_Vendedor_Id
            };

            var json = JsonConvert.SerializeObject(model);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] UsuarioUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Usuario_Nombre) || string.IsNullOrEmpty(model.Usuario_Pass))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoUsuario = new Usuario() {
                    Usuario_Nombre = model.Usuario_Nombre,
                    JefeVentas_Id = model.JefeVentas_Id,
                    Usuario_Rol = model.Rol,
                    Usuario_Vendedor_Id = model.Usuario_Vendedor_Id,
                    Usuario1 = model.Usuario1,
                    Usuario_Pass = model.Usuario_Pass
                };

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

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] UsuarioUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Usuario_Nombre) || string.IsNullOrEmpty(model.Usuario_Pass))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoUsuario = db.Usuario.FirstOrDefault(x => x.Usuario_Id == id);



                nuevoUsuario.Usuario_Nombre = model.Usuario_Nombre;

                    nuevoUsuario.Usuario_Pass = model.Usuario_Pass;
                nuevoUsuario.Usuario_Vendedor_Id = model.Usuario_Vendedor_Id;
                nuevoUsuario.Usuario_Rol = model.Rol;
                nuevoUsuario.Usuario1 = model.Usuario1;
                nuevoUsuario.JefeVentas_Id = model.JefeVentas_Id;


                var resultado = db.SaveChanges();


                validation.Success = resultado > 0;
                if (resultado > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    validation.Message = "Editado con éxito";

                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    validation.Message = "Ocurrió un error al editar Usuario";
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
            var nuevoUsuario = db.Usuario.FirstOrDefault(x => x.Usuario_Id == id);
            if(nuevoUsuario != null)
            {
                db.Usuario.Remove(nuevoUsuario);
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
                    validation.Message = "Ocurrió un error al eliminar Usuario";
                }
            } else
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No existe el usuario.";
            }
            
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}