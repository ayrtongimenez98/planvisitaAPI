using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models.Vendedor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlanVisitaWebAPI.Controllers
{
    public class VendedorController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET api/<controller>
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            var listaVendedors = db.Vendedor.Where(x => x.Vendedor_Nombre.Contains(filtro) || x.Vendedor_Id.ToString().Contains(filtro) || x.Vendedor_Mail.Contains(filtro)).ToList();

            var paginationModel = new PaginationModel<Vendedor>()
            {
                CantidadTotal = listaVendedors.Count,
                Listado = listaVendedors.Skip(skip).Take(take)
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        public HttpResponseMessage Get(int id)
        {
            var Vendedor = db.Vendedor.FirstOrDefault(x => x.Vendedor_Id == id);

            var json = JsonConvert.SerializeObject(Vendedor);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] VendedorUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Nombre) || string.IsNullOrEmpty(model.Id.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoVendedor = new Vendedor();
                nuevoVendedor.Vendedor_Id = model.Id;
                nuevoVendedor.Vendedor_Nombre = model.Nombre;
                nuevoVendedor.Vendedor_FechaCreacion = DateTime.Now;
                nuevoVendedor.Vendedor_FechaLastUpdate = DateTime.Now;
                nuevoVendedor.JefeVentas_Id = model.JefeVentasId;
                nuevoVendedor.Vendedor_Mail = model.Email;

                db.Vendedor.Add(nuevoVendedor);

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
                    validation.Message = "Ocurrió un error al crear Vendedor";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT api/<controller>/5
        public HttpResponseMessage Put(int id, [FromBody] VendedorUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Nombre) || string.IsNullOrEmpty(model.Id.ToString()) || string.IsNullOrEmpty(id.ToString()))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoVendedor = db.Vendedor.FirstOrDefault(x => x.JefeVentas_Id == id);


                nuevoVendedor.Vendedor_Id = model.Id;
                nuevoVendedor.Vendedor_Nombre = model.Nombre;
                nuevoVendedor.Vendedor_FechaLastUpdate = DateTime.Now;
                nuevoVendedor.JefeVentas_Id = model.JefeVentasId;
                nuevoVendedor.Vendedor_Mail = model.Email;

                db.Vendedor.Add(nuevoVendedor);

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
                    validation.Message = "Ocurrió un error al crear Vendedor";
                }
            }
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // DELETE api/<controller>/5
        public HttpResponseMessage Delete(string id)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            validation.Success = false;
            response.StatusCode = HttpStatusCode.BadRequest;
            validation.Message = "Por política de la empresa, no pueden eliminar clientes.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}