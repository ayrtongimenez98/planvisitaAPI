using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.Sucursal;
using PlanVisitaWebAPI.Models.Vendedor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Marcaciones
{
    public class MarcacionesResponseModel
    {
        public int Visita_Id { get; set; }
        public string Periodo { get; set; }
        public DateTime Visita_fecha { get; set; }
        public string Visita_Hora_Entrada { get; set; }
        public string Visita_Hora_Salida { get; set; }
        public int Vendedor_Id { get; set; }
        public string Vendedor { get; set; }
        public string CodCliente { get; set; }
        public string Cliente { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public String Sucursal_Id { get; set; }
        public string Observacion { get; set; }
        public string Visita_Ubicacion_Entrada { get; set; }
        public string Visita_Ubicacion_Salida { get; set; }
        public string Mes { get; set; }
        public string Dia { get; set; }
        public String Año { get; set; }
        
    }
}