using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public class FormThongBaoOLS : Form, ILogoutAwareForm
    {
        private OracleConnection _conn;
        private string _username;
        private string _displayName;

        private Label lblGreeting;
        private Button btnViewAnnouncements;
        private Button btnLogout;

        public bool LogoutRequested { get; private set; }

        public FormThongBaoOLS(string username, string displayName, OracleConnection conn)
        {
            _username = username;
            _displayName = displayName;
            _conn = conn ?? throw new ArgumentNullException(nameof(conn), "Kết nối Oracle không hợp lệ.");

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Hệ thống Bệnh viện";
            Size = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);

            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            Controls.Add(topPanel);

            btnLogout = new Button
            {
                Text = "Đăng xuất",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(130, 45),
                Location = new Point(930, 17),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;
            topPanel.Controls.Add(btnLogout);

            btnViewAnnouncements = new Button
            {
                Text = "Xem thông báo",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(220, 45),
                Location = new Point(690, 17),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White
            };
            btnViewAnnouncements.FlatAppearance.BorderSize = 0;
            btnViewAnnouncements.Click += BtnViewAnnouncements_Click;
            topPanel.Controls.Add(btnViewAnnouncements);

            lblGreeting = new Label
            {
                Text = $"Xin chào {_username.ToUpper()}!",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(480, 27),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            topPanel.Controls.Add(lblGreeting);

            Panel separator = new Panel
            {
                Dock = DockStyle.Top,
                Height = 2,
                BackColor = Color.FromArgb(220, 224, 229)
            };
            Controls.Add(separator);

            Label lblWelcome = new Label
            {
                Text = "HỆ THỐNG QUẢN LÝ BỆNH VIỆN",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(189, 195, 199),
                AutoSize = true,
                Location = new Point(250, 300),
                Anchor = AnchorStyles.None
            };
            Controls.Add(lblWelcome);
        }

        private void BtnViewAnnouncements_Click(object sender, EventArgs e)
        {
            FormChiTietThongBao formChiTiet = new FormChiTietThongBao(_conn);
            formChiTiet.ShowDialog();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            LogoutRequested = true;
            try
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch
            {
            }

            Close();
        }
    }
}
