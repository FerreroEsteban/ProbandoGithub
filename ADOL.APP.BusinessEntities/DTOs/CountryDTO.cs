using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities.DTOs
{
    [DataContract]
    public class CountryDTO
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "flag")]
        public string Flag { get; set; }
        
        [DataMember(Name = "leagues")]
        public List<LeagueDTO> Leagues { get; set; }
    }
}
