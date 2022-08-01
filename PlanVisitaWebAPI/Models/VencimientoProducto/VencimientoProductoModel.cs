using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.VencimientoProducto
{
    public class VencimientoProductoModel
    {
        public int Vencimiento_Id { get; set; }
        public string Vencimiento_Division { get; set; }
        public string Vencimiento_Canal { get; set; }
        public string Vencimiento_Cargo { get; set; }
        public string Vencimiento_Colaborador { get; set; }
        public string Vencimiento_PuntoVentaDireccion { get; set; }
        public DateTime? Vencimiento_FechaCreacion { get; set; }
        public List<VencimientoProductoDetalleModel> Vencimiento_Detalle { get; set; }
    }
}