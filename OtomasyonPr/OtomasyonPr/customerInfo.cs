using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OtomasyonPr
{
    public partial class customerInfo : Form
    {
        private string citizenNo;
        private string connectionString = "Data Source=Aspire-7;Initial Catalog=EczaneOtomasyon;Integrated Security=True";

        public customerInfo(string? citizenNo)
        {
            InitializeComponent();
            this.citizenNo = citizenNo;
        }

        private void customerInfo_Load(object sender, EventArgs e)
        {
            LoadSalesData();

            textBox2.Text = citizenNo;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT name, surname, birthdate, address FROM customers WHERE citizen_no = @citizenNo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@citizenNo", citizenNo);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        textBox1.Text = reader["name"].ToString() + " " + reader["surname"].ToString();
                        textBox3.Text = reader["birthdate"].ToString();
                        textBox4.Text = reader["address"].ToString();
                    }
                }
            }

            using (SqlConnection connection2 = new SqlConnection(connectionString))
            {
                connection2.Open();
                string query2 = "SELECT rpr_license FROM customers WHERE citizen_no = @citizenNo";
                using (SqlCommand command = new SqlCommand(query2, connection2))
                {
                    command.Parameters.AddWithValue("@citizenNo", textBox2.Text);
                    bool hasRedPrescriptionLicense = (bool)command.ExecuteScalar();

                    if (hasRedPrescriptionLicense)
                    {
                        rpr_check.BackColor = Color.LawnGreen;
                    }
                    else
                    {
                        rpr_check.BackColor = Color.IndianRed;
                    }
                }
            }

            string customerCitizenNo = textBox2.Text;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT unpaid_products AS Urun, unpaid_amount AS Adet, unpaid_checked AS Durum, sold_date AS SatısTarihi " +
                               "FROM unpaid " +
                               "WHERE unpaid_customers = @customerCitizenNo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customerCitizenNo", customerCitizenNo);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    emanet_view.AutoGenerateColumns = false; // Otomatik sütun oluşturmayı kapat
                    emanet_view.DataSource = table;

                    DataGridViewTextBoxColumn urunColumn = new DataGridViewTextBoxColumn();
                    urunColumn.DataPropertyName = "Urun"; // Veri tabanındaki sütun adı
                    urunColumn.HeaderText = "Ürün";
                    emanet_view.Columns.Add(urunColumn);

                    DataGridViewTextBoxColumn adetColumn = new DataGridViewTextBoxColumn();
                    adetColumn.DataPropertyName = "Adet"; // Veri tabanındaki sütun adı
                    adetColumn.HeaderText = "Adet";
                    emanet_view.Columns.Add(adetColumn);

                    DataGridViewCheckBoxColumn durumColumn = new DataGridViewCheckBoxColumn();
                    durumColumn.DataPropertyName = "Durum"; // Veri tabanındaki sütun adı
                    durumColumn.HeaderText = "Durum";
                    emanet_view.Columns.Add(durumColumn);

                    DataGridViewTextBoxColumn satısTarihiColumn = new DataGridViewTextBoxColumn();
                    satısTarihiColumn.DataPropertyName = "SatısTarihi"; // Veri tabanındaki sütun adı
                    satısTarihiColumn.HeaderText = "Emanet Tarihi";
                    emanet_view.Columns.Add(satısTarihiColumn);
                }
            }





        }

        private void LoadSalesData()
        {
            string customerId = citizenNo;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT s.sale_id, u.marka, u.urun, s.sale_date, s.quantity_sold " +
                                   "FROM sales s " +
                                   "INNER JOIN urun u ON s.product_id = u.urun_id " +
                                   "WHERE s.customer_id = @customerId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@customerId", customerId);
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            DateTime saleDate;
                            if (DateTime.TryParse(row["sale_date"].ToString(), out saleDate))
                            {
                                row["sale_date"] = saleDate.ToString("dd.MM.yyyy HH:mm");
                            }
                        }

                        soldgridview.DataSource = dataTable;
                        soldgridview.Visible = true;
                        ConfigureSoldGridView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri çekme işlemi sırasında bir hata oluştu: " + ex.Message);
            }
        }

        private void ConfigureSoldGridView()
        {
            soldgridview.Columns["sale_id"].HeaderText = "Satış ID";
            soldgridview.Columns["marka"].HeaderText = "Ürün Markası";
            soldgridview.Columns["urun"].HeaderText = "Ürün Adı";
            soldgridview.Columns["sale_date"].HeaderText = "Satış Tarihi";
            soldgridview.Columns["quantity_sold"].HeaderText = "Satılan Miktar";
        }


        private void customerInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void savebutton_Click(object sender, EventArgs e)
        {
            bool isAnyChange = false;

            //foreach (DataGridViewRow row in emanet_view.Rows)
            //{
            //    if (row.Cells["unpaid_checked"] is DataGridViewCheckBoxCell checkBoxCell && checkBoxCell.Value != null)
            //    {
            //        bool currentValue = (bool)checkBoxCell.Value;
            //        bool originalValue = checkBoxCell.Tag != null && (bool)checkBoxCell.Tag;
            //
            //        if (currentValue != originalValue)
            //        {
            //            string customerCitizenNo = textBox2.Text;
            //            string productName = row.Cells["unpaid_products"].Value.ToString(); 
            //
            //            using (SqlConnection connection = new SqlConnection(connectionString))
            //            {
            //                connection.Open();
            //                string updateQuery = "UPDATE unpaid SET unpaid_checked = @isChecked WHERE unpaid_customers = @customerCitizenNo AND unpaid_products = @productName";
            //                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
            //                {
            //                    updateCommand.Parameters.AddWithValue("@isChecked", currentValue);
            //                    updateCommand.Parameters.AddWithValue("@customerCitizenNo", customerCitizenNo);
            //                    updateCommand.Parameters.AddWithValue("@productName", productName);
            //                    updateCommand.ExecuteNonQuery();
            //                }
            //            }
            //
            //            isAnyChange = true;
            //        }
            //    }
            //}

            if (isAnyChange)
            {
                MessageBox.Show("Değişiklikler başarıyla kaydedildi.");
            }
            else
            {
                DialogResult result = MessageBox.Show("Değişiklik Kaydedilsin mi?", "Kaydetme Onayı", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Değişiklikler başarıyla kaydedildi!");
                }
            }
        }


    }
}
