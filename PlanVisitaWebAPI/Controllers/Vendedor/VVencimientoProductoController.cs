using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models.Vencimiento;
using PlanVisitaWebAPI.Models.VencimientoProducto;
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
    public class VVencimientoProductoController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/VVencimientoProducto
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

                var listaDivisions = db.VencimientoProducto.Where(x => x.Vencimiento_Colaborador == nombreV.Vendedor_Nombre).ToList();
                var listaModelos = listaDivisions.Select(x => new VencimientoProductoModel()
                {
                    Vencimiento_Canal = x.Vencimiento_Canal,
                    Vencimiento_Cargo = x.Vencimiento_Cargo,
                    Vencimiento_Colaborador = x.Vencimiento_Colaborador,
                    Vencimiento_Division = x.Vencimiento_Division,
                    Vencimiento_PuntoVentaDireccion = x.Vencimiento_PuntoVentaDireccion,
                    Vencimiento_Id = x.Vencimiento_Id,
                    Vencimiento_FechaCreacion = x.Vencimiento_FechaCreacion,
                    Vencimiento_Detalle = x.VencimientoProductoDetalle.Select(y => new VencimientoProductoDetalleModel() { VencimientoProductoDetalle_Cantidad_SKU = y.VencimientoProductoDetalle_Cantidad_SKU,
                                                                                                                            VencimientoProductoDetalle_Codigo_Barras = y.VencimientoProductoDetalle_Codigo_Barras,
                                                                                                                            VencimientoProductoDetalle_Descripcion_Producto = y.VencimientoProductoDetalle_Descripcion_Producto,
                                                                                                                            VencimientoProductoDetalle_Id = y.VencimientoProductoDetalle_Id,
                                                                                                                            VencimientoProductoDetalle_Rango_Fecha = y.VencimientoProductoDetalle_Rango_Fecha,
                                                                                                                            VencimientoProducto_Id = y.VencimientoProducto_Id}).ToList()
                }).ToList();

                var paginationModel = new PaginationModel<VencimientoProductoModel>()
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

        // GET: api/VVencimientoProducto/5
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
                var marcacion = db.VencimientoProducto.FirstOrDefault(x => x.Vencimiento_Id == id);

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

        // POST: api/VVencimientoProducto
        public HttpResponseMessage Post([FromBody] VencimientoUpsertModel model)
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
                var newVisita = new VencimientoProducto()
                {
                    Vencimiento_Division= model.Vencimiento_Division,
                    Vencimiento_Canal= model.Vencimiento_Canal,
                    Vencimiento_Cargo= model.Vencimiento_Cargo,
                    Vencimiento_Colaborador= nombreV.Vendedor_Nombre,
                    Vencimiento_PuntoVentaDireccion= model.Vencimiento_PuntoVentaDireccion,
                    Vencimiento_FechaCreacion = DateTime.Now,
                    VencimientoProductoDetalle = model.Vencimiento_Detalle.Select(x => new VencimientoProductoDetalle() { VencimientoProductoDetalle_Cantidad_SKU = x.VencimientoProductoDetalle_Cantidad_SKU, 
                                                                                                                          VencimientoProductoDetalle_Codigo_Barras = x.VencimientoProductoDetalle_Codigo_Barras,
                                                                                                                           VencimientoProductoDetalle_Descripcion_Producto = x.VencimientoProductoDetalle_Descripcion_Producto,
                                                                                                                          VencimientoProductoDetalle_Rango_Fecha = x.VencimientoProductoDetalle_Rango_Fecha}).ToList()
                };
                db.VencimientoProducto.Add(newVisita);

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

        // PUT: api/VVencimientoProducto/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/VVencimientoProducto/5
        public void Delete(int id)
        {
        }
    }
}
