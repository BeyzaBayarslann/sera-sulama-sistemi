🌱 Akıllı Sera Sulama Sistemi | Smart Greenhouse Irrigation System

Bu proje, toprak nemini sürekli izleyerek bitkilerin ihtiyacı olan suyu otomatik olarak sağlayan ve verileri bir masaüstü arayüzü üzerinden raporlayan entegre bir gömülü sistem çalışmasıdır.

📸 Proje Görselleri | Project Gallery

Devre Şeması (Schematic)

Aşağıdaki şema, bileşenlerin Arduino Uno üzerindeki bağlantı noktalarını ve elektriksel akışı göstermektedir.

Uygulama Görüntüleri (Real Life Photos)

Genel Görünüm

Sistem Detayı

Üstten Bakış







📋 Proje Özeti | Project Summary

Bu çalışma, Arduino Uno tabanlı bir otomatik sulama sistemidir. Toprak nem seviyesini ölçerek belirlenen eşik değerine göre pompayı yönetir ve sulama sürelerini EEPROM üzerinde saklar. C# ile geliştirilen masaüstü arayüzü sayesinde anlık veri takibi ve eşik değeri ayarı yapılabilmektedir.

📄 Detaylı Proje Raporu: Projenin akademik detaylarına, algoritma akışına ve sonuç değerlendirmesine buraya tıklayarak ulaşabilirsiniz.

🔌 Donanım Bağlantıları | Hardware Connections

1. Arduino – Toprak Nem Sensörü Bağlantısı

Sensör Pini

Arduino Pini

VCC

5V

GND

GND

A0

A0

Açıklama: Sensörden gelen analog veri Arduino tarafından analogRead(A0) komutu ile okunur.

2. Arduino – Röle Bağlantısı

Röle Pini

Arduino Pini

VCC

5V

GND

GND

IN

Digital Pin 7

Açıklama: Arduino D7 pininden gelen dijital sinyal ile röle tetiklenerek su pompasının devresini tamamlar.

3. Röle – Su Pompası Bağlantısı

Röle Pini

Bağlantı

COM

Adaptör (+)

NO (Normally Open)

Pompa (+)

Pompa (-)

Adaptör (-)

4. Güç Kaynağı Bağlantısı

Bileşen

Güç Kaynağı

Arduino Uno

USB / 9V DC Jack

Su Pompası

Harici 5V/12V Adaptör

📁 Proje Yapısı | Structure

arduino/: Arduino C kontrol kodları.

desktop-app/: C# WinForms masaüstü uygulama kaynak kodları.

docs/: Akademik proje raporu ve dökümantasyon.

images/: Donanım şemaları ve uygulama fotoğrafları.

👩‍💻 Geliştirici | Developer

Beyza Bayarslan Necmettin Erbakan Üniversitesi - Bilgisayar Mühendisliği 

Ders: Gömülü Sistemler
