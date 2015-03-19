﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public partial class ApuestasDeportiva
    {
        private IOddProvider oddProvider;
        public IOddProvider OddProvider
        {
            get 
            {
                if (this.oddProvider == null)
                {
                    this.oddProvider = OddProviderFactory.GetOddProvider(this.Codigo);
                }
                return this.oddProvider;
            }
        }

        public float GetOddPrice(string oddType)
        {
            return this.OddProvider.GetOddValue(oddType, this);
        }
    }
}