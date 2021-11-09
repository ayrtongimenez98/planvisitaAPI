using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.PlanSemanal
{
    public class PlanSemanalResponseModel
    {
        public int PlanSemanal_Id { get; set; }
        public string Periodo { get; set; }
        public DateTime PlanSemanal_Horario { get; set; }
        public string PlanSemanal_Hora_Entrada { get; set; }
        public int Vendedor_Id { get; set; }
        public string Vendedor { get; set; }
        public string CodCliente { get; set; }
        public string Cliente { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public String Sucursal_Id { get; set; }
        public string PlanSemanal_Estado { get; set; }
    }
}