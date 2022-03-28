using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models.Vendedor;
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
    public class VendedorController : ApiController
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
                

                var listaVendedors = db.Vendedor.Where(x => ( x.Vendedor_Nombre.Contains(filtro) || x.Vendedor_Mail.Contains(filtro))).ToList();

                var paginationModel = new PaginationModel<VendedorSucursalResponseModel>()
                {
                    CantidadTotal = listaVendedors.Count,
                    Listado = listaVendedors.Skip(skip).Take(take).Select(x => new VendedorSucursalResponseModel() { 
                                                                                                                     Vendedor_Id = x.Vendedor_Id,
                                                                                                                     Vendedor_Mail = x.Vendedor_Mail,
                                                                                                                     Vendedor_Nombre = x.Vendedor_Nombre,
                                                                                                                     Vendedor_Rol = x.Vendedor_Rol,
                                                                                                                     JefeVentas_Id = x.JefeVentas_Id
                    })
                };
                var json = JsonConvert.SerializeObject(paginationModel);
                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            } else
            {
                var validation = new SystemValidationModel() { 
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
            var Vendedor = db.Vendedor.FirstOrDefault(x => x.Vendedor_Id == id);

            var model = new VendedorSucursalResponseModel()
            {
                Vendedor_Id = Vendedor.Vendedor_Id,
                Vendedor_Mail = Vendedor.Vendedor_Mail,
                Vendedor_Nombre = Vendedor.Vendedor_Nombre,
                Vendedor_Rol = Vendedor.Vendedor_Rol
            };

            var json = JsonConvert.SerializeObject(model);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] VendedorUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Nombre) || model.Id == 0)
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoVendedor = new DB.Vendedor();
                nuevoVendedor.Vendedor_Id = model.Id;
                nuevoVendedor.Vendedor_Nombre = model.Nombre;
                nuevoVendedor.Vendedor_FechaCreacion = DateTime.Now;
                nuevoVendedor.Vendedor_FechaLastUpdate = DateTime.Now;
                nuevoVendedor.JefeVentas_Id = model.JefeVentasId;
                nuevoVendedor.Vendedor_Mail = model.Email;
                nuevoVendedor.Vendedor_Rol = model.Rol;

                
                db.Vendedor.Add(nuevoVendedor);

                

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
                    validation.Message = "Ocurrió un error al crear Vendedor";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] VendedorUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Nombre) || model.Id == 0 || id == 0)
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoVendedor = db.Vendedor.FirstOrDefault(x => x.Vendedor_Id == id);


                
                nuevoVendedor.Vendedor_Nombre = model.Nombre;
                nuevoVendedor.Vendedor_FechaLastUpdate = DateTime.Now;
                nuevoVendedor.Vendedor_Mail = model.Email;
                nuevoVendedor.Vendedor_Rol = model.Rol;

               

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
                    validation.Message = "Ocurrió un error al crear Vendedor";
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
            validation.Message = "Por política de la empresa, no pueden eliminar vendedores.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}