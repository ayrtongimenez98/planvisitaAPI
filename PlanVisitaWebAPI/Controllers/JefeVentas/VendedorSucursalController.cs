using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models.VendedorSucursal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlanVisitaWebAPI.Controllers.JefeVentas
{
    public class VendedorSucursalController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/VendedorSucursal


        // GET: api/VendedorSucursal/5
        public HttpResponseMessage Get([FromBody] VendedorSucursalUpdateModel value, int skip = 0, int take = 10, string filtro = "")
        {
            if(filtro == null)
            {
                filtro = "";
            }
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            string json = null;
            if (string.IsNullOrEmpty(value.Vendedor_Id.ToString()) && string.IsNullOrEmpty(value.Sucursal_Id.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {
                if (value.Vendedor_Id != 0 && value.Sucursal_Id == 0)
                {
                    var listaSucursalesId = db.VendedorCliente.Where(x => x.Vendedor_Id == value.Vendedor_Id).Select(x => x.Sucursal_Id).ToList();
                    var sucursales = db.Sucursal.Where(x => listaSucursalesId.Any(y => y == x.Sucursal_Id) && x.Sucursal_Direccion.Contains(filtro)).ToList();
                    var paginationModel = new PaginationModel<Sucursal>()
                    {
                        CantidadTotal = sucursales.Count,
                        Listado = sucursales.Skip(skip).Take(take)
                    };
                    json = JsonConvert.SerializeObject(paginationModel);
                } else if(value.Vendedor_Id == 0 && value.Sucursal_Id != 0)
                {
                    var listaVendedoresId = db.VendedorCliente.Where(x => x.Sucursal_Id == value.Sucursal_Id).Select(x => x.Vendedor_Id).ToList();
                    var vendedores = db.Vendedor.Where(x => listaVendedoresId.Any(y => y == x.Vendedor_Id) && x.Vendedor_Nombre.Contains(filtro)).ToList();
                    var paginationModel = new PaginationModel<Vendedor>()
                    {
                        CantidadTotal = vendedores.Count,
                        Listado = vendedores.Skip(skip).Take(take)
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
                var newVendedorSucursal = new VendedorCliente();
                newVendedorSucursal.Cantidad_Visitas = value.Cantidad;
                newVendedorSucursal.Sucursal_Id = value.Sucursal_Id;
                newVendedorSucursal.Vendedor_Id = value.Vendedor_Id;
                newVendedorSucursal.Promedio_Ventas = null;

                db.VendedorCliente.Add(newVendedorSucursal);

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
                var newVendedorSucursal = db.VendedorCliente.FirstOrDefault(x => x.Sucursal_Id == value.Sucursal_Id && x.Vendedor_Id == value.Vendedor_Id);
                newVendedorSucursal.Cantidad_Visitas = value.Cantidad;

                db.VendedorCliente.Add(newVendedorSucursal);

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
                var newVendedorSucursal = db.VendedorCliente.FirstOrDefault(x => x.Sucursal_Id == value.Sucursal_Id && x.Vendedor_Id == value.Vendedor_Id);
                newVendedorSucursal.Cantidad_Visitas = value.Cantidad;

                db.VendedorCliente.Remove(newVendedorSucursal);

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
}
