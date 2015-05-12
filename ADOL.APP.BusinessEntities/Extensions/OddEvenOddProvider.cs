using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class OddEvenOddProvider : BaseOddProvider, IOddProvider
    {
        public OddEvenOddProvider()
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
                case "Odd":
                    return (decimal)odd.Odd1;
                case "Even":
                    return (decimal)odd.Odd3;
                default:
                    throw new ArgumentOutOfRangeException("oddType", string.Format("Requested odd type '{0}' is not allowed", oddType));
            }
        }

        private void InitializeOdds()
        {
            //TODO: a futuro esto debería cargarse dinámicamente para hacerlo multi-idioma
            if (this.oddTypes == null)
            {
                this.oddTypes = new Dictionary<string, string>();
            }
            this.oddTypes.Add("Odd", "Par");
            this.oddTypes.Add("Even", "Impar");
        }

        public bool ValidateUserBet(MatchResults results, UserBet userBet, out PaymentStatus status)
        {
            Result matchScore = results.Results.Where(p => p.Name.Equals("FT")).First();
            int total = 0;
            matchScore.Value.Split('-').ToList().ForEach(p =>
            {
                total += int.Parse(p.Trim());
            });
            if ((userBet.BetType.Equals("odd") && total % 2 != 0)
                || (userBet.BetType.Equals("even") && total % 2 == 0))
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
