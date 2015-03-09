using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADOL.APP.CurrentAccountService.ServiceManager;

namespace ADOL.APP.BusinessLayer.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            EventsManager mgr = new EventsManager();
            mgr.UpdateEvents("1");
        }
    }
}
