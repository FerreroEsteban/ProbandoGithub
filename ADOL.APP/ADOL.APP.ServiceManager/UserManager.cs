using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;
using ADOL.APP.CurrentAccountService.DataAccess.DBAccess;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public class UserManager
    {
        public BaseResponse<BE.User> GetUserInformation(string sessionToken)
        {
            UserAccess ua = new UserAccess();
            BaseResponse<BE.User> returnValue = null;
            try
            {
                var user = ua.GetUser(sessionToken);
                returnValue = new BaseResponse<BE.User>(user, ResponseStatus.OK);
            }
            catch (Exception ex)
            {
                returnValue = new BaseResponse<BE.User>(new BE.User(), ResponseStatus.Fail, string.Format("{0} - {1}","Error accessing user data",ex.ToString()));
            }
            
            return returnValue;
        }

        public BaseResponse<BE.User> LogUser(string launchToken)
        {
            BaseRequest req = new BaseRequest();
            BaseResponse<User> returnValue = null;
            req.LaunchToken = launchToken;
            try
            {
                BaseResponse<BaseWalletResponseData> login = UserWalletFacade.ProcessLogin(req);
                if (login.Status.Equals(ResponseStatus.OK))
                {
                    BE.User user = new User();
                    var walletResponse = login.GetData();
                    user.LaunchToken = launchToken;
                    user.SessionToken = walletResponse.SessionToken;
                    user.UID = walletResponse.UserUID;
                    user.NickName = walletResponse.NickName;
                    user.Balance = walletResponse.Balance;

                    UserAccess ua = new UserAccess();


                    returnValue = new BaseResponse<User>(ua.LoginUser(user), ResponseStatus.OK);
                }
                else
                {
                    returnValue = new BaseResponse<User>(new User(), ResponseStatus.Fail, login.Message);
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                return new BaseResponse<User>(new User(), ResponseStatus.Fail, string.Format("Error processing login - {0}", ex.ToString()));
            }
        }
    }
}
