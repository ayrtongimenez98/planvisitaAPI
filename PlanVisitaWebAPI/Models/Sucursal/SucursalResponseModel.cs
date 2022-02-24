using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Sucursal
{
    public class SucursalVendedorResponseModel
    {
        public String Sucursal_Id { get; set; }
        public string Sucursal_Ciudad { get; set; }
        public string Cliente_RazonSocial { get; set; }
        public string Sucursal_Local { get; set; }
        public Nullable<Int16> Canal_Id { get; set; }
        public string Sucursal_Direccion { get; set; }
        public string Cliente_Cod { get; set; }
        public Int32 Cantidad_Visitas { get; set; }
    }
}