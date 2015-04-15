﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Net;
using System.IO;
using System.Dynamic;
using ADOL.APP.CurrentAccountService.Helpers;
using System.Web.Script.Serialization;

namespace ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess
{
    public static class UserWalletFacade
    {
        public static bool ValidateFundsAvailable(BusinessEntities.User user, decimal amount)
        {
            return user.Balance >= amount;
        }

        public static BaseResponse<BaseWalletResponseData> ProcessLogin(BaseRequest request)
        {
            dynamic requestData = new ExpandoObject();
            requestData.token = request.SessionToken;

            dynamic serviceData = new ExpandoObject();

            CallService(ConfigurationHelper.GetConfigurationItem("AuthenticateUserServiceEndpoint"), requestData, out serviceData);

            BaseWalletResponseData data = new BaseWalletResponseData();

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            data.UserUID = serviceData.IUD;
            data.NickName = serviceData.NickName;
            data.Balance = (decimal)serviceData.Balance;
            data.errorCode = (WalletErrorCode)Enum.ToObject(typeof(WalletErrorCode), serviceData.ErrorCode);
            data.ErrorMessage = serviceData.ErrorDescription;
            data.SessionToken = serviceData.Token;
            data.Date = serializer.Deserialize(serviceData.Date, typeof(System.DateTime));

            if (data.errorCode.Equals((int)WalletErrorCode.Success))
            {
                return new BaseResponse<BaseWalletResponseData>(data, ResponseStatus.OK);
            }
            else
            {
                return new BaseResponse<BaseWalletResponseData>(data, ResponseStatus.Fail, string.Format("Service error: {0} - {1}", data.errorCode, data.ErrorMessage));
            }
        }

        public static BaseResponse<BaseWalletResponseData> ProcessBetDebit(DebitRequest request)
        {
            dynamic requestData = new ExpandoObject();
            requestData.Token = request.SessionToken;
            requestData.UID = request.UserUID;
            requestData.TransactionID = request.TransactionID;
            requestData.EventId = request.EventID;
            requestData.EventName = request.EventName;
            requestData.DebitAmount = request.Amount;
            requestData.betDetail = request.BetDetail;

            dynamic serviceData = new ExpandoObject();

            CallService(ConfigurationHelper.GetConfigurationItem("DebitServiceEndpoint"), requestData, out serviceData);

            BaseWalletResponseData data = new BaseWalletResponseData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            data.TransactionID = serviceData.TransactionID;
            data.Balance = (decimal)serviceData.Balance;
            data.errorCode = (WalletErrorCode)Enum.ToObject(typeof(WalletErrorCode), serviceData.ErrorCode);
            data.ErrorMessage = serviceData.ErrorDescription;
            data.SessionToken = serviceData.Token;
            data.Date = serializer.Deserialize(serviceData.Date, typeof(System.DateTime));


            if (data.errorCode.Equals(WalletErrorCode.Success))
            {
                return new BaseResponse<BaseWalletResponseData>(data, ResponseStatus.OK);
            }
            else
            {
                return new BaseResponse<BaseWalletResponseData>(data, ResponseStatus.Fail, string.Format("Service error: {0} - {1}", data.errorCode, data.ErrorMessage));
            }

        }

        public static BaseResponse<BaseWalletResponseData> ProcessBetCredit(CreditRequest request)
        {
            dynamic requestData = new ExpandoObject();
            requestData.Token = request.SessionToken;
            requestData.UID = request.UserUID;
            requestData.TransactionID = request.TransactionID;
            requestData.EventId = request.EventID;
            requestData.EventName = request.EventName;
            requestData.CreditAmount = request.Amount;
            requestData.WinningDetail = request.BetDetail;

            dynamic serviceData = new ExpandoObject();

            CallService(ConfigurationHelper.GetConfigurationItem("CreditServiceEndpoint"), requestData, out serviceData);

            BaseWalletResponseData data = new BaseWalletResponseData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            data.UserUID = serviceData.IUD;
            data.TransactionID = serviceData.TransactionID;
            data.Balance = (decimal)serviceData.Balance;
            data.errorCode = (WalletErrorCode)Enum.ToObject(typeof(WalletErrorCode), serviceData.ErrorCode);
            data.ErrorMessage = serviceData.ErrorDescription;
            data.SessionToken = serviceData.Token;
            data.Date = serializer.Deserialize(serviceData.Date, typeof(System.DateTime));

            if (data.errorCode.Equals(WalletErrorCode.Success))
            {
                return new BaseResponse<BaseWalletResponseData>(data, ResponseStatus.OK);
            }
            else
            {
                return new BaseResponse<BaseWalletResponseData>(data, ResponseStatus.Fail, string.Format("Service error: {0} - {1}", data.errorCode, data.ErrorMessage));
            }
        }

        public static BaseResponse<BaseWalletResponseData> ProcessRollback(BaseRequest request)
        {
            dynamic requestData = new ExpandoObject();
            requestData.Token = request.SessionToken;
            requestData.UID = request.UserUID;
            requestData.TransactionID = request.TransactionID;
            requestData.EventId = request.EventID;
            requestData.EventName = request.EventName;
            requestData.RollbackAmount = request.Amount;

            dynamic serviceData = new ExpandoObject();

            CallService(ConfigurationHelper.GetConfigurationItem("RollbackServiceEndpoint"), requestData, out serviceData);

            BaseWalletResponseData data = new BaseWalletResponseData();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            data.UserUID = serviceData.IUD;
            data.TransactionID = serviceData.TransactionID;
            data.Balance = (decimal)serviceData.Balance;
            data.errorCode = (WalletErrorCode)Enum.ToObject(typeof(WalletErrorCode), serviceData.ErrorCode);
            data.ErrorMessage = serviceData.ErrorDescription;
            data.SessionToken = serviceData.Token;
            data.Date = serializer.Deserialize(serviceData.Date, typeof(System.DateTime));

            if (data.errorCode.Equals(WalletErrorCode.Success))
            {
                return new BaseResponse<BaseWalletResponseData>(data, ResponseStatus.OK);
            }
            else
            {
                return new BaseResponse<BaseWalletResponseData>(data, ResponseStatus.Fail, string.Format("Service error: {0} - {1}", data.errorCode, data.ErrorMessage));
            }
        }

        private static void CallService(string serviceUrl, dynamic requestData, out dynamic responseData)
        {
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            try
            {
                // Create the web request  
                httpRequest = WebRequest.Create(serviceUrl) as HttpWebRequest;
                httpRequest.Method = "POST";
                httpRequest.ContentType = "Application/json";

                httpRequest.ContentLength = requestData.Length;

                StreamWriter requestWriter = new StreamWriter(httpRequest.GetRequestStream(), System.Text.Encoding.ASCII);
                requestWriter.Write(requestData);
                requestWriter.Close();

                // Get response  
                using (httpResponse = httpRequest.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(httpResponse.GetResponseStream());
                    responseData = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                responseData = new ExpandoObject();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                responseData.Token = requestData.Token;
                responseData.UID = requestData.UID;
                responseData.ErrorCode = -1;
                responseData.ErrorMessage = ex.Message + ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                responseData.Date = serializer.Serialize(DateTime.UtcNow);
            }
        }
    }
}