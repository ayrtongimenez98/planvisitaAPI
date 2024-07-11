using PlanVisitaWebAPI.Models.Marca;
using PlanVisitaWebAPI.Models.TipoMueble;
using PlanVisitaWebAPI.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Mobiliario
{
    public class MobiliarioModel
    {
        public int Mobiliario_Id { get; set; }
        public UsuarioResponseModel Usuario { get; set; }
        public MarcaModel Marca { get; set; }
        public TipoMuebleModel TipoMueble { get; set; }
        public System.DateTime Mobiliario_FechaCarga { get; set; }
        public int Mobiliario_Cantidad { get; set; }
        public bool Mobiliario_LucesEncendidas { get; set; }
        public string Mobiliario_Observacion { get; set; }
    }
}