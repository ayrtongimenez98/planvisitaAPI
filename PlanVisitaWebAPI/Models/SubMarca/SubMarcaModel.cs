using PlanVisitaWebAPI.Models.Marca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.SubMarca
{
    public class SubMarcaModel
    {
        public int SubMarca_Id { get; set; }
        public MarcaModel Marca { get; set; }
        public string SubMarca_Nombre { get; set; } = string.Empty;
    }

    public class SubMarcaUpsertModel
    {
        public int SubMarca_Id { get; set; }
        public int Marca_Id { get; set; }
        public string SubMarca_Nombre { get; set; } = string.Empty;
    }
}