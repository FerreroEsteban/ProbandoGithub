
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities.DTOs
{
    [DataContract]
    public class BetDetailDTO
    {
        [DataMember(Name = "oddType")]
        public string OddType { get; set; }

        [DataMember(Name = "oddCode")]
        public string OddCode { get; set; }

    }
}
