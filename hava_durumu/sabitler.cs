using System;
using System.Collections.Generic;
using System.Text;

namespace Ucus_Yolu
{
    class sabitler
    {
        //  static string metar = "ftp://tgftp.nws.noaa.gov/data/observations/metar/stations/";
        // static string uzun_taf = "ftp://tgftp.nws.noaa.gov/data/forecasts/taf/stations/";
        // static string kisa_taf = "ftp://tgftp.nws.noaa.gov/data/forecasts/shorttaf/stations/";
        static string metar = "http://tgftp.nws.noaa.gov/data/observations/metar/stations/";
         static string uzun_taf = "http://tgftp.nws.noaa.gov/data/forecasts/taf/stations/";
         static string kisa_taf = "http://tgftp.nws.noaa.gov/data/forecasts/shorttaf/stations/";

       // static string metar = "http://tgftp.nws.noaa.gov.ru2.gsr.awhoer.net/data/observations/metar/stations/";
       // static string uzun_taf = "http://tgftp.nws.noaa.gov.ru2.gsr.awhoer.net/data/forecasts/taf/stations/";
       // static string kisa_taf = "http://tgftp.nws.noaa.gov.ru2.gsr.awhoer.net/data/forecasts/shorttaf/stations/";

        public static string WebUrlMetar {
            get { return metar; }
        }
        public static string WebUrlKisaTaf {
            get { return kisa_taf; }
        }
        public static string WebUrlUzunTaf {
            get { return uzun_taf; }
        }

        public static string yazici_baslik {
            get { return "Turkish Meteorological Service"; }
        
        }
        public static string yazici_alt {
            get { return "http://program.cihanyeter.com"; }
        
        }
       
    }
}
