# Eczane Otomasyon Sistemi

## Proje Ne İçin Yapıldı?
**PROJE Ondokuz Mayıs Üniversitesi - Bilgisayar Programcılığı bölümü Sistem Analizi ve Tasarımı dersi dönem projesi için yapılmıştır.** Eczanelerde reçete yazma ve yönetme işlemlerini kolaylaştırmak amacıyla geliştirilmiş bir otomasyon sistemidir. Kullanıcı dostu arayüzü ve veritabanı desteği ile hızlı ve güvenilir bir çözüm sunar.
## Özellikler
- **Reçete Oluşturma ve Yönetme:** Reçeteler kolayca oluşturulabilir, düzenlenebilir ve görüntülenebilir.
- **Müşteri Bilgileri Yönetimi:** Müşterilerin bilgileri güvenli bir şekilde saklanır ve yönetilir.
- **Ürün Yönetimi:** Eczanede bulunan ürünler ve stok durumu kolayca takip edilebilir.
- **Veritabanı Desteği:** SQL veritabanı ile güvenilir veri saklama ve hızlı erişim imkanı.

## Kurulum
Projeyi çalıştırmak için aşağıdaki adımları izleyin:

1. **Depoyu Klonlayın:**
   ```sh
   git clone https://github.com/kullaniciadi/eczane-otomasyon.git
   cd eczane-otomasyon
   ```


2. Veritabanı Bağlantısını Ayarlayın:
   - Data Source ve Initial Catalog gibi veritabanı bağlantı ayarlarını projenize uygun şekilde güncelleyin.

csharp

    string connectionString = "Data Source=Aspire-7;Initial Catalog=EczaneOtomasyon;Integrated Security=True";

    Bağımlılıkları Yükleyin:
    Projede kullanılan bağımlılıkları yükleyin.

    Uygulamayı Çalıştırın:
    Projeyi Visual Studio veya benzeri bir IDE ile açarak çalıştırın.

Kullanım

    Müşteri Ekleme: Müşteri bilgilerini girerek yeni müşteri ekleyebilirsiniz.
    Ürün Ekleme: Ürün bilgilerini girerek yeni ürün ekleyebilirsiniz.
    Reçete Oluşturma: Reçete yazma ekranında müşteri ve ürün bilgilerini girerek yeni bir reçete oluşturabilirsiniz.
