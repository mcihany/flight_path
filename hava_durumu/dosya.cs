using System;
using System.Collections.Generic;
using System.Text;
using System.Deployment.Application;

namespace Ucus_Yolu
{
     class dosya
    {
        public  dosya()
        {
            string d1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MCY\Yol_Boyu\icao_list.xml";
            string d2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MCY\Yol_Boyu\siralanmis_icao.xml";
            string d3 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MCY\Yol_Boyu\olaylar.txt";
            if (ApplicationDeployment.IsNetworkDeployed)
            {

                if (System.IO.File.Exists(d1))
                {
                    System.IO.File.Copy(d1, ApplicationDeployment.CurrentDeployment.DataDirectory + @"\icao_list.xml", true);
                    System.IO.File.Delete(d1);
                }
                if (System.IO.File.Exists(d2))
                {
                    System.IO.File.Copy(d2, ApplicationDeployment.CurrentDeployment.DataDirectory + @"\siralanmis_icao.xml", true);
                    System.IO.File.Delete(d2);
                }
                xml_icao = ApplicationDeployment.CurrentDeployment.DataDirectory + @"\icao_list.xml";
                sirali_xml_icao = ApplicationDeployment.CurrentDeployment.DataDirectory + @"\siralanmis_icao.xml";
                olaylar_txt = ApplicationDeployment.CurrentDeployment.DataDirectory + @"\olaylar.txt";
            }
            else
            {
                xml_icao = d1;
                sirali_xml_icao = d2;
                olaylar_txt = d3;
            }

        }
        static string xml_icao;
        static string sirali_xml_icao;
        static string olaylar_txt;
        public static string XmlIcao
        {
            get { return xml_icao; }
        }
        public static string SiraliXmlIcao {
            get { return sirali_xml_icao; }
        }

        public static string OlaylarTxt {
            get { return olaylar_txt; }
        
        }
    }
}
