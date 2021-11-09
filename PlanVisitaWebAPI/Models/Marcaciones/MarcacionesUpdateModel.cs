using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Marcaciones
{
    public class MarcacionesUpdateModel
    {
        public int Visita_Id { get; set; }
        public int Vendedor_Id { get; set; }
        public int Sucursal_Id { get; set; }
        public string Cliente_Cod { get; set; }
        public System.DateTime Visita_fecha { get; set; }
        public int Estado_Id { get; set; }
        public int Motivo_Id { get; set; }
        public string Visita_Observacion { get; set; }
        public string Visita_Ubicacion_Entrada { get; set; }
        public string Visita_Ubicacion_Salida { get; set; }
        public System.DateTime Visita_Hora_Entrada { get; set; }
        public System.DateTime Visita_Hora_Salida { get; set; }
    }
}