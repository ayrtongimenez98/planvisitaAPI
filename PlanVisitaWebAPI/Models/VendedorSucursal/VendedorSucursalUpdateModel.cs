using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.VendedorSucursal
{
    public class VendedorSucursalUpdateModel
    {
        public int Vendedor_Id { get; set; }
        public int Sucursal_Id { get; set; }
        public int Cantidad { get; set; }
        public string Filtro { get; set; }
    }
}