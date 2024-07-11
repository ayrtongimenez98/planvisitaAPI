using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Mobiliario
{
    public class MobiliarioUpsertModel
    {
        public int Mobiliario_Id { get; set; }
        public int Usuario_Id { get; set; }
        public int Marca_Id { get; set; }
        public int TipoMueble_Id { get; set; }
        public System.DateTime Mobiliario_FechaCarga { get; set; }
        public int Mobiliario_Cantidad { get; set; }
        public bool Mobiliario_LucesEncendidas { get; set; }
        public string Mobiliario_Observacion { get; set; }
    }
}