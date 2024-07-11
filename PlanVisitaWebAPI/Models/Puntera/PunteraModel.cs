using PlanVisitaWebAPI.Models.Marca;
using PlanVisitaWebAPI.Models.SubMarca;
using PlanVisitaWebAPI.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Puntera
{
    public class PunteraModel
    {
        public int Puntera_Id { get; set; }
        public UsuarioResponseModel Usuario { get; set; }
        public MarcaModel Marca { get; set; }
        public SubMarcaModel SubMarca { get; set; }
        public System.DateTime Puntera_FechaCarga { get; set; }
        public int Puntera_Cantidad { get; set; }
        public bool Puntera_LucesEncendidas { get; set; }
        public string Puntera_Observacion { get; set; }
    }
}