using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities.DTOs
{
    [DataContract]
    public class EventDTO : MatchDTO
    {
        [DataMember(Name = "apuestasDisponibles")]
        public List<BetDTO> AvailableBets { get; set; }

    }
}
