using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OtomasyonPr
{
    public partial class createUserPage : Form
    {
        public createUserPage()
        {
            InitializeComponent();
        }

        private void createUserPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.ExitThread();
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=Aspire-7;Initial Catalog=EczaneOtomasyon;Integrated Security=True");
            con.Open();

            string checkUserSql = "Select Count(*) from employees where citizen_no = @citizenno or username = @username";
            SqlCommand checkUserCmd = new SqlCommand(checkUserSql, con);
            checkUserCmd.Parameters.AddWithValue("@citizenno", signup_citizenno.Text);
            checkUserCmd.Parameters.AddWithValue("@username", signup_username.Text);
            int userCount = (int)checkUserCmd.ExecuteScalar();
            if (userCount > 0)
            {
                MessageBox.Show("Bu TC numarası veya kullanıcı adı sisteme kayıtlı.");
            }
            else if (string.IsNullOrEmpty(signup_username.Text) ||
                string.IsNullOrEmpty(signup_pass.Text) ||
                string.IsNullOrEmpty(signup_name.Text) ||
                string.IsNullOrEmpty(signup_surname.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrWhiteSpace(signup_citizenno.Text) || signup_citizenno.Text.Length != 11)
            {
                MessageBox.Show("TC No boş veya eksik!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (signup_checkpass.Text != signup_pass.Text)
            {
                MessageBox.Show("Şifreler birbiri ile uyuşmuyor. Lütfen Tekrar Dene");
            }
            else
            {
                string password = signup_pass.Text;
                string hashedPass = HashPassword(password);
                string sql = "INSERT INTO employees (name, surname, citizen_no, username, passwords) VALUES (@name, @surname, @citizenno, @username, @password)";
                SqlCommand com = new SqlCommand(sql, con);
                com.Parameters.AddWithValue("@username", signup_username.Text);
                com.Parameters.AddWithValue("@password", hashedPass);
                com.Parameters.AddWithValue("@name", signup_name.Text);
                com.Parameters.AddWithValue("@surname", signup_surname.Text);
                com.Parameters.AddWithValue("@citizenno", signup_citizenno.Text);
                com.ExecuteNonQuery();

                MessageBox.Show("Kayıt başarıyla tamamlandı.", "Başarılı", MessageBoxButtons.OK);
                Hide();
            }

            con.Close();
        }
    }
}
