﻿using System;
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
            //var amount = float.Parse(((float)rndm.Next(10, 20) / (float)3).ToString("#.##"));
            float amount = 0.25f;
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1,8)].ID, amount, "tw_home"));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1,8)].ID, amount,"tw_draw"));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1,8)].ID, amount,"tw_away"));
            amount = 0.5f;
            usr = Guid.NewGuid();
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_home"));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_draw"));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_away"));
            amount = 0.75f;
            usr = Guid.NewGuid();
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_home"));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_draw"));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_away"));
            amount = 1.75f;
            usr = Guid.NewGuid();
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_home"));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_draw"));
            Assert.IsTrue(bmng.AddUserBet(usr.ToString(), emgr.GetSportEvent("1")[rndm.Next(1, 8)].ID, amount, "tw_away"));

        }

        [TestMethod]
        public void CheckUserBets()
        {
            BetManager mng = new BetManager();
            List<BE.ApuestasDeUsuario> bets = mng.GetUserBets("d09ff82d-4afa-44ae-b8b0-8ba94bc6eaca");
            Assert.IsTrue(bets.Count > 0);
        }
    }   
}
