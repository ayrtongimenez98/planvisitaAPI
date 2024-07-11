using PlanVisitaWebAPI.Models.Marca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.TipoMueble
{
    public class TipoMuebleModel
    {
        public int TipoMueble_Id { get; set; }
        public MarcaModel Marca { get; set; }
        public string TipoMueble_Tipo { get; set; }
    }

    public class TipoMuebleUpsertModel
    {
        public int TipoMueble_Id { get; set; }
        public int Marca_Id { get; set; }
        public string TipoMueble_Tipo { get; set; }
    }
}