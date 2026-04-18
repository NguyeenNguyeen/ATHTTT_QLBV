using System.Data;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = "User Id=SYSTEM;Password=oracle;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SID=xe)));";
        private static readonly Regex OracleIdentifierRegex = new("^[A-Za-z][A-Za-z0-9_$#]*$", RegexOptions.Compiled);

        // Method ghi log lỗi vào file txt
        private void LogError(string errorMessage)
        {
            try
            {
                string logPath = Path.Combine(Application.StartupPath, "error_log.txt");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logContent = $"[{timestamp}] {errorMessage}\n{new string('=', 80)}\n";
                
                File.AppendAllText(logPath, logContent);
                MessageBox.Show($"Lỗi đã được ghi vào: {logPath}", "Log");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi ghi log: {ex.Message}", "Lỗi Ghi Log");
            }
        }

        public Form1()
        {
            InitializeComponent();
            TestConnection();
        }

        // Method test connection khi form khởi tạo
        private void TestConnection()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();
                    conn.Close();
                    MessageBox.Show("✓ Kết nối Oracle thành công!", "Thành Công");
                }
            }
            catch (Exception ex)
            {
                string errorDetail = $"CONNECTION TEST ERROR:\n\n" +
                    $"Connection String: {ConnectionString}\n\n" +
                    $"Error Type: {ex.GetType().Name}\n" +
                    $"Error Message: {ex.Message}\n\n" +
                    $"Stack Trace:\n{ex.StackTrace}";
                
                LogError(errorDetail);
                MessageBox.Show($"✗ Lỗi kết nối:\n{ex.Message}", "Lỗi Kết Nối");
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            AddUserForm form = new AddUserForm();
            form.ShowDialog();
        }

        private void btnEditUser_Click(object sender, EventArgs e)
        {
            string selectedType = cbUserType.SelectedItem?.ToString() ?? "";
            string userId = "";

            if (string.IsNullOrEmpty(selectedType))
            {
                MessageBox.Show("Vui lòng chọn loại danh sách (Nhân viên / Bệnh nhân) và tìm kiếm trước khi sửa.", "Thông báo");
                return;
            }

            if (dgvUsers.CurrentRow != null && dgvUsers.CurrentRow.Index >= 0)
            {
                userId = dgvUsers.CurrentRow.Cells[0].Value?.ToString()?.Trim() ?? "";
            }

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Vui lòng chọn 1 dòng trên lưới dữ liệu để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AddUserForm form = new AddUserForm();
            form.SetupEditMode(selectedType, userId, dgvUsers.CurrentRow);
            form.ShowDialog();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtUserName.Text.Trim();
            string selectedType = cbUserType.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedType))
            {
                MessageBox.Show("Vui lòng chọn loại danh sách muốn xem (Nhân viên / Bệnh nhân).", "Thông báo");
                return;
            }

            // Sử dụng chuỗi kết nối ADMIN_PHANHE1 để truy xuất (tránh lỗi Timeout như trước đó)
            string connectionString = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (selectedType == "Nhân viên")
                        {
                            if (string.IsNullOrEmpty(searchText))
                            {
                                cmd.CommandText = "ADMIN_PHANHE1.SP_XEM_ALL_NHANVIEN";
                            }
                            else
                            {
                                cmd.CommandText = "ADMIN_PHANHE1.SP_XEM_MOT_NHANVIEN";
                                cmd.Parameters.Add("p_MANV", OracleDbType.Varchar2).Value = searchText;
                            }
                        }
                        else if (selectedType == "Bệnh nhân")
                        {
                            if (string.IsNullOrEmpty(searchText))
                            {
                                cmd.CommandText = "ADMIN_PHANHE1.SP_XEM_ALL_BENHNHAN";
                            }
                            else
                            {
                                cmd.CommandText = "ADMIN_PHANHE1.SP_XEM_MOT_BENHNHAN";
                                cmd.Parameters.Add("p_MABN", OracleDbType.Varchar2).Value = searchText;
                            }
                        }

                        // Tham số OUT kiểu RefCursor để lấy bản ghi dữ liệu trả về C#
                        OracleParameter pCursor = new OracleParameter("p_CURSOR", OracleDbType.RefCursor);
                        pCursor.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pCursor);

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgvUsers.DataSource = dt;
                        }
                    }
                }
            }
            catch (OracleException oex)
            {
                string logFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Oracle_Error_Log.txt");
                System.IO.File.WriteAllText(logFile, $"Lỗi tìm kiếm {selectedType}\nMã: {oex.Number}\nLỗi: {oex.Message}");
                MessageBox.Show($"Lỗi Oracle ({oex.Number}): {oex.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            string selectedType = cbUserType.SelectedItem?.ToString();
            string userId = txtUserName.Text.Trim();

            if (string.IsNullOrEmpty(selectedType))
            {
                MessageBox.Show("Vui lòng chọn loại danh sách muốn xóa (Nhân viên / Bệnh nhân).", "Thông báo");
                return;
            }

            // Nếu người dùng không chỉ định mã trong ô txtUserName, thử lấy ở dòng đang chọn trên DataGridView
            if (string.IsNullOrEmpty(userId))
            {
                if (dgvUsers.CurrentRow != null && dgvUsers.CurrentRow.Index >= 0)
                {
                    // Cột đầu tiên (chỉ số 0) thường là "Mã NV" / "Mã BN"
                    userId = dgvUsers.CurrentRow.Cells[0].Value?.ToString()?.Trim() ?? "";
                }
            }

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Vui lòng nhập mã user hoặc chọn 1 dòng trên lưới dữ liệu để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị Pop-up xác nhận
            DialogResult dialogResult = MessageBox.Show($"Bạn có chắc chắn muốn xóa {selectedType.ToLower()} có mã: {userId} hay không?\nMọi dữ liệu liên quan sẽ bị xóa.", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                string connectionString = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";

                try
                {
                    using (OracleConnection conn = new OracleConnection(connectionString))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.StoredProcedure;

                            if (selectedType == "Nhân viên")
                            {
                                cmd.CommandText = "ADMIN_PHANHE1.SP_XOA_NHANVIEN";
                                cmd.Parameters.Add("p_MANV", OracleDbType.Varchar2).Value = userId;
                            }
                            else if (selectedType == "Bệnh nhân")
                            {
                                cmd.CommandText = "ADMIN_PHANHE1.SP_XOA_BENHNHAN";
                                cmd.Parameters.Add("p_MABN", OracleDbType.Varchar2).Value = userId;
                            }

                            // Thực thi hàm xóa
                            cmd.ExecuteNonQuery();

                            MessageBox.Show($"Xóa thành công {selectedType.ToLower()}: {userId}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Sau khi xóa xong, xóa ô tìm kiếm (nếu đang chứa chính user đó) và tự động tải lại danh sách
                            if (txtUserName.Text.Trim() == userId)
                            {
                                txtUserName.Text = "";
                            }
                            btnSearch_Click(sender, e);
                        }
                    }
                }
                catch (OracleException oex)
                {
                    string logFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Oracle_Error_Log.txt");
                    System.IO.File.WriteAllText(logFile, $"Lỗi xóa {selectedType}\nMã: {oex.Number}\nLỗi: {oex.Message}");
                    MessageBox.Show($"Lỗi Oracle ({oex.Number}): {oex.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLoadPrivInfo_Click(object sender, EventArgs e)
        {
            string privType = cbPrivilegeType.SelectedItem?.ToString() ?? "";
            string searchUser = txtSearchPrivUser.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(privType))
            {
                MessageBox.Show("Vui lòng chọn loại quyền cần xem (Trên bảng / Trên cột).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (privType == "Xem quyền trên bảng")
                        {
                            cmd.CommandText = "ADMIN_PHANHE1.SP_XEM_QUYEN_ALL_USER";
                        }
                        else if (privType == "Xem quyền trên cột")
                        {
                            cmd.CommandText = "ADMIN_PHANHE1.SP_XEM_QUYEN_COT_ALL_USER";
                        }
                        else if (privType == "Xem quyền trên view")
                        {
                            cmd.CommandText = "ADMIN_PHANHE1.SP_XEM_QUYEN_VIEW_ALL_USER";
                        }
                        else if (privType == "Xem quyền trên procedure/function")
                        {
                            cmd.CommandText = "ADMIN_PHANHE1.SP_XEM_QUYEN_PROC_ALL_USER";
                        }

                        OracleParameter pCursor = new OracleParameter("p_CURSOR", OracleDbType.RefCursor);
                        pCursor.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pCursor);

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // Lọc dữ liệu theo mã user nếu người dùng có nhập tìm kiếm
                            if (!string.IsNullOrEmpty(searchUser))
                            {
                                // Chú ý: Tên cột phải khớp với alias trả về từ Procedure, ở đây là 'Tên Tài Khoản / Role'
                                dt.DefaultView.RowFilter = $"[Tên Tài Khoản / Role] LIKE '%{searchUser}%'";
                                dgvPrivInfo.DataSource = dt.DefaultView;
                            }
                            else
                            {
                                dgvPrivInfo.DataSource = dt;
                            }
                        }
                    }
                }
            }
            catch (OracleException oex)
            {
                MessageBox.Show($"Lỗi Oracle ({oex.Number}): {oex.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sự kiện cấp quyền (Nhiệm vụ 3)
        /// Gọi procedure ADMIN_PHANHE1.SP_GRANT_PRIVILEGE để cấp quyền cho User/Role
        /// </summary>
        private void btnGrantExecute_Click(object sender, EventArgs e)
        {
            // Kiểm tra input
            string grantee = cbGrantGrantee.SelectedItem?.ToString()?.Trim() ?? "";
            string privilege = GetSelectedPrivileges(); // Hàm lấy quyền từ CheckedListBox
            string objectName = cbGrantObjectName.SelectedItem?.ToString()?.Trim() ?? "";

            if (string.IsNullOrEmpty(grantee))
            {
                MessageBox.Show("Vui lòng chọn User/Role cần cấp quyền.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(privilege))
            {
                MessageBox.Show("Vui lòng chọn loại quyền (SELECT, INSERT, UPDATE, DELETE, EXECUTE).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(objectName))
            {
                MessageBox.Show("Vui lòng chọn đối tượng cơ sở dữ liệu (Bảng/Procedure/...).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy danh sách cột được tick (nếu có)
            string columns = null;
            if ((privilege == "SELECT" || privilege == "UPDATE") && clbGrantColumns.Items.Count > 0)
            {
                var selectedColumns = new List<string>();
                foreach (int index in clbGrantColumns.CheckedIndices)
                {
                    selectedColumns.Add(clbGrantColumns.Items[index]?.ToString() ?? "");
                }

                if (selectedColumns.Count > 0)
                {
                    columns = string.Join(", ", selectedColumns);
                }
            }

            // Xử lý GRANT OPTION
            bool grantOption = chkGrantWithOption.Checked;
            
            // Nếu cbGrantGrantee là một Role, ép grantOption về false (Oracle limitation)
            if (grantee.ToUpper().Contains("ROLE"))
            {
                grantOption = false;
            }

            // Kết nối Oracle
            string connectionString = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "ADMIN_PHANHE1.SP_GRANT_PRIVILEGE";

                        // Thêm các tham số
                        cmd.Parameters.Add("p_GRANTEE", OracleDbType.Varchar2).Value = grantee;
                        cmd.Parameters.Add("p_PRIVILEGE", OracleDbType.Varchar2).Value = privilege;
                        cmd.Parameters.Add("p_OBJECT_NAME", OracleDbType.Varchar2).Value = objectName;
                        
                        // Nếu không có cột hoặc quyền không phải SELECT/UPDATE, truyền null
                        if (!string.IsNullOrEmpty(columns))
                        {
                            cmd.Parameters.Add("p_COLUMNS", OracleDbType.Varchar2).Value = columns;
                        }
                        else
                        {
                            cmd.Parameters.Add("p_COLUMNS", OracleDbType.Varchar2).Value = DBNull.Value;
                        }

                        cmd.Parameters.Add("p_GRANT_OPTION", OracleDbType.Int32).Value = grantOption ? 1 : 0;

                        // Thực thi procedure
                        cmd.ExecuteNonQuery();

                        // Thành công
                        MessageBox.Show(
                            $"✓ Cấp quyền thành công!\n\n" +
                            $"User/Role: {grantee}\n" +
                            $"Quyền: {privilege}\n" +
                            $"Đối tượng: {objectName}" +
                            (columns != null ? $"\nCột: {columns}" : "") +
                            $"\nWith Grant Option: {(grantOption ? "Có" : "Không")}",
                            "Cấp Quyền Thành Công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        // Reload tab thông tin quyền
                        if (tabMain != null && tabMain.TabPages.Count > 4)
                        {
                            btnLoadPrivInfo_Click(sender, e);
                        }

                        // Xóa input sau khi thành công
                        cbGrantGrantee.SelectedIndex = -1;
                        cbGrantObjectType.SelectedIndex = -1;
                        cbGrantObjectName.SelectedIndex = -1;
                        chkGrantWithOption.Checked = false;
                        clbGrantPrivileges.ClearSelected();
                        clbGrantColumns.ClearSelected();
                    }
                }
            }
            catch (OracleException oex)
            {
                string errorMsg = $"Lỗi Oracle ({oex.Number}):\n{oex.Message}";
                
                // Log lỗi ra file
                try
                {
                    string logFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Oracle_Error_Log.txt");
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string logContent = $"[{timestamp}] GRANT PRIVILEGE ERROR\n" +
                        $"User/Role: {grantee}\n" +
                        $"Quyền: {privilege}\n" +
                        $"Đối tượng: {objectName}\n" +
                        $"Lỗi: {errorMsg}\n" +
                        $"{new string('=', 80)}\n";
                    
                    System.IO.File.AppendAllText(logFile, logContent);
                }
                catch { }

                MessageBox.Show(errorMsg, "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Lấy quyền được chọn từ clbGrantPrivileges (CheckedListBox)
        /// </summary>
        private string GetSelectedPrivileges()
        {
            var selectedPrivileges = new List<string>();
            foreach (int index in clbGrantPrivileges.CheckedIndices)
            {
                selectedPrivileges.Add(clbGrantPrivileges.Items[index]?.ToString() ?? "");
            }
            return selectedPrivileges.Count > 0 ? selectedPrivileges[0] : ""; // Lấy quyền đầu tiên nếu cần
        }

        public Form1()
        {
            InitializeComponent();
        }
    }
}
