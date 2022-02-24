using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Usuario
{
    public class UsuarioResponseModel
    {
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public string Usuario_Nombre { get; set; }
        public string Usuario_Pass { get; set; }
        public int JefeVentas_Id { get; set; }
        public string Usuario { get; set; }
        public Nullable<int> Vendedor_Id { get; set; }
        public bool Es_Jefe { get; set; }
        public string Rol { get; set; }
    }
}