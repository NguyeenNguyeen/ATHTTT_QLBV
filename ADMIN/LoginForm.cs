using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public partial class LoginForm : Form
    {
        public static string GlobalConnectionString { get; private set; } = "";
        public static string LoggedInUsername { get; private set; } = "";
        public static string UserRole { get; private set; } = "";
        public static string DoctorName { get; private set; } = "";
        public static OracleConnection? DoctorConnection { get; private set; }

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
            string connString = $"User Id={username};Password={password};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";

            try
            {
                // Thử kết nối với Oracle
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.Open();
                    // Lấy role và tên người dùng
                    string role = GetUserRole(conn, username);
                    string fullName = GetUserFullName(conn, username, role);

                    // Lưu thông tin vào static properties
                    GlobalConnectionString = connString;
                    LoggedInUsername = username;
                    UserRole = role;
                    DoctorName = fullName;
                    DoctorConnection = new OracleConnection(connString); // Tạo connection mới để sử dụng sau này
                    DoctorConnection.Open();

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

        /// <summary>
        /// Lấy role của người dùng (DOCTOR, NURSE, ADMIN, etc.)
        /// </summary>
        private string GetUserRole(OracleConnection conn, string username)
        {
            try
            {
                string manv = username.ToUpper();
                if (manv.StartsWith("C##"))
                {
                    manv = manv.Substring(3);
                }

                if (manv.Contains("ADMIN"))
                    return "ADMIN";

                // Tra cứu vai trò từ bảng NHANVIEN
                string query = "SELECT VAITRO FROM ADMIN_PHANHE1.NHANVIEN WHERE MANV = :manv";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":manv", OracleDbType.Varchar2).Value = manv;
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string vaitro = result.ToString();
                        if (vaitro == "Bác sĩ/Y sĩ" || vaitro == "Lãnh đạo khoa")
                        {
                            return "DOCTOR";
                        }
                        else if (vaitro == "Điều phối viên")
                        {
                            return "DISPATCHER";
                        }
                    }
                }
                
                // Fallback cũ nếu không tìm thấy
                if (manv.StartsWith("NV"))
                    return "DOCTOR";
                
                return "USER";
            }
            catch
            {
                if (username.ToUpper().Contains("ADMIN"))
                    return "ADMIN";
                if (username.ToUpper().Contains("NV"))
                    return "DOCTOR";
                return "USER";
            }
        }

        /// <summary>
        /// Lấy tên đầy đủ người dùng từ bảng NHANVIEN
        /// </summary>
        private string GetUserFullName(OracleConnection conn, string username, string role)
        {
            try
            {
                string manv = username.ToUpper();
                if (manv.StartsWith("C##"))
                {
                    manv = manv.Substring(3);
                }

                string query = "SELECT HOTEN FROM ADMIN_PHANHE1.NHANVIEN WHERE MANV = :manv";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":manv", OracleDbType.Varchar2).Value = manv;
                    object result = cmd.ExecuteScalar();
                    if (result != null) return result.ToString() ?? username;
                }
                return username;
            }
            catch
            {
                return username;
            }
        }
    }
}
