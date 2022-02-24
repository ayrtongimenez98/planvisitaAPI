using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Migracion
{
    public class MigracionesUpsertModel
    {
        public TipoMigracion TipoMigracion { get; set; }
        public int VendedorIdReceptor { get; set; }
        public int VendedorIdEmisor { get; set; }
        public string CodCliente { get; set; }
        public List<MigracionSucursalModel> ListaSucursales { get; set; } = new List<MigracionSucursalModel>();
    }
    public class MigracionSucursalModel
    {
        public int SucursalId  { get; set; }
        public string CodCliente { get; set; }
        public int CantidadVisitas { get; set; }
    }
    public enum TipoMigracion
    {
        CopiarSucursales = 1,
        ReemplazarSucursales = 2,
        MudarSucursalesAVendedor = 3,
        AsignarTodasSucursalesAVendedor = 4
    }
}