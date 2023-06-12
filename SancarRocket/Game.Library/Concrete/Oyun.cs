/*
 * Oğuzhan Yavaş
 * Bilişim Sistemleri Mühendisliği 1. sınıf
 * Nesneye Dayalı Programlama
 * Öğretmen: Muhammed Kotan
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Game.Library.Abstract;
using Game.Library.Enum;
using Timer = System.Windows.Forms.Timer;

namespace Game.Library.Concrete
{
    public class Oyun : IOyun
    {
        #region Fields


        private readonly Timer _kalanSureTimer = new Timer() { Interval = 1000 };
        private readonly Timer _hareketTimer = new Timer() { Interval = 80 };
        private readonly Timer _toplananCisimTimer = new Timer() { Interval = 200 };

        private int kalanZaman;

        private int _yakitSayisi;
        private int _govdeSayisi;
        private int _yazilimSayisi;

        private int _gonderilenRoket;
        private int _kalanRoket;
        private int _uretilecekRoket;
        private int _muhendisSkor;

        private int labelTimer;

        private readonly Panel _oyunPanel;
        private readonly Panel _bilgiPanel;
        private readonly Panel _anaMenuPanel;
        private Muhendis _muhendis;
        private readonly TextBox _oyuncuAdiTextBox;
        private readonly TextBox _oyunSuresiTextBox;
        private readonly TextBox _uretilecekMiktarTextBox;
        private Label _oyunPanelLabel;

        private static readonly Random Random = new Random();

        private readonly List<ToplananCisim> _toplananCisimler = new List<ToplananCisim>();

        #endregion


        #region Events

        public event EventHandler KalanSureDegisti;
        public event EventHandler CisimToplandi;
        public event EventHandler RoketUcuruldu;

        #endregion


        #region Settings

        public bool DevamkeMi { get; private set; }
        public bool OyunDuraklatildiMi { get; set; }
        public int PanelGenisligi { get; private set; }
        public int PanelUzunlugu { get; private set; }
        

        #endregion


        #region Toplanan Cisimler

        public int YakitSayisi
        {
            get => _yakitSayisi;
            private set
            {
                _yakitSayisi = value;
                CisimToplandi?.Invoke(this, EventArgs.Empty);
            }
        }

        public int YazilimSayisi
        {
            get => _yazilimSayisi;
            private set
            {
                _yazilimSayisi = value;
                CisimToplandi?.Invoke(this, EventArgs.Empty);
            }
        }

        public int GovdeSayisi
        {
            get => _govdeSayisi;
            private set
            {
                _govdeSayisi = value;
                CisimToplandi?.Invoke(this, EventArgs.Empty);
            }
        }


        private void ToplananCisimTimer_Tick(object sender, EventArgs e)
        {
            ToplananCisimOlustur();
        }

        private void HareketTimer_Tick(object sender, EventArgs e)
        {
            ToplananCisimleriHareketEttir();
        }


        private void CisimOlustur(ToplananCisim toplananCisim)
        {
            bool konumlarAyniMi;

            konumlarAyniMi = ToplananCisimUstUsteGeliyorMu(toplananCisim);
            if (konumlarAyniMi)
            {
                ToplananCisimOlustur();
                return;
            }

            _toplananCisimler.Add(toplananCisim);
            _oyunPanel.Controls.Add(toplananCisim);
        }

        private void ToplananCisimOlustur()
        {
            int sayi = Random.Next(7);


            if (sayi == 1)
            {
                var govde = new Govde(PanelUzunlugu, PanelGenisligi);
                CisimOlustur(govde);
            }
            else if (sayi >= 2 && sayi <= 3)
            {
                var yakit = new Yakit(PanelUzunlugu, PanelGenisligi);
                CisimOlustur(yakit);
            }
            else if (sayi >= 4 && sayi <= 6)
            {
                var yazilim = new Yazilim(PanelUzunlugu, PanelGenisligi);
                CisimOlustur(yazilim);
            }

            if (KalanSure % 10 == 0)
            {
                var uzayli = new Uzayli(PanelUzunlugu, PanelGenisligi);

                _toplananCisimler.Add(uzayli);
                _oyunPanel.Controls.Add(uzayli);
                KalanSure--;

            }
        }
        private bool ToplananCisimUstUsteGeliyorMu(ToplananCisim toplananCisim)
        {
            foreach (var _toplananCisim in _toplananCisimler)
            {
                if ((_toplananCisim.Top <= toplananCisim.Bottom && _toplananCisim.Left <= toplananCisim.Right)
                    || _toplananCisim.Top <= toplananCisim.Bottom && _toplananCisim.Right >= toplananCisim.Left)
                {
                    return true;
                }
            }

            return false;
        }

        private void ToplananCisimleriHareketEttir()
        {
            OyunHiziniHesapla();

            for (int i = _toplananCisimler.Count - 1; i >= 0; i--)
            {
                if (_toplananCisimler.Count <= 0) return;
                

                var toplananCisim = _toplananCisimler[i];
                toplananCisim.HareketEttir(Yon.Down);

                var yereDustuMu = toplananCisim.YereDustuMu();
                var muhendiseDegdiMi = _muhendis.Left <= toplananCisim.Right && _muhendis.Right >= toplananCisim.Left
                                                                       && toplananCisim.Bottom >=
                                                                       (PanelUzunlugu - _muhendis.Height);

                switch (yereDustuMu)
                {
                    case true:

                        {
                            _toplananCisimler.Remove(toplananCisim);
                            _oyunPanel.Controls.Remove(toplananCisim);
                        }
                        break;

                    case false:
                        if (muhendiseDegdiMi)
                        {
                            if (toplananCisim.GetType().Name == "Yazilim")
                            {
                                YazilimSayisi++;
                                _toplananCisimler.Remove(toplananCisim);
                                _oyunPanel.Controls.Remove(toplananCisim);
                            }
                            else if (toplananCisim.GetType().Name == "Yakit")
                            {
                                YakitSayisi++;
                                _toplananCisimler.Remove(toplananCisim);
                                _oyunPanel.Controls.Remove(toplananCisim);
                            }
                            else if (toplananCisim.GetType().Name == "Govde")
                            {
                                GovdeSayisi++;
                                _toplananCisimler.Remove(toplananCisim);
                                _oyunPanel.Controls.Remove(toplananCisim);
                            }
                            else if (toplananCisim.GetType().Name == "Uzayli")
                            {
                                var sayi = Random.Next(100);

                                _oyunPanelLabel.Visible = true;

                                if (sayi >= 0 && sayi < 50)
                                {
                                    YazilimSayisi += 3;
                                    YakitSayisi += 2;
                                    GovdeSayisi += 1;
                                    _oyunPanelLabel.Text = "Fazladan Bir Roket Daha Ürettiniz.";
                                }
                                else if (sayi >= 50 && sayi < 100)
                                {
                                    if (YazilimSayisi >= 3 && YakitSayisi >= 2 && GovdeSayisi >= 1)
                                    {
                                        YazilimSayisi -= 3;
                                        YakitSayisi -= 2;
                                        GovdeSayisi -= 1;
                                        TamamlananUrun--;
                                        KalanUrun++;
                                        _oyunPanelLabel.Text = "Roketlerinizden Birisi Patladı!";
                                    }
                                    else
                                    {
                                        _oyunPanelLabel.Text = "Herhangi Bir Şey Değişmedi!";
                                    }
                                }

                                _toplananCisimler.Remove(toplananCisim);
                                _oyunPanel.Controls.Remove(toplananCisim);
                            }
                        }

                        UrunHesapla();
                        break;
                }
            }
        }




        #endregion


        #region Timers
        public int KalanSure
        {
            get => kalanZaman;
            private set
            {
                kalanZaman = value;

                KalanSureDegisti?.Invoke(this, EventArgs.Empty);
            }
        }

        private void KalanSureTimer_Tick(object sender, EventArgs e)
        {
            KalanSure--;

            if (_oyunPanelLabel.Visible)
            {
                labelTimer++;
            }

            if (labelTimer > 2)
            {
                _oyunPanelLabel.Visible = false;
                labelTimer = 0;
            }

            if (KalanSure < 5)
            {
                _oyunPanel.Controls.Add(_oyunPanelLabel);
                _oyunPanelLabel.Visible = true;
                _oyunPanelLabel.Text = $"Oyunun Bitmesine Son {KalanSure} Saniye!";
            }
        }

        private void ZamanlayicilariBaslat()
        {
            _kalanSureTimer.Start();
            _toplananCisimTimer.Start();
            _hareketTimer.Start();
        }

        private void ZamanlayicilariDurdur()
        {
            _kalanSureTimer.Stop();
            _toplananCisimTimer.Stop();
            _hareketTimer.Stop();
        }
        #endregion


        #region Urun/Skor
        public int TamamlananUrun
        {
            get => _gonderilenRoket;
            set
            {
                _gonderilenRoket = value;
                RoketUcuruldu?.Invoke(this, EventArgs.Empty);
            }
        }
        public int KalanUrun
        {
            get => _kalanRoket;
            set
            {
                _kalanRoket = value;
                RoketUcuruldu?.Invoke(this, EventArgs.Empty);
            }
        }

        private void UrunHesapla()
        {
            var govdeSayisi = GovdeSayisi;
            var yakitSayisi = YakitSayisi;
            var yazilimSayisi = YazilimSayisi;
            while (govdeSayisi >= 1 && yakitSayisi >= 2 && yazilimSayisi >= 3)
            {

                for (int i = 0; i < TamamlananUrun; i++)
                {
                    govdeSayisi -= 1;
                    yakitSayisi -= 2;
                    yazilimSayisi -= 3;
                }

                if (govdeSayisi >= 1 && yakitSayisi >= 2 && yazilimSayisi >= 3)
                {
                    govdeSayisi -= 1;
                    yakitSayisi -= 2;
                    yazilimSayisi -= 3;
                    TamamlananUrun++;
                    KalanUrun = _uretilecekRoket - TamamlananUrun;
                    if (KalanUrun <= 0)
                    {
                        Done();
                    }
                }
            }
        }
        private void SkorHesapla()
        {
            while (GovdeSayisi >= 1)
            {
                _muhendisSkor += 15;
                GovdeSayisi--;
            }

            while (YakitSayisi >= 1)
            {
                _muhendisSkor += 15;
                YakitSayisi--;
            }

            while (YazilimSayisi >= 1)
            {
                _muhendisSkor += 15;
                YazilimSayisi--;
            }

            while (TamamlananUrun >= 1)
            {
                _muhendisSkor += 100;
                TamamlananUrun--;
            }
        }
        private void SkoruYaz()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (_muhendisSkor > 0)
            {
                using (StreamWriter streamWriter = File.AppendText(path + @"\topfive.txt"))
                {
                    streamWriter.Write($"0◘{_oyuncuAdiTextBox.Text}◘{DateTime.Now}◘{_muhendisSkor}\n");
                }
            }
        }

        #endregion


        #region Panels
        private void airSupportLabelOlustur()
        {
            _oyunPanelLabel = new Label();
            _oyunPanel.Controls.Add(_oyunPanelLabel);
            _oyunPanelLabel.Location = new Point((_oyunPanel.Width - 300) / 2, _oyunPanel.Height-115);
            _oyunPanelLabel.ForeColor = Color.DarkRed;
            _oyunPanelLabel.BackColor = Color.Transparent;
            _oyunPanelLabel.TextAlign = ContentAlignment.TopLeft;
            _oyunPanelLabel.Font = new Font(FontFamily.GenericSansSerif, 18);
            _oyunPanelLabel.AutoSize = true;
        }

        private bool TextBoxlarinDegerleriGecerliMi()
        {
            var sureText = _oyunSuresiTextBox.Text;
            foreach (var harf in sureText)
            {
                if (!char.IsDigit(harf))
                {
                    MessageBox.Show("Zamansız Savaş Mı Olur? Lütfen Savaş Süresi Yaz!");
                    return false;
                }
            }

            var uretilecekMiktarText = _uretilecekMiktarTextBox.Text;
            foreach (var harf in uretilecekMiktarText)
            {
                if (!char.IsDigit(harf))
                {
                    MessageBox.Show("Uçaksız Savaş Olmaz! Lütfen En Azından Bir Jet Gönder.");
                    return false;
                }
            }

            return true;
        }

        public void PanelleriAyarla()
        {
            if (!DevamkeMi)
            {
                _bilgiPanel.Visible = false;
                _anaMenuPanel.Visible = true;
            }
            else if (DevamkeMi)
            {
                _bilgiPanel.Visible = true;
                _anaMenuPanel.Visible = false;
                airSupportLabelOlustur();
            }
        }

        private void OyunPaneliTemizle()
        {
            _oyunPanel.Controls.Clear();
            _oyunPanel.Controls.Add(_anaMenuPanel);
            _oyunPanel.Refresh();
            _toplananCisimler.Clear();
        }

        #endregion


        #region Game
        private void ToplayiciOlustur()
        {
            _muhendis = new Muhendis(PanelUzunlugu, PanelGenisligi);
            _oyunPanel.Controls.Add(_muhendis);
        }
        public void Start()
        {
            if (DevamkeMi) return;

            DevamkeMi = true;
            OyunDuraklatildiMi = false;

            if (TextBoxlarinDegerleriGecerliMi())
            {
                _uretilecekRoket = int.Parse(_uretilecekMiktarTextBox.Text);

                OyunBaslangıcınıAyarla();
                PanelleriAyarla();
                ZamanlayicilariBaslat();
                ToplayiciOlustur();
            }
            else
            {
                Done();
            }
        }

        public void Done()
        {
            if (!DevamkeMi) return;

            DevamkeMi = false;

            ZamanlayicilariDurdur();
            SkorHesapla();
            SkoruYaz();
            OyunPaneliTemizle();
            PanelleriAyarla();
        }

        public void StartStop()
        {
            if (_kalanSureTimer.Enabled && DevamkeMi)
            {
                OyunDuraklatildiMi = true;
                ZamanlayicilariDurdur();
                if (KalanSure > 0)
                {
                    kalanZaman--;
                }
            }
            else if (DevamkeMi)
            {
                OyunDuraklatildiMi = false;
                ZamanlayicilariBaslat();
            }
        }

        public void HareketEt(Yon yon)
        {
            if (DevamkeMi)
            {
                _muhendis.HareketEttir(yon);
            }
        }



        private void OyunBaslangıcınıAyarla()
        {
            int oyunSuresi = int.Parse(_oyunSuresiTextBox.Text);
            KalanSure = oyunSuresi;

            PanelUzunlugu = _oyunPanel.Height;
            PanelGenisligi = _oyunPanel.Width - _bilgiPanel.Width;

            _muhendisSkor = 0;
            YakitSayisi = 0;
            YazilimSayisi = 0;
            GovdeSayisi = 0;

            TamamlananUrun = 0;
            if (_uretilecekRoket <= 0)
            {
                MessageBox.Show("Üretilecek Miktar 0'dan Büyük Olmalıdır");
                Done();
            }
            KalanUrun = _uretilecekRoket;
        }


        public void OyunHiziniHesapla()
        {
            int baslangicSayisi = int.Parse(_oyunSuresiTextBox.Text);

            if (KalanSure <= baslangicSayisi * 0.2)
            {
                _hareketTimer.Interval = 30;
            }
            else if (KalanSure <= baslangicSayisi * 0.3)
            {
                _hareketTimer.Interval = 40;
            }
            else if (KalanSure <= baslangicSayisi * 0.4)
            {
                _hareketTimer.Interval = 50;
            }
            else if (KalanSure <= baslangicSayisi * 0.6)
            {
                _hareketTimer.Interval = 60;
            }
            else if (KalanSure <= baslangicSayisi * 0.8)
            {
                _hareketTimer.Interval = 70;
            }
        }

        #endregion

        public Oyun(Panel oyunPanel, Panel bilgiPanel, Panel anaMenuPanel,TextBox oyuncuAdiTextBox,
            TextBox oyunSuresiTextBox, TextBox uretilecekMilktarTextBox)
        {
            _kalanSureTimer.Tick += KalanSureTimer_Tick;
            _toplananCisimTimer.Tick += ToplananCisimTimer_Tick;
            _hareketTimer.Tick += HareketTimer_Tick;

            _oyunPanel = oyunPanel;
            _bilgiPanel = bilgiPanel;
            _anaMenuPanel = anaMenuPanel;
            _oyuncuAdiTextBox = oyuncuAdiTextBox;
            _oyunSuresiTextBox = oyunSuresiTextBox;
            _uretilecekMiktarTextBox = uretilecekMilktarTextBox;
        }
    }
}