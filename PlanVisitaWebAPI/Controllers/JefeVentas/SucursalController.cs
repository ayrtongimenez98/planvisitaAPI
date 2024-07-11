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
using System.Web.Http.Cors;

namespace PlanVisitaWebAPI.Controllers.JefeVentas
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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
            var listaSucursals = new List<ClientesHBFDataSetAttribute>();
            var lista = new List<ClientesHBFDataSetAttribute>();
            if (MemoryCacher.GetValue("listaSucursales") == null)
            {
                lista = db.Database.SqlQuery<ClientesHBFDataSetAttribute>(@"select s.Cliente_Cod as cardcode,
c.Cliente_RazonSocial as cardfname,
Convert(varchar,s.Sucursal_Id) as [Address],
s.Sucursal_Ciudad as city,
s.Sucursal_Direccion as street,
0 as GroupCode,
'' as GroupName,
'' as Address2,
'Activo' as Estado
from Sucursal s
inner join Cliente c on c.Cliente_Cod = s.Cliente_Cod").ToList<ClientesHBFDataSetAttribute>();
                MemoryCacher.Add("listaSucursales", lista, DateTimeOffset.UtcNow.AddDays(1));
            }
            else
            {
                lista = (List<ClientesHBFDataSetAttribute>)MemoryCacher.GetValue("listaSucursales");
            }
            lista = lista.Where(x => x.street != null && x.city != null && x.cardcode != null).ToList();
            if (string.IsNullOrEmpty(codCliente))
            {
                listaSucursals = lista.Where(x => x.street.ToString().Contains(filtro) || x.city.ToString().Contains(filtro) || x.cardcode.ToString().Contains(filtro)).ToList();
            } else
            {
                listaSucursals = lista.Where(x => (x.street.Contains(filtro) || x.city.Contains(filtro)) && x.cardcode == codCliente).ToList();
            }

            var paginationModel = new PaginationModel<SucursalResponseListModel>()
            {
                CantidadTotal = listaSucursals.Count,
                Listado = listaSucursals.Skip(skip).Take(take).Select(x => new SucursalResponseListModel() { Cliente_Cod = x.cardcode, Sucursal_Ciudad = x.city, Sucursal_Direccion = x.street, Sucursal_Id = Convert.ToInt32(x.Address), Sucursal_Nombre = x.Address2, Cliente_RazonSocial = x.cardfname })
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
                Sucursal_Id = Convert.ToInt32(Sucursal.Sucursal_Id),
                Sucursal_Nombre = Sucursal.Sucursal_Ciudad
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
