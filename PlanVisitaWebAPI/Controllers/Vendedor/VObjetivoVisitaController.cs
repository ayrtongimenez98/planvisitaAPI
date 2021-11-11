using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlanVisitaWebAPI.Controllers.Vendedor
{
    public class VObjetivoVisitaController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            var listaDivisions = db.ObjetivoVisita.Where(x => x.ObjetivoVisita_Descripcion.Contains(filtro)).ToList();

            var paginationModel = new PaginationModel<ObjetivoVisitaResponseModel>()
            {
                CantidadTotal = listaDivisions.Count,
                Listado = listaDivisions.Skip(skip).Take(take).Select(x => new ObjetivoVisitaResponseModel() { ObjetivoVisita_Descripcion = x.ObjetivoVisita_Descripcion, ObjetivoVisita_Id = x.ObjetivoVisita_Id})
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
    public class ObjetivoVisitaResponseModel {
        public int ObjetivoVisita_Id { get; set; }
        public string ObjetivoVisita_Descripcion { get; set; }
    }
}
