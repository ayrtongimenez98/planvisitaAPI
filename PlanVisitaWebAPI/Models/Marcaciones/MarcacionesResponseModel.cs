using PlanVisitaWebAPI.DB;
using PlanVisitaWebAPI.Models.Sucursal;
using PlanVisitaWebAPI.Models.Vendedor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Marcaciones
{
    public class MarcacionesResponseModel
    {
        public int Visita_Id { get; set; }
        public VendedorSucursalResponseModel Vendedor { get; set; }
        public SucursalVendedorResponseModel Sucursal { get; set; }
        public ClienteResponseModel Cliente { get; set; }
        public DateTime Visita_fecha { get; set; }
        public Estado Estado { get; set; }
        public Motivo Motivo { get; set; }
        public string Visita_Observacion { get; set; }
        public string Visita_Ubicacion { get; set; }
        public DateTime Visita_Hora { get; set; }
    }
}