using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class BaseResponse<T>
    {
        private T data;
        public ResponseStatus Status { get; set; }
        public string Message { get; set; }

        public BaseResponse(T responseData, ResponseStatus status, string message = "")
        {
            this.data = responseData;
            this.Status = status;
            this.Message  = message;
        }
        
        public T GetData()
        {
            return (T)data;
        }
    }

    public enum ResponseStatus
    {
        OK = 1,
        Fail = 2
    }
}
