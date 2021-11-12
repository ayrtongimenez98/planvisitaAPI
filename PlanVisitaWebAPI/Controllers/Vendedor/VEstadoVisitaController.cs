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
    public class VEstadoVisitaController : ApiController
    {
        private PLAN_VISITAEntities db = new PLAN_VISITAEntities();
        public HttpResponseMessage Get(string filtro = "", int take = 10, int skip = 0)
        {
            if (filtro == null)
                filtro = "";
            var listaDivisions = db.Estado.Where(x => x.Estado_Nombre.Contains(filtro)).ToList();

            var paginationModel = new PaginationModel<EstadoVisitaResponseModel>()
            {
                CantidadTotal = listaDivisions.Count,
                Listado = listaDivisions.Skip(skip).Take(take).Select(x => new EstadoVisitaResponseModel() { Estado_Descripcion = x.Estado_Nombre, Estado_Id = x.Estado_Id })
            };
            var json = JsonConvert.SerializeObject(paginationModel);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
    public class EstadoVisitaResponseModel
    {
        public int Estado_Id { get; set; }
        public string Estado_Descripcion { get; set; }
    }
}
