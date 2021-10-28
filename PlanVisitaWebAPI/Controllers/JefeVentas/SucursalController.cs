using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.Sucursal;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlanVisitaWebAPI.Controllers.JefeVentas
{
    public class SucursalController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(string filtro = "", string codCliente = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            if (codCliente == null)
                codCliente = "";
            var listaSucursals = new List<Sucursal>();

            if (string.IsNullOrEmpty(codCliente))
            {
                listaSucursals = db.Sucursal.Where(x => x.Sucursal_Direccion.Contains(filtro) || x.Sucursal_Ciudad.Contains(filtro) || x.Cliente_Cod.Contains(filtro)).ToList();
            } else
            {
                listaSucursals = db.Sucursal.Where(x => (x.Sucursal_Direccion.Contains(filtro) || x.Sucursal_Ciudad.Contains(filtro)) && x.Cliente_Cod == codCliente).ToList();
            }

            var paginationModel = new PaginationModel<SucursalResponseListModel>()
            {
                CantidadTotal = listaSucursals.Count,
                Listado = listaSucursals.Skip(skip).Take(take).Select(x => new SucursalResponseListModel() { Cliente_Cod = x.Cliente_Cod, Sucursal_Ciudad = x.Sucursal_Ciudad, Sucursal_Direccion = x.Sucursal_Direccion, Sucursal_FechaCreacion = x.Sucursal_FechaCreacion, Sucursal_FechaLastUpdate = x.Sucursal_FechaLastUpdate, Sucursal_Id = x.Sucursal_Id })
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            var Sucursal = db.Sucursal.FirstOrDefault(x => x.Sucursal_Id == id);
            var sucursalModel = new SucursalResponseListModel() { 
                Cliente_Cod = Sucursal.Cliente_Cod, 
                Sucursal_Ciudad = Sucursal.Sucursal_Ciudad, 
                Sucursal_Direccion = Sucursal.Sucursal_Direccion, 
                Sucursal_FechaCreacion = Sucursal.Sucursal_FechaCreacion, 
                Sucursal_FechaLastUpdate = Sucursal.Sucursal_FechaLastUpdate, 
                Sucursal_Id = Sucursal.Sucursal_Id
            };


            var json = JsonConvert.SerializeObject(sucursalModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] SucursalUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Sucursal_Ciudad) || string.IsNullOrEmpty(model.Sucursal_Direccion) || string.IsNullOrEmpty(model.Cliente_Cod))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {
                var nuevoSucursal = new Sucursal();
                nuevoSucursal.Sucursal_Ciudad = model.Sucursal_Ciudad;
                nuevoSucursal.Cliente_Cod = model.Cliente_Cod;
                nuevoSucursal.Sucursal_Direccion = model.Sucursal_Direccion;
                nuevoSucursal.Sucursal_FechaCreacion = DateTime.Now;
                nuevoSucursal.Sucursal_FechaLastUpdate = DateTime.Now;

                db.Sucursal.Add(nuevoSucursal);

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
                    validation.Message = "Ocurrió un error al crear Sucursal";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }


        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] SucursalUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Sucursal_Ciudad) || string.IsNullOrEmpty(model.Sucursal_Direccion) || string.IsNullOrEmpty(model.Cliente_Cod))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoSucursal = db.Sucursal.FirstOrDefault(x => x.Sucursal_Id == id);

                nuevoSucursal.Sucursal_Ciudad = model.Sucursal_Ciudad;
                nuevoSucursal.Cliente_Cod = model.Cliente_Cod;
                nuevoSucursal.Sucursal_Direccion = model.Sucursal_Direccion;
                nuevoSucursal.Sucursal_FechaLastUpdate = DateTime.Now;

                db.Sucursal.Add(nuevoSucursal);

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
                    validation.Message = "Ocurrió un error al crear Sucursal";
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
            validation.Message = "Por política de la empresa, no se pueden eliminar sucursales.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}
