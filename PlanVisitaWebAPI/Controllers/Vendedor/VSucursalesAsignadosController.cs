using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models.Sucursal;
using PlanVisitaWebAPI.Models.Vendedor;
using PlanVisitaWebAPI.Models.VendedorSucursal;
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
    public class VSucursalesAsignadosController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/VendedorSucursal


        // GET: api/VendedorSucursal/5
        public HttpResponseMessage Get(int skip = 0, int take = 10, string filtro = "")
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            if (filtro == null)
            {
                filtro = "";
            }
            
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            string json = null;
            if ((!headers.Contains("userToken")))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "Credenciales invalidas.";
            }
            else
            {
                string token = headers.GetValues("userToken").First();
                var vendedor = Convert.ToInt32(token);
                var sucursales = db.Database.SqlQuery<SucursalVendedorResponseModel>(@"select Cast(vc.Sucursal_Id as varchar) as Sucursal_Id, 
case when h.city IS NULL then(select s.Sucursal_Ciudad from Sucursal s where s.Cliente_Cod = vc.Cliente_Cod and s.Sucursal_Id = vc.Sucursal_Id) COLLATE Modern_Spanish_CI_AS else h.city end as Sucursal_Ciudad, 
case when h.street IS NULL then(select s.Sucursal_Direccion from Sucursal s where s.Cliente_Cod= vc.Cliente_Cod and s.Sucursal_Id = vc.Sucursal_Id) COLLATE Modern_Spanish_CI_AS  else h.street end as Sucursal_Direccion, 
h.Address2 as Sucursal_Local, 
h.GroupCode as Canal_Id,
vc.Cliente_Cod as Cliente_Cod, 
case when h.cardfname IS null then(select c.Cliente_RazonSocial from Cliente c where c.Cliente_Cod = vc.Cliente_Cod) COLLATE Modern_Spanish_CI_AS else h.cardfname end as Cliente_RazonSocial, 
vc.Cantidad_Visitas as Cantidad_Visitas
from VendedorClienteSAP vc
left join V_Clientes_HBF h on vc.Cliente_Cod COLLATE Modern_Spanish_CI_AS = h.cardcode
and vc.Sucursal_Id = h.Address where vc.Vendedor_Id = "  + vendedor).ToList<SucursalVendedorResponseModel>();


                var cantidad = sucursales.Count;
                var sucursalesModels = sucursales.Skip(skip).Take(take).ToList();
                var paginationModel = new PaginationModel<SucursalVendedorResponseModel>()
                {
                    CantidadTotal = cantidad,
                    Listado = sucursalesModels
                };
                json = JsonConvert.SerializeObject(paginationModel);
            }

            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST: api/VendedorSucursal
        public HttpResponseMessage Post([FromBody] VendedorSucursalUpdateModel value)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(value.Vendedor_Id.ToString()) || string.IsNullOrEmpty(value.Sucursal_Id.ToString()) || string.IsNullOrEmpty(value.Cantidad.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {
                
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT: api/VendedorSucursal/5
        public HttpResponseMessage Put([FromBody] VendedorSucursalUpdateModel value)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(value.Vendedor_Id.ToString()) || string.IsNullOrEmpty(value.Sucursal_Id.ToString()) || string.IsNullOrEmpty(value.Cantidad.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {
                
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // DELETE: api/VendedorSucursal/5
        public HttpResponseMessage Delete([FromBody] VendedorSucursalUpdateModel value)
        {

            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(value.Vendedor_Id.ToString()) || string.IsNullOrEmpty(value.Sucursal_Id.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {
                
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }

    public class ClienteSucursalModel
    {
        public int Sucursal_Id { get; set; }
        public string Cliente_Cod { get; set; }
    }
}
