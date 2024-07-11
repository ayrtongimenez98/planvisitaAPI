using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models.Vencimiento;
using PlanVisitaWebAPI.Models.Puntera;
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
    public class VPunteraController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/VPuntera
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
                var usuario = nombreV.Usuario.First();
                var usuarioEntity = db.Usuario.FirstOrDefault(x => x.Usuario_Id == usuario.Usuario_Id);

                var listaDivisions = db.Puntera.Where(x => x.Usuario_Id == usuarioEntity.Usuario_Id).ToList();
                var listaModelos = listaDivisions.Select(x => new PunteraModel()
                {
                    Marca = new Models.Marca.MarcaModel() { Marca_Id = x.Marca_Id, Marca_Nombre = x.Marcas.Marca_Nombre },
                    Puntera_Cantidad = x.Puntera_Cantidad,
                    Puntera_FechaCarga = x.Puntera_FechaCarga,
                    Puntera_Id = x.Puntera_Id,
                    Puntera_LucesEncendidas = x.Puntera_LucesEncendidas,
                    Puntera_Observacion = x.Puntera_Observacion,
                    SubMarca = new Models.SubMarca.SubMarcaModel() { SubMarca_Id = x.SubMarca_Id, SubMarca_Nombre = x.SubMarca.SubMarca_Nombre },
                    Usuario = new Models.Usuario.UsuarioResponseModel()
                    {
                        Es_Jefe = x.Usuario.Usuario_Rol != "J",
                        Id = x.Usuario_Id,
                        JefeVentas_Id = x.Usuario.JefeVentas_Id,
                        Rol = x.Usuario.Usuario_Rol,
                        Usuario_Id = x.Usuario_Id,
                        Usuario_Nombre = x.Usuario.Usuario_Nombre,
                        Vendedor_Id = x.Usuario.Usuario_Vendedor_Id
                    }
                }).ToList();

                var paginationModel = new PaginationModel<PunteraModel>()
                {
                    CantidadTotal = listaModelos.Count,
                    Listado = listaModelos.Skip(skip).Take(take)
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

        // GET: api/VPuntera/5
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
                var marcacion = db.Puntera.FirstOrDefault(x => x.Puntera_Id == id);

                if (marcacion != null)
                {
                    var listaDivisions = db.Puntera.Where(x => x.Puntera_Id == id).ToList();
                    var modelo = listaDivisions.Select(x => new PunteraModel()
                    {
                        Marca = new Models.Marca.MarcaModel() { Marca_Id = x.Marca_Id, Marca_Nombre = x.Marcas.Marca_Nombre },
                        Puntera_Cantidad = x.Puntera_Cantidad,
                        Puntera_FechaCarga = x.Puntera_FechaCarga,
                        Puntera_Id = x.Puntera_Id,
                        Puntera_LucesEncendidas = x.Puntera_LucesEncendidas,
                        Puntera_Observacion = x.Puntera_Observacion,
                        SubMarca = new Models.SubMarca.SubMarcaModel() { SubMarca_Id = x.SubMarca_Id, SubMarca_Nombre = x.SubMarca.SubMarca_Nombre },
                        Usuario = new Models.Usuario.UsuarioResponseModel()
                        {
                            Es_Jefe = x.Usuario.Usuario_Rol != "J",
                            Id = x.Usuario_Id,
                            JefeVentas_Id = x.Usuario.JefeVentas_Id,
                            Rol = x.Usuario.Usuario_Rol,
                            Usuario_Id = x.Usuario_Id,
                            Usuario_Nombre = x.Usuario.Usuario_Nombre,
                            Vendedor_Id = x.Usuario.Usuario_Vendedor_Id
                        }
                    }).ToList().First();
                    var json = JsonConvert.SerializeObject(modelo);
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

        // POST: api/VPuntera
        public HttpResponseMessage Post([FromBody] PunteraUpsertModel model)
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
                var usuario = nombreV.Usuario.First();
                var newVisita = new Puntera()
                {
                    Usuario_Id = usuario.Usuario_Id,
                    Marca_Id = model.Marca_Id,
                    Puntera_Cantidad = model.Puntera_Cantidad,
                    Puntera_FechaCarga = model.Puntera_FechaCarga,
                    Puntera_LucesEncendidas = model.Puntera_LucesEncendidas,
                    Puntera_Observacion = model.Puntera_Observacion,
                    SubMarca_Id = model.SubMarca_Id
                };
                db.Puntera.Add(newVisita);

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

        // PUT: api/VPuntera/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/VPuntera/5
        public void Delete(int id)
        {
        }
    }
}
