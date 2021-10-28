using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.PlanSemanal
{
    public class PlanSemanalDetalleUpdateModel
    {
        public int PlanSemanal_Id { get; set; }
        public string PlanSemanal_DiaSemana { get; set; }
        public int Sucursal_Id { get; set; }
        public System.DateTime PlanSemanal_Horario { get; set; }
        public int ObjetivoVisita_Id { get; set; }
    }
}