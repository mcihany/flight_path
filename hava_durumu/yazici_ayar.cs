using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Deployment.Application;
using System.IO;

namespace Ucus_Yolu
{
    class yazici_ayar:IDisposable
    {

        static string yazici_ayar_xml_adres;
       static string  yazici_ad_ozellik="yazici_adi";
       static string kullanilan_yazici_ozellik = "kullanimda_mi";
       static string nokta_vurusmu_ozellik = "nokta_vurus_mu";
       ~yazici_ayar()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposeDurumu)
    {
        if(disposeDurumu==true)
        {
            // Managed kaynaklar için Dispose metodu uygulanır.
           
        }
      
        // UnManaged kaynaklar temizlenir.
    }

    #region IDisposable Members

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

       public  yazici_ayar() {
           string d = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MCY\Yol_Boyu\yazici_ayar.xml";
           if (ApplicationDeployment.IsNetworkDeployed)
           {
               if (File.Exists(d))
               {
                   File.Copy(d, ApplicationDeployment.CurrentDeployment.DataDirectory + @"\yazici_ayar.xml", true);
                   File.Delete(d);

               }
               yazici_ayar_xml_adres = ApplicationDeployment.CurrentDeployment.DataDirectory + @"\yazici_ayar.xml";
           }
           else
           {
               if (File.Exists(d)) {
                   yazici_ayar_xml_adres = d;
               }

           }

             
      
           
       }
        public void yazicilar_ayar_ad_kaydet(string yazici_adi)
        {
         
            XmlDocument xml_dosya = new XmlDocument();
           
            if (System.IO.File.Exists(yazici_ayar_xml_adres)) { 
            
            xml_dosya.Load(yazici_ayar_xml_adres);
            bool var_mi = false;

            Int16 i;
            for (i = 0; i < xml_dosya.ChildNodes[1].ChildNodes.Count; i++)
            {
                for (Int16 j = 0; j < xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes.Count; j++)
                {
                    if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].LocalName == yazici_ad_ozellik && xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].InnerText == yazici_adi)
                    {
                        var_mi = true;
                        break;
                    }

                }
                if (var_mi)
                    break;

            }
            if (!var_mi)
            {
                XmlNode yacilar_yazici_alt_eleman = xml_dosya.CreateNode(XmlNodeType.Element, "yazici", "");
                XmlNode yazici_ad_eleman = xml_dosya.CreateNode(XmlNodeType.Element, yazici_ad_ozellik, "");
                XmlNode yazici_kullanimdami_eleman = xml_dosya.CreateNode(XmlNodeType.Element, yazici_ad_ozellik, "");
                XmlElement yazici_document_element = xml_dosya.DocumentElement;
                yazici_document_element.AppendChild(yacilar_yazici_alt_eleman);
                yazici_ad_eleman.InnerText = yazici_adi;
                yazici_document_element.ChildNodes[i].AppendChild(yazici_ad_eleman);
                xml_dosya.Save(yazici_ayar_xml_adres);
               
            }


                
            }

            xml_dosya = null;
           
            
        }


        public bool yazicilar_ayar_kullanilan_yazici_belirle(string yazici_adi)
        {

            XmlDocument xml_dosya = new XmlDocument();
            if (System.IO.File.Exists(yazici_ayar_xml_adres)) { 
                    xml_dosya.Load(yazici_ayar_xml_adres);

            Int16 i;
            for (i = 0; i < xml_dosya.ChildNodes[1].ChildNodes.Count; i++)
            {
                Int32 yazici_ozellik_sayi = xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes.Count;
                for (Int16 j = 0; j < yazici_ozellik_sayi; j++)
                {
                    if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].LocalName == yazici_ad_ozellik)
                    {

                        string kullanilan_deger;
                        if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].InnerText == yazici_adi)
                        {
                            kullanilan_deger = "1";
                        }
                        else
                        {
                            kullanilan_deger = "0";
                        }
                        bool var_mi = false;
                        for (Int16 k = 0; k < yazici_ozellik_sayi; k++)
                        {
                            if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[k].LocalName == kullanilan_yazici_ozellik)
                            {
                                xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[k].InnerText = kullanilan_deger;
                                xml_dosya.Save(yazici_ayar_xml_adres);
                                var_mi = true;
                                break;
                            }
                        }
                        if (!var_mi)
                        {

                            XmlNode yazici_eleman = xml_dosya.CreateNode(XmlNodeType.Element, kullanilan_yazici_ozellik, "");

                            XmlElement yazici_document_element = xml_dosya.DocumentElement;


                            yazici_eleman.InnerText = kullanilan_deger;


                            yazici_document_element.ChildNodes[i].AppendChild(yazici_eleman);


                            xml_dosya.Save(yazici_ayar_xml_adres);
                        }

                    }

                }


            }
            
            }


            xml_dosya = null;
            return true;
        }
        public string kullanilan_yazici_ad_ver()
        {
            string yazici = null;
            XmlDocument xml_dosya = new XmlDocument();
            if (System.IO.File.Exists(yazici_ayar_xml_adres))
            {
             xml_dosya.Load(yazici_ayar_xml_adres);
            bool var_mi = false;

            Int16 i;
            for (i = 0; i < xml_dosya.ChildNodes[1].ChildNodes.Count; i++)
            {
                for (Int16 j = 0; j < xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes.Count; j++)
                {


                    if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].LocalName == kullanilan_yazici_ozellik &&
                        xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].InnerText == "1")
                    {


                        var_mi = true;
                        break;
                    }

                }
                if (var_mi)
                    break;

            }
            if (var_mi)
            {
                for (Int16 m = 0; m < xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes.Count; m++)
                {
                    if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[m].LocalName == yazici_ad_ozellik)
                    {
                        yazici = xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[m].InnerText;
                    }

                }

            }

            }

            xml_dosya = null;
            return yazici;
        }
        public bool yazici_ayar_kaydet(Int16 islem, string yazici_adi, string deger)
        {

            XmlDocument xml_dosya = new XmlDocument();
            if (System.IO.File.Exists(yazici_ayar_xml_adres))
            {
             xml_dosya.Load(yazici_ayar_xml_adres);
            bool var_mi = false;

            Int16 i;
            for (i = 0; i < xml_dosya.ChildNodes[1].ChildNodes.Count; i++)
            {
                for (Int16 j = 0; j < xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes.Count; j++)
                {
                    if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].LocalName == yazici_ad_ozellik &&
                        xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].InnerText == yazici_adi)
                    {


                        var_mi = true;
                        break;
                    }

                }
                if (var_mi)
                    break;

            }
            string ozellik_ad = null;
            if (islem == 1)
            {
                ozellik_ad = nokta_vurusmu_ozellik;
            }
            else if (islem == 2)
            {
                ozellik_ad = kullanilan_yazici_ozellik;
            }
            

            bool ozellik_bulundumu = false;
            if (var_mi)
            {
                Int32 yazici_ozellik_sayi = xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes.Count;
                for (Int16 k = 0; k < yazici_ozellik_sayi; k++)
                {
                    if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[k].LocalName == ozellik_ad)
                    {
                        xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[k].InnerText = deger;
                        xml_dosya.Save(yazici_ayar_xml_adres);
                        ozellik_bulundumu = true;
                        break;
                    }
                }
                if (!ozellik_bulundumu)
                {
                    XmlNode yazici_eleman = xml_dosya.CreateNode(XmlNodeType.Element, ozellik_ad, "");

                    XmlElement yazici_document_element = xml_dosya.DocumentElement;


                    yazici_eleman.InnerText = deger;


                    yazici_document_element.ChildNodes[i].AppendChild(yazici_eleman);


                    xml_dosya.Save(yazici_ayar_xml_adres);

                }

            }

            }


            xml_dosya = null;

            return true;
        }


        public ArrayList yazici_ayar_ver(string yazici)
        {
            ArrayList ayarlar = new ArrayList();
            XmlDocument xml_dosya = new XmlDocument();
            if (System.IO.File.Exists(yazici_ayar_xml_adres))
            {
             xml_dosya.Load(yazici_ayar_xml_adres);
            bool var_mi = false;
          
            Int16 i;
            for (i = 0; i < xml_dosya.ChildNodes[1].ChildNodes.Count; i++)
            {
                for (Int16 j = 0; j < xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes.Count; j++)
                {
                    if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].LocalName == yazici_ad_ozellik &&
                        xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[j].InnerText == yazici)
                    {


                        var_mi = true;
                        break;
                    }

                }
                if (var_mi)
                    break;

            }


            if (var_mi)
            {
                ayarlar = new ArrayList(xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes.Count);

                for (Int16 m = 0; m < xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes.Count; m++)
                {
                    if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[m].LocalName == kullanilan_yazici_ozellik)
                    {
                        ayarlar.Add(new string[] { "0", xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[m].InnerText });
                    }
                    else if (xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[m].LocalName == nokta_vurusmu_ozellik)
                    {

                        ayarlar.Add(new string[] { "1", xml_dosya.ChildNodes[1].ChildNodes[i].ChildNodes[m].InnerText });
                    }
                   

                }

            }

            }

            xml_dosya = null;
            return ayarlar;
        }


    }
}
