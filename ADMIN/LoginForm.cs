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
            bool connected = false;
            OracleException lastException = null;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Phân giải chuỗi kết nối dựa trên Tên đăng nhập & mật khẩu cung cấp
            // Thử cả hai chuỗi kết nối với fallback logic
            string[] connectionStrings = new string[]
            {
                $"User Id={username};Password={password};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));",
                $"User Id={username};Password={password};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SID=xe))));"
            };

            bool connected = false;
            OracleException lastException = null;

            foreach (string connString in connectionStrings)
            {
                try
                {
                    // Thử kết nối với Oracle
                    using (OracleConnection conn = new OracleConnection(connString))
                    {
                        conn.Open();
                        // Kết nối thành công -> Đăng nhập thành công
                        GlobalConnectionString = connString;
                        connected = true;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        break;
                    }
                }
                catch (OracleException ex)
                {
                    lastException = ex;
                    // Tiếp tục thử chuỗi kết nối tiếp theo
                }
                catch (Exception ex)
                {
                    // Tiếp tục thử chuỗi kết nối tiếp theo
                }
            }

            if (!connected)
            {
                if (lastException != null)
                {
                    if (lastException.Number == 1017) // ORA-01017: invalid username/password; logon denied
                    {
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi Đăng Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu ({lastException.Number}):\n{lastException.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình kết nối.", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
