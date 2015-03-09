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
using System.Data.Entity.Validation;

namespace ADOL.APP.CurrentAccountService.DataAccess.ServiceAccess
{
    public class BookmakerAccess
    {
        public List<EventosDeportivo> PullEvents(string[] deporteID)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            String Xml;
            XmlDocument doc = null;
            List<EventosDeportivo> sports = new List<EventosDeportivo>();

            string uriTemplate = @"http://xml2.txodds.com/feed/odds/xml.php?ident=discoverytx&passwd=57t6y67&bid=126&cnid={0}&mgid={1}&psid={2}";
            //string[] allowedSports = new string[] { "1" };
            string[] allowedGroups = new string[] { "465", "467" };
            string[] allowedLeags = new string[] { "1002", "1111" };

            string requestUri = string.Format(uriTemplate,
                                                string.Join(",", allowedGroups),
                                                string.Join(",", allowedLeags),
                                                string.Join(",", deporteID));

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

                //Console.WriteLine("liga: " + match.SelectSingleNode("group").Attributes["cname"].Value);
                //Console.WriteLine(string.Format("local: {0} ({1}).", match.SelectSingleNode("hteam").InnerText, node.SelectSingleNode("o1").InnerText));
                //Console.WriteLine(string.Format("visitante: {0} ({1}).", match.SelectSingleNode("ateam").InnerText, node.SelectSingleNode("o3").InnerText));
                //Console.WriteLine(string.Format("empate: {0}", node.SelectSingleNode("o2").InnerText));
                //Console.WriteLine("hora: " + DateTime.Parse(match.SelectSingleNode("time").InnerText).ToShortDateString());

                EventosDeportivo sportEvent = new EventosDeportivo();
                sportEvent.Codigo = match.Attributes["id"].Value;
                sportEvent.CodigoLiga = match.SelectSingleNode("group").Attributes["mgid"].Value;
                sportEvent.CodigoPais = match.SelectSingleNode("group").Attributes["cnid"].Value;
                //sportEvent.Activo = true;
                sportEvent.Nombre = string.Format("{0} - {1}",
                                                 match.SelectSingleNode("hteam").InnerText,
                                                  match.SelectSingleNode("ateam").InnerText);
                sportEvent.Inicio = DateTime.Parse(match.SelectSingleNode("time").InnerText);
                sportEvent.Fin = sportEvent.Inicio.AddMinutes((double)90);

                ApuestasDeportiva userBet = new ApuestasDeportiva();
                userBet.Acualizado = DateTime.Parse(offer[offer.Count - 1].Attributes["last_updated"].Value);
                userBet.Nombre = offer[offer.Count - 1].Attributes["otname"].Value;

                XmlNodeList odds = offer[offer.Count - 1].SelectNodes("odds");
                XmlNode odd = odds[odds.Count - 1];

                userBet.Odd1 = float.Parse(odd.SelectSingleNode("o1").InnerText, System.Globalization.NumberStyles.AllowDecimalPoint);
                userBet.Odd2 = float.Parse(odd.SelectSingleNode("o2").InnerText, System.Globalization.NumberStyles.AllowDecimalPoint);
                userBet.Odd3 = float.Parse(odd.SelectSingleNode("o3").InnerText, System.Globalization.NumberStyles.AllowDecimalPoint);
                userBet.Codigo = offer[offer.Count - 1].Attributes["otname"].Value;
                userBet.TipoApuesta = offer[offer.Count - 1].Attributes["otname"].Value;
                sportEvent.ApuestasDeportivas.Add(userBet);
                sports.Add(sportEvent);
            }
            return sports;
        }
    }
}