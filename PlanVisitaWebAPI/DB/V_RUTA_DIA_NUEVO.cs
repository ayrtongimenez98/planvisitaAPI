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
    
    public partial class V_RUTA_DIA_NUEVO
    {
        public string Periodo { get; set; }
        public string Visitador { get; set; }
        public int Legajo { get; set; }
        public int JefeVentas_Id { get; set; }
        public int SucursalId { get; set; }
        public string Cliente { get; set; }
        public string Direccion { get; set; }
        public string DiaSemana { get; set; }
        public int NroSemana { get; set; }
        public string HoraEntrada { get; set; }
        public string Objetivo { get; set; }
        public string Fecha { get; set; }
    }
}