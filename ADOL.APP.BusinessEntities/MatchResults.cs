using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class MatchResults
    {
        public string MatchID { get; set; }

        public IList<Result> Results { get; set; }

        public IList<PeriodDetail> Details { get; set; }

        public IList<Score> Scores { get; set; }

        public static MatchResults Parse(XmlNode match)
        { 
            MatchResults matchResult = new MatchResults();

            XmlNode result = match.SelectSingleNode("results");
            if (result != null && !string.IsNullOrEmpty(result.InnerText) && result.SelectSingleNode("status").InnerText.ToUpper().Contains("FIN"))
            {
                matchResult.MatchID = match.Attributes["id"].Value;
                foreach (XmlNode singleResult in result.SelectNodes("result"))
                {
                    if (matchResult.Results == null)
                        matchResult.Results = new List<Result>();
                    matchResult.Results.Add(Result.Parse(singleResult));
                }

                XmlNode periods = result.SelectSingleNode("periods");
                if (periods != null && periods.ChildNodes != null && periods.ChildNodes.Count > 0)
                {
                    foreach (XmlNode period in periods.SelectNodes("period"))
                    { 
                        PeriodDetail periodDetail;
                        if (PeriodDetail.TryParse(period, out periodDetail))
                        {
                            if (matchResult.Details == null)
                                matchResult.Details = new List<PeriodDetail>();
                            matchResult.Details.Add(periodDetail);
                        }
                    }
                }

                XmlNode scorers = result.SelectSingleNode("scorers");
                if (scorers != null && scorers.ChildNodes != null && scorers.ChildNodes.Count > 0)
                {
                    foreach (XmlNode score in scorers.SelectNodes("score"))
                    {
                        if (matchResult.Scores == null)
                            matchResult.Scores = new List<Score>();
                        matchResult.Scores.Add(Score.Parse(score));
                    }
                }
            }

            return matchResult;
        }
    }

    public class Result
    {
        public Result(string id, string name, string value)
        {
            this.ID = id;
            this.Name = name;
            this.Value = value;
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public static Result Parse(XmlNode result)
        {
            return new Result(result.Attributes["id"].Value, result.Attributes["name"].Value, result.Attributes["value"].Value);
        }
    }

    public class PeriodDetail
    {
        public PeriodDetail(string id, string detailType, string name, string value)
        {
            this.ID = id;
            this.DetailType = detailType;
            this.Name = name;
            this.Value = value;
        }
        public PeriodDetail() { }
        public string ID { get; set; }
        public string DetailType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public static bool TryParse(XmlNode detailNode, out PeriodDetail periodDetail)
        {
            periodDetail = new PeriodDetail();
            if(string.IsNullOrEmpty(detailNode.SelectSingleNode("detail").Attributes["value"].Value))
            {
                return false;
            }
            
            periodDetail.ID = detailNode.Attributes["id"].Value;
            periodDetail.Name = detailNode.Attributes["name"].Value; 
            periodDetail.DetailType = detailNode.SelectSingleNode("detail").Attributes["name"].Value;
            periodDetail.Value = detailNode.SelectSingleNode("detail").Attributes["value"].Value;
            return true;
        }
    }

    public class Score
    {
        public Score(string name, string period, string team, int time, string scoreType)
        {
            this.Name = name;
            this.Period = period;
            this.Team = team;
            this.ScoreType = scoreType;
        }
        public Score() { }
        public string Name { get; set; }
        public string Period { get; set; }
        public string Team { get; set; }
        public int Time { get; set; }
        public string ScoreType { get; set; }

        public static Score Parse(XmlNode scoreNode)
        {
            return new Score(
                scoreNode.Attributes["name"].Value,
                scoreNode.Attributes["period"].Value,
                scoreNode.Attributes["team"].Value,
                int.Parse(scoreNode.Attributes["time"].Value),
                scoreNode.Attributes["type"].Value ?? string.Empty
                );
        }
    }
}
