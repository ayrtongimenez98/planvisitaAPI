using Newtonsoft.Json;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models;
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
    public class VMotivoVisitaController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            var listaDivisions = db.Motivo.Where(x => x.Motivo_Descripcion.Contains(filtro)).ToList();

            var paginationModel = new PaginationModel<MotivoVisitaResponseModel>()
            {
                CantidadTotal = listaDivisions.Count,
                Listado = listaDivisions.Skip(skip).Take(take).Select(x => new MotivoVisitaResponseModel() { Motivo_Descripcion = x.Motivo_Descripcion, Motivo_Id = x.Motivo_Id })
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
    public class MotivoVisitaResponseModel
    {
        public int Motivo_Id { get; set; }
        public string Motivo_Descripcion { get; set; }
    }
}
