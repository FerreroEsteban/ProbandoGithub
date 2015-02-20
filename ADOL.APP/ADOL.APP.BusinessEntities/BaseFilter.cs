using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toqueyva.Framework.CurrentAccountService
{
    public class BaseFilter
    {
        string ApplicationToken { get; set; }
        string AccountToken { get; set; }
        DateTime DateFrom { get; set; }
        DateTime DateTo { get; set; }
    }
}
