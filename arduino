#define SENSOR_PIN A0     // Nem sensörünün bağlı olduğu pin
#define RELAY_PIN  7      // Röle pinine bağlı pin

int esik = 600;          // Başlangıç nem eşiği
unsigned long pompaAcilmaZamani = 0; // Pompa açılma zamanı
bool pompaAcik = false;   // Pompanın açık olup olmadığını takip eder

// EEPROM'a veri yazma fonksiyonu
void eepromYaz(int adres, unsigned long veri) { 
    while (EECR & (1 << EEPE));         // Önce EEPROM yazma işlemi bitene kadar bekle
    EEAR = adres;                       // EEPROM adresini ayarla
    EEDR = (veri >> 8) & 0xFF;          // Yüksek 8 biti veri kaydediciye yaz
    EECR |= (1 << EEMPE);               // EEPROM master yazma izni
    EECR |= (1 << EEPE);                // EEPROM yazma başlat
    delay(10);                          // Güvenlik için kısa bir bekleme

    EEAR = adres + 1;                   // Sonraki EEPROM adresine geç (alt 8 bit için)
    EEDR = veri & 0xFF;                 // Alt 8 biti veri kaydediciye yaz
    EECR |= (1 << EEMPE);               // Master yazma izni
    EECR |= (1 << EEPE);                // Yazma işlemini başlat
}

// EEPROM'dan veri okuma fonksiyonu
unsigned long eepromOku(int adres) {
    while (EECR & (1 << EEPE));           // EEPROM yazma işlemi bitene kadar bekle
    EEAR = adres;                         // Okunacak EEPROM adresi
    EECR |= (1 << EERE);                  // Okuma işlemini başlat
    unsigned long yuksek = EEDR;          // Yüksek 8 bit okunur

    EEAR = adres + 1;                     // Diğer bayta geç
    EECR |= (1 << EERE);                  // Okuma işlemi başlatılır
    unsigned long dusuk = EEDR;           // Alt 8 bit okunur

    return (yuksek << 8) | dusuk;         // 2 bayt birleştirilip 16 bitlik değer elde edilir
}

// Seri karakter gönderme
void seriYolla(char c) {
    while (!(UCSR0A & (1 << UDRE0)));     // Veri gönderme tamponu boş olana kadar bekle
    UDR0 = c;                             // Karakteri seri port üzerinden gönder
}
// Seri metin gönderme
void seriYollaMetin(const char* str) {
    while (*str) seriYolla(*str++);       // Her karakteri sırayla gönder
}

void setup() {
    // ---------- SERİ HABERLEŞMEYİ BAŞLAT -----------
    UBRR0H = 0;                           // Baud rate üst biti
    UBRR0L = 103;                         // 9600 baud için alt bit (16 MHz için)
    UCSR0B = (1 << RXEN0) | (1 << TXEN0); // Alıcı ve verici aktif hale getirilir
    UCSR0C = (1 << UCSZ01) | (1 << UCSZ00); // 8 bit veri gönderimi ayarlanır

    // ---------- RÖLEYİ BAŞLANGIÇTA KAPALI TUT -----------
    DDRD |= (1 << RELAY_PIN);            // Röle pinini çıkış olarak ayarla
    PORTD &= ~(1 << RELAY_PIN);          // Röleyi kapat

    // ---------- EEPROM'DAN DAHA ÖNCE KAYDEDİLMİŞ POMPA ÇALIŞMA SÜRESİNİ OKU -----------
    unsigned long oncekiSure = eepromOku(0);     // EEPROM'dan oku
    seriYollaMetin("EEPROM'DAN OKUNAN ONCEKI SURE: ");
    char buffer[10];
    itoa(oncekiSure, buffer, 10);         // Sayıyı string'e çevir
    seriYollaMetin(buffer);
    seriYollaMetin(" sn\n");
}

void loop() {
    // ---------- NEM DEĞERİNİ OKU -----------
    int nem = analogRead(SENSOR_PIN);     // A0 pininden analog nem değeri oku

    // ---------- NEM DEĞERİNİ SERİ PORTA GÖNDER -----------
    seriYollaMetin("NEM: ");
    char nemStr[10];
    itoa(nem, nemStr, 10);
    seriYollaMetin(nemStr);
    seriYollaMetin("\n");

    // ---------- EĞER NEM EŞİKTEN BÜYÜKSE VE POMPA KAPALIYSA, POMPAYI AÇ -----------
    if (nem > esik && !pompaAcik) {
        pompaAcik = true;                      // Pompa artık açık
        pompaAcilmaZamani = millis();          // Açılma zamanını kaydet
        PORTD &= ~(1 << RELAY_PIN);            // Röleyi aktif et (pompa çalışır)

        seriYollaMetin("POMPA ACILDI\n");
    }

    // ---------- EĞER NEM EŞİĞE DÜŞTÜYSE VE POMPA AÇIKSA, POMPAYI KAPAT -----------
    else if (nem <= esik && pompaAcik) {
        pompaAcik = false;                     // Pompa artık kapalı
        PORTD |= (1 << RELAY_PIN);             // Röleyi kapat (pompa durur)
        unsigned long kapanma = millis();      // Kapanma zamanı
        unsigned long fark = (kapanma - pompaAcilmaZamani) / 1000;  // Açık kalma süresi (saniye)

        seriYollaMetin("POMPA KAPANDI\n");
        seriYollaMetin("SURE: ");
        char saniye[10];
        itoa(fark, saniye, 10);
        seriYollaMetin(saniye);
        seriYollaMetin(" sn\n");

        eepromYaz(0, fark);                    // Bu süreyi EEPROM’a kaydet
    }

    // ---------- SERİ PORTTAN GELEN + VE - TUŞLARI İLE EŞİK DEĞERİNİ AYARLA -----------
    if (UCSR0A & (1 << RXC0)) {                // Eğer seri porttan veri geldiyse
        char gelen = UDR0;                     // Gelen karakteri oku
        if (gelen == '+') esik += 50;          // '+' ise eşik 50 artar
        else if (gelen == '-') esik -= 50;     // '-' ise eşik 50 azalır
    }

    // ---------- BİR SONRAKİ OKUMA İÇİN 500 ms BEKLE -----------
    delay(500);
}



