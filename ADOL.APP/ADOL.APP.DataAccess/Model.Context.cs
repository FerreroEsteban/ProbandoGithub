﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ADOLDBEntities : DbContext
    {
        public ADOLDBEntities()
            : base("name=ADOLDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ApuestasDeportiva> ApuestasDeportivas { get; set; }
        public DbSet<ApuestasDeUsuario> ApuestasDeUsuarios { get; set; }
        public DbSet<Deporte> Deportes { get; set; }
        public DbSet<EventosDeportivo> EventosDeportivos { get; set; }
    }
}
