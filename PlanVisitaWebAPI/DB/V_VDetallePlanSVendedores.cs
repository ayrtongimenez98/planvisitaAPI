//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlanVisitaWebAPI.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class V_VDetallePlanSVendedores
    {
        public int PlanSemanalId { get; set; }
        public int NroSemana { get; set; }
        public string Periodo { get; set; }
        public string DiaSemana { get; set; }
        public int VendedorId { get; set; }
        public string NombreVendedor { get; set; }
        public int SucursalId { get; set; }
        public string SucursalDireccion { get; set; }
        public string SucursalCiudad { get; set; }
        public int CodigoJefe { get; set; }
        public string NombreJefe { get; set; }
        public int Cantidad { get; set; }
        public int ObjetivoVisita { get; set; }
        public string Cliente_Cod { get; set; }
        public string Cliente_RazonSocial { get; set; }
    }
}