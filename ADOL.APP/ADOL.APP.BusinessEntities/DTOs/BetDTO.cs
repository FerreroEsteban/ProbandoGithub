using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities.DTOs
{
    [DataContract]
    public class BetDTO
    {
        [DataMember(Name = "ID")]
        public int ID { get; set; }

        [DataMember(Name = "oddType")]
        public string OddType { get; set; }

        [DataMember(Name = "oddCollection")]
        public List<OddDTO> OddCollection { get; set; }

    }
}
