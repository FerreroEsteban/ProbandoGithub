using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOL.APP.CurrentAccountService.BusinessEntities
{
    public static class OddTypes
    {
        public static class ThreeWay
        {
            public static int Code = 0;
        }
        public static class ThreeWayHalfTime
        {
            public static int Code = 65536;
        }
        public static class ThreeWaySecondHalf
        {
            public static int Code = 327680;
        }
        public static class DobleChance
        {
            public static int Code = 5242880;
        }
        public static class DobleChanceHalfTime
        {
            public static int Code = 5308416;
        }
        public static class DrawNoBet
        {
            public static int Code = 6291457;
        }
        public static class OddEven
        {
            public static int Code = 2097153;
        }

        public static class BothTeamsToScore
        {
            public static int Code = 11534337;
        }
        public static class BothTeamsToScoreHalfTime
        {
            public static int Code = 11599873;
        }
        public static class BothTeamsToScoreFullTime
        {
            public static int Code = 11862017;
        }
    }
}
