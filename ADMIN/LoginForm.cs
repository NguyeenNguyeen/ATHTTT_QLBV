using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public partial class LoginForm : Form
    {
        public static string GlobalConnectionString { get; private set; } = "";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Phân giải chuỗi kết nối dựa trên Tên đăng nhập & mật khẩu cung cấp
            string connString = $"User Id={username};Password={password};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SID=xe)));";

            try
            {
                // Thử kết nối với Oracle
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();
                    // Kết nối thành công -> Đăng nhập thành công
                    GlobalConnectionString = connString;
                    this.DialogResult = DialogResult.OK; 
                    this.Close();
                }
            }
            catch (OracleException ex)
            {
                if (ex.Number == 1017) // ORA-01017: invalid username/password; logon denied
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi Đăng Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu ({ex.Number}):\n{ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
