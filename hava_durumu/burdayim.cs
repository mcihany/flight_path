using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Deployment.Application;

namespace Ucus_Yolu
{
    class burdayim
    {

        public static Type TipGetir()
        {
            return (typeof(burdayim));
        }

        public static void BeniKaydet()
        {
            ThreadStart ts = new ThreadStart(beni_kaydet);
            Thread t = new Thread(ts);
            t.Start();
        }

        private static void beni_kaydet()
        {
            try
            {
                com.siparisyonetimi.servis.Service d = new Ucus_Yolu.com.siparisyonetimi.servis.Service();
                bool sonuc, sonuc2;
                string prog = Application.ProductName;
                string bil = Environment.MachineName;
                string versiyon = Application.ProductVersion;
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    versiyon += " | " + ApplicationDeployment.CurrentDeployment.CurrentVersion;
                }
                d.beni_kaydet(prog, bil, versiyon,out sonuc, out sonuc2);
            }
            catch (Exception ex)
            {


                olaylar.logyaz(TipGetir().FullName + " | " + MethodBase.GetCurrentMethod().Name + " >> " + ex.Message);

            }



        }

    }
}
