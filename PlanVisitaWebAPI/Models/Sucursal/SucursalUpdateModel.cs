using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Sucursal
{
    public class SucursalUpdateModel
    {
        public int Sucursal_Id { get; set; }
        public string Sucursal_Ciudad { get; set; }
        public string Sucursal_Direccion { get; set; }
        public string Cliente_Cod { get; set; }
    }
}