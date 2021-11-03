using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.PlanSemanal;
using PlanVisitaWebAPI.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlanVisitaWebAPI.Controllers.Vendedor
{
    public class VPlanSemanalController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/VPlanSemanal
        public HttpResponseMessage Get([FromBody] PlanSemanalFiltroModel model)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            if (model.Filtro == null)
                model.Filtro = "";

            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                string token = headers.GetValues("userToken").First();
                var vendedorId = Convert.ToInt32(token);

                var plan = db.V_VDetallePlanSVendedores.Where(x => x.VendedorId == vendedorId).ToList();
                //plan = plan.Where(x => x.PlanSemanal_Horario.Date >= model.Fecha_Desde.Date && x.PlanSemanal_Horario.Date <= model.Fecha_Hasta && (x.Cliente_RazonSocial.Contains(model.Filtro) || x.SucursalDireccion.Contains(model.Filtro))).ToList();

                var paginationModel = new PaginationModel<V_VDetallePlanSVendedores>()
                {
                    CantidadTotal = plan.Count,
                    Listado = plan.Skip(model.Skip).Take(model.Take)
                };
                var json = JsonConvert.SerializeObject(paginationModel);
                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            } else
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

        // POST: api/VPlanSemanal
        public HttpResponseMessage Post([FromBody] PlanSemanalUpdateModel model)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            

            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                if (string.IsNullOrEmpty(model.PlanSemanal_Periodo) || model.PlanSemanal_NroSemana == 0 || !model.Detalle.Any())
                {
                    var validation = new SystemValidationModel()
                    {
                        Success = false,
                        Message = "Datos incompletos."
                    };
                    var json = JsonConvert.SerializeObject(validation);
                    response = Request.CreateResponse(HttpStatusCode.BadRequest);
                    response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                } else
                {
                    string token = headers.GetValues("userToken").First();
                    var vendedorId = Convert.ToInt32(token);

                    var cabecera = new PlanSemanal()
                    {
                        PlanSemanal_Estado = "N",
                        PlanSemanal_NroSemana = model.PlanSemanal_NroSemana,
                        PlanSemanal_Periodo = model.PlanSemanal_Periodo,
                        Vendedor_Id = vendedorId,
                        PlanSemanalDetalles = model.Detalle.Select(x => new PlanSemanalDetalle() { ObjetivoVisita_Id = x.ObjetivoVisita_Id, 
                                                                                                  PlanSemanal_DiaSemana = x.PlanSemanal_DiaSemana, 
                                                                                                  PlanSemanal_Horario = x.PlanSemanal_Horario, 
                                                                                                  Sucursal_Id = x.Sucursal_Id }).ToList()
                    };

                    db.PlanSemanal.Add(cabecera);

                    var resultado = db.SaveChanges();

                    var validation = new SystemValidationModel()
                    {
                        Success = resultado > 0,
                        Message = resultado > 0 ? "Agregado con éxito." : "Credenciales incorrectas."
                    };
                    var json = JsonConvert.SerializeObject(validation);
                    response = Request.CreateResponse(resultado > 0 ? HttpStatusCode.OK : HttpStatusCode.Conflict);
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

        // PUT: api/VPlanSemanal/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/VPlanSemanal/5
        public void Delete(int id)
        {
        }
    }
}
