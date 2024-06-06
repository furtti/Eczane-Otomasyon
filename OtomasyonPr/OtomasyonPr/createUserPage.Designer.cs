namespace OtomasyonPr
{
    partial class createUserPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            signup_username = new TextBox();
            label1 = new Label();
            signup_pass = new TextBox();
            signup_checkpass = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            signup_name = new TextBox();
            signup_citizenno = new MaskedTextBox();
            signup_surname = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            button1.Location = new Point(24, 307);
            button1.Name = "button1";
            button1.Size = new Size(245, 41);
            button1.TabIndex = 0;
            button1.Text = "Kullanıcı Oluştur";
            button1.TextAlign = ContentAlignment.TopCenter;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // signup_username
            // 
            signup_username.Location = new Point(24, 142);
            signup_username.Multiline = true;
            signup_username.Name = "signup_username";
            signup_username.Size = new Size(210, 34);
            signup_username.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(24, 111);
            label1.Name = "label1";
            label1.Size = new Size(120, 28);
            label1.TabIndex = 3;
            label1.Text = "Kullanıcı Adı";
            // 
            // signup_pass
            // 
            signup_pass.Location = new Point(24, 219);
            signup_pass.Multiline = true;
            signup_pass.Name = "signup_pass";
            signup_pass.PasswordChar = '●';
            signup_pass.Size = new Size(210, 34);
            signup_pass.TabIndex = 4;
            // 
            // signup_checkpass
            // 
            signup_checkpass.Location = new Point(251, 219);
            signup_checkpass.Multiline = true;
            signup_checkpass.Name = "signup_checkpass";
            signup_checkpass.PasswordChar = '●';
            signup_checkpass.Size = new Size(210, 34);
            signup_checkpass.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(24, 188);
            label2.Name = "label2";
            label2.Size = new Size(51, 28);
            label2.TabIndex = 6;
            label2.Text = "Şifre";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(250, 188);
            label3.Name = "label3";
            label3.Size = new Size(108, 28);
            label3.TabIndex = 7;
            label3.Text = "Şifre Tekrar";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(250, 111);
            label6.Name = "label6";
            label6.Size = new Size(65, 28);
            label6.TabIndex = 16;
            label6.Text = "TC No";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(250, 34);
            label7.Name = "label7";
            label7.Size = new Size(80, 28);
            label7.TabIndex = 15;
            label7.Text = "Soyisim";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F);
            label8.Location = new Point(24, 34);
            label8.Name = "label8";
            label8.Size = new Size(47, 28);
            label8.TabIndex = 12;
            label8.Text = "İsim";
            // 
            // signup_name
            // 
            signup_name.Location = new Point(24, 65);
            signup_name.Multiline = true;
            signup_name.Name = "signup_name";
            signup_name.Size = new Size(210, 34);
            signup_name.TabIndex = 11;
            // 
            // signup_citizenno
            // 
            signup_citizenno.Location = new Point(251, 142);
            signup_citizenno.Mask = "00000000000";
            signup_citizenno.Name = "signup_citizenno";
            signup_citizenno.Size = new Size(210, 27);
            signup_citizenno.TabIndex = 17;
            signup_citizenno.ValidatingType = typeof(int);
            // 
            // signup_surname
            // 
            signup_surname.Location = new Point(251, 65);
            signup_surname.Multiline = true;
            signup_surname.Name = "signup_surname";
            signup_surname.Size = new Size(210, 34);
            signup_surname.TabIndex = 18;
            // 
            // createUserPage
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 396);
            Controls.Add(signup_surname);
            Controls.Add(signup_citizenno);
            Controls.Add(label6);
            Controls.Add(label7);
            Controls.Add(label8);
            Controls.Add(signup_name);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(signup_checkpass);
            Controls.Add(signup_pass);
            Controls.Add(label1);
            Controls.Add(signup_username);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "createUserPage";
            RightToLeft = RightToLeft.No;
            ShowIcon = false;
            Text = "Kullanıcı Oluştur";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox signup_username;
        private Label label1;
        private TextBox signup_pass;
        private TextBox signup_checkpass;
        private Label label2;
        private Label label3;
        private Label label6;
        private Label label7;
        private Label label8;
        private TextBox signup_name;
        private MaskedTextBox signup_citizenno;
        private TextBox signup_surname;
    }
}