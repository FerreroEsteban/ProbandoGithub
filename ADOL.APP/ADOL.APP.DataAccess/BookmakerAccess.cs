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


            foreach (var sportCode in activeSports.Select(p => p.Code).Distinct())
            {
                foreach (var league in activeSports.Where(s => s.Code == sportCode).Select(p => p.League).Distinct())
                {
                    string uriTemplate = @"http://xml2.txodds.com/feed/odds/xml.php?ident=discoverytx&passwd=57t6y67&bid=126&mgid={0}&spid={1}&days=14";

                    var sportlea = activeSports.Select(p => p.Code).Distinct().ToList();
                    string requestUri = string.Format(uriTemplate,
                                                string.Join(",", league),//allowedLeags),
                                                string.Join(",", sportCode));

                    // Create the web request  
                    request = WebRequest.Create(requestUri) as HttpWebRequest;

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

                            BE.SportBet userBet = new BE.SportBet();
                            userBet.LastUpdate = DateTime.Parse(offer[offer.Count - 1].Attributes["last_updated"].Value);
                            userBet.Name = offer[offer.Count - 1].Attributes["otname"].Value;

                            XmlNodeList odds = offer[offer.Count - 1].SelectNodes("odds");
                            XmlNode odd = odds[odds.Count - 1];

                            userBet.Odd1 = decimal.Parse(odd.SelectSingleNode("o1").InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                            userBet.Odd2 = decimal.Parse(odd.SelectSingleNode("o2").InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                            userBet.Odd3 = decimal.Parse(odd.SelectSingleNode("o3").InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                            userBet.Code = offer[offer.Count - 1].Attributes["otname"].Value;
                            sportEvent.SportBets.Add(userBet);
                            sports.Add(sportEvent);

                        }
                    }
                }
            }

            return sports;
        }

        public List<BE.MatchResults> PullResults(string sportCode)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            String Xml;
            XmlDocument doc = null;

            string uriTemplate = string.Format(@"http://xml2.tip-ex.com/feed/result/xml.php?ident=discoverytx&passwd=57t6y67&spid={0}&mgid=1002,1111&date=yesterday", sportCode);

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
                XmlNode result = match.SelectSingleNode("results");

                if (result != null && !string.IsNullOrEmpty(result.InnerText) && result.SelectSingleNode("status").InnerText.ToUpper().Contains("FIN"))
                {
                    BE.MatchResults matchResult = new BE.MatchResults();
                    matchResult.MatchID = match.Attributes["id"].Value;
                    foreach (XmlNode singleResult in result.SelectNodes("result"))
                    {
                        if (matchResult.Results == null)
                            matchResult.Results = new Dictionary<string, string>();
                        matchResult.Results.Add(singleResult.Attributes["name"].Value, singleResult.Attributes["value"].Value);
                    }
                    returnValue.Add(matchResult);
                }

            }
            return returnValue;
        }
    }
}