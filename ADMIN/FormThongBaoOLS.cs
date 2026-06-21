using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public class FormThongBaoOLS : Form
    {
        private OracleConnection _conn;
        private string _username;
        private string _displayName;

        private Label lblGreeting;
        private Button btnViewAnnouncements;
        private Button btnLogout;

        public FormThongBaoOLS(string username, string displayName, OracleConnection conn)
        {
            _username = username;
            _displayName = displayName;
            _conn = conn ?? throw new ArgumentNullException(nameof(conn), "Kết nối Oracle không hợp lệ.");

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Hệ thống Bệnh viện";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250); // Nền xám nhạt hiện đại

            // Header Panel
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            this.Controls.Add(topPanel);

            // ĐĂNG XUẤT Button
            btnLogout = new Button
            {
                Text = "ĐĂNG XUẤT",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(130, 45),
                Location = new Point(930, 17),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(231, 76, 60), // Nút đỏ
                ForeColor = Color.White
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;
            topPanel.Controls.Add(btnLogout);

            // Xem bảng "Thông báo" Button
            btnViewAnnouncements = new Button
            {
                Text = "XEM BẢNG THÔNG BÁO",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(220, 45),
                Location = new Point(690, 17),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219), // Nút xanh
                ForeColor = Color.White
            };
            btnViewAnnouncements.FlatAppearance.BorderSize = 0;
            btnViewAnnouncements.Click += BtnViewAnnouncements_Click;
            topPanel.Controls.Add(btnViewAnnouncements);

            // XIN CHÀO Label
            lblGreeting = new Label
            {
                Text = $"XIN CHÀO {_username.ToUpper()}!",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(480, 27),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            topPanel.Controls.Add(lblGreeting);
            
            // Add a separator line below the top panel
            Panel separator = new Panel
            {
                Dock = DockStyle.Top,
                Height = 2,
                BackColor = Color.FromArgb(220, 224, 229)
            };
            this.Controls.Add(separator);

            // Dòng chữ chào mừng ở giữa màn hình
            Label lblWelcome = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ BỆNH VIỆN",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(189, 195, 199),
                AutoSize = true,
                Location = new Point(250, 300),
                Anchor = AnchorStyles.None
            };
            this.Controls.Add(lblWelcome);
        }

        private void BtnViewAnnouncements_Click(object sender, EventArgs e)
        {
            FormChiTietThongBao formChiTiet = new FormChiTietThongBao(_conn);
            formChiTiet.ShowDialog();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có thực sự muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_conn != null && _conn.State == ConnectionState.Open)
                    {
                        _conn.Close();
                    }
                }
                catch { }
                
                this.Hide();
                using (LoginForm loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        if (LoginForm.UserRole == "DOCTOR")
                        {
                            FormBacSi formBacSi = new FormBacSi(LoginForm.LoggedInUsername, LoginForm.DoctorName, LoginForm.DoctorConnection);
                            formBacSi.ShowDialog();
                        }
                        else if (LoginForm.UserRole == "DISPATCHER")
                        {
                            FormDieuPhoiVien formDieuPhoiVien = new FormDieuPhoiVien(LoginForm.LoggedInUsername, LoginForm.DoctorName, LoginForm.DoctorConnection);
                            formDieuPhoiVien.ShowDialog();
                        }
                        else if (LoginForm.UserRole == "OLS_USER")
                        {
                            FormThongBaoOLS formOls = new FormThongBaoOLS(LoginForm.LoggedInUsername, LoginForm.DoctorName, LoginForm.DoctorConnection);
                            formOls.ShowDialog();
                        }
                        else
                        {
                            Form1 formAdmin = new Form1();
                            formAdmin.ShowDialog();
                        }
                    }
                }
                this.Close();
            }
        }
    }
}
