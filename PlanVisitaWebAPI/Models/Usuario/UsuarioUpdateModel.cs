using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Usuario
{
    public class UsuarioUpdateModel
    {
        public int Usuario_Id { get; set; }
        public string Usuario_Nombre { get; set; }
        public string Usuario_Pass { get; set; }
        public int JefeVentas_Id { get; set; }
        public string Usuario1 { get; set; }
        public Nullable<int> Usuario_Vendedor_Id { get; set; }
        public string Rol { get; set; }
    }
}