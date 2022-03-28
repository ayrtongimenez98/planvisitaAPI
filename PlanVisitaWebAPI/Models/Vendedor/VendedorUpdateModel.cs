using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Vendedor
{
    public class VendedorUpdateModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int JefeVentasId { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public List<int> Canales { get; set; }
    }
}