using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public class ThreeWayOddProvider : BaseOddProvider, IOddProvider
    {
        public ThreeWayOddProvider()
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

        public float GetOddValue(string oddType, ApuestasDeportiva odd)
        {
            switch (oddType)
            { 
                case "tw_home":
                    return (float)odd.Odd1;
                case "tw_draw":
                    return (float)odd.Odd2;
                case "tw_away":
                    return (float)odd.Odd3;
                default:
                    throw new ArgumentOutOfRangeException("oddType", string.Format("Requested odd type '{0}' is not allowed",oddType));
            }
        }

        private void InitializeOdds()
        {
            //TODO: a futuro esto debería cargarse dinámicamente para hacerlo multi-idioma
            if (this.oddTypes == null)
            {
                this.oddTypes = new Dictionary<string, string>();
            }
            this.oddTypes.Add("tw_home", "Local");
            this.oddTypes.Add("tw_draw", "Empate");
            this.oddTypes.Add("tw_away", "Visitante");
        }
    }
}
