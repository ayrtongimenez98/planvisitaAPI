using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Marcaciones
{
    public class MarcacionesFiltroModel
    {
        public int Vendedor_Id { get; set; }
        public string Cliente_Cod { get; set; }
        public DateTime Fecha_Desde { get; set; }
        public DateTime Fecha_Hasta { get; set; }
        public string Filtro { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}