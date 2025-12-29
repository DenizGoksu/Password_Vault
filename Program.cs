using System;
using System.Text;
using Microsoft.Data.Sqlite;
// Kullanacağımız kütüphaneler


namespace SifreProjesi
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqliteConnection("Data Source=Sifreler.db"))
            {
                connection.Open();

                // Tabloyu oluşturuyoruz
                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = "CREATE TABLE IF NOT EXISTS Sifreler (Site TEXT PRIMARY KEY, GizliSifre TEXT)";
                createTableCmd.ExecuteNonQuery();

                while (true) // Program biz çıkana kadar dönsün
                {
                    Console.WriteLine("\n--- ŞİFRE YÖNETİM PANELİ ---");
                    Console.WriteLine("1- Yeni Şifre Ekle");
                    Console.WriteLine("2- Tüm Şifreleri Listele");
                    Console.WriteLine("3- Şifre Güncelle");
                    Console.WriteLine("4- Site Sil");
                    Console.WriteLine("5- Çıkış");
                    Console.Write("Seçiminiz: ");
                    string secim = Console.ReadLine();

                    if (secim == "1")
                    {
                        Console.Write("Site Adı: "); // EKLEME
                        string site = Console.ReadLine();
                        Console.Write("Şifre: ");
                        string sifre = Sifrele(Console.ReadLine(), 'K');

                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "INSERT OR IGNORE INTO Sifreler VALUES ($site, $sifre)";
                        cmd.Parameters.AddWithValue("$site", site);
                        cmd.Parameters.AddWithValue("$sifre", sifre);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("[+] Kaydedildi.");
                    }
                    // Program.cs içindeki Listeleme (Seçenek 2) kısmını şu şekilde güncelle:

                    else if (secim == "2")
                    {
                        Console.WriteLine("\n--- KAYITLI SİTELER VE ŞİFRELER ---");
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "SELECT * FROM Sifreler";
                        using (var reader = cmd.ExecuteReader())
                    {
                    while (reader.Read()) 
                    {
                        string siteAdi = reader.GetString(0);
                        string veritabanindakiSifre = reader.GetString(1);

                        // Şifreli veriyi tekrar XOR metoduna sokuyoruz
                        string cozulmusSifre = Sifrele(veritabanindakiSifre, 'K');

                        Console.WriteLine($"Site: {siteAdi.PadRight(15)} | Şifreli: {veritabanindakiSifre.PadRight(10)} | Çözülmüş: {cozulmusSifre}");
                    }
                }
}
                    else if (secim == "3") // GÜNCELLEME
                    {
                        Console.Write("Şifresini değiştirmek istediğiniz site: ");
                        string site = Console.ReadLine();
                        Console.Write("Yeni Şifre: ");
                        string yeniSifre = Sifrele(Console.ReadLine(), 'K');

                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "UPDATE Sifreler SET GizliSifre = $sifre WHERE Site = $site";
                        cmd.Parameters.AddWithValue("$sifre", yeniSifre);
                        cmd.Parameters.AddWithValue("$site", site);
                        int etkilenen = cmd.ExecuteNonQuery();

                        if (etkilenen > 0) Console.WriteLine("[+] Şifre başarıyla güncellendi.");
                        else Console.WriteLine("[-] Site bulunamadı.");
                    }
                    else if (secim == "4") // SİLME 
                    {
                        Console.Write("Silmek istediğiniz site adı: ");
                        string site = Console.ReadLine();

                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "DELETE FROM Sifreler WHERE Site = $site";
                        cmd.Parameters.AddWithValue("$site", site);
                        int etkilenen = cmd.ExecuteNonQuery();

                        if (etkilenen > 0) Console.WriteLine("[+] Kayıt silindi.");
                        else Console.WriteLine("[-] Site bulunamadı.");
                    }
                    else if (secim == "5") break;
                }
            }
        }

        static string Sifrele(string text, char key)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text) sb.Append((char)(c ^ key));
            return sb.ToString();
        }
    }
}