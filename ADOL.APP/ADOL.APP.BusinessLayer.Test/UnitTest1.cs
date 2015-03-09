using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADOL.APP.CurrentAccountService.ServiceManager;
using ADOL.APP.CurrentAccountService.DataAccess;

namespace ADOL.APP.BusinessLayer.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            EventsManager mgr = new EventsManager();
            mgr.UpdateEvents();
        }

        [TestMethod]
        public void TestMethod2()
        {
            EventsManager mgr = new EventsManager();
            var eventos = mgr.GetSportEvent("1");
            Assert.IsTrue(eventos.Count > 0);
            foreach (var evento in eventos)
            {
                Assert.IsTrue(evento.ApuestasDeportivas.Count > 0);
            }
        }
    }
}
