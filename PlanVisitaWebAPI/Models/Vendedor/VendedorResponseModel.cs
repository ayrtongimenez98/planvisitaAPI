using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.Canal;

namespace PlanVisitaWebAPI.Models.Vendedor
{
    public class VendedorSucursalResponseModel
    {
        public int Vendedor_Id { get; set; }
        public string Vendedor_Nombre { get; set; }
        public int JefeVentas_Id { get; set; }
        public string Vendedor_Mail { get; set; }
        public DateTime Vendedor_FechaCreacion { get; set; }
        public DateTime Vendedor_FechaLastUpdate { get; set; }
        public string Vendedor_Rol { get; set; }
        public int Cantidad_Visitas { get; set; }
    }
}