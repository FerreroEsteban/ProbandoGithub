using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities.DTOs
{
    [DataContract]
    public class OddDTO
    {
        [DataMember(Name = "ID")]
        public string ID { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "price")]
        public decimal Price { get; set; }
    }
}
