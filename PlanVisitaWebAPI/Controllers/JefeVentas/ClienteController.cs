﻿using PlanVisitaWebAPI.DB;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Clientes;
using System;
using PlanVisitaWebAPI.Models.Shared;
using System.Web.Http.Cors;

namespace PlanVisitaWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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
            

            var json = JsonConvert.SerializeObject(cliente);
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
        public HttpResponseMessage Put(string id, [FromBody] ClienteUpdateModel model)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();

            if (string.IsNullOrEmpty(model.Nombre) || string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(id))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "No ha enviado los datos requeridos.";
            }
            else
            {

                var nuevoCliente = db.Cliente.FirstOrDefault(x => x.Cliente_Cod == id);

                nuevoCliente.Cliente_RazonSocial = model.Nombre;
                nuevoCliente.Cliente_FechaLastUpdate = DateTime.Now;

                db.Cliente.Add(nuevoCliente);

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
                    validation.Message = "Ocurrió un error al crear cliente";
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
            validation.Message = "Por política de la empresa, no puede eliminar clientes.";
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}