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

namespace PlanVisitaWebAPI.Controllers.JefeVentas
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VendedorSucursalController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/VendedorSucursal


        // GET: api/VendedorSucursal/5
        public HttpResponseMessage Get(int skip = 0, int take = 10, string filtro = "", int vendedor = 0, int sucursal = 0, string cliente = "", bool asignado = true)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            if (filtro == null)
            {
                filtro = "";
            }
            if (cliente == null) {
                cliente = "";
            }
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            string json = null;
            if ((string.IsNullOrEmpty(vendedor.ToString()) && string.IsNullOrEmpty(sucursal.ToString())) || (!headers.Contains("jefeToken")))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {
                string token = headers.GetValues("jefeToken").First();
                if (vendedor != 0 && (sucursal == 0 && cliente == ""))
                {
                    var sucursales = new List<SucursalVendedorResponseModel>();
                    if (asignado) {
                        sucursales = db.Database.SqlQuery<SucursalVendedorResponseModel>("select Cast(h.Address as varchar) as Sucursal_Id, h.city as Sucursal_Ciudad, h.street as Sucursal_Direccion, h.Address2 as Sucursal_Local, h.GroupCode as Canal_Id,h.cardcode as Cliente_Cod, h.cardfname as Cliente_RazonSocial, vc.Cantidad_Visitas as Cantidad_Visitas from VendedorClienteSAP vc inner join V_Clientes_HBF h on vc.Cliente_Cod COLLATE Modern_Spanish_CI_AS = h.cardcode and vc.Sucursal_Id  = h.Address where h.cardfname like '%" + filtro + "%' and vc.Vendedor_Id = " + vendedor).ToList<SucursalVendedorResponseModel>();
                    } else
                    {
                        sucursales = db.Database.SqlQuery<SucursalVendedorResponseModel>("select Cast(h.Address as varchar) as Sucursal_Id, h.city as Sucursal_Ciudad, h.street as Sucursal_Direccion, h.Address2 as Sucursal_Local, h.GroupCode as Canal_Id,h.cardcode as Cliente_Cod, h.cardfname as Cliente_RazonSocial from V_Clientes_HBF h where concat(h.cardcode, h.Address) not in (select concat(vc.Cliente_Cod COLLATE Modern_Spanish_CI_AS, vc.Sucursal_Id) from VendedorClienteSAP vc where vc.Vendedor_Id=" + vendedor + ") and  h.cardfname like '%" + filtro + "%'").ToList<SucursalVendedorResponseModel>();
                    }
                    
                    var cantidad = sucursales.Count;
                    var sucursalesModels = sucursales.Skip(skip).Take(take);
                    var paginationModel = new PaginationModel<SucursalVendedorResponseModel>()
                    {
                        CantidadTotal = cantidad,
                        Listado = sucursalesModels
                    };
                    json = JsonConvert.SerializeObject(paginationModel);
                } else if(vendedor == 0 && sucursal != 0 && cliente != "")
                {
                    var vendedores = db.Database.SqlQuery<VendedorSucursalResponseModel>("select v.Vendedor_Id, v.Vendedor_Nombre, v.Vendedor_Mail, v.Vendedor_Rol, vc.Cantidad_Visitas from VendedorClienteSAP vc inner join Vendedor v on vc.Vendedor_Id = v.Vendedor_Id where vc.Sucursal_Id = " + sucursal + " and vc.Cliente_Cod = '" + cliente + "'").ToList<VendedorSucursalResponseModel>();

                    var cantidad = vendedores.Count;
                    var sucursalesModels = vendedores.Skip(skip).Take(take);
                    var paginationModel = new PaginationModel<VendedorSucursalResponseModel>()
                    {
                        CantidadTotal = cantidad,
                        Listado = sucursalesModels
                    };
                    json = JsonConvert.SerializeObject(paginationModel);
                } else
                {
                    validation.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    validation.Message = "No ha enviado los datos correctamente.";

                    json = JsonConvert.SerializeObject(validation);
                }
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
                var newVendedorSucursal = new VendedorClienteSAP();
                newVendedorSucursal.Cantidad_Visitas = value.Cantidad;
                newVendedorSucursal.Sucursal_Id = value.Sucursal_Id;
                newVendedorSucursal.Vendedor_Id = value.Vendedor_Id;
                newVendedorSucursal.Cliente_Cod = value.Cliente_Cod;
                newVendedorSucursal.Promedio_Ventas = null;

                db.VendedorClienteSAP.Add(newVendedorSucursal);

                var resultado = db.SaveChanges();


                validation.Success = resultado > 0;
                if (resultado > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    validation.Message = "Añadido con éxito";

                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    validation.Message = "Ocurrió un error al añadir";
                }
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
                var newVendedorSucursal = db.VendedorClienteSAP.FirstOrDefault(x => x.Sucursal_Id == value.Sucursal_Id && x.Cliente_Cod == value.Cliente_Cod && x.Vendedor_Id == value.Vendedor_Id);
                newVendedorSucursal.Cantidad_Visitas = value.Cantidad;

               

                var resultado = db.SaveChanges();


                validation.Success = resultado > 0;
                if (resultado > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    validation.Message = "Actualizado con éxito";

                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    validation.Message = "Ocurrió un error al actualizar";
                }
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
                var newVendedorSucursal = db.VendedorClienteSAP.FirstOrDefault(x => x.Sucursal_Id == value.Sucursal_Id && x.Cliente_Cod == value.Cliente_Cod  && x.Vendedor_Id == value.Vendedor_Id);
                newVendedorSucursal.Cantidad_Visitas = value.Cantidad;

                db.VendedorClienteSAP.Remove(newVendedorSucursal);

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
                    validation.Message = "Ocurrió un error al eliminar Sucursal";
                }
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
