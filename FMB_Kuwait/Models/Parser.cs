using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMB_Kuwait.Models
{
    public class Parser
    {
        public Parser()
        {
        }

        public static bool ParseBoolean(string s)
        {
            try
            {
                string tmpS = s.ToUpperInvariant();
                return (tmpS == "TRUE" || tmpS == "YES" || tmpS == "1");
            }
            catch
            {
                return false;
            }
        }

        public static int ParseInt(string s)
        {
            int usi = 0;
            Int32.TryParse(s, out usi);
            // use default locale setting
            return usi;
        }

        public static short ParseShort(string s)
        {
            short usi = 0;
            Int16.TryParse(s, out usi);
            // use default locale setting
            return usi;
        }

        public static long ParseLong(string s)
        {
            long usl = 0;
            Int64.TryParse(s, out usl);
            // use default locale setting
            return usl;
        }

        public static float ParseSingle(string s)
        {
            float uss = 0;
            Single.TryParse(s, out uss);
            return uss;
        }

        public static double ParseDouble(string s)
        {
            double usd = 0;
            Double.TryParse(s, out usd);
            return usd;
        }

        public static decimal ParseDecimal(string s)
        {
            decimal usd = default(decimal);
            Decimal.TryParse(s, out usd);
            return usd;
        }

        public static DateTime ParseDateTime(string s)
        {
            try
            {
                return System.DateTime.Parse(s);
            }
            catch
            {
                return System.DateTime.MinValue;
            }
        }

        public static Guid ParseGuid(string s)
        {
            Guid usguid = default(Guid);
            try
            {
                usguid = new Guid(s);
            }
            catch (Exception ex)
            {
                usguid = Guid.Empty;
            }

            return usguid;
        }
    }
}