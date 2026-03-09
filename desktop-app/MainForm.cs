using System;
using System.IO.Ports;
using System.IO;
using System.Windows.Forms;

namespace SulamaProjesi
{
    public partial class MainForm : Form
    {
        private SerialPort serialPort;
        private int esikDegeri = 600;

        private readonly string gunlukDosya = Path.Combine(Application.StartupPath, "gunluk_rapor.txt");
        private readonly string haftalikDosya = Path.Combine(Application.StartupPath, "haftalik_rapor.txt");
        private readonly string aylikDosya = Path.Combine(Application.StartupPath, "aylik_rapor.txt");

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                serialPort = new SerialPort("COM7", 9600);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                labelEşik.Text = $"Eşik: {esikDegeri}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Seri port açılamadı: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            SendToArduino("+");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            SendToArduino("-");
        }

        private void BtnGunluk_Click(object sender, EventArgs e)
        {
            RaporuGoster(gunlukDosya, "Günlük Rapor");
        }

        private void BtnHaftalik_Click(object sender, EventArgs e)
        {
            RaporuGoster(haftalikDosya, "Haftalık Rapor");
        }

        private void BtnAylik_Click(object sender, EventArgs e)
        {
            RaporuGoster(aylikDosya, "Aylık Rapor");
        }

        private void RaporuGoster(string dosyaYolu, string raporAdi)
        {
            listBox1.Items.Clear();

            try
            {
                if (File.Exists(dosyaYolu))
                {
                    string[] satirlar = File.ReadAllLines(dosyaYolu);
                    string bugun = DateTime.Now.ToString("yyyy-MM-dd");

                    int gosterilen = 0;

                    foreach (string satir in satirlar)
                    {
                        if (satir.StartsWith(bugun))
                        {
                            listBox1.Items.Add(satir);
                            gosterilen++;
                        }
                    }

                    if (gosterilen == 0)
                    {
                        listBox1.Items.Add("Bugüne ait kayıt bulunamadı.");
                    }
                }
                else
                {
                    listBox1.Items.Add($"{raporAdi} bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Rapor gösterilirken hata oluştu: " + ex.Message);
            }
        }


        private void LogYaz(string mesaj)
        {
            try
            {
                string zamanliMetin = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {mesaj}{Environment.NewLine}";
                File.AppendAllText(gunlukDosya, zamanliMetin);
                File.AppendAllText(haftalikDosya, zamanliMetin);
                File.AppendAllText(aylikDosya, zamanliMetin);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log yazılamadı: " + ex.Message);
            }
        }

        private void SendToArduino(string komut)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.WriteLine(komut);

                    if (komut == "+")
                    {
                        esikDegeri++;
                        labelEşik.Text = $"Eşik: {esikDegeri}";
                        LogYaz($"Eşik artırıldı. Yeni eşik: {esikDegeri}");
                        listBox1.Items.Add($"Eşik artırıldı: {esikDegeri}");
                    }
                    else if (komut == "-")
                    {
                        esikDegeri--;
                        labelEşik.Text = $"Eşik: {esikDegeri}";
                        LogYaz($"Eşik azaltıldı. Yeni eşik: {esikDegeri}");
                        listBox1.Items.Add($"Eşik azaltıldı: {esikDegeri}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Arduino'ya veri gönderilemedi: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Seri port açık değil.");
            }
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine();
            Invoke(new Action(() =>
            {
               // listBox1.Items.Clear(); // Temizle
                //listBox1.Items.Add(data); // Yeni veriyi ekle

                try
                {
                    if (data.StartsWith("NEM:"))
                    {
                        // Örnek gelen veri: NEM: 780
                        string[] parca = data.Split(':');
                        if (parca.Length > 1)
                        {
                            string nemStr = parca[1].Trim();
                            labelNemm.Text = $"Nem: {nemStr}";
                        }
                    }
                    else if (data == "POMPA ACILDI")
                    {
                        LogYaz("Pompa açıldı.");
                    }
                    else if (data.StartsWith("POMPA KAPANDI"))
                    {
                        string[] parcala = data.Split(':');
                        if (parcala.Length > 1)
                        {
                            string sure = parcala[1].Trim();
                            LogYaz($"Pompa kapandı. Süre: {sure} sn");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Dosyaya yazma hatası: " + ex.Message);
                }
            }));
        }


      

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Seri port kapatılamadı: " + ex.Message);
                }
            }
        }
    }
}

