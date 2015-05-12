using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities.DTOs
{
    [DataContract]
    public class ActionResultDTO
    {
        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "userNick")]
        public string UserNick { get; set; }

        [DataMember(Name = "userBalance")]
        public decimal UserBalance { get; set; }

        [DataMember(Name = "sports")]
        public List<SportDTO> Sports { get; set; }

        [DataMember(Name = "bets")]
        public List<UserBetDTO> Bets { get; set; }

    }
}
