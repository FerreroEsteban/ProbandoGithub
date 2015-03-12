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
        public void UpdateEvents()
        {
            EventsManager mgr = new EventsManager();
            mgr.UpdateEvents();
        }

        [TestMethod]
        public void GetSportsEvents()
        {
            EventsManager mgr = new EventsManager();
            var eventos = mgr.GetSportEvent("1");
            Assert.IsTrue(eventos.Count > 0);
            foreach (var evento in eventos)
            {
                Assert.IsTrue(evento.ApuestasDeportivas.Count > 0);
            }
        }

        [TestMethod]
        public void AddUserBets()
        {
            EventsManager emgr = new EventsManager();
            BetManager bmng = new BetManager();
            var usr = Guid.NewGuid();
            var rndm = new Random();
            var amount = ((float)(rndm.Next(10,20) / 3));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1,10)].ID, amount));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 10)].ID, amount));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 10)].ID, amount));
        }
    }
}
