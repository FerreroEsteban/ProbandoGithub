//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ADOL.APP.CurrentAccountService.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class ApuestasDeportiva
    {
        public ApuestasDeportiva()
        {
            this.ApuestasDeUsuarios = new HashSet<ApuestasDeUsuario>();
        }
    
        public int ID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public System.DateTime Acualizado { get; set; }
        public int EventoID { get; set; }
        public double Odd1 { get; set; }
        public Nullable<double> Odd2 { get; set; }
        public Nullable<double> Odd3 { get; set; }
        public Nullable<double> Odd4 { get; set; }
        public string TipoApuesta { get; set; }
    
        public virtual ICollection<ApuestasDeUsuario> ApuestasDeUsuarios { get; set; }
        public virtual EventosDeportivo EventosDeportivo { get; set; }
    }
}
