using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities.DTOs
{
    [DataContract]
    public class MatchDTO
    {
        [DataMember(Name = "ID")]
        public int ID { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "nombre")]
        public string Name { get; set; }

        [DataMember(Name = "local")]
        public string Local { get; set; }

        [DataMember(Name = "visitante")]
        public string Visitante { get; set; }

        [DataMember(Name = "date")]
        public string Date { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }

    }
}
