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
    
    public partial class TipoMueble
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TipoMueble()
        {
            this.Mobiliario = new HashSet<Mobiliario>();
        }
    
        public int TipoMueble_Id { get; set; }
        public int Marca_Id { get; set; }
        public string TipoMueble_Tipo { get; set; }
    
        public virtual Marcas Marcas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mobiliario> Mobiliario { get; set; }
    }
}