using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Puntera
{
    public class PunteraUpsertModel
    {
        public int Puntera_Id { get; set; }
        public int Usuario_Id { get; set; }
        public int Marca_Id { get; set; }
        public int SubMarca_Id { get; set; }
        public System.DateTime Puntera_FechaCarga { get; set; }
        public int Puntera_Cantidad { get; set; }
        public bool Puntera_LucesEncendidas { get; set; }
        public string Puntera_Observacion { get; set; }
    }
}