using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeckinGKafe.UI
{
    // masaların taşınması için bir event oluşturmamız gerekiyor bu yüzden
    // BİRİNCİ ADIM OLARAK!!!! 
    //event ile ilgili argümanları taşıyacak tür(class) EventArgs'dan miras alınarak oluşturulur.

    public class MasaTasindiEventArgs : EventArgs
    {
        public int EskiMasaNo { get; private set; }
        public int YeniMasaNo { get; private set; }

        public MasaTasindiEventArgs(int eskiMasaNo,int yeniMasNo)
        {
            EskiMasaNo = eskiMasaNo;
            YeniMasaNo = yeniMasNo;
        }

    }
}
