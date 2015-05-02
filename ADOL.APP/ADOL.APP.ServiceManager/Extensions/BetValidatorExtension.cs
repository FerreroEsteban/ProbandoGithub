using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADOL.APP.CurrentAccountService.BusinessEntities;

namespace ADOL.APP.CurrentAccountService.ServiceManager
{
    public static class BetValidatorExtension
    {
        public static BaseResponse<bool> ValidateBet(string code, decimal value, decimal amount, decimal balance, bool isLive = false)
        {
            try
            {
                if (amount > 4000M)
                    return new BaseResponse<bool>(false, ResponseStatus.OK, "supera el monto máximo para apuestas ($4000)");
                if (amount > balance)
                    return new BaseResponse<bool>(false, ResponseStatus.OK, "supera el balance en su cuenta");

                decimal max = 0M;
                switch (code)
                {
                    case "1":
                        max = 30000M;
                        break;
                    case "codigo_de_tennis": //TODO: reemplazar por el código del ténis
                        max = 25000M;
                        break;
                    case "código_de_basquet": //TOTO: reemplazar por el código del basquet
                        max = 15000M;
                        break;
                }
                if (isLive)
                    max = 25000M;

                if (value * amount > max)
                    return new BaseResponse<bool>(false, ResponseStatus.OK, string.Format("supera el máximo para este tipo de apuesta (${0}).", max));

                return new BaseResponse<bool>(true, ResponseStatus.OK);
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>(false, ResponseStatus.Fail, ex.Message);
            }
        }

        public static BaseResponse<bool> ValidateBet(string[] codes, decimal value, decimal amount, decimal balance, bool isLive = false)
        {
            try
            {
                if (amount > 4000M)
                    return new BaseResponse<bool>(false, ResponseStatus.OK, "supera el monto máximo para apuestas ($4000)");
                if(amount > balance)
                    return new BaseResponse<bool>(false, ResponseStatus.OK, "supera el balance en su cuenta");

                decimal max = 50000M;
                codes.Distinct().ToList().ForEach(p => {
                    switch (p)
                    {
                        case "1":
                            if(max > 50000M)
                                max = 50000M;
                            break;
                        case "codigo_de_tennis": //TODO: reemplazar por el código del ténis
                            if(max > 25000M)
                                max = 25000M;
                            break;
                        case "código_de_basquet": //TOTO: reemplazar por el código del basquet
                            if(max > 15000M)
                                max = 15000M;
                            break;
                    }
                });
                if (isLive)
                    max = 25000M;

                if (value * amount > max)
                    return new BaseResponse<bool>(false, ResponseStatus.OK, string.Format("supera el máximo para este tipo de apuesta (${0}).", max));

                return new BaseResponse<bool>(true, ResponseStatus.OK);
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>(false, ResponseStatus.Fail, ex.Message);
            }
        }
    }
}
/*
Fútbol: $30,000.-
Combinada Fútbol: $ 50,000.-
Combinada varios deportes: Max. deporte más bajo.
Tenis: $ 25,000.-
Basket: $15,000.-
Deportes en vivo: $ 25.000.-

El monto máximo a apostar es de $4000.
 */