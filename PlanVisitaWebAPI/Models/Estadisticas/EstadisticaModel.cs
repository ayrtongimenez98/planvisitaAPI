using PlanVisitaWebAPI.Models.Marcaciones;
using PlanVisitaWebAPI.Models.Sucursal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Estadisticas
{
    public class EstadisticaModel
    {
        public int Vendedor_Id { get; set; }
        public string Vendedor_Nombre { get; set; }
        public int Objetivo { get; set; }
        public int Realizado { get; set; }
        public int Porcentaje { get; set; }
        public int HechoPorDia { get; set; }
        public int ObjetivoPorDia { get; set; }
        public int CantidadSucursalesNoVisitados { get; set; }
        public int CantidadSucursalesVisitados { get; set; }
        public List<SucursalVendedorResponseModel> SucursalesNoVisitados { get; set; }
        public List<MarcacionesResponseModel> SucursalesVisitados { get; set; }
    }
}