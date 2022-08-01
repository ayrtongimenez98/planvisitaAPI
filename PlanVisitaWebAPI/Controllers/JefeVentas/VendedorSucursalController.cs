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

                    var lista = new List<ClientesHBFDataSetAttribute>();
                    if (MemoryCacher.GetValue("listaClientes") == null)
                    {
                        lista = db.Database.SqlQuery<ClientesHBFDataSetAttribute>(@"select s.Cliente_Cod as cardcode,
c.Cliente_RazonSocial as cardfname,
Convert(varchar, s.Sucursal_Id) as [Address],
s.Sucursal_Ciudad as city,
s.Sucursal_Direccion as street,
0 as GroupCode,
'' as GroupName,
'' as Address2,
'Activo' as Estado
from Sucursal s
inner
join Cliente c on c.Cliente_Cod = s.Cliente_Cod").ToList<ClientesHBFDataSetAttribute>();
                        MemoryCacher.Add("listaClientes", lista, DateTimeOffset.UtcNow.AddDays(1));
                    }
                    else
                    {
                        lista = (List<ClientesHBFDataSetAttribute>)MemoryCacher.GetValue("listaClientes");
                    }
                    lista = lista.Where(x => x.street != null && x.city != null && x.cardcode != null).ToList();

                    if (asignado) {
                        sucursales = db.Database.SqlQuery<SucursalVendedorResponseModel>(@"select Cast(vc.Sucursal_Id as varchar) as Sucursal_Id, 
h.Sucursal_Ciudad, 
h.Sucursal_Direccion, 
NULL as Sucursal_Local, 
NULL as Canal_Id,
vc.Cliente_Cod as Cliente_Cod, 
c.Cliente_RazonSocial, 
vc.Cantidad_Visitas as Cantidad_Visitas
from VendedorClienteSAP vc
join Sucursal h on vc.Sucursal_Id = h.Sucursal_Id and vc.Cliente_Cod = h.Cliente_Cod
join Cliente c on h.Cliente_Cod = c.Cliente_Cod
 where vc.Vendedor_Id =" + vendedor).ToList<SucursalVendedorResponseModel>();
                        sucursales = sucursales.Where(x => !string.IsNullOrEmpty(x.Cliente_RazonSocial)).Where(x => x.Cliente_RazonSocial.ToLower().Contains(filtro.ToLower()) || x.Cliente_Cod.Contains(filtro)).ToList();
                    } else
                    {
                        var asignados = db.Database.SqlQuery<SucursalVendedorResponseModel>(@"select Cast(vc.Sucursal_Id as varchar) as Sucursal_Id, 
h.Sucursal_Ciudad, 
h.Sucursal_Direccion, 
NULL as Sucursal_Local, 
NULL as Canal_Id,
vc.Cliente_Cod as Cliente_Cod, 
c.Cliente_RazonSocial, 
vc.Cantidad_Visitas as Cantidad_Visitas
from VendedorClienteSAP vc
join Sucursal h on vc.Sucursal_Id = h.Sucursal_Id and vc.Cliente_Cod = h.Cliente_Cod
join Cliente c on h.Cliente_Cod = c.Cliente_Cod
 where vc.Vendedor_Id =" + vendedor).ToList<SucursalVendedorResponseModel>();
                        var asignadosText = asignados.Select(x => x.Cliente_Cod + x.Sucursal_Id);
                        sucursales = lista.Where(x => !x.cardcode.Contains("CLIENTE NUEVO")).Where(x => !asignadosText.Contains(x.cardcode+x.Address)).Select(x => new SucursalVendedorResponseModel() { Cliente_Cod = x.cardcode, Cliente_RazonSocial = x.cardfname, Sucursal_Ciudad = x.city, Sucursal_Direccion = x.street, Sucursal_Id = x.Address }).ToList();
                        sucursales = sucursales.Where(x => !string.IsNullOrEmpty(x.Cliente_RazonSocial)).Where(x => x.Cliente_RazonSocial.ToLower().Contains(filtro.ToLower()) || x.Cliente_Cod.Contains(filtro)).ToList();
                        
                    }
                    
                    var cantidad = sucursales.Count;
                    var sucursalesModels = sucursales.Skip(skip).Take(take).ToList();
                    sucursalesModels = Utilities.CompararSeparar(sucursalesModels);
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
                if (string.IsNullOrEmpty(value.Cliente_Cod))
                {
                    var lista = db.VendedorClienteSAP.Where(x => x.Vendedor_Id == value.Vendedor_Id).ToList();
                    db.VendedorClienteSAP.RemoveRange(lista);
                }
                else {
                    var newVendedorSucursal = db.VendedorClienteSAP.FirstOrDefault(x => x.Sucursal_Id == value.Sucursal_Id && x.Cliente_Cod == value.Cliente_Cod && x.Vendedor_Id == value.Vendedor_Id);


                    db.VendedorClienteSAP.Remove(newVendedorSucursal);
                }

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
