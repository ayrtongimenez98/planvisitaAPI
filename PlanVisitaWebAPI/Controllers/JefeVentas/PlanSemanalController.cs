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

namespace PlanVisitaWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PlanSemanalController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(DateTime Fecha_Desde, DateTime Fecha_Hasta, int Vendedor_Id = 0, string Cliente_Cod = "", string Filtro = "", int Skip = 0, int Take = 10)
        {

            HttpRequestMessage re = Request;
            var headers = re.Headers;
            if (Filtro == null)
                Filtro = "";

            var response = new HttpResponseMessage();
            if (headers.Contains("jefeToken") && headers.GetValues("jefeToken") != null && !string.IsNullOrEmpty(headers.GetValues("jefeToken").First()))
            {
                string token = headers.GetValues("jefeToken").First();
                var jefeVentasId = Convert.ToInt32(token);

                var PlanSemanalList = new List<PlanSemanalResponseModel>();
                var paginationModel = new PaginationModel<PlanSemanalResponseModel>();
                var PlanSemanalQuery = db.Database.SqlQuery<PlanSemanalResponseModel>(@"
                   SELECT vs.PlanSemanal_Id,  
                    	   FORMAT (vs.PlanSemanal_Horario, 'yyyy_MM') as Periodo, 
                    	   vs.PlanSemanal_Horario,
                           CAST(FORMAT(vs.PlanSemanal_Horario, 'hh:mm') AS varchar) AS PlanSemanal_Hora_Entrada, 
                    	   vs.Vendedor_Id, 
                    	   v.Vendedor_Nombre as Vendedor, 
                    	   c.cardcode as CodCliente, 
                    	   c.cardfname as Cliente, 
                    	   c.city as Ciudad, 
                    	   c.street as Direccion, 
                    	   c.Address as Sucursal_Id, 
                    	   vs.PlanSemanal_Estado
                    FROM PlanSemanalSAP vs 
                    inner join Vendedor v on vs.Vendedor_Id = v.Vendedor_Id
                    inner join V_Clientes_HBF c on vs.Cliente_Cod COLLATE Modern_Spanish_CI_AS = c.cardcode and vs.Sucursal_Id = c.Address
                 ").ToList<PlanSemanalResponseModel>();

                if (Vendedor_Id != 0)
                {
                    PlanSemanalQuery = PlanSemanalQuery.Where(x => x.Vendedor_Id == Vendedor_Id).ToList();
                }

                if (Cliente_Cod != null)
                {
                    PlanSemanalQuery = PlanSemanalQuery.Where(x => x.CodCliente == Cliente_Cod).ToList();
                }

                if (Fecha_Desde.Date != Convert.ToDateTime("01/01/0001").Date && Fecha_Hasta.Date != Convert.ToDateTime("01/01/0001").Date)
                {
                    PlanSemanalQuery = PlanSemanalQuery.Where(x => x.PlanSemanal_Horario >= Fecha_Desde && x.PlanSemanal_Horario <= Fecha_Hasta).ToList();
                }
                PlanSemanalList = PlanSemanalQuery.ToList();

                paginationModel.CantidadTotal = PlanSemanalList.Count;
                paginationModel.Listado = PlanSemanalList.Skip(Skip).Take(Take);

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
            HttpRequestMessage re = Request;
            var headers = re.Headers;


            var response = new HttpResponseMessage();
            if (headers.Contains("jefeToken") && headers.GetValues("jefeToken") != null && !string.IsNullOrEmpty(headers.GetValues("jefeToken").First()))
            {
                string token = headers.GetValues("jefeToken").First();
                var jefeVentasId = Convert.ToInt32(token);
                var marcacion = db.V_Visitas_Detalle.FirstOrDefault(x => x.Visita_Id == id);

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
                        Message = "No existe la visita buscada."
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

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] List<PlanSemanalUpdateModel> list)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;

            var validation = new SystemValidationModel();
            var response = new HttpResponseMessage();
            if (headers.Contains("jefeToken") && headers.GetValues("jefeToken") != null && !string.IsNullOrEmpty(headers.GetValues("jefeToken").First()))
            {
                string token = headers.GetValues("jefeToken").First();
                var jefeVentasId = Convert.ToInt32(token);

                var modelList = new List<PlanSemanalSAP>();

                foreach (var value in list)
                {
                    var newPlan = new PlanSemanalSAP()
                    {
                        Cliente_Cod = value.Cliente_Cod,
                        PlanSemanal_Estado = value.PlanSemanal_Estado,
                        Sucursal_Id = value.Sucursal_Id,
                        Vendedor_Id = value.Vendedor_Id,
                        PlanSemanal_Horario = value.PlanSemanal_Horario,
                        ObjetivoVisita_Id = value.ObjetivoVisita_Id
                    };

                    modelList.Add(newPlan);
                }
                

                db.PlanSemanalSAP.AddRange(modelList);

                var resultado = db.SaveChanges();

                if (resultado > 0)
                {
                    validation.Success = false;
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

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            validation.Success = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            validation.Message = "Por política de la empresa, no puede editar planes.";
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
            validation.Message = "Por política de la empresa, no puede eliminar plan.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}