//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Toqueyva.Framework.CurrentAccountService.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Deporte
    {
        public Deporte()
        {
            this.ApuestasDeportivas = new HashSet<ApuestasDeportiva>();
            this.EventosDeportivos = new HashSet<EventosDeportivo>();
        }
    
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool Activo { get; set; }
    
        public virtual ICollection<ApuestasDeportiva> ApuestasDeportivas { get; set; }
        public virtual ICollection<EventosDeportivo> EventosDeportivos { get; set; }
    }
}
