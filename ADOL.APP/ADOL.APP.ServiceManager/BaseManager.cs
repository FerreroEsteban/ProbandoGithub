using ADOL.APP.CurrentAccountService.BusinessEntities;
using ADOL.APP.CurrentAccountService.DataAccess.DBAccess;
using ADOL.APP.CurrentAccountService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public abstract class BaseManager
    {
        private User currentUser = null;

        public BaseManager()
        {
            UpdateCurrentUserData();
        }
        
        protected virtual User GetSessionUser()
        {
            return currentUser;
        }

        private void UpdateCurrentUserData()
        {
            if (!string.IsNullOrEmpty(RequestContextHelper.SessionToken) && string.IsNullOrEmpty(RequestContextHelper.UserName))
            {
                UserAccess ua = new UserAccess();
                var user = ua.GetUser(RequestContextHelper.SessionToken);
                if (user != null)
                {
                    this.UpdateCurrentUserData(user);
                }
            }
        }

        protected void UpdateCurrentUserData(User user)
        {
            this.currentUser = user;

            RequestContextHelper.UserName = user.NickName;
            RequestContextHelper.UserBalance = user.Balance;
            RequestContextHelper.SessionToken = user.SessionToken;
        }
    }
}
