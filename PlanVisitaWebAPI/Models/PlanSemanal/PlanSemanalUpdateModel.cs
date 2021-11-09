using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.PlanSemanal
{
    public class PlanSemanalUpdateModel
    {
        public int PlanSemanal_Id { get; set; }
        public int Sucursal_Id { get; set; }
        public string Cliente_Cod { get; set; }
        public System.DateTime PlanSemanal_Horario { get; set; }
        public int Vendedor_Id { get; set; }
        public int ObjetivoVisita_Id { get; set; }
        public string PlanSemanal_Estado { get; set; }
    }
}