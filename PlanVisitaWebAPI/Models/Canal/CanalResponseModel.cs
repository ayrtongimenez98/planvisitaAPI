using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Canal
{
    public class CanalResponseModel
    {
        public int Canal_Id { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
    }
}