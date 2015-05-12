using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class DobleChanceOddProvider : BaseOddProvider, IOddProvider
    {
        public DobleChanceOddProvider()
        {
            InitializeOdds();
        }

        public IDictionary<string, string> GetAvailables()
        {
            return this.oddTypes;
        }

        public string GetOddName(string oddType)
        {
            return this.oddTypes[oddType];
        }

        public decimal GetOddValue(string oddType, SportBet odd)
        {
            switch (oddType)
            {
                case "1x":
                    return (decimal)odd.Odd1;
                case "x2":
                    return (decimal)odd.Odd2;
                case "12":
                    return (decimal)odd.Odd3;
                default:
                    throw new ArgumentOutOfRangeException("oddType", string.Format("Requested odd type '{0}' is not allowed", oddType));
            }
        }

        private void InitializeOdds()
        {
            if (this.oddTypes == null)
            {
                this.oddTypes = new Dictionary<string, string>();
            }
            this.oddTypes.Add("1x", "Local / Empate");
            this.oddTypes.Add("x2", "Empate / Visitante");
            this.oddTypes.Add("12", "Local / Visitante");
        }

        public bool ValidateUserBet(MatchResults results, UserBet userBet, out PaymentStatus status)
        {
            Result matchScore = results.Results.Where(p => p.Name.Equals("FT")).First();
            int home = int.Parse(matchScore.Value.Split('-')[0].Trim());
            int away = int.Parse(matchScore.Value.Split('-')[1].Trim());
            if (((userBet.BetType.Equals("1x") && home >= away )
                || (userBet.BetType.Equals("x2") && home <= away)
                || (userBet.BetType.Equals("12") && home != away)))
            {
                status = PaymentStatus.PayBack;
                return true;
            }
            else
            {
                status = PaymentStatus.NoHit;
                return false;
            }
        }
    }
}
