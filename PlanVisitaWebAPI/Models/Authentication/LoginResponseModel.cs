using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Authentication
{
    public class LoginResponseModel
    {
        public string Id { get; set; }
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public int Usuario_Id { get; set; }
        public string Email { get; set; }
        public bool Es_Jefe { get; set; }
        public string Rol { get; set; }
    }
}