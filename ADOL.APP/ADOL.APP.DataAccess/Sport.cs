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
    
    public partial class Sport
    {
        public Sport()
        {
            this.SportEvents = new HashSet<SportEvent>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string League { get; set; }
        public string Country { get; set; }
        public string MenuFlagKey { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public string ProviderID { get; set; }
        public string InternalName { get; set; }
        public string CountryName { get; set; }
        public string RegionID { get; set; }
        public string RegionName { get; set; }
        public string TournamentID { get; set; }
        public string TournamentName { get; set; }
    
        public virtual ICollection<SportEvent> SportEvents { get; set; }
    }
}
