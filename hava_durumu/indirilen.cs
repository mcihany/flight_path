using System;
using System.Collections.Generic;
using System.Text;

namespace Ucus_Yolu
{
   public class indirilen:IDisposable
    {
       ~indirilen()
           {
               Dispose(false);
           }

           protected virtual void Dispose(bool disposeDurumu)
           {
               if (disposeDurumu == true)
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

       private UInt16 indirilme_indeks;
       public UInt16 IndirilmeIndeks {
           get {
               return indirilme_indeks;
           }
           set {
               indirilme_indeks = value;
           }
       }
        private string icoa;
        public string Icao {
            get {
                return icoa;
            }
            set {

                icoa = value;
            }

        }
        private bool yuklendi_mi=false;
        public bool Yuklendimi {
            get {
                return yuklendi_mi;
            }
            set {
                yuklendi_mi = value;
            }
        }
        private bool listelendi_mi = false;
        public bool Listelendimi
        {
            get
            {
                return listelendi_mi;
            }
            set
            {
                listelendi_mi = value;
            }
        }
        private rasatlar rasat_list;
        public rasatlar RasatList {
            get {
                return rasat_list;
            }
            set {
                rasat_list = value;
            }
        }

       public class rasatlar:IDisposable {
           ~rasatlar()
           {
             
               Dispose(false);
           }

           protected virtual void Dispose(bool disposeDurumu)
           {
               if (disposeDurumu == true)
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


            private string metar_rasat;
            public string MetarRasat {
                get {
                    return metar_rasat;
                }
                set {
                    metar_rasat = value;
                }
            }
            private string metar_rasat_adres;
            public string MetarRasatAdres
            {
                get
                {
                    return metar_rasat_adres;
                }
                set
                {
                    metar_rasat_adres = value;
                }
            }

            private string uzun_taf_rasat;
            public string UzunTafRasat
            {
                get
                {
                    return uzun_taf_rasat;
                }
                set
                {
                    uzun_taf_rasat = value;
                }
            }

            private string uzun_taf_rasat_adres;
            public string UzunTafRasatAdres
            {
                get
                {
                    return uzun_taf_rasat_adres;
                }
                set
                {
                    uzun_taf_rasat_adres = value;
                }
            }
            private string kisa_taf_rasat;
            public string KisaTafRasat
            {
                get
                {
                    return kisa_taf_rasat;
                }
                set
                {
                    kisa_taf_rasat = value;
                }
            }
            private string kisa_taf_adres;
            public string KisaTafAdres
            {
                get
                {
                    return kisa_taf_adres;
                }
                set
                {
                    kisa_taf_adres = value;
                }
            }
            private bool metar_yuklendi_mi = false;
            public bool MetarYuklendimi
            {
                get
                {
                    return metar_yuklendi_mi;
                }
                set
                {
                    metar_yuklendi_mi = value;
                }
            }
            private bool kisa_taf_yuklendi_mi = false;
            public bool KisaTafYuklendimi
            {
                get
                {
                    return kisa_taf_yuklendi_mi;
                }
                set
                {
                    kisa_taf_yuklendi_mi = value;
                }
            }
            private bool uzun_taf_yuklendi_mi = false;
            public bool UzunTafYuklendimi
            {
                get
                {
                    return uzun_taf_yuklendi_mi;
                }
                set
                {
                    uzun_taf_yuklendi_mi = value;
                }
            }
        }

    }
}
