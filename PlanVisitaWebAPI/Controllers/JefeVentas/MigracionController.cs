using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.Migracion;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models.Sucursal;
using PlanVisitaWebAPI.Models.VendedorSucursal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace PlanVisitaWebAPI.Controllers.JefeVentas
{
    public class MigracionController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/Migracion
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Migracion/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Migracion
        public HttpResponseMessage Post([FromBody] MigracionesUpsertModel value)
        {
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            var resultado = 0;
            switch (value.TipoMigracion)
            {
                case TipoMigracion.CopiarSucursales:
                    var listaSucursales = db.VendedorClienteSAP.Where(x => x.Vendedor_Id == value.VendedorIdEmisor).ToList();
                    var listaAAgregar = listaSucursales.Select(x => new VendedorClienteSAP() {Vendedor_Id = value.VendedorIdReceptor, Cantidad_Visitas = x.Cantidad_Visitas, Cliente_Cod = x.Cliente_Cod, Sucursal_Id = x.Sucursal_Id }).ToList();
                    db.VendedorClienteSAP.AddRange(listaAAgregar);
                    resultado = db.SaveChanges();
                    break;
                case TipoMigracion.ReemplazarSucursales:
                    var listaSucursales2 = db.VendedorClienteSAP.Where(x => x.Vendedor_Id == value.VendedorIdEmisor).ToList();
                    var listaSucursalesActual = db.VendedorClienteSAP.Where(x => x.Vendedor_Id == value.VendedorIdReceptor).ToList();
                    var listaAAgregar2 = listaSucursales2.Select(x => new VendedorClienteSAP() { Vendedor_Id = value.VendedorIdReceptor, Cantidad_Visitas = x.Cantidad_Visitas, Cliente_Cod = x.Cliente_Cod, Sucursal_Id = x.Sucursal_Id }).ToList();
                    db.VendedorClienteSAP.RemoveRange(listaSucursalesActual);
                    db.VendedorClienteSAP.AddRange(listaAAgregar2);
                    resultado = db.SaveChanges();
                    break;
                case TipoMigracion.MudarSucursalesAVendedor:
                    var listaSucursales3 = db.VendedorClienteSAP.Where(x => x.Vendedor_Id == value.VendedorIdEmisor).ToList();
                    var listaAAgregar3 = listaSucursales3.Select(x => new VendedorClienteSAP() { Vendedor_Id = value.VendedorIdReceptor, Cantidad_Visitas = x.Cantidad_Visitas, Cliente_Cod = x.Cliente_Cod, Sucursal_Id = x.Sucursal_Id }).ToList();
                    db.VendedorClienteSAP.RemoveRange(listaSucursales3);
                    db.VendedorClienteSAP.AddRange(listaAAgregar3);
                    resultado = db.SaveChanges();
                    break;
                case TipoMigracion.AsignarTodasSucursalesAVendedor:
                    var listaSucursals = new List<ClientesHBFDataSetAttribute>();
                    var lista = new List<ClientesHBFDataSetAttribute>();
                    if (MemoryCacher.GetValue("listaClientes") == null)
                    {
                        lista = db.Database.SqlQuery<ClientesHBFDataSetAttribute>("exec sp_Clientes_Hbf; ").ToList<ClientesHBFDataSetAttribute>();
                        MemoryCacher.Add("listaClientes", lista, DateTimeOffset.UtcNow.AddDays(1));
                    }
                    else
                    {
                        lista = (List<ClientesHBFDataSetAttribute>)MemoryCacher.GetValue("listaClientes");
                    }
                    lista = lista.Where(x => x.street != null && x.city != null && x.cardcode != null).ToList();
                    listaSucursals = lista.Where(x => x.cardcode == value.CodCliente).ToList();
                    var listaAAsignar = listaSucursals.Select(x => new VendedorClienteSAP() { Cantidad_Visitas = 1, Cliente_Cod = x.cardcode, Sucursal_Id = Convert.ToInt32(x.Address), Vendedor_Id = value.VendedorIdReceptor}).ToList();
                    db.VendedorClienteSAP.AddRange(listaAAsignar);
                    resultado = db.SaveChanges();
                    break;
                default:
                    validation.Success = false;
                    response.StatusCode = HttpStatusCode.OK;
                    validation.Message = "No ha enviado los datos requeridos.";
                    break;
                }
            
            var json = JsonConvert.SerializeObject(validation);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // PUT: api/Migracion/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Migracion/5
        public void Delete(int id)
        {
        }
    }
}
