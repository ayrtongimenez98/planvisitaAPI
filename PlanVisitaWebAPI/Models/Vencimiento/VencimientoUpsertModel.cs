﻿using PlanVisitaWebAPI.Models.VencimientoProducto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Vencimiento
{
    public class VencimientoUpsertModel
    {
        public int Vencimiento_Id { get; set; }
        public string Vencimiento_Division { get; set; }
        public string Vencimiento_Canal { get; set; }
        public string Vencimiento_Cargo { get; set; }
        public string Vencimiento_PuntoVentaDireccion { get; set; }
        public List<VencimientoProductoDetalleModel> Vencimiento_Detalle { get; set; }
    }
}