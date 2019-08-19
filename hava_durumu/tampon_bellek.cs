using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Reflection;
namespace Ucus_Yolu
{
    class tampon_bellek
    {
       static ThreadStart ts1 = new ThreadStart(metar_liste_indir);
       static Thread t1 = new Thread(ts1);
       static ThreadStart ts2 = new ThreadStart(kisa_taf_liste_indir);
       static Thread t2 = new Thread(ts2);
       static ThreadStart ts3 = new ThreadStart(uzun_taf_liste_indir);
       static Thread t3 = new Thread(ts3);
       public class rasatlar {
           public bool metar { get; set; }
           public bool kisa_taf { get; set; }
           public bool uzun_taf { get; set; }
           public Int32 metar_sayi { get; set; }
           public Int32 kisa_taf_sayi { get; set; }
           public Int32 uzun_taf_sayi { get; set; }
       }
       public static bool ilk_indimi_durum_ver() {
           if (r.kisa_taf && r.metar && r.uzun_taf)
           {
               return true;
           }
           else {
               return false;
           }


       
       }
       public static rasatlar r=new rasatlar();
       public static Boolean DonguDevam
       {
           get;
           set;
       }
       public static void dongu_durdur() {
           DonguDevam = false;
           try { t1.Abort(); }
           catch { }
           try { t2.Abort(); }
           catch { }
           try { t3.Abort(); }
           catch { }
       }
       static public  void dongu_baslat() {
           basla();
       }
       static public void basla() {
           DonguDevam = true;
            icao_list_yukle();
       //    DeleteCache.temizle();
            cache_Temizle();
                t2.Start();
                t3.Start();
                t1.Start();
                
            

       }
       static List<string> icolar = new List<string>();
       public static Int32 IcaoSayi { get { return icolar.Count; } }
       private static void icao_list_yukle()
       {
           
           icolar.Clear();
         
           XmlDocument xmldosya = new XmlDocument();
           if (System.IO.File.Exists(dosya.XmlIcao))
           {
               try
               {
                   xmldosya.Load(dosya.XmlIcao);
                   for (Int32 i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                   {
                       for (Int32 j = 0; j < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes.Count; j++)
                       {
                           listeye_ekle(Convert.ToString(xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j].InnerText));
                           
                       }
                   }
               }
               catch { }
           }
           xmldosya = null;
       }

        private static void listeye_ekle(string icao)
        {
            bool eklenecekmi = true;
            for (Int32 i = 0; i < icolar.Count; i++)
            {
                if (!string.IsNullOrEmpty(icolar[i]) && icolar[i] == icao)
                {
                    eklenecekmi = false;
                    break;
                }
            }
            if (eklenecekmi) {
                icolar.Add(icao);
            }
        }

        static private void metar_liste_indir()
        {
            while (DonguDevam)
            {
                
                if (icolar != null)
                {

                    for (Int32 i = 0; i < icolar.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(icolar[i]))
                        {
                            icao_indir(icolar[i], Rasatlar.Metar);

                        }
                       if(!r.metar) r.metar_sayi++;
                    }

                }
                r.metar = true;

            }

           

        }
        static private void kisa_taf_liste_indir()
        {
            while (DonguDevam)
            {
               
                if (icolar != null)
                {

                    for (Int32 i = 0; i < icolar.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(icolar[i]))
                        {

                            icao_indir(icolar[i], Rasatlar.KisaTaf);

                        }
                       if(!r.kisa_taf) r.kisa_taf_sayi++;
                    }

                }
                r.kisa_taf = true;
            }
           
            

        }
        static private void uzun_taf_liste_indir()
        {

            while (DonguDevam)
            {
               
                if (icolar != null)
                {

                    for (Int32 i = 0; i < icolar.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(icolar[i]))
                        {

                            icao_indir(icolar[i], Rasatlar.UzunTaf);
                        }
                       if(!r.uzun_taf) r.uzun_taf_sayi++;
                    }

                }
                r.uzun_taf = true;
            }
            
        }



        static private void icao_indir(string icao, Rasatlar rasat)
        {

            using (WebClient web_istek = new WebClient())
            {
                web_istek.Proxy = null;
                web_istek.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);
                web_istek.Headers.Add(HttpRequestHeader.CacheControl, "max-age=600");
               
                string adres = null;
                if (rasat == Rasatlar.Metar)
                {
                   
                    adres = sabitler.WebUrlMetar + icao + ".TXT";
                }
                else if (rasat == Rasatlar.KisaTaf)
                {
                    adres = sabitler.WebUrlKisaTaf + icao + ".TXT";
                }
                else if (rasat == Rasatlar.UzunTaf)
                {
                    adres = sabitler.WebUrlUzunTaf + icao + ".TXT";
                }
                if (!string.IsNullOrEmpty(adres))
                {
                    try
                    {
                        
                        web_istek.DownloadString(new Uri(adres));
                    }
                    catch(Exception ex)
                    {
                        //olaylar.logyaz(TipGetir().FullName + " | " + MethodBase.GetCurrentMethod().Name + " >> İcao=" + icao + " Rasat=" + rasat.ToString() + " " + ex.Message);  
                    }
                }
            }
        }
        public static Type TipGetir()
        {
            return (typeof(tampon_bellek));
        } 
       

        public enum Rasatlar {Metar,KisaTaf,UzunTaf,Yok }
        private static void cache_Temizle()
        {
            if (System.Web.HttpContext.Current != null)
            {
                System.Collections.IDictionaryEnumerator MyCache = System.Web.HttpContext.Current.Cache.GetEnumerator();
                while (MyCache.MoveNext())
                {
                    var currentKey = MyCache.Key.ToString();
                    System.Web.HttpContext.Current.Cache.Remove(currentKey);
                }
            }
        }
    }
}
