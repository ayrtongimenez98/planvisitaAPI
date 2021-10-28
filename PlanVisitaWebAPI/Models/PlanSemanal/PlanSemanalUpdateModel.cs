using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.PlanSemanal
{
    public class PlanSemanalUpdateModel
    {
        public int PlanSemanal_Id { get; set; }
        public string PlanSemanal_Periodo { get; set; }
        public int PlanSemanal_NroSemana { get; set; }
        public IEnumerable<PlanSemanalDetalleUpdateModel> Detalle { get; set; }
    }
}