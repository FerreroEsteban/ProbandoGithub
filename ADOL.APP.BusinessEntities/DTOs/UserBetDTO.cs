using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities.DTOs
{
    [DataContract]
    public class UserBetDTO : BetDetailDTO
    {
        [DataMember(Name = "betId")]
        public int ID { get; set; }

        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "simple")]
        public bool Simple { get; set; }

        [DataMember(Name = "composed")]
        public bool Composed { get; set; }
        
        [DataMember(Name = "price")]
        public decimal Price { get; set; }

        [DataMember(Name = "match")]
        public MatchDTO Match { get; set; }

        [DataMember(Name = "betInfo")]
        public List<BetInfoItemDTO> BetInfo { get; set; }

    }
}
