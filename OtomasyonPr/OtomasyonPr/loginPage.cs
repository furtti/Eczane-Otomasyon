using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

namespace OtomasyonPr
{
    public partial class loginPage : Form
    {
        string connectionString = "Data Source=Aspire-7;Initial Catalog=EczaneOtomasyon;Integrated Security=True";

        public loginPage()
        {
            InitializeComponent();
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


        private void loginPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string password = guna2TextBox2.Text;
            string hashedPass = HashPassword(password);
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT * FROM employees WHERE username=@username AND passwords=@password";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", guna2TextBox1.Text);
                        cmd.Parameters.AddWithValue("@password", hashedPass);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                mainPage mainForm = new mainPage();
                                mainForm.FormClosed += (s, args) => this.Close();
                                mainForm.Show();

                                this.Hide();
                            }
                            else
                            {
                                label3.Visible = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Baðlantý hatasý: " + ex.Message);
                }
            }
        }

        private async void guna2Button2_Click(object sender, EventArgs e)
        {
            createUserPage mainForm = new createUserPage();
            mainForm.FormClosed += (s, args) => this.Close();
            mainForm.Show();

            this.Hide();
        }
    }
}
