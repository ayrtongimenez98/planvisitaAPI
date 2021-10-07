using PlanVisitaWebAPI.DB;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Clientes;
using System;
using PlanVisitaWebAPI.Models.Shared;

namespace PlanVisitaWebAPI.Controllers
{
    public class ClienteController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            var listaClientes = db.Cliente.Where(x => x.Cliente_RazonSocial.Contains(filtro) || x.Cliente_Cod.Contains(filtro)).ToList();

            var paginationModel = new PaginationModel<Cliente>() { 
                CantidadTotal = listaClientes.Count,
                Listado = listaClientes.Skip(skip).Take(take)
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(string id, string filtroS = "", int takeS = 10, int skipS = 0)
        {
            if (filtroS == null)
                filtroS = "";
            var cliente = db.Cliente.FirstOrDefault(x => x.Cliente_Cod == id);
            var sucursales = db.Sucursal.Where(x => x.Cliente_Cod == id && (x.Sucursal_Direccion.Contains(filtroS) || x.Sucursal_Id.ToString().Contains(filtroS) || x.Sucursal_Ciudad.Contains(filtroS))).ToList();
            
            var clienteModel = new ClienteResponseModel() { 
                Cliente = cliente,
                Sucursales = new PaginationModel<Sucursal>() { 
                    CantidadTotal = sucursales.Count,
                    Listado = sucursales.Skip(skipS).Take(takeS)
                }
            };

            var json = JsonConvert.SerializeObject(clienteModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] ClienteUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Nombre) || string.IsNullOrEmpty(model.Id))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            } else { 

                var nuevoCliente = new Cliente();
                nuevoCliente.Cliente_Cod = model.Id;
                nuevoCliente.Cliente_RazonSocial = model.Nombre;
                nuevoCliente.Cliente_FechaCreacion = DateTime.Now;
                nuevoCliente.Cliente_FechaLastUpdate = DateTime.Now;

                db.Cliente.Add(nuevoCliente);

                var resultado = db.SaveChanges();

            
                validation.Success = resultado > 0;
                if (resultado > 0)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    validation.Message = "Creado con éxito";
                
                } else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    validation.Message = "Ocurrió un error al crear cliente";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}