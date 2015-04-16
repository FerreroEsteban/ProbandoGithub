using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities.DTOs
{
    [DataContract]
    public class BetInfoItemDTO
    {
        [DataMember(Name = "match")]
        public MatchDTO Match { get; set; }

        [DataMember(Name = "betDetail")]
        public BetDetailDTO BetDetail { get; set; }

    }
}
