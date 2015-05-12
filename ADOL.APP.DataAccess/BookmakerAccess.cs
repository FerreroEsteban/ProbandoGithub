using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Net;
using System.Xml;
using System.Dynamic;
using ADOL.APP.CurrentAccountService.DataAccess;
using BE = ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Data.Entity.Validation;
using ADOL.APP.CurrentAccountService.BusinessEntities;
using System.Configuration;
using ADOL.APP.CurrentAccountService.Helpers;

namespace ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess
{
    public class BookmakerAccess
    {
        public List<BE.SportEvent> PullEvents(List<BE.Sport> activeSports)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            String Xml;
            XmlDocument doc = null;
            List<BE.SportEvent> sports = new List<BE.SportEvent>();

            var oddTypes = new int[] { OddTypes.ThreeWay.Code, OddTypes.ThreeWayHalfTime.Code, OddTypes.ThreeWaySecondHalf.Code, 
                OddTypes.OddEven.Code, OddTypes.DrawNoBet.Code, OddTypes.DobleChance.Code };

            foreach (var sportCode in activeSports.Select(p => p.Code).Distinct())
            {
                foreach (var league in activeSports.Where(s => s.Code == sportCode).Select(p => p.League).Distinct())
                {
                    //string baseOddUrl = @"http://xml2.txodds.com/feed/odds/xml.php?ident=discoverytx&passwd=57t6y67";
                    //string uriTemplate = @"http://xml2.txodds.com/feed/odds/xml.php?ident=discoverytx&passwd=57t6y67&bid=126&mgid={0}&spid={1}&days=14";

                    StringBuilder uriBuilder = new StringBuilder();
                    uriBuilder.AppendFormat(ConfigurationHelper.EventsServiceURL, 
                                            ConfigurationHelper.ServiceUSR, 
                                            ConfigurationHelper.ServicePWD);
                    uriBuilder.AppendFormat("&bid=126,42&mgid={0}&spid={1}&days=14&ot={2}",
                                            string.Join(",", league),
                                            string.Join(",", sportCode),
                                            string.Join(",", oddTypes));

                    

                    //var sportlea = activeSports.Select(p => p.Code).Distinct().ToList();

                    // Create the web request  
                    request = WebRequest.Create(uriBuilder.ToString()) as HttpWebRequest;

                    // Get response  
                    using (response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream  
                        StreamReader reader = new StreamReader(response.GetResponseStream());

                        Xml = reader.ReadToEnd();
                        doc = new XmlDocument();
                        doc.LoadXml(Xml);
                    }

                    //XmlNamespaceManager nmgr = new XmlNamespaceManager(doc.NameTable);
                    //nmgr.AddNamespace("rest", "http://schemas.microsoft.com/search/local/ws/rest/v1");
                    XmlNodeList matches = doc.SelectNodes("matches/match");

                    var nodeList = new List<XmlNode>(matches.Cast<XmlNode>());

                    foreach (XmlNode match in nodeList)
                    {
                        XmlNodeList offer = match.SelectNodes("bookmaker/offer");


                        string torunamentID = match.SelectSingleNode("group").Attributes["id"].Value;
                        if (activeSports.Any(p => p.TournamentID.Equals(torunamentID)))
                        {
                            BE.SportEvent sportEvent = new BE.SportEvent();
                            sportEvent.Code = match.Attributes["id"].Value;
                            sportEvent.LeagueCode = match.SelectSingleNode("group").Attributes["id"].Value;
                            sportEvent.CountryCode = match.SelectSingleNode("group").Attributes["cnid"].Value;


                            sportEvent.Name = string.Format("{0} - {1}",
                                                             match.SelectSingleNode("hteam").InnerText,
                                                              match.SelectSingleNode("ateam").InnerText);
                            sportEvent.Init = DateTime.Parse(match.SelectSingleNode("time").InnerText);
                            sportEvent.End = sportEvent.Init.AddMinutes((double)90); //cambiar por un proveedor que pueda distinguir por deporte
                            sportEvent.Home = match.SelectSingleNode("hteam").InnerText;
                            sportEvent.Away = match.SelectSingleNode("ateam").InnerText;
                            sportEvent.SportID = activeSports.Where(p => p.TournamentID.Equals(torunamentID)).First().ID;

                            foreach (var oddId in oddTypes)
                            {
                                XmlNode xmlOdd = match.SelectSingleNode("bookmaker[@bid=126]/offer[@ot=" + oddId + "]");
                             
                                if (xmlOdd == null)
                                {
                                    xmlOdd = match.SelectSingleNode("bookmaker[@bid=42]/offer[@ot=" + oddId + "]");
                                }

                                if (xmlOdd != null)
                                {
                                    BE.SportBet userBet = new BE.SportBet();
                                    userBet.LastUpdate = DateTime.Parse(xmlOdd.Attributes["last_updated"].Value);
                                    userBet.Name = xmlOdd.Attributes["otname"].Value.Trim();

                                    XmlNodeList odds = xmlOdd.SelectNodes("odds");
                                    XmlNode odd = odds[odds.Count - 1];

                                    userBet.Odd1 = decimal.Parse(odd.SelectSingleNode("o1").InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                                    userBet.Odd2 = decimal.Parse(odd.SelectSingleNode("o2").InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                                    userBet.Odd3 = decimal.Parse(odd.SelectSingleNode("o3").InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                                    userBet.Code = xmlOdd.Attributes["otname"].Value.Trim();
                                    sportEvent.SportBets.Add(userBet);
                                }

                            }
                            sports.Add(sportEvent);

                        }
                    }
                }
            }

            return sports;
        }

        public List<BE.MatchResults> PullResults(string[] leagues)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            String Xml;
            XmlDocument doc = null;

            string uriTemplate = string.Format(ConfigurationHelper.ResultServiceURL, 
                                                ConfigurationHelper.ServiceUSR, 
                                                ConfigurationHelper.ServicePWD, 
                                                string.Join(",", leagues));

            // Create the web request  
            request = WebRequest.Create(uriTemplate) as HttpWebRequest;


            // Get response  
            using (response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                Xml = reader.ReadToEnd();
                doc = new XmlDocument();
                doc.LoadXml(Xml);
            }


            //XmlNamespaceManager nmgr = new XmlNamespaceManager(doc.NameTable);
            //nmgr.AddNamespace("rest", "http://schemas.microsoft.com/search/local/ws/rest/v1");
            XmlNodeList matches = doc.SelectNodes("matches/match");

            var nodeList = new List<XmlNode>(matches.Cast<XmlNode>());
            List<BE.MatchResults> returnValue = new List<BE.MatchResults>();
            foreach (XmlNode match in nodeList)
            {
                returnValue.Add(MatchResults.Parse(match));
            }
            return returnValue;
        }
    }
}