using SeckinGKafe.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeckinGKafe.UI
{
    public partial class AnaForm : Form
    {
        KafeVeri db = new KafeVeri();

        public AnaForm()
        {
            VerileriOku();
            InitializeComponent();
            Icon = Resource.king_128_44159;
            masalarImageList.Images.Add("bos", Resource.dinning_table);
            masalarImageList.Images.Add("dolu", Resource.third_party);
            MasalariOlustur();
            //daha sonra kaldırılmak üzere örnek ürün ekleyelim.
            
            
        }

        private void VerileriOku()
        {
            // verileri oku ve deserialize et...
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonSerializer.Deserialize<KafeVeri>(json);
            }

            //Eğer hata alırsan dosya yoktur yada bozulmuştur.
            catch (Exception)
            {
                db = new KafeVeri();
                OrnekUrunOlustur();
            }
        }
        private void OrnekUrunOlustur()
        {
            db.Urunler.Add(new Urun() { UrunAd = "Çay", BirimFiyat = 4.00m });
            db.Urunler.Add(new Urun() { UrunAd = "Kahve", BirimFiyat = 4.00m });
            db.Urunler.Add(new Urun() { UrunAd = "Poğaça", BirimFiyat = 4.00m });
            db.Urunler.Add(new Urun() { UrunAd = "Simit", BirimFiyat = 4.00m });
        }
        private void MasalariOlustur()
        {
            ListViewItem lvi;

            for (int i = 1; i <= db.MasaAdet; i++)
            {
                lvi = new ListViewItem();
                lvi.Tag = i;
                lvi.Text = "Masa" + i;
                lvi.ImageKey = MasaDoluMu(i) ? "dolu" : "bos";
                lvwMasalar.Items.Add(lvi);
            }

        }

        private bool MasaDoluMu(int masaNo)
        {
            return db.AktifSiparisler.Any(x => x.MasaNo == masaNo);
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem lvi = lvwMasalar.SelectedItems[0];
            int masaNo = (int)lvi.Tag;
            lvi.ImageKey = "dolu";
            // masada siparis yoksa siparis oluştursun.
            Siparis siparis = SiparisBul(masaNo);

            if (siparis==null)
            {
                siparis = new Siparis() { MasaNo = masaNo };
                db.AktifSiparisler.Add(siparis);
            }
            // Bu siparişi başka bir formda aç

            SiparisForm siparisForm = new SiparisForm(db,siparis);

            // EVENT OLUŞTURMADA 5. ADIM: Event'e oluşturulan metotu atamak

            siparisForm.MasaTasindi += SiparisForm_MasaTasindi;

            siparisForm.ShowDialog();

            if (siparis.Durum !=SiparisDurum.Aktif)
            {
                lvi.ImageKey = "bos";
            }

        }
        // EVENT OLUŞTURMADA 4. ADIM: Event'e atanacak metotu 
        // event delegesinin dönüş tipi ve argüman çeşitlerine uygun olarak oluşturmak
        private void SiparisForm_MasaTasindi(object sender, MasaTasindiEventArgs e)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                int masaNo = (int)lvi.Tag;
                if (masaNo == e.EskiMasaNo)
                {
                    lvi.ImageKey = "bos";
                }
                else if (masaNo == e.YeniMasaNo)
                {
                    lvi.ImageKey = "dolu";
                }

            }
        }

        private Siparis SiparisBul(int masaNo)
        {
            //linq metodu ==> return db.AktifSiparisler.FirstOrDefault(x=>x.MasaNo==masaNo);

            foreach (var item in db.AktifSiparisler)
            {
                if (item.MasaNo==masaNo)
                {
                    return item;
                }
            }
            return null;

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == tsmiUrunler)
            {
                new UrunlerForm(db).ShowDialog();
            }
            else if (e.ClickedItem == tsmiGecmisSiparisler)
            {
                new GecmisSiparislerForm(db).ShowDialog();
            }
        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            VerileriKaydet();
        }

        private void VerileriKaydet()
        {
            //json'ın verileri okunaklı(identation) sekilde kaydetmesini sağlar.
            var options = new JsonSerializerOptions() { WriteIndented = true };

            string json = JsonSerializer.Serialize(db, options);
            File.WriteAllText("veri.json", json);
        }
    }
}
