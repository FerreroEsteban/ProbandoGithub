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
                case "tw_home":
                    return (decimal)odd.Odd1;
                case "tw_draw":
                    return (decimal)odd.Odd2;
                case "tw_away":
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
            this.oddTypes.Add("tw_home", "Local");
            this.oddTypes.Add("tw_draw", "Empate");
            this.oddTypes.Add("tw_away", "Visitante");
        }

        public bool ValidateUserBet(MatchResults results, UserBet userBet)
        {
            return false;
        }
    }
}
