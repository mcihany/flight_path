using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Net;
using System.Collections;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Deployment.Application;
using System.Net.Cache;

namespace Ucus_Yolu
{

    public partial class Form1 : Form
    {


        public static string[] icaos = { };
        public static string yazdirilacak = null;
        dosya dossya;
        private static RequestCachePolicy rcp = new RequestCachePolicy(cache_secenek_ver());

        public Form1()
        {
            InitializeComponent();

            dossya = new dosya();
            tampon_bellek.basla();
            burdayim.BeniKaydet();
            ilk_durum_kontrol_timer.Start();
        }


        private static Int32 max_satir_karakter_sayi = 90;
        private void Form1_Load(object sender, EventArgs e)
        {
            ulkecomboyukle(string.Empty);
            ulkeListGoster();
            yazici_list_yenile(true);
            tab1ulkelist.Select();
            CheckForIllegalCrossThreadCalls = true;
            hemen_yazdir_durum_yukle();
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                versiyon_lbl.Text = "Versiyon: " + ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            icaoEkle();
        }

        private bool icaokontrol(string veri)
        {

            veri = veri.Trim();

            for (Int32 i = 0; i < veri.Length; i++)
            {
                if ((!char.IsLetter(veri[i]) && (i + 1) % 5 != 0) || (veri.Length < 4) || ((veri.Length + 1) % 5 != 0) || (((i + 1) % 5 == 0) &&
                    (!char.IsWhiteSpace(veri[i]))))
                {
                    MessageBox.Show("İcao Formatı Yanlış\nÖrnek: LTBS", "Yanlış Format", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

            }

            return true;

        }

        private Stack icaostackver(string veri)
        {
            Stack icaostack = new Stack();
            veri = veri.ToUpper();
            for (Int32 i = 0; i < veri.Length; i++)
            {
                if ((i + 2) % 5 == 0)
                {
                    icaostack.Push(veri.Substring(i - 3, 4));

                }
            }

            return icaostack;
        }

        private void icaoEkle()
        {
            //havalimanı icao ekleme
            string ulke = comboBox1.Text;
            string icao = textBox1.Text;
            Stack icaostack = icaostackver(icao);
            if (!string.IsNullOrEmpty(icao) && !string.IsNullOrEmpty(ulke))
            {
                if (icaokontrol(icao))
                {

                    if (ulkevarmi(dosya.XmlIcao, ulke) != -1)
                    {

                        Int32 icaosayi = icaostack.Count;
                        for (Int32 m = 0; m < icaosayi; m++)
                        {
                            XmlDocument xmldosya = new XmlDocument();
                            if (System.IO.File.Exists(dosya.XmlIcao))
                            {
                                xmldosya.Load(dosya.XmlIcao);
                                XmlNode ad = xmldosya.CreateNode(XmlNodeType.Element, "icao", "");
                                icao = Convert.ToString(icaostack.Pop());

                                if (icaovarmi(ulke, icao.ToUpper()) == false)
                                {

                                    for (Int32 i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                                    {
                                        if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].LocalName == ulke)
                                        {

                                            ad.InnerText = icao.ToUpper();
                                            XmlElement eleman = xmldosya.DocumentElement;
                                            eleman.ChildNodes[0].ChildNodes[i].AppendChild(ad);
                                            xmldosya.Save(dosya.XmlIcao);
                                            textBox1.Clear();
                                            textBox1.Focus();
                                            icaoListGoster();
                                        }

                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Bu İcao Zaten Var! >> " + icao, "Kayıtlı İcao", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                            xmldosya = null;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Bu Ülke Daha Önce Eklenmemiş.Önce Ülkeyi Ekleyiniz!", "Bu Ülke Kayıtlı Değil", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    }
                }

            }
            else
            {
                MessageBox.Show("Seçilmesi veya girilmesi gereken veriler var!", "Eksik Veri", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            icaostack = null;

        }


        private bool icaovarmi(string ulke, string icao)
        {
            XmlDocument xmldosya = new XmlDocument();
            if (System.IO.File.Exists(dosya.XmlIcao))
            {
                xmldosya.Load(dosya.XmlIcao);
                for (Int32 i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                {
                    if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].LocalName == ulke)
                    {
                        for (Int32 j = 0; j < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes.Count; j++)
                        {
                            if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j].InnerText == icao)
                            {
                                return true;
                            }

                        }

                    }

                }
            }
            xmldosya = null;

            return false;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            ulkeEkle();

        }

        private string boslukDoldur(string veri)
        {
            for (Int32 i = 0; i < veri.Length; i++)
            {
                if (Convert.ToChar(veri[i]) == Convert.ToChar(char.ConvertFromUtf32(32)))
                {
                    veri = veri.Replace(char.ConvertFromUtf32(32), "_");

                }

            }
            return veri;
        }

        private void ulkeEkle()
        {
            //ülke ekleme

            string ulke = boslukDoldur(textBox2.Text);
            if (!string.IsNullOrEmpty(ulke))
            {
                XmlDocument xmldosya = new XmlDocument();
                if (System.IO.File.Exists(dosya.XmlIcao))
                {
                    xmldosya.Load(dosya.XmlIcao);
                    XmlNode ad = xmldosya.CreateNode(XmlNodeType.Element, ulke, "");
                    if (ulkevarmi(dosya.XmlIcao, ulke) == -1)
                    {
                        XmlElement eleman = xmldosya.DocumentElement;
                        eleman.ChildNodes[0].AppendChild(ad);
                        xmldosya.Save(dosya.XmlIcao);
                        ulkecomboyukle(string.Empty);
                        ulkeListGoster();
                        textBox2.Clear();
                        textBox2.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Bu Ülke Zaten Var!", "Kayıtlı Ülke", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
                xmldosya = null;

            }
            else
            {
                MessageBox.Show("Lütfen Herhangi Bir Ülke Adı Giriniz!", "Ülke Adı Girilmemiş", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }


        private Int32 ulkevarmi(string adres, string veri)
        {
            XmlDocument xmldosya = new XmlDocument();
            if (System.IO.File.Exists(adres))
            {
                xmldosya.Load(adres);
                for (Int32 i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                {
                    if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].LocalName == veri)
                    {
                        return i;
                    }
                }
            }

            xmldosya = null;
            return -1;
        }

        private void ulkecomboyukle(string secilmis_ulke)
        {


            comboBox1.Items.Clear();
            tab1ulkelist.Items.Clear();
            XmlDocument xmldosya = new XmlDocument();
            if (System.IO.File.Exists(dosya.XmlIcao))
            {
                xmldosya.Load(dosya.XmlIcao);
                string icao;
                XmlDocument siralanmis_xmldosya = new XmlDocument();
                if (System.IO.File.Exists(dosya.SiraliXmlIcao))
                {
                    siralanmis_xmldosya.Load(dosya.SiraliXmlIcao);
                    string[] siralanmis = icao_sirala(siralanmis_xmldosya);

                    for (Int32 j = 0; j < siralanmis.Length; j++)
                    {
                        bool eklenecekmi = false;
                        for (Int32 h = 0; h < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; h++)
                        {
                            if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[h].LocalName == siralanmis[j])
                            {
                                eklenecekmi = true;
                                break;
                            }
                        }
                        if (eklenecekmi == true)
                        {
                            tab1ulkelist.Items.Add(siralanmis[j]);
                        }



                    }
                    for (Int32 i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                    {
                        icao = Convert.ToString(xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].LocalName);
                        comboBox1.Items.Add(icao);
                        bool varmi = false;
                        for (Int32 j = 0; j < siralanmis.Length; j++)
                        {
                            if (siralanmis[j] == icao)
                            {
                                varmi = true;
                                break;
                            }

                        }
                        if (varmi == false)
                        {
                            tab1ulkelist.Items.Add(icao);
                        }


                    }
                }


                if (comboBox1.Items.Count != 0)
                {
                    comboBox1.Sorted = true;
                    comboBox1.SelectedIndex = 0;
                }

                if (tab1ulkelist.Items.Count != 0)
                {
                    if (string.IsNullOrEmpty(secilmis_ulke))
                    {
                        tab1ulkelist.SetSelected(0, true);
                    }
                    else
                    {
                        for (Int32 list_sayi = 0; list_sayi < tab1ulkelist.Items.Count; list_sayi++)
                        {
                            if (Convert.ToString(tab1ulkelist.Items[list_sayi]) == secilmis_ulke)
                            {
                                tab1ulkelist.SetSelected(list_sayi, true);
                                break;
                            }

                        }

                    }


                }
                siralanmis_xmldosya = null;
            }

            xmldosya = null;

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                icaoEkle();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                ulkeEkle();

            }

        }

        private void ulkeListGoster()
        {
            listBox1.Items.Clear();
            XmlDocument xmldosya = new XmlDocument();
            if (System.IO.File.Exists(dosya.XmlIcao))
            {

                xmldosya.Load(dosya.XmlIcao);
                for (Int32 i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                {
                    listBox1.Items.Add(Convert.ToString(xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].LocalName));
                }

                if (comboBox1.Items.Count != 0)
                {
                    comboBox1.Sorted = true;
                    comboBox1.SelectedIndex = 0;
                }
            }
            xmldosya = null;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            icaoListGoster();

        }

        private void icaoListGoster()
        {
            if (listBox1.SelectedIndex != -1)
            {
                XmlDocument xmldosya = new XmlDocument();
                if (System.IO.File.Exists(dosya.XmlIcao))
                {
                    xmldosya.Load(dosya.XmlIcao);
                    for (Int32 i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                    {
                        if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].LocalName == Convert.ToString(listBox1.SelectedItem))
                        {
                            listBox2.Items.Clear();
                            for (Int32 j = 0; j < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes.Count; j++)
                            {

                                listBox2.Items.Add(xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j].InnerText);

                            }

                        }

                    }

                }
                xmldosya = null;
            }

        }
        private void ulke_sil()
        {
            //ülke sil
            DialogResult dialogsonuc;
            XmlDocument xmldosya;

            if (listBox1.SelectedIndex != -1)
            {
                dialogsonuc = MessageBox.Show(listBox1.SelectedItem + " Silinecek, Eminmisiniz?", "Silme Onay", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dialogsonuc == DialogResult.OK)
                {
                    xmldosya = new XmlDocument();
                    if (System.IO.File.Exists(dosya.XmlIcao))
                    {

                        xmldosya.Load(dosya.XmlIcao);

                        string ulke = Convert.ToString(listBox1.SelectedItem);
                        XmlNode xmlulke;

                        for (Int32 i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                        {
                            if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].LocalName == Convert.ToString(listBox1.SelectedItem))
                            {
                                xmlulke = xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i];
                                xmldosya.ChildNodes[1].ChildNodes[0].RemoveChild(xmlulke);
                                xmldosya.Save(dosya.XmlIcao);
                                ulkecomboyukle(string.Empty);
                                ulkeListGoster();
                            }

                        }

                    }
                    xmldosya = null;
                }
            }

        }
        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ulke_sil();

        }
        private void icao_sil()
        {
            DialogResult dialogsonuc;

            if (listBox2.SelectedIndex != -1)
            {
                dialogsonuc = MessageBox.Show(listBox2.SelectedItem + " Silinecek, Eminmisiniz?", "Silme Onay", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dialogsonuc == DialogResult.OK)
                {
                    XmlDocument xmldosya = new XmlDocument();
                    if (System.IO.File.Exists(dosya.XmlIcao))
                    {
                        xmldosya.Load(dosya.XmlIcao);
                        XmlNode xmlulke;
                        for (Int32 i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                        {
                            if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].LocalName == Convert.ToString(listBox1.SelectedItem))
                            {
                                for (Int32 j = 0; j < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes.Count; j++)
                                {

                                    if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j].InnerXml == Convert.ToString(listBox2.SelectedItem))
                                    {

                                        xmlulke = xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j];
                                        xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].RemoveChild(xmlulke);
                                        xmldosya.Save(dosya.XmlIcao);
                                        icaoListGoster();
                                    }

                                }
                            }

                        }

                    }
                    xmldosya = null;
                }
            }
        }
        private void silToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            icao_sil();
        }


        private string[] ulke_icaolist_getir(string ulke)
        {

            string[] icaolist = { };
            Int32 icaosayi;
            Int32 i;
            XmlDocument xmldosya = new XmlDocument();
            bool ulke_bulundu = false;
            if (System.IO.File.Exists(dosya.XmlIcao))
            {
                xmldosya.Load(dosya.XmlIcao);
                // string secilenulke = Convert.ToString(web1icaolistcombo.SelectedItem);
                for (i = 0; i < xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count; i++)
                {
                    if (xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].LocalName == ulke)
                    {


                        ulke_bulundu = true;
                        break;
                    }

                }
                if (ulke_bulundu == true)
                {
                    icaosayi = xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes.Count;
                    icaolist = new string[icaosayi + icaos.Length + 1];

                    Int32 k;
                    Int32 icsayi = icaos.Length;
                    for (Int32 c = 0; c < icaos.Length; c++)
                    {
                        for (Int32 m = 0; m < icaos.Length; m++)
                        {
                            if (icaos[c] == icaos[m] && c > m)
                            {
                                icsayi--;
                                break;

                            }

                        }
                    }


                    icaolist[0] = Convert.ToString(icsayi);
                    for (k = 1; k < icaos.Length + 1; k++)
                    {

                        icaolist[k] = Convert.ToString(icaos[k - 1]);
                    }

                    for (Int32 j = 0; j < icaosayi; j++)
                    {
                        icaolist[k + j] = xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[j].InnerText;

                    }

                    for (Int32 m = 0; m < icaolist.Length; m++)
                    {
                        for (Int32 c = 0; c < icaolist.Length; c++)
                        {
                            if (icaolist[m] == icaolist[c] && m > c)
                            {
                                icaolist[m] = null;
                            }
                        }
                    }
                }

            }
            xmldosya = null;
            return icaolist;
        }

        private void tab1arabuton1_Click(object sender, EventArgs e)
        {
            tab1arabuton1.Text = "Bekleyiniz";
            tab1arabuton1.Enabled = false;
            veri_ara();


        }




        private string[] icao_sirala(XmlDocument xmldosya)
        {
            string[] siralanmis;
            Int32 ulke_sayi = xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes.Count;
            Int32[] sayiya_gore_sira = new Int32[ulke_sayi];
            DateTime[] tarihe_gore_sira = new DateTime[ulke_sayi];

            for (Int32 i = 0; i < ulke_sayi; i++)
            {
                sayiya_gore_sira[i] = Convert.ToInt32(xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[0].InnerText);

                tarihe_gore_sira[i] = Convert.ToDateTime(xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[i].ChildNodes[1].InnerText);
            }

            string[,] dizi = new string[ulke_sayi, 2];

            for (Int32 m = 0; m < ulke_sayi; m++)
            {
                for (Int32 k = 0; k < ulke_sayi; k++)
                {
                    if (tarihe_gore_sira[m] == Convert.ToDateTime(xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[k].ChildNodes[1].InnerText))
                    {
                        string ulke = xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[k].LocalName;
                        Int32 sayi = Convert.ToInt32(xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[k].ChildNodes[0].InnerText);
                        DateTime tarih = Convert.ToDateTime(xmldosya.ChildNodes[1].ChildNodes[0].ChildNodes[k].ChildNodes[1].InnerText);
                        TimeSpan hh = DateTime.UtcNow - tarih;
                        double fark = sayi;
                        if (hh.Days != 0)
                        {
                            fark = Convert.ToDouble(sayi) / hh.Days;
                        }



                        for (Int32 c = 0; c < dizi.Length / 2; c++)
                        {
                            if (dizi[c, 0] == null && dizi[c, 1] == null)
                            {
                                dizi[c, 0] = ulke;

                                dizi[c, 1] = Convert.ToString(fark);

                                break;
                            }


                        }

                        break;
                    }


                }





            }

            double gecici_sayi = 0;
            string gecici_ulke = null;
            for (Int32 i = 0; i < dizi.Length / 2; i++)
            {
                for (Int32 k = 0; k < dizi.Length / 2; k++)
                {

                    if ((Convert.ToDouble(dizi[i, 1]) > Convert.ToDouble(dizi[k, 1])))
                    {
                        gecici_sayi = Convert.ToDouble(dizi[i, 1]);
                        gecici_ulke = dizi[i, 0];

                        dizi[i, 1] = dizi[k, 1];
                        dizi[i, 0] = dizi[k, 0];

                        dizi[k, 1] = Convert.ToString(gecici_sayi);
                        dizi[k, 0] = Convert.ToString(gecici_ulke);


                    }

                }







            }


            siralanmis = new string[dizi.Length / 2];

            for (Int32 i = 0; i < dizi.Length / 2; i++)
            {
                siralanmis[i] = dizi[i, 0];

            }


            return siralanmis;

        }

        private void siralanmis_icao_ekle(string ulke)
        {

            XmlDocument xmldosya = new XmlDocument();
            if (System.IO.File.Exists(dosya.SiraliXmlIcao))
            {
                xmldosya.Load(dosya.SiraliXmlIcao);
                XmlElement eleman = xmldosya.DocumentElement;
                Int32 ulke_yer = ulkevarmi(dosya.SiraliXmlIcao, ulke);
                if (ulke_yer == -1)
                {



                    XmlNode ad = xmldosya.CreateNode(XmlNodeType.Element, ulke, "");


                    XmlNode sayi = xmldosya.CreateNode(XmlNodeType.Element, "sayi", "");
                    sayi.InnerText = Convert.ToString(1);
                    XmlNode tarih = xmldosya.CreateNode(XmlNodeType.Element, "tarih", "");
                    tarih.InnerText = Convert.ToString(DateTime.UtcNow);
                    eleman.ChildNodes[0].AppendChild(ad).AppendChild(sayi);
                    eleman.ChildNodes[0].AppendChild(ad).AppendChild(tarih);
                    xmldosya.Save(dosya.SiraliXmlIcao);

                }
                else
                {
                    Int32 indirilme_sayi = Convert.ToInt32(eleman.ChildNodes[0].ChildNodes[ulke_yer].ChildNodes[0].InnerText);
                    indirilme_sayi++;
                    eleman.ChildNodes[0].ChildNodes[ulke_yer].ChildNodes[0].InnerText = Convert.ToString(indirilme_sayi);
                    eleman.ChildNodes[0].ChildNodes[ulke_yer].ChildNodes[1].InnerText = Convert.ToString(DateTime.UtcNow);
                    xmldosya.Save(dosya.SiraliXmlIcao);

                }

            }

            xmldosya = null;
        }

        private string esittir_kontrol(string veri)
        {

            for (Int32 i = 0; i < veri.Length; i++)
            {
                if (veri[i] == Convert.ToChar("="))
                {
                    veri = veri.Insert(i, "\n").Remove(i + 1, 1);
                    veri = veri.Insert(i + 1, "   ");
                }


            }

            return veri;

        }
        private static List<indirilen> indirilenler;
        private static string indirilen_ulke = null;
        private Stopwatch sure_olcer;
        private static bool aranabilirmi;
        private void veri_ara()
        {

            if (aranabilirmi)
            {
                // string metar_adres, kisa_taf_adres, uzun_taf_adres;
                string ulke = Convert.ToString(tab1ulkelist.SelectedItem);
                indirilen_ulke = ulke;
                string[] icaos = ulke_icaolist_getir(ulke);
                if (icaos.Length > 0)
                {
                    string adres1metar, adres1uzuntaf, adres1kisataf;
                    richTextBox1.Clear();
                    indirilenler = new List<indirilen>();
                    tab1progresslabel1.Text = "% 0";
                    indirilen_ulke_label.Text = "Veri İndiriliyor Lütfen Bekleyiniz...";
                    UInt16 null_olmayan_icao_sayac = 0;

                    timer1.Stop();

                    for (Int32 i = 1; i < icaos.Length; i++)
                    {
                        if (icaos[i] != null && !string.IsNullOrEmpty(icaos[i]))
                        {
                            using (indirilen veri = new indirilen())
                            {
                                veri.Icao = icaos[i];
                                veri.IndirilmeIndeks = (null_olmayan_icao_sayac);
                                null_olmayan_icao_sayac++;

                                adres1metar = sabitler.WebUrlMetar + icaos[i] + ".TXT";
                                adres1uzuntaf = sabitler.WebUrlUzunTaf + icaos[i] + ".TXT";
                                adres1kisataf = sabitler.WebUrlKisaTaf + icaos[i] + ".TXT";
                                using (indirilen.rasatlar rlar = new indirilen.rasatlar())
                                {
                                    rlar.MetarRasatAdres = adres1metar;
                                    rlar.UzunTafRasatAdres = adres1uzuntaf;
                                    rlar.KisaTafAdres = adres1kisataf;
                                    veri.RasatList = rlar;
                                    indirilenler.Add(veri);
                                }
                            }

                        }
                    }
                    rich_doldur.Start();
                    sure_olcer = Stopwatch.StartNew();


                    progressBar1.Maximum = indirilenler.Count;
                    progressBar1.Value = 0;


                    for (UInt16 d = 0; d < indirilenler.Count; d++)
                    {

                        if (indirilenler[d] != null)
                        {
                            using (indirilen r = indirilenler[d])
                            {
                                int workerThreads;
                                int portThreads;
                                ThreadPool.GetMaxThreads(out workerThreads, out portThreads);
                                ThreadPool.SetMaxThreads(workerThreads + 3, portThreads + 3);

                                ParameterizedThreadStart TMetar = new ParameterizedThreadStart(ayri_islem_parca_veri_indir_metar);
                                ParameterizedThreadStart TKisaTaf = new ParameterizedThreadStart(ayri_islem_parca_veri_indir_kisa_taf);
                                ParameterizedThreadStart TUzunTaf = new ParameterizedThreadStart(ayri_islem_parca_veri_indir_uzun_taf);
                                Thread t_metar = new Thread(TMetar);

                                Thread t_kisa_taf = new Thread(TKisaTaf);
                                Thread t_uzun_taf = new Thread(TUzunTaf);

                                t_uzun_taf.Start(r.Icao);
                                t_metar.Start(r.Icao);
                                t_kisa_taf.Start(r.Icao);
                            }


                        }

                    }


                    siralanmis_icao_ekle(ulke);

                }
                else
                {
                    MessageBox.Show("Yanlış giden bir şeyler var.Ülke seçmeyi deneyiniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                tab1ulkelist.Focus();
            }
        }
        static Int32 sayac;

        private void rich_goster()
        {
            if (indirilenler != null)
            {




                for (UInt16 i = 0; i < indirilenler.Count; i++)
                {
                    if (indirilenler[i] != null)
                    {
                        using (indirilen r = indirilenler[i])
                        {
                            if (sayac == r.IndirilmeIndeks)
                            {
                                if (r.Yuklendimi == true && r.Listelendimi == false)
                                {
                                    /*
                                    int sure = 1000;
                                    Random rnd = new Random(DateTime.Now.Millisecond);
                                    sure = rnd.Next(3000, 15000);
                                    Thread.Sleep(sure);
                                     * */
                                    Int32 icao_rich_yer = tab1richbox_1.Text.IndexOf(r.Icao);
                                    r.Listelendimi = true;
                                    //richTextBox1.Text += r.IndirilmeIndeks + "\n";
                                    richTextBox1.Text += "-->" + r.RasatList.MetarRasat + "\n";

                                    richTextBox1.Text += string.IsNullOrEmpty(r.RasatList.UzunTafRasat)?null: "-->" + r.RasatList.UzunTafRasat + "\n";

                                    richTextBox1.Text += string.IsNullOrEmpty(r.RasatList.KisaTafRasat)?null: "-->" + r.RasatList.KisaTafRasat+"\n";

                                    richTextBox1.Text += "\n";

                                    indirilen_ulke_label.Text = "İndirilen İcao: " + r.Icao;
                                    sayac++;
                                    progressBar1.Value = sayac;
                                    tab1progresslabel1.Text = "% " + Convert.ToString(((progressBar1.Value * 100) / progressBar1.Maximum));
                                    text_rich_renklendir(icao_rich_yer, r.Icao.Length, Color.DarkGreen);
                                }

                            }
                        }


                    }

                }


                if (sayac == indirilenler.Count)
                {
                    rich_doldur.Stop();
                    // gecen_sure_timer.Stop();
                    string tamamlandi_metin = null;
                    tab1arabuton1.Text = "ARA";
                    tab1arabuton1.Enabled = true;
                    sure_olcer.Stop();
                    TimeSpan s = sure_olcer.Elapsed;

                    double sure = s.TotalMilliseconds / 1000;
                    if (sure <= 20)
                    {
                        tamamlandi_metin = indirilen_ulke + " Hava Raporu " +
                          String.Format("{0:0.#}", sure) + "  Saniyede" + " Tamamlandı.(" + indirilenler.Count.ToString() +
                           "/" + (indirilenler.Count - sayac).ToString() + ")";
                    }
                    else
                    {
                        tamamlandi_metin = indirilen_ulke + " Hava Raporu Tamamlandı.(" + indirilenler.Count.ToString() +
                               "/" + (indirilenler.Count - sayac).ToString() + ")";
                    }

                    indirilen_ulke_label.Text = tamamlandi_metin;
                    string secilen_ulke = Convert.ToString(tab1ulkelist.SelectedItem);
                    ulkecomboyukle(secilen_ulke);

                    sayac = 0;
                    if (Properties.Settings.Default.hemen_yazdir)
                    {
                        metni_yazdir();
                    }
                    else
                    {
                        timer1.Start();
                    }
                    indirilenler = null;
                }
            }


        }




        private void ayri_islem_parca_veri_indir_metar(object icao)
        {

            using (WebClient web1metar = new WebClient())
            {
                web1metar.Proxy = null;
                web1metar.CachePolicy = rcp;
                web1metar.DownloadStringCompleted += new DownloadStringCompletedEventHandler(web1metar_DownloadStringCompleted);
                using (indirilen r = eleman_ver(Convert.ToString(icao)))
                {
                    if (r != null)
                    {
                        if (r.RasatList.MetarYuklendimi == false && !string.IsNullOrEmpty(r.Icao))
                        {
                            try
                            {
                                web1metar.DownloadStringAsync(new Uri(r.RasatList.MetarRasatAdres), r.Icao);
                                web1metar.Dispose();
                            }
                            catch { }

                        }
                    }

                }
            }


        }

        private void ayri_islem_parca_veri_indir_kisa_taf(object icao)
        {

            using (WebClient web1kisataf = new WebClient())
            {
                web1kisataf.Proxy = null;
                web1kisataf.CachePolicy = rcp;
                web1kisataf.DownloadStringCompleted += new DownloadStringCompletedEventHandler(web1kisataf_DownloadStringCompleted);
                using (indirilen r = eleman_ver(Convert.ToString(icao)))
                {
                    if (r != null)
                    {
                        if (r.RasatList.KisaTafYuklendimi == false && !string.IsNullOrEmpty(r.Icao))
                        {
                            try
                            {
                                web1kisataf.DownloadStringAsync(new Uri(r.RasatList.KisaTafAdres), r.Icao);
                                web1kisataf.Dispose();
                            }
                            catch { }

                        }
                    }
                }

            }



        }

        private void ayri_islem_parca_veri_indir_uzun_taf(object icao)
        {
            using (WebClient web1uzuntaf = new WebClient())
            {
                web1uzuntaf.Proxy = null;
                web1uzuntaf.CachePolicy = rcp;
                web1uzuntaf.DownloadStringCompleted += new DownloadStringCompletedEventHandler(web1uzuntaf_DownloadStringCompleted);
                using (indirilen r = eleman_ver(Convert.ToString(icao)))
                {

                    if (r != null)
                    {
                        if (r.RasatList.UzunTafYuklendimi == false && !string.IsNullOrEmpty(r.Icao))
                        {
                            try
                            {
                                web1uzuntaf.DownloadStringAsync(new Uri(r.RasatList.UzunTafRasatAdres), r.Icao);
                                web1uzuntaf.Dispose();
                            }
                            catch { }

                        }
                    }
                }
            }





        }

        private indirilen eleman_ver(string icao)
        {
            if (indirilenler != null)
            {

                for (UInt16 i = 0; i < indirilenler.Count; i++)
                {
                    if (indirilenler[i] != null)
                    {
                        using (indirilen r = indirilenler[i])
                        {
                            if (r.Icao == icao)
                            {
                                return r;
                            }
                        }

                    }

                }
            }


            return null;

        }
        void web1kisataf_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            string icao = e.UserState.ToString();
            indirilen r = null;
            if (indirilenler != null)
            {
                for (UInt16 i = 0; i < indirilenler.Count; i++)
                {
                    if (indirilenler[i] != null)
                    {

                        if (indirilenler[i].Icao == icao)
                        {
                            r = indirilenler[i];

                            break;
                        }

                    }
                }

            }

            if (r != null)
            {
                string rasat = null;
                if (e.Cancelled)
                {
                    rasat = e.UserState.ToString() + " Cancelled";

                }
                else if (e.Error != null)
                {
                    olaylar.logyaz(e.UserState.ToString() + " No Data");
                    rasat = null;
                   // rasat = e.UserState.ToString() + " No Data";
                }
                else if (!string.IsNullOrEmpty(e.Result) && icao != null)
                {
                    rasat = e.Result;

                    Int32 icoa_yer = rasat.IndexOf(icao);
                    if (icoa_yer > -1)
                    {
                        try
                        {
                            DateTime kisa_taf_tarih;
                            kisa_taf_tarih = Convert.ToDateTime(rasat.Substring(0, rasat.IndexOf('\n')));
                            TimeSpan kisa_taf_fark = DateTime.UtcNow.Subtract(kisa_taf_tarih);
                            if (kisa_taf_fark.TotalHours <= 18)
                            {
                                rasat = rasat.Remove(0, rasat.IndexOf(icao)).Replace("\n", "");
                                rasat = bosluk_ye(rasat);
                                rasat = esittir_kontrol(rasat);
                                rasat = yazici_max_karakter_kontrol(rasat, max_satir_karakter_sayi);
                            }
                            else
                            {
                                rasat = null;
                                olaylar.logyaz(icao + " Short Taf Out Of Date");
                                //rasat = icao + " Short Taf Out Of Date";
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        //rasat = e.UserState.ToString() + " Url Broken Error";
                        rasat = null;
                        olaylar.logyaz(e.UserState.ToString() + " Url Broken Error");

                    }


                }
                else if (string.IsNullOrEmpty(e.Result))
                {
                    olaylar.logyaz(e.UserState.ToString() + " No Data");
                    rasat = null;
                   // rasat = e.UserState.ToString() + " No Data";

                }
                if (r.RasatList.KisaTafYuklendimi == false)
                {
                    r.RasatList.KisaTafRasat = rasat;
                    r.RasatList.KisaTafYuklendimi = true;
                    if (r.RasatList.UzunTafYuklendimi == true && r.RasatList.MetarYuklendimi == true)
                    {
                        r.Yuklendimi = true;
                    }
                }

            }



            r.Dispose();

        }

        void web1uzuntaf_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string icao = e.UserState.ToString();
            indirilen r = null;
            if (indirilenler != null)
            {
                for (UInt16 i = 0; i < indirilenler.Count; i++)
                {
                    if (indirilenler[i] != null)
                    {

                        if (indirilenler[i].Icao == icao)
                        {
                            r = indirilenler[i];
                            break;
                        }

                    }
                }

            }

            if (r != null)
            {
                string rasat = null;
                if (e.Cancelled)
                {
                    rasat = e.UserState.ToString() + " Cancelled";
                }
                else if (e.Error != null)
                {
                    olaylar.logyaz(e.UserState.ToString() + " No Data");
                    rasat = null;
                    //rasat = e.UserState.ToString() + " No Data";
                }
                else if (!string.IsNullOrEmpty(e.Result) && icao != null)
                {
                    rasat = e.Result;

                    Int32 icoa_yer = rasat.IndexOf(icao);
                    if (icoa_yer > -1)
                    {
                        try
                        {
                            DateTime taf_tarih = Convert.ToDateTime(rasat.Substring(0, rasat.IndexOf('\n')));

                            TimeSpan fark = DateTime.UtcNow.Subtract(taf_tarih);
                            if (fark.TotalHours <= 36)
                            {
                                rasat = rasat.Remove(0, icoa_yer).Replace("\n", "");
                                rasat = bosluk_ye(rasat);
                                rasat = esittir_kontrol(rasat);
                                rasat = yazici_max_karakter_kontrol(rasat, max_satir_karakter_sayi);

                            }
                            else
                            {

                                olaylar.logyaz(icao + " Long Taf Out Of Date");
                                rasat = null;
                                //rasat = icao + " Long Taf Out Of Date";
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        olaylar.logyaz(e.UserState.ToString() + " Url Broken Error");
                        rasat = null;
                       // rasat = e.UserState.ToString() + " Url Broken Error";

                    }

                }
                else if (string.IsNullOrEmpty(e.Result))
                {
                    olaylar.logyaz(e.UserState.ToString() + " No Data");
                    rasat = null;
                   // rasat = e.UserState.ToString() + " No Data";

                }
                if (r.RasatList.UzunTafYuklendimi == false)
                {
                    r.RasatList.UzunTafRasat = rasat;
                    r.RasatList.UzunTafYuklendimi = true;
                    if (r.RasatList.KisaTafYuklendimi == true && r.RasatList.MetarYuklendimi == true)
                    {
                        r.Yuklendimi = true;
                    }
                }

            }
            r.Dispose();
        }

        void web1metar_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string icao = e.UserState.ToString();
            indirilen r = null;
            if (indirilenler != null)
            {

                for (UInt16 i = 0; i < indirilenler.Count; i++)
                {
                    if (indirilenler[i] != null)
                    {

                        if (indirilenler[i].Icao == icao)
                        {
                            r = indirilenler[i];
                            break;
                        }

                    }
                }
            }

            if (r != null)
            {
                string rasat = null;
                if (e.Cancelled)
                {
                    rasat = e.UserState.ToString() + " Cancelled";
                }
                else if (e.Error != null)
                {
                    rasat = e.UserState.ToString() + " No Data";
                }
                else if (!string.IsNullOrEmpty(e.Result) && icao != null)
                {
                    rasat = e.Result;

                    Int32 icoa_yer = rasat.IndexOf(icao);
                    if (icoa_yer > -1)
                    {
                        try
                        {
                            DateTime metar_tarih = Convert.ToDateTime(rasat.Substring(0, rasat.IndexOf('\n')));

                            TimeSpan fark = DateTime.UtcNow.Subtract(metar_tarih);
                            if (fark.Hours <= 5)
                            {
                                //baştaki gereksiz bilgiler silinir
                                rasat = rasat.Remove(0, icoa_yer).Replace("\n", ""); ;
                                rasat = esittir_kontrol(rasat);
                                //yazıcı için max karakter uygulması yapılır
                                rasat = yazici_max_karakter_kontrol(rasat, max_satir_karakter_sayi);

                            }
                            else
                            {
                                rasat = icao + " Metar Out of Date";
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        rasat = e.UserState.ToString() + " Url Broken  Error";

                    }


                }
                else if (string.IsNullOrEmpty(e.Result))
                {
                    rasat = e.UserState.ToString() + " No Data";

                }
                if (r.RasatList.MetarYuklendimi == false)
                {
                    r.RasatList.MetarRasat = rasat;
                    r.RasatList.MetarYuklendimi = true;
                    if (r.RasatList.UzunTafYuklendimi == true && r.RasatList.KisaTafYuklendimi == true)
                    {
                        r.Yuklendimi = true;
                    }
                }

            }
            r.Dispose();
        }
        private string yazici_max_karakter_kontrol(string veri, Int32 sinir)
        {
            Int32 sayac = 0;
            Int32 isaret = 0;
            Stack isaretlenen = new Stack();

            for (Int32 i = 0; i < veri.Length; i++)
            {
                sayac++;
                if (veri[i] == Convert.ToChar("\n"))
                {
                    sayac = 0;
                }
                if (sayac >= sinir)
                {

                    isaret = i;
                    while (veri[isaret] != ' ')
                    {
                        isaret--;
                        if (veri[isaret] == ' ')
                        {
                            isaretlenen.Push(isaret);
                            sayac = 0;
                        }
                    }
                }

            }
            Int32 sayi = isaretlenen.Count;
            Int32 isaretyeri = 0;
            for (Int32 i = 0; i < sayi; i++)
            {
                isaretyeri = Convert.ToInt32(isaretlenen.Pop());
                veri = veri.Insert(isaretyeri, "\n   ").Remove(isaretyeri + 1, 1);
            }
            isaretlenen = null;
            return veri;
        }

        private string bosluk_ye(string veri)
        {
            Int32 yer = 0;
            Stack isaretlenen = new Stack();
            for (Int32 i = 0; i < veri.Length; i++)
            {
                if (veri[i] == ' ')
                {

                    if (i - 1 == yer)
                    {
                        isaretlenen.Push(i);
                    }
                    yer = i;
                }
            }
            Int32 sayi = isaretlenen.Count;
            for (Int32 i = 0; i < sayi; i++)
            {
                veri = veri.Remove(Convert.ToInt32(isaretlenen.Pop()), 1);
            }
            isaretlenen = null;

            return veri;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //yazdirma butonu
            metni_yazdir();

        }

        private static Int32 sayfa_sayi = 0;
        private void metni_yazdir()
        {
            using (PrintDocument yazdir = new PrintDocument())
            {
                timer1.Stop();
                yazdirilacak = richTextBox1.Text;
                string yazici_adi = null;
                bool yazdirilacakmi = false;
                yazdir.PrintPage += new PrintPageEventHandler(OnPrintDocument);
                sayfa_sayi = 0;
                for (Int32 i = 0; i < yazici_list.Items.Count; i++)
                {
                    if (yazici_list.GetItemChecked(i) == true)
                    {
                        yazici_adi = yazici_list.GetItemText(yazici_list.Items[i]);
                        yazdirilacakmi = true;
                        break;
                    }

                }

                if (yazdirilacakmi == true)
                {
                    using (yazici_ayar yazicilar_ayar_sinif = new yazici_ayar())
                    {
                        Int16 nokta_vurus_mu = -1;

                        ArrayList yazici_ozellikler = yazicilar_ayar_sinif.yazici_ayar_ver(yazici_adi);
                        if (yazici_ozellikler.Count > 0)
                        {
                            for (Int32 h = 0; h < yazici_ozellikler.Count; h++)
                            {
                                string[] satir = (string[])yazici_ozellikler[h];
                                if (satir[0] == "1")
                                {
                                    if (satir[1] == "1")
                                    {
                                        nokta_vurus_mu = 1;
                                    }
                                    else if (satir[1] == "0")
                                    {
                                        nokta_vurus_mu = 0;
                                    }
                                    break;
                                }

                            }

                        }
                        if (nokta_vurus_mu == -1)
                        {

                            MessageBox.Show("Lütfen Yazıcınızın Nokta Vuruşlu Olup Olmadığını Ayarlar Kısmından Seçiniz !", "Yazıcı Özellik Seçiniz", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        }
                        else if (nokta_vurus_mu == 1)
                        {
                            for (Int32 m = 0; m < 24; m++)
                            {
                                yazdirilacak = yazdirilacak + "\n";

                            }
                            try
                            {
                                yazdiriliyor_label.Text = "Yazdirilmaya Başlandi...";
                                RawPrinterHelper.SendStringToPrinter(yazici_adi, yazdirilacak);
                                //MessageBox.Show("Yazıcı: " + yazici_adi + "\n" + "Tip: Nokta Vuruş");
                                yazdiriliyor_label.Text = "Yazdirildi...";
                            }
                            catch
                            {
                                yazdiriliyor_label.Text = "Hata Meydana Geldi.Yazdirilamadi...";
                            }


                        }
                        else if (nokta_vurus_mu == 0)
                        {
                            yazdiriliyor_label.Text = "Yazdirilmaya Başlandi...";
                            yazdir.PrinterSettings.PrinterName = yazici_adi;
                            yazicisatirsayi = 0;
                            try
                            {
                                Application.DoEvents();
                                yazdir.Print();
                                //   MessageBox.Show("Yazıcı: " + yazici_adi + "\n" + "Tip: Normal");
                                yazdiriliyor_label.Text = "Yazdirildi...";
                                yazdirildi_timer.Start();
                            }
                            catch
                            {
                                yazdiriliyor_label.Text = "Hata Meydana Geldi.Yazdirilamadi...";
                            }

                        }


                    }



                }
                else
                {
                    MessageBox.Show("Listeden Bir Yazıcı Seçiniz", "Yazıcı Seçiniz", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
        }
        //yazdir butonunda sıfırlanıyor
        Int32 yazicisatirsayi = 0;
        public void OnPrintDocument(object sender, PrintPageEventArgs e)
        {
            Int32 max_satir_sayi = 63;
            Font font_baslik = new Font("Verdana", 18,FontStyle.Bold);
            Font font_alt = new Font("Verdana", 8);
            sayfa_sayi++;
            using (Font font = new Font("Verdana", 8.5f))
            {

                float leftMargin = e.MarginBounds.Left;
                float topMargin = e.MarginBounds.Top;
                Int32 sayac = 0;
               
                while (richTextBox1.Lines.Length > yazicisatirsayi)
                {
                    sayac++;
                    if (sayac == 1) {
                        e.Graphics.DrawImage(Properties.Resources.meteo_logo, new Point(10, 10));
                        e.Graphics.DrawImage(Properties.Resources.meteo_logo, new Point(710, 10));
                        e.Graphics.DrawString(sabitler.yazici_baslik, font_baslik, Brushes.DarkRed, leftMargin+30, topMargin / 3 + 15);
                        e.Graphics.DrawString("UTC: " + DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToShortTimeString() +"   Page: "+sayfa_sayi.ToString()
                         + "/" + Math.Ceiling((Convert.ToDecimal(richTextBox1.Lines.Length) / max_satir_sayi)).ToString(),
                         new Font("Verdana", 9, FontStyle.Bold), Brushes.DarkRed, leftMargin + 30, (topMargin / 3) + 50);

                        e.Graphics.DrawString(richTextBox1.Lines[yazicisatirsayi], font, Brushes.Black, leftMargin / 3, topMargin/3  + 90);
                    }
                    else if (sayac == max_satir_sayi || yazicisatirsayi == richTextBox1.Lines.Length - 1)
                    {
                        e.Graphics.DrawString(richTextBox1.Lines[yazicisatirsayi], font, Brushes.Black, leftMargin / 3, topMargin  + (sayac * 15)+10);

                        e.Graphics.DrawString(sabitler.yazici_alt, font_alt, Brushes.DarkRed, leftMargin * 2, topMargin  + (sayac * 15)+40);
                    }
                    else
                    {
                        e.Graphics.DrawString(richTextBox1.Lines[yazicisatirsayi], font, Brushes.Black, leftMargin / 3, topMargin  + (sayac * 15)+10);
                    }


                    if (sayac >= max_satir_sayi)
                    {
                        break;
                    }

                    yazicisatirsayi++;
                }

                if (yazicisatirsayi < richTextBox1.Lines.Length)
                {
                    e.HasMorePages = true;
                }
                else
                {

                    e.HasMorePages = false;
                }
            }
            font_baslik.Dispose();
            font_alt.Dispose();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }


        private void yazici_list_yenile(bool belirlenecekmi)
        {
            using (yazici_ayar yazicilar_ayar_sinif = new yazici_ayar())
            {
                try
                {
                    PrinterSettings.StringCollection yazici_liste = PrinterSettings.InstalledPrinters;
                    yazici_list.Items.Clear();
                    ayar_yazici_list.Items.Clear();
                    for (Int32 i = 0; i < yazici_liste.Count; i++)
                    {
                        yazici_list.Items.Add(yazici_liste[i]);
                        ayar_yazici_list.Items.Add(yazici_liste[i]);

                        if (yazici_liste[i] == yazicilar_ayar_sinif.kullanilan_yazici_ad_ver())
                        {
                            yazici_list.SetItemChecked(i, true);
                            //ayar_yazici_list.SetSelected(i, true);
                        }

                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }
            }




        }



        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (yazici_ayar yazicilar_ayar_sinif = new yazici_ayar())
            {
                if (yazici_list.CheckedItems.Count != 1)
                {
                    for (Int32 i = 0; i < yazici_list.Items.Count; i++)
                    {
                        if (yazici_list.GetItemChecked(i) == true && yazici_list.GetSelected(i) == false)
                        {
                            yazici_list.SetItemChecked(i, false);

                        }
                    }



                    Int32 secilen_yazici_index = yazici_list.SelectedIndex;
                    string secilen_yazici = null;
                    if (secilen_yazici_index != -1)
                    {
                        secilen_yazici = yazici_list.Items[secilen_yazici_index].ToString();

                        yazicilar_ayar_sinif.yazicilar_ayar_ad_kaydet(yazici_list.Items[secilen_yazici_index].ToString());
                        if (yazici_list.GetItemChecked(secilen_yazici_index))
                        {


                            yazicilar_ayar_sinif.yazicilar_ayar_kullanilan_yazici_belirle(yazici_list.CheckedItems[0].ToString());

                        }
                        else
                        {

                            yazicilar_ayar_sinif.yazici_ayar_kaydet(2, secilen_yazici, "0");
                        }




                    }

                }

            }

        }



        private void ekmeydanekle()
        {
            Stack icaoliste = null;
            if (icaokontrol(textBox3.Text.Trim()))
            {
                icaoliste = icaostackver(textBox3.Text.Trim());
                Int32 sayi = icaoliste.Count;

                if (icaos.Length == 0)
                {
                    icaos = new String[sayi];
                }
                else
                {
                    Array.Resize(ref icaos, sayi + icaos.Length);
                }


                for (Int32 i = 0; i < sayi; i++)
                {
                    string veri = Convert.ToString(icaoliste.Pop());
                    icaos[icaos.Length - i - 1] = veri;

                }
                textBox3.Clear();
                tab1comboyukle();

            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                ekmeydanekle();
            }

        }

        private void tab1comboyukle()
        {

            string[] ulke_icao = ulke_icaolist_getir(Convert.ToString(tab1ulkelist.SelectedItem));
            tab1richbox_1.Clear();
            for (Int32 i = 1; i < ulke_icao.Length; i++)
            {
                if (ulke_icao[i] != null)
                {
                    tab1richbox_1.Text += ulke_icao[i] + " ";
                }
            }

            Int32 carpimsayi = 0;
            Int32 ekmeydansayi = 0;
            if (ulke_icao != null && ulke_icao.Length != 0)
            {
                ekmeydansayi = Convert.ToInt32(ulke_icao[0]);

            }

            if (ekmeydansayi == 1)
            {
                carpimsayi = 4;

            }
            else if (ekmeydansayi > 1)
            {
                carpimsayi = 5;
            }
            text_rich_renklendir(0, Convert.ToInt32(ekmeydansayi) * carpimsayi, Color.Red);

            ulke_icao = null;
        }
        public void text_rich_renklendir(Int32 secim_baslangic, Int32 secim_uzunluk, Color renk)
        {
            using (Font fnt = new Font("", 9F, FontStyle.Italic, GraphicsUnit.Point))
            {
                try
                {
                    tab1richbox_1.SelectionStart = secim_baslangic;
                    tab1richbox_1.SelectionLength = secim_uzunluk;
                    tab1richbox_1.SelectionFont = fnt;
                    tab1richbox_1.SelectionColor = renk;
                }
                catch { }
            }

        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                tab1comboyukle();

            }
            else if (tabControl1.SelectedIndex == 1)
            {
                cache_Secenek_yukle();


            }
        }

        private void tab1yenilebtn_Click(object sender, EventArgs e)
        {
            icaos = new String[0];

            tab1comboyukle();
        }

        private void tab1eklebtn_Click(object sender, EventArgs e)
        {
            ekmeydanekle();
            textBox3.Focus();
        }

        private void tab1ulkelist_SelectedIndexChanged(object sender, EventArgs e)
        {
            icaos = new String[0];
            tab1comboyukle();
        }

        private void tab1ulkelist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

                veri_ara();

            }
        }
        static bool label_renk = false;
        static UInt16 timer1_sayac = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1_sayac <= 13)
            {
                if (label_renk == true)
                {
                    indirilen_ulke_label.ForeColor = Color.Black;
                    label_renk = false;
                }
                else
                {
                    indirilen_ulke_label.ForeColor = Color.Red;
                    label_renk = true;
                }
                timer1_sayac++;
            }
            else
            {
                timer1.Stop();
                timer1_sayac = 0;
            }



        }

        private void yazdirildi_timer_Tick(object sender, EventArgs e)
        {
            yazdiriliyor_label.Text = "";
            yazdirildi_timer.Stop();
        }



        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Uçuş Yolu Programı Uçakların gideceği yol üzerindeki meydanların\nmetar-taf verilerini elde etmek için yapılmıştır.\nProgramda çoğu ülkülerin yol boyu icao verileri bulunmaktadır.\nİsterdeniz bu bilgileri değiştirebilir ya da yenilerini ekleyebilirsiniz.\nYol Boyu Metar-Taf bilgileri http://www.noaa.gov/ adresinden elde edilmektedir.\nElde ettiğiniz bilgileri yazdırabilirsiniz.\nİletişim: mcyeter@gmail.com", "Hakkında", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ayar_yazici_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (yazici_ayar yazicilar_ayar_sinif = new yazici_ayar())
            {



                string secilen_yazici = null;
                if (ayar_yazici_list.SelectedIndex != -1)
                {
                    secilen_yazici = ayar_yazici_list.SelectedItem.ToString();
                }

                if (!string.IsNullOrEmpty(secilen_yazici))
                {

                    ArrayList yazici_ozellikler = yazicilar_ayar_sinif.yazici_ayar_ver(secilen_yazici);
                    if (yazici_ozellikler.Count > 0)
                    {
                        bool ayar_bulundumu = false;
                        for (Int32 h = 0; h < yazici_ozellikler.Count; h++)
                        {
                            string[] satir = (string[])yazici_ozellikler[h];

                            if (satir[0] == "1" && satir[1] == "1")
                            {
                                ayar_bulundumu = true;
                                ayar_n_v_yazici_r_btn.Checked = true;
                            }
                            else if (satir[0] == "1" && satir[1] == "0")
                            {
                                ayar_bulundumu = true;
                                ayar_n_yazici_r_btn.Checked = true;
                            }



                        }
                        if (!ayar_bulundumu)
                        {
                            ayar_n_v_yazici_r_btn.Checked = false;
                            ayar_n_yazici_r_btn.Checked = false;
                        }

                    }
                    else
                    {
                        ayar_n_v_yazici_r_btn.Checked = false;
                        ayar_n_yazici_r_btn.Checked = false;

                    }


                }
                else
                {
                    ayar_n_v_yazici_r_btn.Checked = false;
                    ayar_n_yazici_r_btn.Checked = false;
                }
            }
        }

        private void ayar_yazici_kaydet_btn_Click(object sender, EventArgs e)
        {
            using (yazici_ayar yazicilar_ayar_sinif = new yazici_ayar())
            {


                string secilen_yazici = null;
                if (ayar_yazici_list.SelectedIndex != -1)
                {
                    secilen_yazici = ayar_yazici_list.SelectedItem.ToString();
                }

                if (!string.IsNullOrEmpty(secilen_yazici))
                {
                    if (ayar_n_yazici_r_btn.Checked || ayar_n_v_yazici_r_btn.Checked)
                    {
                        string nokta_vurus_mu = null; ;
                        if (ayar_n_v_yazici_r_btn.Checked)
                        {
                            nokta_vurus_mu = "1";
                        }
                        else if (ayar_n_yazici_r_btn.Checked)
                        {
                            nokta_vurus_mu = "0";
                        }
                        yazicilar_ayar_sinif.yazicilar_ayar_ad_kaydet(secilen_yazici);
                        yazicilar_ayar_sinif.yazici_ayar_kaydet(1, secilen_yazici, nokta_vurus_mu);


                    }
                    else
                    {
                        MessageBox.Show("Yazıcınızın Nokta Vuruşlu Olup Olmadığını Seçiniz", "Yazıcı Çeşidini Seçiniz", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }


                }
                else
                {
                    MessageBox.Show("Lütfen Listeden Bir Yazıcı Seçiniz.", "Yazıcı Seçiniz", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void yazdir_chcckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (hemen_yazdir_chcckbox.Checked)
            {
                Properties.Settings.Default.hemen_yazdir = true;
            }
            else
            {
                Properties.Settings.Default.hemen_yazdir = false;
            }
            Properties.Settings.Default.Save();
        }


        private void hemen_yazdir_durum_yukle()
        {
            if (Properties.Settings.Default.hemen_yazdir)
            {
                hemen_yazdir_chcckbox.Checked = true;
            }
            else
            {
                hemen_yazdir_chcckbox.Checked = false;
            }
        }

        private void rich_doldur_Tick(object sender, EventArgs e)
        {

            rich_goster();
        }

        private void ulke_sil_btn_Click(object sender, EventArgs e)
        {
            ulke_sil();
        }

        private void icao_sil_btn_Click(object sender, EventArgs e)
        {
            icao_sil();
        }

        private void burdayim_servis_timer_Tick(object sender, EventArgs e)
        {
            burdayim.BeniKaydet();
        }

        private void ilk_durum_kontrol_timer_Tick(object sender, EventArgs e)
        {
            if (tampon_bellek.ilk_indimi_durum_ver())
            {
                aranabilirmi = true;
                tab1arabuton1.Enabled = true;
                tab1arabuton1.Text = "ARA";
                ilk_durum_kontrol_timer.Stop();
                progressBar1.Value = progressBar1.Maximum;
                tab1progresslabel1.Text = "% 100 Yüklendi";

            }
            else
            {

                aranabilirmi = false;
                tab1arabuton1.Enabled = false;
                tab1arabuton1.Text = "Liste Yükleniyor Bekleyiniz...";
                double toplam_icao = tampon_bellek.IcaoSayi * 3;
                double inen_toplam_icao = tampon_bellek.r.kisa_taf_sayi + tampon_bellek.r.metar_sayi + tampon_bellek.r.uzun_taf_sayi;

                if (inen_toplam_icao < toplam_icao)
                {
                    double deger = 100 * (inen_toplam_icao / toplam_icao);
                    tab1progresslabel1.Text = "% " + Convert.ToString(((progressBar1.Value * 100) / progressBar1.Maximum)) + " Yükleniyor..";
                    progressBar1.Value = Convert.ToInt32(deger);


                }

            }
        }

        private static RequestCacheLevel cache_secenek_ver()
        {

            RequestCacheLevel rcl;

            string ayar = Properties.Settings.Default.cache_secenek;
            switch (ayar)
            {
                case "BypassCache":
                    rcl = RequestCacheLevel.BypassCache;
                    break;
                case "CacheIfAvailable":
                    rcl = RequestCacheLevel.CacheIfAvailable;
                    break;
                case "CacheOnly":
                    rcl = RequestCacheLevel.CacheOnly;
                    break;
                case "Default":
                    rcl = RequestCacheLevel.Default;
                    break;
                case "NoCacheNoStore":
                    rcl = RequestCacheLevel.NoCacheNoStore;
                    break;
                case "Reload":
                    rcl = RequestCacheLevel.Reload;
                    break;
                case "Revalidate":
                    rcl = RequestCacheLevel.Revalidate;
                    break;
                default:
                    rcl = RequestCacheLevel.CacheIfAvailable;
                    break;

            }


            return rcl;
        }

        private void cache_Secenek_yukle()
        {

            RequestCacheLevel rcl = cache_secenek_ver();
            switch (rcl)
            {
                case RequestCacheLevel.BypassCache:
                    radio_BypassCache.Checked = true;
                    break;
                case RequestCacheLevel.CacheIfAvailable:
                    radio_CacheIfAvailable.Checked = true;
                    break;
                case RequestCacheLevel.CacheOnly:
                    radio_CacheOnly.Checked = true;
                    break;
                case RequestCacheLevel.Default:
                    radio_Default.Checked = true;
                    break;
                case RequestCacheLevel.NoCacheNoStore:
                    radio_NoCacheNoStore.Checked = true;
                    break;
                case RequestCacheLevel.Reload:
                    radio_Reload.Checked = true;
                    break;
                case RequestCacheLevel.Revalidate:
                    radio_Revalidate.Checked = true;
                    break;

            }

        }

        private void radio_BypassCache_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_BypassCache.Checked)
            {
                rcp = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                Properties.Settings.Default.cache_secenek = "BypassCache";
                Properties.Settings.Default.Save();
            }
        }

        private void radio_CacheIfAvailable_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_CacheIfAvailable.Checked)
            {
                rcp = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                Properties.Settings.Default.cache_secenek = "CacheIfAvailable";
                Properties.Settings.Default.Save();
            }
        }

        private void radio_CacheOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_CacheOnly.Checked)
            {
                rcp = new RequestCachePolicy(RequestCacheLevel.CacheOnly);
                Properties.Settings.Default.cache_secenek = "CacheOnly";
                Properties.Settings.Default.Save();
            }
        }

        private void radio_Default_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_Default.Checked)
            {
                rcp = new RequestCachePolicy(RequestCacheLevel.Default);
                Properties.Settings.Default.cache_secenek = "Default";
                Properties.Settings.Default.Save();
            }
        }

        private void radio_Reload_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_Reload.Checked)
            {
                rcp = new RequestCachePolicy(RequestCacheLevel.Reload);
                Properties.Settings.Default.cache_secenek = "Reload";
                Properties.Settings.Default.Save();
            }
        }

        private void radio_NoCacheNoStore_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_NoCacheNoStore.Checked)
            {
                rcp = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                Properties.Settings.Default.cache_secenek = "NoCacheNoStore";
                Properties.Settings.Default.Save();
            }
        }

        private void radio_Revalidate_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_Revalidate.Checked)
            {
                rcp = new RequestCachePolicy(RequestCacheLevel.Revalidate);
                Properties.Settings.Default.cache_secenek = "Revalidate";
                Properties.Settings.Default.Save();
            }
        }

       






    }


    public class RawPrinterHelper
    {

        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "Mcy Yazilim Program Dosya";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
    }

}

