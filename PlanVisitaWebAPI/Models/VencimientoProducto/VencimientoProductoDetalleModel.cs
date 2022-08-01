using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.VencimientoProducto
{
    public class VencimientoProductoDetalleModel
    {
        public int VencimientoProductoDetalle_Id { get; set; }
        public int VencimientoProducto_Id { get; set; }
        public string VencimientoProductoDetalle_Codigo_Barras { get; set; }
        public string VencimientoProductoDetalle_Descripcion_Producto { get; set; }
        public System.DateTime VencimientoProductoDetalle_Rango_Fecha { get; set; }
        public string VencimientoProductoDetalle_Cantidad_SKU { get; set; }
    }
}