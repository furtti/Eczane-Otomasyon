using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Guna.UI2.WinForms;

namespace OtomasyonPr
{


    public partial class mainPage : Form
    {
        private string connectionString = "Data Source=Aspire-7;Initial Catalog=EczaneOtomasyon;Integrated Security=True";


        public mainPage()
        {
            InitializeComponent();
            clockTimer.Start();
            TotalUnpaid();
            CustomersLoad();
            MedicineLoad();
            UpdateTotalPriceLabel();
        }

        private void customerInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Uygulamayı kapat
        }
        private void UpdateTotalPriceLabel()
        {
            decimal totalPrice = 0;

            foreach (DataGridViewRow row in sepet_view.Rows)
            {
                if (row.Cells["toplam_fiyat"].Value != null)
                {
                    decimal rowTotalPrice = Convert.ToDecimal(row.Cells["toplam_fiyat"].Value);
                    totalPrice += rowTotalPrice;
                }
            }
            label15.Text = totalPrice.ToString();
        }

        public void ReceteGenerator()
        {
            Random random = new Random();

            long[] customerIds = { 47812514520, 60991290804, 96541225410, 35411258412, 39885200141 };

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (long customerId in customerIds)
                {
                    string receteId = GetExistingReceteId(connection, customerId) ?? GenerateRandomCode(6, connection);

                    for (int i = 0; i < 4; i++) // Her müşteri için 4 reçete oluştur
                    {
                        int urunId = random.Next(2, 51);
                        int adet = random.Next(1, 6);
                        DateTime tarih = DateTime.Now;

                        string query = "INSERT INTO recete (recete_id, urun_id, customer_id, adet, tarih) VALUES (@recete_id, @urun_id, @customer_id, @adet, @tarih)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@recete_id", receteId);
                            command.Parameters.AddWithValue("@urun_id", urunId.ToString());
                            command.Parameters.AddWithValue("@customer_id", customerId.ToString());
                            command.Parameters.AddWithValue("@adet", adet.ToString());
                            command.Parameters.AddWithValue("@tarih", tarih.ToString("yyyy-MM-dd HH:mm:ss"));

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        static string GetExistingReceteId(SqlConnection connection, long customerId)
        {
            string query = "SELECT TOP 1 recete_id FROM recete WHERE customer_id = @customer_id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@customer_id", customerId.ToString());

                object result = command.ExecuteScalar();
                return result?.ToString();
            }
        }

        static string GenerateRandomCode(int length, SqlConnection connection)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] code = new char[length];

            string newCode;
            do
            {
                for (int i = 0; i < length; i++)
                {
                    code[i] = chars[random.Next(chars.Length)];
                }
                newCode = new string(code);
            }
            while (ReceteIdExists(connection, newCode));

            return newCode;
        }

        static bool ReceteIdExists(SqlConnection connection, string receteId)
        {
            string query = "SELECT COUNT(*) FROM recete WHERE recete_id = @recete_id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@recete_id", receteId);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private void MedicineLoad()
        {
            ilac_view.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            ilac_view.Columns.Add("urun_id", "Ürün ID");
            ilac_view.Columns.Add("marka", "Marka");
            ilac_view.Columns.Add("urun", "Ürün");
            ilac_view.Columns.Add("stok", "Stok Durumu");
            ilac_view.Columns.Add("fiyat", "Adet Fiyat");

            // ilac2_view'in stilini ve sütunlarını ayarlayın
            ilac_view.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            if (ilac_view.Columns.Count == 0)
            {
                ilac_view.Columns.Add("urun_id", "Ürün ID");
                ilac_view.Columns.Add("marka", "Marka");
                ilac_view.Columns.Add("urun", "Ürün");
                ilac_view.Columns.Add("stok", "Stok Durumu");
                ilac_view.Columns.Add("fiyat", "Adet Fiyat");
            }

            // ilac2_view için sütunları ayarlayın
            ilac2_view.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            if (ilac2_view.Columns.Count == 0)
            {
                ilac2_view.Columns.Add("urun_id", "Ürün ID");
                ilac2_view.Columns.Add("marka", "Marka");
                ilac2_view.Columns.Add("urun", "Ürün");
                ilac2_view.Columns.Add("stok", "Stok Durumu");
                ilac2_view.Columns.Add("fiyat", "Adet Fiyat");
            }

            // depo_view için sütunları ayarlayın
            depo_view.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            if (depo_view.Columns.Count == 0)
            {
                depo_view.Columns.Add("urun_id", "Ürün ID");
                depo_view.Columns.Add("marka", "Marka");
                depo_view.Columns.Add("urun", "Ürün");
                depo_view.Columns.Add("miktar", "Miktar");
                depo_view.Columns.Add("fiyat", "Adet Fiyat");
            }

            string query = "SELECT urun_id, marka, urun, stok, fiyat FROM urun";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        string stokDurumu = row["stok"].ToString();
                        // ilac_view'e veri ekle
                        ilac_view.Rows.Add(row["urun_id"], row["marka"], row["urun"], stokDurumu, row["fiyat"]);
                        // ilac2_view'e veri ekle
                        ilac2_view.Rows.Add(row["urun_id"], row["marka"], row["urun"], stokDurumu, row["fiyat"]);
                    }
                }
            }
            sepet_view.Columns.Add("urun_id", "Ürün ID");
            sepet_view.Columns.Add("marka", "Marka");
            sepet_view.Columns.Add("urun", "Ürün");
            sepet_view.Columns.Add("adet", "Adet");
            sepet_view.Columns.Add("toplam_fiyat", "Toplam Fiyat");
        }
        private void TotalUnpaid()
        {
            string query = "SELECT COUNT(DISTINCT unpaid_customers) AS total_customers FROM unpaid";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        unpaid_customer.Text = result.ToString();
                    }
                    else
                    {
                        unpaid_customer.Text = "Hasta Yok.";
                    }
                }
            }
            string query3 = "SELECT SUM(unpaid_amount) FROM unpaid";
            using (SqlConnection con3 = new SqlConnection(connectionString))
            {
                con3.Open();
                using (SqlCommand command = new SqlCommand(query3, con3))
                {
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        unpaid_product.Text = result.ToString() + " Kutu";
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;

            currentclock.Text = currentTime.ToString("HH:mm:ss");
        }

        private void CustomersLoad()
        {
            hasta_view.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            string query = "SELECT * FROM customers";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    hasta_view.ReadOnly = true;
                    hasta_view.DataSource = table;
                    Dictionary<string, string> columnHeaders = new Dictionary<string, string>
        {
            { "customer_id", "ID" },
            { "name", "Ad" },
            { "surname", "Soyad" },
            { "citizen_no", "T.C No" },
            { "address", "Adres Bilgisi" },
            { "rpr_license", "KR Lisansı" },
            { "birthdate", "Doğum Tarihi" },
        };
                    foreach (var columnHeader in columnHeaders)
                    {
                        if (table.Columns.Contains(columnHeader.Key))
                        {
                            hasta_view.Columns[columnHeader.Key].HeaderText = columnHeader.Value;
                        }
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM customers WHERE citizen_no LIKE @searchTerm";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchTerm", "%" + textBox1.Text + "%");
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    hasta_view.DataSource = table;
                }
            }
        }

        private void button2_Click(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = ilac_view.Rows[e.RowIndex];
                DateTime date = DateTime.Now;
                int adet;
                if (!int.TryParse(textBox5.Text, out adet))
                {
                    MessageBox.Show("Lütfen geçerli bir sayı girin.");
                    return;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand getStockCommand = new SqlCommand("SELECT stok FROM urunler WHERE urun = @urun", connection);
                    getStockCommand.Parameters.AddWithValue("@urun", selectedRow.Cells["urun"].Value);
                    int stok;
                    using (SqlDataReader reader = getStockCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stok = (int)reader["stok"];
                        }
                        else
                        {
                            MessageBox.Show("Ürün bulunamadı.");
                            return;
                        }
                    }
                    if (stok >= adet)
                    {
                        stok -= adet;
                        SqlCommand updateStockCommand = new SqlCommand("UPDATE urunler SET stok = @stok WHERE urun = @urun", connection);
                        updateStockCommand.Parameters.AddWithValue("@stok", stok);
                        updateStockCommand.Parameters.AddWithValue("@urun", selectedRow.Cells["urun"].Value);
                        updateStockCommand.ExecuteNonQuery();

                        SqlCommand insertCommand = new SqlCommand("INSERT INTO products (product_name, product_price, product_quantity, product_expiration_date) VALUES (@urun, @fiyat, @adet, @date)", connection);
                        insertCommand.Parameters.AddWithValue("@urun", selectedRow.Cells["urun"].Value);
                        insertCommand.Parameters.AddWithValue("@fiyat", selectedRow.Cells["fiyat"].Value);
                        insertCommand.Parameters.AddWithValue("@adet", adet);
                        insertCommand.Parameters.AddWithValue("@date", date);
                        insertCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Stokta yeterli ürün yok.");
                    }
                }
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchText = textBox2.Text;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM customers WHERE citizen_no LIKE @searchText";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        hasta_view.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama sırasında bir hata oluştu: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Satır seçilmediyse hata mesajı ver
            if (ilac_view.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir ürün seçin.");
                return;
            }

            DataGridViewRow selectedRow = ilac_view.SelectedRows[0];
            int urunId = Convert.ToInt32(selectedRow.Cells["urun_id"].Value);
            string marka = Convert.ToString(selectedRow.Cells["marka"].Value);
            string urun = Convert.ToString(selectedRow.Cells["urun"].Value);
            int stok = Convert.ToInt32(selectedRow.Cells["stok"].Value);
            decimal fiyat = Convert.ToDecimal(selectedRow.Cells["fiyat"].Value);

            // textBox5'ten girilen adeti al
            int adet;
            if (!int.TryParse(textBox5.Text, out adet))
            {
                MessageBox.Show("Lütfen geçerli bir sayı girin.");
                return;
            }

            // Stok yeterli değilse uyarı ver
            if (stok - adet < 0)
            {
                MessageBox.Show("Stok yeterli değil.");
                return;
            }

            // sepet_view DataGridView'inde aynı ürün ID'sine sahip satır var mı kontrol et
            bool urunEklendi = false;
            foreach (DataGridViewRow row in sepet_view.Rows)
            {
                if (Convert.ToInt32(row.Cells["urun_id"].Value) == urunId)
                {
                    // Aynı ürün ID'sine sahip satır varsa adeti ve toplam fiyatı güncelle
                    int yeniAdet = Convert.ToInt32(row.Cells["adet"].Value) + adet;
                    row.Cells["adet"].Value = yeniAdet;
                    row.Cells["toplam_fiyat"].Value = yeniAdet * fiyat;
                    urunEklendi = true;
                    break;
                }
            }

            if (!urunEklendi)
            {
                // sepet_view DataGridView'ine satırı ekleyin
                sepet_view.Rows.Add(urunId, marka, urun, adet, adet * fiyat);
            }

            // ilac_view'den stok miktarını düşün
            selectedRow.Cells["stok"].Value = stok - adet;

            // textBox5'i temizle
            textBox5.Clear();
            UpdateTotalPriceLabel();
        }
        private void sepet_view_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateTotalPriceLabel();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn != null)
            {
                textBox5.Text += btn.Text;
            }
        }


        private void char_delete_Click(object sender, EventArgs e)
        {
            UpdateTotalPriceLabel();
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Lütfen bir adet girin.");
                return;
            }

            try
            {
                if (sepet_view.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen silmek istediğiniz bir ürünü seçin.");
                    return;
                }

                DataGridViewRow selectedRow = sepet_view.SelectedRows[0];
                int urunId = Convert.ToInt32(selectedRow.Cells["urun_id"].Value);
                int adet = Convert.ToInt32(textBox5.Text);

                DataGridViewRow foundRow = null;
                foreach (DataGridViewRow row in sepet_view.Rows)
                {
                    if (Convert.ToInt32(row.Cells["urun_id"].Value) == urunId)
                    {
                        foundRow = row;
                        break;
                    }
                }

                if (foundRow == null)
                {
                    MessageBox.Show("Ürün sepetinizde bulunamadı.");
                    return;
                }

                int mevcutAdet = Convert.ToInt32(foundRow.Cells["adet"].Value);

                if (adet > mevcutAdet)
                {
                    MessageBox.Show("Stoktan fazla ürün çıkaramazsınız.");
                    return;
                }

                int yeniAdet = mevcutAdet - adet;

                if (yeniAdet <= 0)
                {
                    sepet_view.Rows.Remove(foundRow);
                }
                else
                {
                    foundRow.Cells["adet"].Value = yeniAdet;
                }

                UpdateTotalPriceLabel();

                foreach (DataGridViewRow row in ilac_view.Rows)
                {
                    if (Convert.ToInt32(row.Cells["urun_id"].Value) == urunId)
                    {
                        int stok = Convert.ToInt32(row.Cells["stok"].Value);
                        row.Cells["stok"].Value = stok + adet;
                        break;
                    }
                }
                textBox5.Clear();
            }
            catch (FormatException)
            {
                MessageBox.Show("Geçersiz giriş formatı.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }


        }
        private void searchbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchText = searchbox.Text;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM urun WHERE urun LIKE @searchText";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        ilac_view.Rows.Clear();
                        foreach (DataRow row in table.Rows)
                        {
                            ilac_view.Rows.Add(row["urun_id"], row["marka"], row["urun"], row["stok"], row["fiyat"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama sırasında bir hata oluştu: " + ex.Message);
            }
        }
        private void CheckRedPrescription()
        {
            List<int> productIds = new List<int>();
            foreach (DataGridViewRow row in sepet_view.Rows)
            {
                int urunId = Convert.ToInt32(row.Cells["urun_id"].Value);
                productIds.Add(urunId);
            }

            Dictionary<int, bool> rprRequiredDict = new Dictionary<int, bool>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (int urunId in productIds)
                {
                    string query = "SELECT rpr_required FROM urun WHERE urun_id = @urunId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@urunId", urunId);
                        bool rprRequired = (bool)command.ExecuteScalar();
                        rprRequiredDict.Add(urunId, rprRequired);
                    }
                }
            }
            string citizenNo = textBox3.Text;
            bool isCustomerValid = false;
            bool hasRedPrescription = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT rpr_license FROM customers WHERE citizen_no = @citizenNo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@citizenNo", citizenNo);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        isCustomerValid = true;
                        hasRedPrescription = (bool)result;
                    }
                }
            }
            if (isCustomerValid)
            {
                if (hasRedPrescription)
                {
                    // İşlemi tamamla
                }
                else
                {
                    MessageBox.Show("Kırmızı Reçeteye sahip değil");
                }
            }
            else
            {
                MessageBox.Show("Geçersiz müşteri. Lütfen geçerli bir müşteri seçin.");
            }
        }

        private void showbutton_Click(object sender, EventArgs e)
        {
            if (hasta_view.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = hasta_view.SelectedRows[0];
                string citizenNo = selectedRow.Cells["citizen_no"].Value.ToString();

                customerInfo ilacForm = new customerInfo(citizenNo);
                ilacForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Lütfen bir satır seçin.");
            }
        }
        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            depo PRForm = new depo();
            PRForm.Show();
        }
        private void guna2GradientButton5_Click(object sender, EventArgs e)
        {
            string citizenNo = textBox3.Text;
            bool isCustomerValid = false;
            bool isRedPrescriptionEligible = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM customers WHERE citizen_no = @citizenNo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@citizenNo", citizenNo);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        isCustomerValid = true;
                    }
                }

                if (isCustomerValid)
                {
                    // Check for red prescription
                    string checkQuery = "SELECT rpr_required FROM urun WHERE urun_id = @urunId";
                    foreach (DataGridViewRow row in sepet_view.Rows)
                    {
                        int urunId = Convert.ToInt32(row.Cells["urun_id"].Value);
                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@urunId", urunId);
                            bool rprRequired = (bool)checkCommand.ExecuteScalar();
                            if (rprRequired)
                            {
                                isRedPrescriptionEligible = true;
                                break;
                            }
                        }
                    }

                    if (isRedPrescriptionEligible)
                    {
                        // Check customer's red prescription license
                        string licenseQuery = "SELECT rpr_license FROM customers WHERE citizen_no = @citizenNo";
                        using (SqlCommand licenseCommand = new SqlCommand(licenseQuery, connection))
                        {
                            licenseCommand.Parameters.AddWithValue("@citizenNo", citizenNo);
                            bool hasRedPrescriptionLicense = (bool)licenseCommand.ExecuteScalar();
                            if (!hasRedPrescriptionLicense)
                            {
                                MessageBox.Show("Kırmızı Reçeteye sahip değil");
                                return;
                            }
                        }
                    }

                    if (sepet_view.Rows.Count == 0)
                    {
                        MessageBox.Show("Sepette ürün bulunmamaktadır.");
                        return;
                    }
                    decimal totalPrice = 0;
                    foreach (DataGridViewRow row in sepet_view.Rows)
                    {
                        decimal rowTotalPrice = Convert.ToDecimal(row.Cells["toplam_fiyat"].Value);
                        totalPrice += rowTotalPrice;
                    }

                    foreach (DataGridViewRow row in sepet_view.Rows)
                    {
                        int urunId = Convert.ToInt32(row.Cells["urun_id"].Value);
                        int quantity = Convert.ToInt32(row.Cells["adet"].Value);

                        string updateStokQuery = "UPDATE urun SET stok = stok - @quantity WHERE urun_id = @urunId";

                        using (SqlCommand updateStokCommand = new SqlCommand(updateStokQuery, connection))
                        {
                            updateStokCommand.Parameters.AddWithValue("@quantity", quantity);
                            updateStokCommand.Parameters.AddWithValue("@urunId", urunId);
                            updateStokCommand.ExecuteNonQuery();
                        }
                    }

                    using (SqlConnection connection2 = new SqlConnection(connectionString))
                    {
                        connection2.Open();
                        foreach (DataGridViewRow row in sepet_view.Rows)
                        {
                            string productName = row.Cells["urun"].Value.ToString();
                            int quantity = Convert.ToInt32(row.Cells["adet"].Value);
                            decimal productPrice = Convert.ToDecimal(row.Cells["toplam_fiyat"].Value);
                            DateTime soldDate = DateTime.Now;

                            string insertQuery = "INSERT INTO unpaid (unpaid_customers, unpaid_products, unpaid_amount, unpaid_checked, sold_date) VALUES (@citizenNo, @productName, @quantity, 0, @soldDate)";
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection2))
                            {
                                insertCommand.Parameters.AddWithValue("@citizenNo", citizenNo);
                                insertCommand.Parameters.AddWithValue("@productName", productName);
                                insertCommand.Parameters.AddWithValue("@quantity", quantity);
                                insertCommand.Parameters.AddWithValue("@soldDate", soldDate);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                        sepet_view.Rows.Clear();
                        UpdateTotalPriceLabel();
                        TotalUnpaid();
                        MessageBox.Show("İlaçlar Emanet Olarak Satıldı!");
                    }
                }
                else
                {
                    MessageBox.Show("Geçersiz müşteri. Lütfen geçerli bir müşteri seçin.");
                }
            }
        }

        private void guna2GradientButton6_Click(object sender, EventArgs e)
        {
            string citizenNo = textBox3.Text;
            bool isCustomerValid = false;
            bool isRedPrescriptionEligible = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM customers WHERE citizen_no = @citizenNo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@citizenNo", citizenNo);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        isCustomerValid = true;
                    }
                }

                if (isCustomerValid)
                {
                    // Check for red prescription
                    string checkQuery = "SELECT rpr_required FROM urun WHERE urun_id = @urunId";
                    foreach (DataGridViewRow row in sepet_view.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int urunId;
                        if (int.TryParse(row.Cells["urun_id"].Value.ToString(), out urunId))
                        {
                            using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                            {
                                checkCommand.Parameters.AddWithValue("@urunId", urunId);
                                bool rprRequired = (bool)checkCommand.ExecuteScalar();
                                if (rprRequired)
                                {
                                    isRedPrescriptionEligible = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (isRedPrescriptionEligible)
                    {
                        // Check customer's red prescription license
                        string licenseQuery = "SELECT rpr_license FROM customers WHERE citizen_no = @citizenNo";
                        using (SqlCommand licenseCommand = new SqlCommand(licenseQuery, connection))
                        {
                            licenseCommand.Parameters.AddWithValue("@citizenNo", citizenNo);
                            bool hasRedPrescriptionLicense = (bool)licenseCommand.ExecuteScalar();
                            if (!hasRedPrescriptionLicense)
                            {
                                MessageBox.Show("Kırmızı Reçeteye sahip değil");
                                return;
                            }
                        }
                    }

                    if (sepet_view.Rows.Count == 0)
                    {
                        MessageBox.Show("Sepette ürün bulunmamaktadır.");
                        return;
                    }

                    decimal totalPrice = 0;
                    foreach (DataGridViewRow row in sepet_view.Rows)
                    {
                        if (row.IsNewRow) continue;

                        decimal rowTotalPrice;
                        if (decimal.TryParse(row.Cells["toplam_fiyat"].Value.ToString(), out rowTotalPrice))
                        {
                            totalPrice += rowTotalPrice;
                        }
                    }

                    foreach (DataGridViewRow row in sepet_view.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int urunId;
                        int quantity;
                        if (int.TryParse(row.Cells["urun_id"].Value.ToString(), out urunId) &&
                            int.TryParse(row.Cells["adet"].Value.ToString(), out quantity))
                        {
                            string updateStokQuery = "UPDATE urun SET stok = stok - @quantity WHERE urun_id = @urunId";
                            using (SqlCommand updateStokCommand = new SqlCommand(updateStokQuery, connection))
                            {
                                updateStokCommand.Parameters.AddWithValue("@quantity", quantity);
                                updateStokCommand.Parameters.AddWithValue("@urunId", urunId);
                                updateStokCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    string salesQuery = "INSERT INTO sales (product_id, customer_id, sale_date, quantity_sold) VALUES (@urunId, @citizenNo, @saleDate, @quantity)";
                    using (SqlCommand command = new SqlCommand(salesQuery, connection))
                    {
                        foreach (DataGridViewRow row in sepet_view.Rows)
                        {
                            if (row.IsNewRow) continue;

                            int urunId;
                            int quantity;
                            DateTime saleDate = DateTime.Now;
                            if (int.TryParse(row.Cells["urun_id"].Value.ToString(), out urunId) &&
                                int.TryParse(row.Cells["adet"].Value.ToString(), out quantity))
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@urunId", urunId);
                                command.Parameters.AddWithValue("@citizenNo", citizenNo);
                                command.Parameters.AddWithValue("@saleDate", saleDate);
                                command.Parameters.AddWithValue("@quantity", quantity);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    sepet_view.Rows.Clear();
                    UpdateTotalPriceLabel();
                    TotalUnpaid();
                    MessageBox.Show("İlaçlar Başarıyla Satıldı!");
                }
                else
                {
                    MessageBox.Show("Geçersiz müşteri. Lütfen geçerli bir müşteri seçin.");
                }
            }
        }

        private void guna2GradientButton7_Click(object sender, EventArgs e)
        {
            // Sepet view'de bir satır seçili değilse hata mesajı göster
            if (sepet_view.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz bir ürünü seçin.");
                return;
            }

            // Seçilen satırın verilerini al
            DataGridViewRow selectedRow = sepet_view.SelectedRows[0];
            int urunId = Convert.ToInt32(selectedRow.Cells["urun_id"].Value);
            int adet = Convert.ToInt32(selectedRow.Cells["adet"].Value);

            // ilac_view'deki stok miktarını güncelle
            foreach (DataGridViewRow row in ilac_view.Rows)
            {
                if (Convert.ToInt32(row.Cells["urun_id"].Value) == urunId)
                {
                    int stok = Convert.ToInt32(row.Cells["stok"].Value);
                    row.Cells["stok"].Value = stok + adet;
                    break;
                }
            }

            // Sepet view'den satırı sil
            sepet_view.Rows.Remove(selectedRow);
            UpdateTotalPriceLabel();
        }

        private void guna2GradientButton10_Click(object sender, EventArgs e)
        {
            string receteId = textBox1.Text;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                SELECT r.recete_id, r.urun_id, u.urun, r.customer_id, r.adet, r.tarih 
                FROM recete r 
                JOIN urun u ON r.urun_id = u.urun_id 
                WHERE r.recete_id = @receteId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@receteId", receteId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            recete_view.DataSource = dataTable;

                            // Adjust column headers
                            recete_view.Columns["recete_id"].HeaderText = "Reçete ID";
                            recete_view.Columns["urun_id"].HeaderText = "Ürün ID";
                            recete_view.Columns["urun"].HeaderText = "Ürün Adı";
                            recete_view.Columns["customer_id"].HeaderText = "Müşteri ID";
                            recete_view.Columns["adet"].HeaderText = "Adet";
                            recete_view.Columns["tarih"].HeaderText = "Tarih";


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }


        private void guna2GradientButton9_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        foreach (DataGridViewRow row in sepet_view.Rows)
                        {
                            if (row.IsNewRow) continue;

                            string urunId = row.Cells["urun_id"].Value.ToString();
                            string customerId = row.Cells["customer_id"].Value.ToString();
                            string quantitySold = row.Cells["adet"].Value.ToString();
                            string receteId = row.Cells["recete_id"].Value.ToString();

                            // Insert into sales table
                            string salesQuery = "INSERT INTO sales (product_id, customer_id, sale_date, quantity_sold) VALUES (@productId, @customerId, @saleDate, @quantitySold)";

                            using (SqlCommand command = new SqlCommand(salesQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@productId", urunId);
                                command.Parameters.AddWithValue("@customerId", customerId);
                                command.Parameters.AddWithValue("@saleDate", DateTime.Now);
                                command.Parameters.AddWithValue("@quantitySold", quantitySold);

                                command.ExecuteNonQuery();
                            }

                            // Delete from recete table
                            string deleteReceteQuery = "DELETE FROM recete WHERE recete_id = @receteId";

                            using (SqlCommand deleteCommand = new SqlCommand(deleteReceteQuery, connection, transaction))
                            {
                                deleteCommand.Parameters.AddWithValue("@receteId", receteId);
                                deleteCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Veriler başarıyla sales tablosuna aktarıldı ve recete tablosundan silindi!");

                        // Clear the DataGridView
                        recete_view.Rows.Clear();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Bir hata oluştu: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı kurulurken bir hata oluştu: " + ex.Message);
            }
        }

        private void guna2GradientButton4_Click(object sender, EventArgs e)
        {
            string version = "1.0.0";
            string developer = "Furkan Gül, İsa Frat Kanlı, Koray Ahmet Gül";
            string updates = "Gelecek güncellemelerde eklenmesi planlanan modüller: Depo Sipariş Takip,Envanter Yönetimi";
            string message = $"Otomasyon Sürümü: {version}\nDeveloper ve Tasarımcı: {developer}\n\n{updates}";

            MessageBox.Show(message, "Versiyon Bilgileri ve Güncellemeler", MessageBoxButtons.OK);
        }
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = guna2TextBox1.Text.Trim();

            string query = "SELECT urun_id, marka, urun, stok, fiyat FROM urun WHERE urun LIKE @searchText";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    // ilac_view ve ilac2_view'i temizle
                    ilac_view.Rows.Clear();
                    ilac2_view.Rows.Clear();

                    foreach (DataRow row in table.Rows)
                    {
                        string stokDurumu = row["stok"].ToString();
                        // ilac_view'e veri ekle
                        ilac_view.Rows.Add(row["urun_id"], row["marka"], row["urun"], stokDurumu, row["fiyat"]);
                        // ilac2_view'e veri ekle
                        ilac2_view.Rows.Add(row["urun_id"], row["marka"], row["urun"], stokDurumu, row["fiyat"]);
                    }
                }
            }
        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            // ilac2_view'de seçili olan satır var mı kontrol edin
            if (ilac2_view.SelectedRows.Count > 0)
            {
                // Seçili satırdaki verileri alın
                DataGridViewRow selectedRow = ilac2_view.SelectedRows[0];
                string urunId = selectedRow.Cells["urun_id"].Value.ToString();
                string marka = selectedRow.Cells["marka"].Value.ToString();
                string urun = selectedRow.Cells["urun"].Value.ToString();
                string fiyat = selectedRow.Cells["fiyat"].Value.ToString();
                int stok = Convert.ToInt32(selectedRow.Cells["stok"].Value);

                // Miktar değerini alın
                int miktar;
                if (int.TryParse(guna2TextBox2.Text, out miktar) && miktar > 0)
                {
                    if (stok >= miktar)
                    {
                        // Stok durumu ve miktarı ekleyin
                        string stokDurumu = (stok - miktar > 0) ? "Yeterli" : "Yetersiz";

                        // Toplam fiyatı hesaplayın
                        decimal toplamFiyat = miktar * Convert.ToDecimal(fiyat);

                        // depo_view'e yeni satır ekleyin
                        depo_view.Rows.Add(urunId, marka, urun, miktar.ToString(), toplamFiyat.ToString("0.00"));
                    }
                    else
                    {
                        MessageBox.Show("Yetersiz stok.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Geçerli bir miktar giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Lütfen ilac2_view'de bir ürün seçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2GradientButton8_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    foreach (DataGridViewRow row in depo_view.Rows)
                    {
                        string urunId = row.Cells["urun_id"].Value.ToString();
                        int miktar = Convert.ToInt32(row.Cells["miktar"].Value);

                        string updateQuery = "UPDATE urun SET stok = stok - @miktar WHERE urun_id = @urunId";
                        using (SqlCommand command = new SqlCommand(updateQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@miktar", miktar);
                            command.Parameters.AddWithValue("@urunId", urunId);
                            command.ExecuteNonQuery();
                        }
                    }

                    // depo_view'i temizle
                    depo_view.Rows.Clear();

                    // İşlem başarılı mesajını göster
                    MessageBox.Show("Ürünler başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}