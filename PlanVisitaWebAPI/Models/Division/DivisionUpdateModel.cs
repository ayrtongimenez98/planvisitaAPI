using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlanVisitaWebAPI.Models.Division
{
    public class DivisionUpdateModel
    {
        public int Division_Id { get; set; }
        public string Division_Nombre { get; set; }
        public int Empresa_Id { get; set; }
    }
}