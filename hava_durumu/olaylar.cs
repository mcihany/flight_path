using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
namespace Ucus_Yolu
{
    static class olaylar
    {

        private static string dosya_adres = dosya.OlaylarTxt;
        public static void logyaz(string veri)
        {
            ParameterizedThreadStart pts = new ParameterizedThreadStart(yeni_islem_log_yaz);
            Thread t = new Thread(pts);
            t.Start(veri);
        }
        static private object kilit = new object();
        static private void yeni_islem_log_yaz(object veri)
        {
            lock (kilit)
            {

                try
                {
                    if (!File.Exists(dosya_adres))
                    {
                        File.Create(dosya_adres).Close();

                    }
                    string zaman = Convert.ToString(DateTime.Now);
                    if (string.IsNullOrEmpty(veri.ToString()))
                    {
                        veri = "Hata Mesajı Yok!";
                    }
                    File.AppendAllText(dosya_adres, "\r\n" + zaman + " >> " + veri);
                  
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Olay Dosyası İşlemlerinde Hata Meydana Geldi.Hata: " + ex.Message,"Log Hatası",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                }
            }
        }

    }
}
