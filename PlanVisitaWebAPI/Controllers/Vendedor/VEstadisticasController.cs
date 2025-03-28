﻿using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using PlanVisitaWebAPI.Models.Estadisticas;
using PlanVisitaWebAPI.Models.Marcaciones;
using PlanVisitaWebAPI.Models.Shared;
using PlanVisitaWebAPI.Models.Sucursal;
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
    public class VEstadisticasController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        // GET: api/Estadistica
        public HttpResponseMessage Get(DateTime fechaDesde, DateTime fechaHasta)
        {
            HttpRequestMessage re = Request;
            var headers = re.Headers;
            var response = Request.CreateResponse();
            var validation = new SystemValidationModel();
            string json = null;

            if ((!headers.Contains("userToken")))
            {
                validation.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                validation.Message = "Credenciales invalidas.";
                json = JsonConvert.SerializeObject(validation);
            }
            else
            {
                string token = headers.GetValues("userToken").First();
                var vendedor = Convert.ToInt32(token);
                var item = db.Vendedor.First(x => x.Vendedor_Id == vendedor);
                var listaEstadistica = new List<EstadisticaModel>();
                var asignados = db.Database.SqlQuery<SucursalVendedorResponseModel>(@"select Cast(vc.Sucursal_Id as varchar) as Sucursal_Id, 
s.Sucursal_Ciudad as Sucursal_Ciudad,
s.Sucursal_Direccion as Sucursal_Direccion,
NULL as Sucursal_Local,
NULL as Canal_Id,
vc.Cliente_Cod as Cliente_Cod,
c.Cliente_RazonSocial as Cliente_RazonSocial,
vc.Cantidad_Visitas as Cantidad_Visitas
from VendedorClienteSAP vc
join Sucursal s on vc.Sucursal_Id = s.Sucursal_Id and s.Cliente_Cod = vc.Cliente_Cod
join Cliente c on c.Cliente_Cod = vc.Cliente_Cod
where vc.Vendedor_Id= " + item.Vendedor_Id).ToList<SucursalVendedorResponseModel>();
                var diasHabiles = Helpers.Helpers.BusinessDaysUntil(fechaDesde, fechaHasta);
                var objetivo = db.PlanSemanalSAP.Where(x => x.Vendedor_Id == item.Vendedor_Id).ToList().Where(x => x.PlanSemanal_Horario.Date >= fechaDesde.Date && x.PlanSemanal_Horario <= fechaHasta.Date).ToList();
                var realizado = db.VisitaSAP.Where(x => x.Vendedor_Id == item.Vendedor_Id).ToList().Where(x => x.Visita_fecha.Date >= fechaDesde.Date && x.Visita_fecha.Date <= fechaHasta.Date).ToList();
                var visitasHechas = traerMarcaciones(item.Vendedor_Id, fechaDesde, fechaHasta);
                var visitasDistintas = visitasHechas.GroupBy(p => new { p.CodCliente, p.Sucursal_Id }).Select(g => g.First()).ToList();
                var codigoSucursalHecho = realizado.Select(x => x.Cliente_Cod + x.Sucursal_Id).ToList();
                var noRealizados = asignados.Where(x => !codigoSucursalHecho.Contains(x.Cliente_Cod + x.Sucursal_Id)).ToList();
                var estadistica = new EstadisticaModel();

                estadistica.Vendedor_Id = item.Vendedor_Id;
                estadistica.Vendedor_Nombre = item.Vendedor_Nombre;
                estadistica.Objetivo = asignados.Count;
                estadistica.Realizado = realizado.Count;
                estadistica.SucursalesVisitados = visitasDistintas;
                estadistica.SucursalesNoVisitados = noRealizados;
                estadistica.ObjetivoPorDia = Convert.ToInt32(asignados.Count / diasHabiles);
                estadistica.HechoPorDia = Convert.ToInt32(realizado.Count / diasHabiles);
                estadistica.Porcentaje = Convert.ToInt32((realizado.Count * 100) / asignados.Count);
                estadistica.CantidadSucursalesNoVisitados = noRealizados.Count;
                estadistica.CantidadSucursalesVisitados = visitasDistintas.Count;
                listaEstadistica.Add(estadistica);

                var paginationModel = new PaginationModel<EstadisticaModel>()
                {
                    CantidadTotal = listaEstadistica.Count,
                    Listado = listaEstadistica
                };
                json = JsonConvert.SerializeObject(paginationModel);
            }
            
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        // GET: api/Estadistica/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Estadistica
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Estadistica/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Estadistica/5
        public void Delete(int id)
        {
        }


        private List<MarcacionesResponseModel> traerMarcaciones(int Vendedor_Id, DateTime Fecha_Desde, DateTime Fecha_Hasta)
        {
            var marcacionesQuery = db.Database.SqlQuery<MarcacionesResponseModel>(@"
                   SELECT vs.Visita_Id,  
                    	   FORMAT (vs.Visita_fecha, 'yyyy_MM') as Periodo, 
                    	   vs.Visita_fecha, 
                           CAST(FORMAT(vs.Visita_Hora_Entrada, 'hh:mm') AS varchar) AS Visita_Hora_Entrada, 
                    	   CAST(FORMAT(vs.Visita_Hora_Salida, 'hh:mm')AS varchar) AS Visita_Hora_Salida, 
                    	   vs.Vendedor_Id, 
                    	   v.Vendedor_Nombre as Vendedor, 
                    	   vs.Cliente_Cod as CodCliente, 
                    	   c.Cliente_RazonSocial as Cliente,
                    	   s.Sucursal_Ciudad as Ciudad, 
                            s.Sucursal_Direccion as Direccion, 
                            cast(vs.Sucursal_Id as varchar) as Sucursal_Id, 
                    	   vs.Visita_Observacion as Observacion, 
                    	   vs.Visita_Ubicacion_Entrada, 
                    	   vs.Visita_Ubicacion_Salida,
                            DATENAME(MONTH, vs.Visita_fecha) AS Mes,
                            DATENAME(YEAR, vs.Visita_fecha) AS Año,
                            DATENAME(WEEKDAY, vs.Visita_fecha) AS Dia
                    FROM VisitaSAP vs 
                    inner join Vendedor v on vs.Vendedor_Id = v.Vendedor_Id
                    inner join Sucursal s on vs.Sucursal_Id  = s.Sucursal_Id and vs.Cliente_Cod = s.Cliente_Cod
					inner join Cliente c on vs.Cliente_Cod = c.Cliente_Cod
                    inner join Motivo m on m.Motivo_Id = vs.Motivo_Id
                    inner join Estado e on vs.Estado_Id = e.Estado_Id").ToList<MarcacionesResponseModel>();

            if (Vendedor_Id != 0)
            {
                marcacionesQuery = marcacionesQuery.Where(x => x.Vendedor_Id == Vendedor_Id).ToList();
            }

            if (Fecha_Desde.Date != Convert.ToDateTime("01/01/0001").Date && Fecha_Hasta.Date != Convert.ToDateTime("01/01/0001").Date)
            {
                marcacionesQuery = marcacionesQuery.Where(x => x.Visita_fecha.Date >= Fecha_Desde.Date && x.Visita_fecha.Date <= Fecha_Hasta.Date).ToList();
            }

            return marcacionesQuery;

        }

    }
}
