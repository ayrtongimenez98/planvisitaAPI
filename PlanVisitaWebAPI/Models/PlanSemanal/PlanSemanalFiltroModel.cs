using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.PlanSemanal
{
    public class PlanSemanalFiltroModel
    {
        public DateTime Fecha_Desde { get; set; }
        public DateTime Fecha_Hasta { get; set; }
        public string Filtro { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
}