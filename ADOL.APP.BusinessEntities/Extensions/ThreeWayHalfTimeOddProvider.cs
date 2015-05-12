using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class ThreeWayHalfTimeOddProvider : BaseOddProvider, IOddProvider
    {
        public ThreeWayHalfTimeOddProvider()
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
                case "twht_home":
                    return (decimal)odd.Odd1;
                case "twht_draw":
                    return (decimal)odd.Odd2;
                case "twht_away":
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
            this.oddTypes.Add("twht_home", "Primer Tiempo Local");
            this.oddTypes.Add("twht_draw", "Primer Tiempo Empate");
            this.oddTypes.Add("twht_away", "Primer Tiempo Visitante");
        }

        public bool ValidateUserBet(MatchResults results, UserBet userBet, out PaymentStatus status)
        {
            PeriodDetail periodDetail = results.Details.Where(p => p.Name.Equals("1HF")).FirstOrDefault();
            if (periodDetail != null)
            {
                int home = int.Parse(periodDetail.Value.Split('-')[0].Trim());
                int away = int.Parse(periodDetail.Value.Split('-')[1].Trim());
                if (((userBet.BetType.Equals("twht_home") && home > away)
                    || (userBet.BetType.Equals("twht_draw") && home == away)
                    || (userBet.BetType.Equals("twht_away") && home < away)))
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
