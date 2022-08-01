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
using System.Web.Http.Cors;

namespace PlanVisitaWebAPI.Controllers.Vendedor
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VPlanSemanalController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/VPlanSemanal
        public HttpResponseMessage Get(DateTime Fecha_Desde, DateTime Fecha_Hasta, string Cliente_Cod = "", string Filtro = "", int Skip = 0, int Take = 10)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            if (Filtro == null)
                Filtro = "";

            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                string token = headers.GetValues("userToken").First();
                var vendedorId = Convert.ToInt32(token);

                var PlanSemanalList = new List<PlanSemanalResponseModel>();
                var paginationModel = new PaginationModel<PlanSemanalResponseModel>();
                var PlanSemanalQuery = db.Database.SqlQuery<PlanSemanalResponseModel>(@"
                   SELECT vs.PlanSemanal_Id,  
                    	   FORMAT (vs.PlanSemanal_Horario, 'yyyy_MM') as Periodo, 
                    	   vs.PlanSemanal_Horario,
                           CAST(FORMAT(vs.PlanSemanal_Horario, 'hh:mm') AS varchar) AS PlanSemanal_Hora_Entrada, 
                    	   vs.Vendedor_Id, 
                    	   v.Vendedor_Nombre as Vendedor, 
                    	   c.Cliente_Cod as CodCliente,
                    	   c.Cliente_RazonSocial as Cliente,
                    	   s.Sucursal_Ciudad as Ciudad, 
                            s.Sucursal_Direccion as Direccion, 
                            cast(vs.Sucursal_Id as varchar) as Sucursal_Id, 
                    	   vs.PlanSemanal_Estado
                    FROM PlanSemanalSAP vs 
                    inner join Vendedor v on vs.Vendedor_Id = v.Vendedor_Id
                    inner join Sucursal s on vs.Cliente_Cod = s.Cliente_Cod and vs.Sucursal_Id = s.Sucursal_Id
					inner join Cliente c on vs.Cliente_Cod = s.Cliente_Cod
                 ").ToList<PlanSemanalResponseModel>();

                if (vendedorId != 0)
                {
                    PlanSemanalQuery = PlanSemanalQuery.Where(x => x.Vendedor_Id == vendedorId).ToList();
                }

                if (Cliente_Cod != null)
                {
                    PlanSemanalQuery = PlanSemanalQuery.Where(x => x.CodCliente == Cliente_Cod).ToList();
                }

                if (Fecha_Desde.Date != Convert.ToDateTime("01/01/0001").Date && Fecha_Hasta.Date != Convert.ToDateTime("01/01/0001").Date)
                {
                    PlanSemanalQuery = PlanSemanalQuery.Where(x => x.PlanSemanal_Horario.Date >= Fecha_Desde.Date && x.PlanSemanal_Horario.Date <= Fecha_Hasta.Date).ToList();
                }
                PlanSemanalList = PlanSemanalQuery.OrderBy(x => x.PlanSemanal_Horario).ToList();



                paginationModel.CantidadTotal = PlanSemanalList.Count;
                paginationModel.Listado = PlanSemanalList.Skip(Skip).Take(Take);

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
        public HttpResponseMessage Post([FromBody] List<PlanSemanalUpdateModel> list)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            var validation = new SystemValidationModel();
            var response = new HttpResponseMessage();
            if (headers.Contains("userToken") && headers.GetValues("userToken") != null && !string.IsNullOrEmpty(headers.GetValues("userToken").First()))
            {
                string token = headers.GetValues("userToken").First();
                var vendedor = Convert.ToInt32(token);
                var modelList = new List<PlanSemanalSAP>();

                foreach (var value in list)
                {
                    var newPlan = new PlanSemanalSAP()
                    {
                        Cliente_Cod = value.Cliente_Cod,
                        PlanSemanal_Estado = value.PlanSemanal_Estado,
                        Sucursal_Id = value.Sucursal_Id,
                        Vendedor_Id = vendedor,
                        PlanSemanal_Horario = value.PlanSemanal_Horario,
                        ObjetivoVisita_Id = value.ObjetivoVisita_Id
                    };

                    modelList.Add(newPlan);
                }


                db.PlanSemanalSAP.AddRange(modelList);

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
                validation = new SystemValidationModel()
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
