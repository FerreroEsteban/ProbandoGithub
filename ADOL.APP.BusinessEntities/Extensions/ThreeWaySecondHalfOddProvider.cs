using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class ThreeWaySecondHalfOddProvider : BaseOddProvider, IOddProvider
    {
        public ThreeWaySecondHalfOddProvider()
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
                case "twsh_home":
                    return (decimal)odd.Odd1;
                case "twsh_draw":
                    return (decimal)odd.Odd2;
                case "twsh_away":
                    return (decimal)odd.Odd3;
                default:
                    throw new ArgumentOutOfRangeException("oddType", string.Format("Requested odd type '{0}' is not allowed",oddType));
            }
        }

        private void InitializeOdds()
        {
            if (this.oddTypes == null)
            {
                this.oddTypes = new Dictionary<string, string>();
            }
            this.oddTypes.Add("twsh_home", "Local");
            this.oddTypes.Add("twsh_draw", "Empate");
            this.oddTypes.Add("twsh_away", "Visitante");
        }

        public bool ValidateUserBet(MatchResults results, UserBet userBet, out PaymentStatus status)
        {
            PeriodDetail periodDetail = results.Details.Where(p => p.Name.Equals("2HF")).FirstOrDefault();
            if (periodDetail != null)
            {
                int home = int.Parse(periodDetail.Value.Split('-')[0].Trim());
                int away = int.Parse(periodDetail.Value.Split('-')[1].Trim());
                if( ((userBet.BetType.Equals("twsh_home") && home > away)
                    || (userBet.BetType.Equals("twsh_draw") && home == away)
                    || (userBet.BetType.Equals("twsh_away") && home < away)))
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
            status = PaymentStatus.withoutInformation;
            return false;
        }
    }
}
