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
    
    public partial class VisitaDual
    {
        public System.Guid VisitaDual_Id { get; set; }
        public int Vendedor_Id { get; set; }
        public int Sucursal_Id { get; set; }
        public System.DateTime VisitaDual_Hora_Entrada { get; set; }
        public string VisitaDual_Ubicacion_Entrada { get; set; }
        public Nullable<System.DateTime> VisitaDual_Hora_Salida { get; set; }
        public string VisitaDual_Ubicacion_Salida { get; set; }
        public System.DateTime VisitaDual_Fecha { get; set; }
        public string VisitaDual_Observacion { get; set; }
    
        public virtual VendedorCliente VendedorCliente { get; set; }
    }
}
