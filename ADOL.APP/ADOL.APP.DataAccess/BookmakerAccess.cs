﻿using System;
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
        public List<BE.EventosDeportivo> PullEvents(List<BE.Deporte> activeSports)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            String Xml;
            XmlDocument doc = null;
            List<BE.EventosDeportivo> sports = new List<BE.EventosDeportivo>();

            string uriTemplate = @"http://xml2.txodds.com/feed/odds/xml.php?ident=discoverytx&passwd=57t6y67&bid=126&cnid={0}&mgid={1}&psid={2}";
            //string[] allowedSports = new string[] { "1" };
            //string[] allowedGroups = new string[] { "465", "467" };
            //string[] allowedLeags = new string[] { "1002", "1111" };

            string requestUri = string.Format(uriTemplate,
                                                string.Join(",", activeSports.Select(p => p.Pais).Distinct().ToArray()),// allowedGroups),
                                                string.Join(",", activeSports.Select(p => p.Liga).Distinct().ToArray()),//allowedLeags),
                                                string.Join(",", activeSports.Select(p => p.Codigo).Distinct().ToArray()));

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

                BE.EventosDeportivo sportEvent = new BE.EventosDeportivo();
                sportEvent.Codigo = match.Attributes["id"].Value;
                sportEvent.CodigoLiga = match.SelectSingleNode("group").Attributes["mgid"].Value;
                sportEvent.CodigoPais = match.SelectSingleNode("group").Attributes["cnid"].Value;
                
                sportEvent.Nombre = string.Format("{0} - {1}",
                                                 match.SelectSingleNode("hteam").InnerText,
                                                  match.SelectSingleNode("ateam").InnerText);
                sportEvent.Inicio = DateTime.Parse(match.SelectSingleNode("time").InnerText);
                sportEvent.Fin = sportEvent.Inicio.AddMinutes((double)90);
                sportEvent.Local = match.SelectSingleNode("hteam").InnerText;
                sportEvent.Visitante = match.SelectSingleNode("ateam").InnerText;
                sportEvent.DeporteID = activeSports.Where(p => p.Liga.Equals(match.SelectSingleNode("group").Attributes["mgid"].Value)).First().ID;

                BE.ApuestasDeportiva userBet = new BE.ApuestasDeportiva();
                userBet.Acualizado = DateTime.Parse(offer[offer.Count - 1].Attributes["last_updated"].Value);
                userBet.Nombre = offer[offer.Count - 1].Attributes["otname"].Value;

                XmlNodeList odds = offer[offer.Count - 1].SelectNodes("odds");
                XmlNode odd = odds[odds.Count - 1];

                userBet.Odd1 = decimal.Parse(odd.SelectSingleNode("o1").InnerText,System.Globalization.NumberFormatInfo.InvariantInfo);
                userBet.Odd2 = decimal.Parse(odd.SelectSingleNode("o2").InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                userBet.Odd3 = decimal.Parse(odd.SelectSingleNode("o3").InnerText,System.Globalization.NumberFormatInfo.InvariantInfo);
                userBet.Codigo = offer[offer.Count - 1].Attributes["otname"].Value;
                sportEvent.ApuestasDeportivas.Add(userBet);
                sports.Add(sportEvent);
            }
            return sports;
        }

        public List<BE.MatchResults> PullResults(string sportCode)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            String Xml;
            XmlDocument doc = null;

            string uriTemplate = string.Format(@"http://xml2.tip-ex.com/feed/result/xml.php?ident=discoverytx&passwd=57t6y67&spid={0}&date=today", sportCode);

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