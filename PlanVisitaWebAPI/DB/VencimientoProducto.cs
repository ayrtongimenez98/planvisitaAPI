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
    
    public partial class VencimientoProducto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VencimientoProducto()
        {
            this.VencimientoProductoDetalle = new HashSet<VencimientoProductoDetalle>();
        }
    
        public int Vencimiento_Id { get; set; }
        public string Vencimiento_Division { get; set; }
        public string Vencimiento_Canal { get; set; }
        public string Vencimiento_Cargo { get; set; }
        public string Vencimiento_Colaborador { get; set; }
        public string Vencimiento_PuntoVentaDireccion { get; set; }
        public Nullable<System.DateTime> Vencimiento_FechaCreacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VencimientoProductoDetalle> VencimientoProductoDetalle { get; set; }
    }
}
