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

            // Hai chuỗi kết nối cho hai Service Name khác nhau
            string connStringXepdb1 = $"User Id={username};Password={password};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xepdb1)));";
            string connStringXe = $"User Id={username};Password={password};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xe)));";

            OracleConnection conn = null;
            string finalConnString = "";

            try
            {
                // 1. Thử kết nối với xepdb1 trước (Dành cho U1-U8 và OLS)
                conn = new OracleConnection(connStringXepdb1);
                conn.Open();
                finalConnString = connStringXepdb1;
            }
            catch (OracleException ex) when (ex.Number == 1017 || ex.Number == 12514 || ex.Number == 1016)
            {
                // Nếu sai user/pass hoặc service name (ORA-01017, ORA-12514), thử kết nối với xe (Dành cho C##ADMIN, NV001, v.v.)
                try
                {
                    if (conn != null) { conn.Dispose(); }
                    conn = new OracleConnection(connStringXe);
                    conn.Open();
                    finalConnString = connStringXe;
                }
                catch (OracleException ex2)
                {
                    if (conn != null) { conn.Dispose(); }
                    if (ex2.Number == 1017 || ex2.Number == 1016)
                    {
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi Đăng Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu ({ex2.Number}):\n{ex2.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
            }
            catch (OracleException ex)
            {
                if (conn != null) { conn.Dispose(); }
                MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu ({ex.Number}):\n{ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                if (conn != null) { conn.Dispose(); }
                MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Nếu đến đây tức là conn đã Open thành công
            try
            {
                string role = GetUserRole(conn, username);
                string fullName = GetUserFullName(conn, username, role);

                GlobalConnectionString = finalConnString;
                LoggedInUsername = username;
                UserRole = role;
                DoctorName = fullName;
                DoctorConnection = conn; // Giữ connection này luôn

                this.DialogResult = DialogResult.OK; 
                this.Close();
            }
            catch (Exception ex)
            {
                conn.Dispose();
                MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Lấy role của người dùng (DOCTOR, DISPATCHER, TECHNICIAN, PATIENT, ADMIN, etc.)
        /// </summary>
        private string GetUserRole(OracleConnection conn, string username)
        {
            try
            {
                string normalizedUser = username.Trim().ToUpper();
                string accountCode = StripCommonUserPrefix(normalizedUser);

                // Kiểm tra nếu là tài khoản OLS từ U1 đến U8
                if (accountCode == "U1" || accountCode == "U2" || accountCode == "U3" || accountCode == "U4" ||
                    accountCode == "U5" || accountCode == "U6" || accountCode == "U7" || accountCode == "U8")
                {
                    return "OLS_USER";
                }

                if (accountCode.Contains("ADMIN"))
                    return "ADMIN";

                string roleFromSession = GetRoleFromSession(conn);
                if (!string.IsNullOrEmpty(roleFromSession))
                    return roleFromSession;

                // Tra cứu vai trò từ bảng NHANVIEN
                string query = "SELECT VAITRO FROM ADMIN_PHANHE1.NHANVIEN WHERE MANV = :manv";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":manv", OracleDbType.Varchar2).Value = accountCode;
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
                        else if (vaitro == "Kỹ thuật viên")
                        {
                            return "TECHNICIAN";
                        }
                    }
                }

                using (OracleCommand cmd = new OracleCommand(
                    "SELECT COUNT(*) FROM ADMIN_PHANHE1.BENHNHAN WHERE MABN = :mabn", conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":mabn", OracleDbType.Varchar2).Value = accountCode;
                    object result = cmd.ExecuteScalar();
                    if (result != null && Convert.ToInt32(result) > 0)
                        return "PATIENT";
                }
                
                return "USER";
            }
            catch
            {
                string accountCode = StripCommonUserPrefix(username.Trim().ToUpper());
                if (accountCode.Contains("ADMIN"))
                    return "ADMIN";
                if (accountCode.StartsWith("BN"))
                    return "PATIENT";
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
                string accountCode = StripCommonUserPrefix(username.Trim().ToUpper());

                if (role == "PATIENT")
                {
                    string patientQuery = "SELECT TENBN FROM ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN";
                    using (OracleCommand cmd = new OracleCommand(patientQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null) return result.ToString() ?? username;
                    }
                }

                if (role == "TECHNICIAN")
                {
                    string technicianQuery = "SELECT HOTEN FROM ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN";
                    using (OracleCommand cmd = new OracleCommand(technicianQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null) return result.ToString() ?? username;
                    }
                }

                if (accountCode == "U1" || accountCode == "U2" || accountCode == "U3" || accountCode == "U4" ||
                    accountCode == "U5" || accountCode == "U6" || accountCode == "U7" || accountCode == "U8")
                {
                    switch (accountCode)
                    {
                        case "U1": return "Giám đốc U1 (Toàn viện)";
                        case "U2": return "Lãnh đạo U2 (TM - HCM)";
                        case "U3": return "Lãnh đạo U3 (TK - HN)";
                        case "U4": return "Nhân viên U4 (TK - HCM)";
                        case "U5": return "Nhân viên U5 (TM - HCM)";
                        case "U6": return "Lãnh đạo U6 (TM - HCM)";
                        case "U7": return "Lãnh đạo U7 (Toàn viện)";
                        case "U8": return "Nhân viên U8 (TH - HN)";
                        default: return username;
                    }
                }

                string query = "SELECT HOTEN FROM ADMIN_PHANHE1.NHANVIEN WHERE MANV = :manv";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":manv", OracleDbType.Varchar2).Value = accountCode;
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

        private static string StripCommonUserPrefix(string username)
        {
            return username.StartsWith("C##") ? username.Substring(3) : username;
        }

        private string GetRoleFromSession(OracleConnection conn)
        {
            const string query = @"
                SELECT ROLE
                FROM SESSION_ROLES
                WHERE ROLE IN ('ROLE_YSI_BACSI', 'ROLE_DIEUPHOIVIEN', 'ROLE_KYTHUATVIEN', 'ROLE_BENHNHAN')";

            using (OracleCommand cmd = new OracleCommand(query, conn))
            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string role = reader.GetString(0);
                    if (role == "ROLE_YSI_BACSI")
                        return "DOCTOR";
                    if (role == "ROLE_DIEUPHOIVIEN")
                        return "DISPATCHER";
                    if (role == "ROLE_KYTHUATVIEN")
                        return "TECHNICIAN";
                    if (role == "ROLE_BENHNHAN")
                        return "PATIENT";
                }
            }

            return "";
        }
    }
}
