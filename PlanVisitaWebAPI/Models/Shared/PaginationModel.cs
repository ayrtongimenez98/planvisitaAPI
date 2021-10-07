using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models
{
    public class PaginationModel<T>
    {
        public IEnumerable<T> Listado { get; set; }
        public int CantidadTotal { get; set; }
    }
}