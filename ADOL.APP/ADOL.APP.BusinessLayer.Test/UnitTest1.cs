using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ADOL.APP.CurrentAccountService.ServiceManager;
using ADOL.APP.CurrentAccountService.DataAccess;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Collections.Generic;

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
        public void CheckResults()
        {
            EventsManager mgr = new EventsManager();
            mgr.CheckResults("1");
        }

        [TestMethod]
        public void GetSportsEvents()
        {
            EventsManager mgr = new EventsManager();
            var events = mgr.GetSportEvents("1");
            Assert.IsTrue(events.Count > 0);
            foreach (var sportEvent in events)
            {
                Assert.IsTrue(sportEvent.SportBets.Count > 0);
            }
        }

        //[TestMethod]
        //public void AddUserBets()
        //{
        //    EventsManager emgr = new EventsManager();
        //    BetManager bmng = new BetManager();
        //    var usr = Guid.NewGuid();
        //    var rndm = new Random();
        //    //var amount = float.Parse(((float)rndm.Next(10, 20) / (float)3).ToString("#.##"));
        //    decimal amount = 0.25M;
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1,8)].ID, amount, "tw_home"));
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1,8)].ID, amount,"tw_draw"));
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1,8)].ID, amount,"tw_away"));
        //    amount = 0.5M;
        //    usr = Guid.NewGuid();
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_home"));
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_draw"));
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_away"));
        //    amount = 0.75M;
        //    usr = Guid.NewGuid();
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_home"));
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_draw"));
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_away"));
        //    amount = 1.75M;
        //    usr = Guid.NewGuid();
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_home"));
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_draw"));
        //    Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_away"));

        //}

        //[TestMethod]
        //public void CheckUserBets()
        //{
        //    BetManager mng = new BetManager();
        //    List<BE.UserBet> bets = mng.GetUserBets("d09ff82d-4afa-44ae-b8b0-8ba94bc6eaca");
        //    Assert.IsTrue(bets.Count > 0);
        //}
    }   
}
