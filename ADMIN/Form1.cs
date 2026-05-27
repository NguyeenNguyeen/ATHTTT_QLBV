using System.Data;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public partial class Form1 : Form
    {
        private static readonly Regex OracleIdentifierRegex = new("^[A-Za-z][A-Za-z0-9_$#]*$", RegexOptions.Compiled);
        private static readonly Regex OracleObjectNameRegex = new("^[A-Za-z][A-Za-z0-9_$#]*(\\.[A-Za-z][A-Za-z0-9_$#]*)?$", RegexOptions.Compiled);
        private const string AdminUsername = "ADMIN_PHANHE1";
        private const string AdminPassword = "Admin@123456";
        private PermissionManager _permManager = new PermissionManager();

        private static string BuildAdminConnectionString()
        {
            return $"User Id={AdminUsername};Password={AdminPassword};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SID=xe)));";
        }

        private static string NormalizeRoleName(string input)
        {
            return (input ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static string NormalizeObjectName(string input)
        {
            return (input ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static bool RoleExists(OracleConnection conn, string roleName)
        {
            try
            {
                using (OracleCommand checkCmd = new OracleCommand("SELECT COUNT(1) FROM DBA_ROLES WHERE ROLE = :p_role", conn))
                {
                    checkCmd.BindByName = true;
                    checkCmd.Parameters.Add("p_role", OracleDbType.Varchar2).Value = roleName;
                    object result = checkCmd.ExecuteScalar() ?? 0;
                    return Convert.ToInt32(result) > 0;
                }
            }
            catch (OracleException oex) when (oex.Number == 1031)
            {
                return true;
            }
        }

        private static bool RoleHasTablePrivilege(OracleConnection conn, string roleName, string privilege, string tableName)
        {
            string owner = string.Empty;
            string objectName = tableName;

            if (tableName.Contains('.'))
            {
                string[] parts = tableName.Split('.');
                if (parts.Length == 2)
                {
                    owner = parts[0];
                    objectName = parts[1];
                }
            }

            string privilegeCondition = privilege == "ALL"
                ? "PRIVILEGE IN ('SELECT','INSERT','UPDATE','DELETE')"
                : "PRIVILEGE = :p_priv";

            string ownerCondition = string.IsNullOrEmpty(owner) ? string.Empty : " AND OWNER = :p_owner";
            string sql = $"SELECT COUNT(1) FROM DBA_TAB_PRIVS WHERE GRANTEE = :p_role AND TABLE_NAME = :p_table{ownerCondition} AND {privilegeCondition}";

            try
            {
                using (OracleCommand checkCmd = new OracleCommand(sql, conn))
                {
                    checkCmd.BindByName = true;
                    checkCmd.Parameters.Add("p_role", OracleDbType.Varchar2).Value = roleName;
                    checkCmd.Parameters.Add("p_table", OracleDbType.Varchar2).Value = objectName;

                    if (!string.IsNullOrEmpty(owner))
                    {
                        checkCmd.Parameters.Add("p_owner", OracleDbType.Varchar2).Value = owner;
                    }

                    if (privilege != "ALL")
                    {
                        checkCmd.Parameters.Add("p_priv", OracleDbType.Varchar2).Value = privilege;
                    }

                    object result = checkCmd.ExecuteScalar() ?? 0;
                    return Convert.ToInt32(result) > 0;
                }
            }
            catch (OracleException oex) when (oex.Number == 1031)
            {
                return true;
            }
        }

        private static bool ContainsInsufficientPrivileges(OracleException oex)
        {
            return oex.Number == 1031 || oex.Message.Contains("ORA-01031", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ContainsRoleNotFound(OracleException oex)
        {
            return oex.Number == 1435
                || oex.Number == 65048
                || oex.Message.Contains("ORA-01435", StringComparison.OrdinalIgnoreCase)
                || oex.Message.Contains("ORA-65048", StringComparison.OrdinalIgnoreCase)
                || oex.Message.Contains("user does not exist", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ContainsInvalidCommonRoleName(OracleException oex)
        {
            return oex.Number == 65096
                || oex.Message.Contains("ORA-65096", StringComparison.OrdinalIgnoreCase)
                || oex.Message.Contains("invalid common user or role name", StringComparison.OrdinalIgnoreCase);
        }

        private static void ExecuteCreateRoleAndGrantFallback(OracleConnection conn, string roleName, string privilege, string tableName)
        {
            try
            {
                using (OracleCommand createCmd = new OracleCommand($"CREATE ROLE {roleName}", conn))
                {
                    createCmd.CommandType = CommandType.Text;
                    createCmd.ExecuteNonQuery();
                }
            }
            catch (OracleException createEx) when (createEx.Number == 1921)
            {
            }

            using (OracleCommand grantCmd = new OracleCommand($"GRANT {privilege} ON {tableName} TO {roleName}", conn))
            {
                grantCmd.CommandType = CommandType.Text;
                grantCmd.ExecuteNonQuery();
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
            string connectionString = BuildAdminConnectionString();

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

        private void LoadRoleList(string? focusRoleName = null)
        {
            using (OracleConnection conn = new OracleConnection(BuildAdminConnectionString()))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_XEM_TAT_CA_ROLE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    OracleParameter pCursor = new OracleParameter("p_recordset", OracleDbType.RefCursor);
                    pCursor.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(pCursor);

                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dgvRoles.DataSource = null;
                        dgvRoles.Rows.Clear();
                        dgvRoles.Columns.Clear();

                        dgvRoles.AutoGenerateColumns = true;
                        dgvRoles.DataSource = dt;

                        if (!string.IsNullOrEmpty(focusRoleName) && dgvRoles.Rows.Count > 0)
                        {
                            for (int i = 0; i < dgvRoles.Rows.Count; i++)
                            {
                                string rowRole = dgvRoles.Rows[i].Cells[0].Value?.ToString()?.Trim().ToUpperInvariant() ?? string.Empty;
                                if (rowRole == focusRoleName)
                                {
                                    dgvRoles.ClearSelection();
                                    dgvRoles.Rows[i].Selected = true;
                                    if (dgvRoles.Rows[i].Cells.Count > 0)
                                    {
                                        dgvRoles.CurrentCell = dgvRoles.Rows[i].Cells[0];
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsRoleVisibleInGrid(string roleName)
        {
            foreach (DataGridViewRow row in dgvRoles.Rows)
            {
                if (row.IsNewRow || row.Cells.Count == 0)
                {
                    continue;
                }

                string gridRole = row.Cells[0].Value?.ToString()?.Trim() ?? string.Empty;
                if (string.Equals(gridRole, roleName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void btnLoadRoles_Click(object sender, EventArgs e)
        {
            try
            {
                LoadRoleList();

                if (dgvRoles.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu Role trả về từ procedure SP_XEM_TAT_CA_ROLE.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (OracleException oex)
            {
                MessageBox.Show($"Lỗi Oracle ({oex.Number}): {oex.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddRole_Click(object sender, EventArgs e)
        {
            string roleName = NormalizeRoleName(txtRoleName.Text);
            string privilege = cbRolePrivilege.SelectedItem?.ToString()?.Trim().ToUpperInvariant() ?? string.Empty;
            string tableName = NormalizeObjectName(txtRoleTableName.Text);
            string effectiveRoleName = roleName;
            bool usedCommonRolePrefix = false;

            if (string.IsNullOrEmpty(roleName))
            {
                MessageBox.Show("Vui lòng nhập tên Role.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(privilege))
            {
                MessageBox.Show("Vui lòng chọn quyền cần cấp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Vui lòng nhập tên bảng cần cấp quyền.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!OracleIdentifierRegex.IsMatch(roleName))
            {
                MessageBox.Show("Tên Role không hợp lệ. Chỉ dùng chữ cái, số và ký tự _ $ #, bắt đầu bằng chữ cái.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!OracleObjectNameRegex.IsMatch(tableName))
            {
                MessageBox.Show("Tên bảng không hợp lệ. Dùng định dạng TEN_BANG hoặc OWNER.TEN_BANG.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OracleConnection conn = new OracleConnection(BuildAdminConnectionString()))
                {
                    conn.Open();
                    try
                    {
                        using (OracleCommand cmd = new OracleCommand("SP_TAO_ROLE_VA_CAP_QUYEN", conn))
                        {
                            cmd.BindByName = true;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("p_ten_role", OracleDbType.Varchar2).Value = effectiveRoleName;
                            cmd.Parameters.Add("p_quyen", OracleDbType.Varchar2).Value = privilege;
                            cmd.Parameters.Add("p_ten_bang", OracleDbType.Varchar2).Value = tableName;

                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (OracleException procEx) when (ContainsInvalidCommonRoleName(procEx) && !effectiveRoleName.StartsWith("C##", StringComparison.OrdinalIgnoreCase))
                    {
                        effectiveRoleName = "C##" + effectiveRoleName;
                        usedCommonRolePrefix = true;

                        using (OracleCommand cmd = new OracleCommand("SP_TAO_ROLE_VA_CAP_QUYEN", conn))
                        {
                            cmd.BindByName = true;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("p_ten_role", OracleDbType.Varchar2).Value = effectiveRoleName;
                            cmd.Parameters.Add("p_quyen", OracleDbType.Varchar2).Value = privilege;
                            cmd.Parameters.Add("p_ten_bang", OracleDbType.Varchar2).Value = tableName;

                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (OracleException procEx) when (ContainsInsufficientPrivileges(procEx) || ContainsRoleNotFound(procEx))
                    {
                        try
                        {
                            ExecuteCreateRoleAndGrantFallback(conn, effectiveRoleName, privilege, tableName);
                        }
                        catch (OracleException fallbackEx) when (ContainsInvalidCommonRoleName(fallbackEx) && !effectiveRoleName.StartsWith("C##", StringComparison.OrdinalIgnoreCase))
                        {
                            effectiveRoleName = "C##" + effectiveRoleName;
                            usedCommonRolePrefix = true;
                            ExecuteCreateRoleAndGrantFallback(conn, effectiveRoleName, privilege, tableName);
                        }
                    }

                    bool roleExists = RoleExists(conn, effectiveRoleName);
                    bool roleGranted = RoleHasTablePrivilege(conn, effectiveRoleName, privilege, tableName);

                    if (!roleExists || !roleGranted)
                    {
                        try
                        {
                            ExecuteCreateRoleAndGrantFallback(conn, effectiveRoleName, privilege, tableName);
                        }
                        catch (OracleException retryEx) when (ContainsInsufficientPrivileges(retryEx) || ContainsRoleNotFound(retryEx))
                        {
                        }
                        catch (OracleException retryEx) when (ContainsInvalidCommonRoleName(retryEx) && !effectiveRoleName.StartsWith("C##", StringComparison.OrdinalIgnoreCase))
                        {
                            effectiveRoleName = "C##" + effectiveRoleName;
                            usedCommonRolePrefix = true;
                            ExecuteCreateRoleAndGrantFallback(conn, effectiveRoleName, privilege, tableName);
                        }

                        roleExists = RoleExists(conn, effectiveRoleName);
                        roleGranted = RoleHasTablePrivilege(conn, effectiveRoleName, privilege, tableName);
                    }

                    LoadRoleList(effectiveRoleName);
                    bool roleVisible = IsRoleVisibleInGrid(effectiveRoleName);

                    if (!roleExists || !roleGranted)
                    {
                        if (roleVisible)
                        {
                            MessageBox.Show($"Role {effectiveRoleName} đã hiển thị trong danh sách, nhưng chưa xác minh chắc chắn quyền {privilege} trên {tableName} từ view hệ thống. Vui lòng kiểm tra thêm ở tab Thông tin quyền.", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Không thể tạo/cấp quyền hoàn tất cho role {effectiveRoleName}. Cần kiểm tra quyền DB của {AdminUsername} (CREATE ROLE và quyền GRANT trên bảng mục tiêu).", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        return;
                    }

                    if (usedCommonRolePrefix)
                    {
                        MessageBox.Show($"Đã tạo role theo chuẩn common role: {effectiveRoleName} và cấp quyền {privilege} trên {tableName}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Đã tạo role (nếu chưa có) và cấp quyền {privilege} trên {tableName} cho {effectiveRoleName}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    txtRoleName.Text = "";
                    txtRoleTableName.Text = "";
                }
            }
            catch (OracleException oex)
            {
                MessageBox.Show($"Lỗi Oracle ({oex.Number}): {oex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteRole_Click(object sender, EventArgs e)
        {
            string roleName = NormalizeRoleName(txtRoleName.Text);

            if (string.IsNullOrEmpty(roleName) && dgvRoles.CurrentRow != null && dgvRoles.CurrentRow.Index >= 0)
            {
                roleName = NormalizeRoleName(dgvRoles.CurrentRow.Cells[0].Value?.ToString() ?? string.Empty);
                txtRoleName.Text = roleName;
            }

            if (string.IsNullOrEmpty(roleName))
            {
                MessageBox.Show("Vui lòng nhập tên role hoặc chọn role trên danh sách để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string effectiveRoleName = roleName;
            if (!effectiveRoleName.StartsWith("C##", StringComparison.OrdinalIgnoreCase) && IsRoleVisibleInGrid("C##" + effectiveRoleName))
            {
                effectiveRoleName = "C##" + effectiveRoleName;
            }

            DialogResult confirm = MessageBox.Show($"Bạn có chắc muốn xóa role {effectiveRoleName} không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (OracleConnection conn = new OracleConnection(BuildAdminConnectionString()))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("SP_XOA_ROLE", conn))
                    {
                        cmd.BindByName = true;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_ten_role", OracleDbType.Varchar2).Value = effectiveRoleName;
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadRoleList();

                if (IsRoleVisibleInGrid(effectiveRoleName))
                {
                    MessageBox.Show($"Procedure đã chạy nhưng role {effectiveRoleName} vẫn còn trong danh sách. Vui lòng kiểm tra quyền DROP ROLE hoặc phần EXCEPTION trong SP_XOA_ROLE.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show($"Đã xóa role {effectiveRoleName}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtRoleName.Text = string.Empty;
            }
            catch (OracleException oex)
            {
                if (oex.Number == 20003 || oex.Message.Contains("ORA-20003", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Role {effectiveRoleName} không tồn tại trong hệ thống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (oex.Number == 20004 || oex.Message.Contains("ORA-20004", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Procedure SP_XOA_ROLE trả về lỗi chi tiết:\n{oex.Message}", "Lỗi xóa role", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show($"Lỗi Oracle ({oex.Number}): {oex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string connectionString = BuildAdminConnectionString();

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

            string connectionString = BuildAdminConnectionString();

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

        private void LoadGranteeList()
        {
            try
            {
                var grantees = _permManager.GetAllGrantees();
                cbGrantGrantee.Items.Clear();
                cbRevokeGrantee.Items.Clear();
                foreach (var grantee in grantees)
                {
                    cbGrantGrantee.Items.Add(grantee);
                    cbRevokeGrantee.Items.Add(grantee);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách Grantee: {ex.Message}");
            }
        }

        private void LoadObjectNames(string objectType, ComboBox targetCombo)
        {
            try
            {
                var objects = _permManager.GetObjectNamesByType(objectType);
                targetCombo.Items.Clear();
                foreach (var obj in objects)
                    targetCombo.Items.Add(obj);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách đối tượng: {ex.Message}");
            }
        }

        private void LoadColumns(string tableName, CheckedListBox targetList)
        {
            try
            {
                var columns = _permManager.GetColumnsOfTable(tableName);
                targetList.Items.Clear();
                foreach (var col in columns)
                    targetList.Items.Add(col);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải cột: {ex.Message}");
            }
        }

        private void cbGrantObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = cbGrantObjectType.SelectedItem?.ToString() ?? "";
            LoadObjectNames(selectedType, cbGrantObjectName);
            clbGrantPrivileges.Items.Clear();

            // Ẩn checkbox phân quyền cấp cột nếu không phải TABLE/VIEW
            if (selectedType == "TABLE" || selectedType == "VIEW")
            {
                clbGrantPrivileges.Items.AddRange(new object[] { "SELECT", "INSERT", "UPDATE", "DELETE" });
                chkGrantColumnLevel.Visible = true;
                clbGrantColumns.Visible = chkGrantColumnLevel.Checked;
            }
            else if (selectedType == "PROCEDURE" || selectedType == "FUNCTION")
            {
                clbGrantPrivileges.Items.Add("EXECUTE");
                chkGrantColumnLevel.Visible = false;
                clbGrantColumns.Visible = false;
            }

            chkGrantColumnLevel.Checked = false; // Reset khi đổi object type
        }

        private void cbGrantObjectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string objType = cbGrantObjectType.SelectedItem?.ToString() ?? "";
            string objName = cbGrantObjectName.SelectedItem?.ToString() ?? "";

            // Chỉ load columns khi checkbox "phân quyền cấp cột" được check
            if (chkGrantColumnLevel.Checked && (objType == "TABLE" || objType == "VIEW") && !string.IsNullOrEmpty(objName))
                LoadColumns(objName, clbGrantColumns);
        }

        private void chkGrantColumnLevel_CheckedChanged(object sender, EventArgs e)
        {
            string objType = cbGrantObjectType.SelectedItem?.ToString() ?? "";
            string objName = cbGrantObjectName.SelectedItem?.ToString() ?? "";

            if (chkGrantColumnLevel.Checked)
            {
                // Hiện column list khi check
                clbGrantColumns.Visible = true;
                if (!string.IsNullOrEmpty(objName))
                    LoadColumns(objName, clbGrantColumns);
            }
            else
            {
                // Ẩn column list khi uncheck
                clbGrantColumns.Visible = false;
                clbGrantColumns.Items.Clear();
            }
        }

        private void clbGrantPrivileges_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Kiểm tra xem có SELECT hoặc UPDATE được chọn không
            bool hasSelectOrUpdate = false;

            for (int i = 0; i < clbGrantPrivileges.Items.Count; i++)
            {
                if (i == e.Index)
                {
                    // Item hiện tại sẽ được check/uncheck
                    if (e.NewValue == CheckState.Checked)
                    {
                        string itemText = clbGrantPrivileges.Items[i].ToString();
                        if (itemText == "SELECT" || itemText == "UPDATE")
                            hasSelectOrUpdate = true;
                    }
                }
                else
                {
                    // Kiểm tra item khác
                    if (clbGrantPrivileges.GetItemChecked(i)) ;
                    {
                        string itemText = clbGrantPrivileges.Items[i].ToString();
                        if (itemText == "SELECT" || itemText == "UPDATE")
                            hasSelectOrUpdate = true;
                    }
                }
            }

            // Enable/Disable checkbox "Phân quyền cấp cột"
            chkGrantColumnLevel.Enabled = hasSelectOrUpdate;

            // Nếu không có SELECT/UPDATE, tự động uncheck và ẩn column list
            if (!hasSelectOrUpdate)
            {
                chkGrantColumnLevel.Checked = false;
                clbGrantColumns.Visible = false;
                clbGrantColumns.Items.Clear();
            }
        }

        private void cbRevokeObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = cbRevokeObjectType.SelectedItem?.ToString() ?? "";
            LoadObjectNames(selectedType, cbRevokeObjectName);
            clbRevokePrivileges.Items.Clear();

            if (selectedType == "TABLE" || selectedType == "VIEW")
            {
                clbRevokePrivileges.Items.AddRange(new object[] { "SELECT", "INSERT", "UPDATE", "DELETE" });
                chkRevokeColumnLevel.Visible = true;
                clbRevokeColumns.Visible = chkRevokeColumnLevel.Checked;
            }
            else if (selectedType == "PROCEDURE" || selectedType == "FUNCTION")
            {
                clbRevokePrivileges.Items.Add("EXECUTE");
                chkRevokeColumnLevel.Visible = false;
                clbRevokeColumns.Visible = false;
            }

            chkRevokeColumnLevel.Checked = false;
        }

        private void cbRevokeObjectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string objType = cbRevokeObjectType.SelectedItem?.ToString() ?? "";
            string objName = cbRevokeObjectName.SelectedItem?.ToString() ?? "";

            if (chkRevokeColumnLevel.Checked && (objType == "TABLE" || objType == "VIEW") && !string.IsNullOrEmpty(objName))
                LoadColumns(objName, clbRevokeColumns);
        }

        private void chkRevokeColumnLevel_CheckedChanged(object sender, EventArgs e)
        {
            string objType = cbRevokeObjectType.SelectedItem?.ToString() ?? "";
            string objName = cbRevokeObjectName.SelectedItem?.ToString() ?? "";

            if (chkRevokeColumnLevel.Checked)
            {
                clbRevokeColumns.Visible = true;
                if (!string.IsNullOrEmpty(objName))
                    LoadColumns(objName, clbRevokeColumns);
            }
            else
            {
                clbRevokeColumns.Visible = false;
                clbRevokeColumns.Items.Clear();
            }
        }

        private void clbRevokePrivileges_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Kiểm tra xem có SELECT hoặc UPDATE được chọn không
            bool hasSelectOrUpdate = false;

            for (int i = 0; i < clbRevokePrivileges.Items.Count; i++)
            {
                if (i == e.Index)
                {
                    // Item hiện tại sẽ được check/uncheck
                    if (e.NewValue == CheckState.Checked)
                    {
                        string itemText = clbRevokePrivileges.Items[i].ToString();
                        if (itemText == "SELECT" || itemText == "UPDATE")
                            hasSelectOrUpdate = true;
                    }
                }
                else
                {
                    // Kiểm tra item khác
                    if (clbRevokePrivileges.GetItemChecked(i))
                    {
                        string itemText = clbRevokePrivileges.Items[i].ToString();
                        if (itemText == "SELECT" || itemText == "UPDATE")
                            hasSelectOrUpdate = true;
                    }
                }
            }

            // Enable/Disable checkbox "Phân quyền cấp cột"
            chkRevokeColumnLevel.Enabled = hasSelectOrUpdate;

            // Nếu không có SELECT/UPDATE, tự động uncheck và ẩn column list
            if (!hasSelectOrUpdate)
            {
                chkRevokeColumnLevel.Checked = false;
                clbRevokeColumns.Visible = false;
                clbRevokeColumns.Items.Clear();
            }
        }

        private void btnGrantExecute_Click(object sender, EventArgs e)
        {
            string grantee = cbGrantGrantee.SelectedItem?.ToString() ?? "";
            string objType = cbGrantObjectType.SelectedItem?.ToString() ?? "";
            string objName = cbGrantObjectName.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrEmpty(grantee) || string.IsNullOrEmpty(objType) || string.IsNullOrEmpty(objName))
            {
                MessageBox.Show("Vui lòng chọn Grantee, Loại đối tượng và Tên đối tượng", "Thông báo");
                return;
            }

            if (clbGrantPrivileges.CheckedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 quyền", "Thông báo");
                return;
            }

            // Kiểm tra: nếu check "Phân quyền cấp cột" thì phải chọn ít nhất 1 cột
            if (chkGrantColumnLevel.Checked && clbGrantColumns.CheckedItems.Count == 0)
            {
                MessageBox.Show("Bạn đã chọn 'Phân quyền cấp cột' nhưng chưa chọn cột nào.\n\nVui lòng:\n- Chọn cột cụ thể, hoặc\n- Bỏ check 'Phân quyền cấp cột' để cấp quyền trên toàn bảng", "Thông báo");
                return;
            }

            try
            {
                int successCount = 0;
                foreach (string privilege in clbGrantPrivileges.CheckedItems)
                {
                    string columns = "";

                    // Nếu cấp quyền cấp cột (chỉ cho SELECT/UPDATE)
                    if (chkGrantColumnLevel.Checked && (privilege == "SELECT" || privilege == "UPDATE"))
                    {
                        // Tạo danh sách cột cách nhau bằng dấu phẩy
                        columns = string.Join(",", clbGrantColumns.CheckedItems.Cast<string>());
                    }

                    // Gọi SP_GRANT_ANY_OBJECT - xử lý mọi loại object
                    _permManager.GrantPrivilege(grantee, privilege, objName, columns, chkGrantWithOption.Checked);
                    successCount++;
                }
                MessageBox.Show($"Cấp quyền thành công cho {grantee}! ({successCount} quyền)", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cấp quyền: {ex.Message}", "Lỗi");
            }
        }

        private void btnRevokeExecute_Click(object sender, EventArgs e)
        {
            string grantee = cbRevokeGrantee.SelectedItem?.ToString() ?? "";
            string objType = cbRevokeObjectType.SelectedItem?.ToString() ?? "";
            string objName = cbRevokeObjectName.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrEmpty(grantee) || string.IsNullOrEmpty(objType) || string.IsNullOrEmpty(objName))
            {
                MessageBox.Show("Vui lòng chọn Grantee, Loại đối tượng và Tên đối tượng", "Thông báo");
                return;
            }

            if (clbRevokePrivileges.CheckedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 quyền để thu hồi", "Thông báo");
                return;
            }

            // Kiểm tra: nếu check "Phân quyền cấp cột" thì phải chọn ít nhất 1 cột
            if (chkRevokeColumnLevel.Checked && clbRevokeColumns.CheckedItems.Count == 0)
            {
                MessageBox.Show("Bạn đã chọn 'Phân quyền cấp cột' nhưng chưa chọn cột nào.\n\nVui lòng:\n- Chọn cột cụ thể, hoặc\n- Bỏ check 'Phân quyền cấp cột' để thu hồi quyền trên toàn bảng", "Thông báo");
                return;
            }

            try
            {
                int successCount = 0;
                foreach (string privilege in clbRevokePrivileges.CheckedItems)
                {
                    _permManager.RevokePrivilege(grantee, privilege, objName);
                    successCount++;
                }
                MessageBox.Show($"Thu hồi quyền thành công từ {grantee}! ({successCount} quyền)", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thu hồi quyền: {ex.Message}", "Lỗi");
            }
        }

        public Form1()
        {
            InitializeComponent();
            if (cbRolePrivilege.Items.Count > 0)
            {
                cbRolePrivilege.SelectedIndex = 0;
            }
            LoadGranteeList();
            InitializePermissionLevelControls();
            try
            {
                LoadRoleList();
            }
            catch
            {
            }
        }

        private void InitializePermissionLevelControls()
        {
            // Ẩn checkbox phân quyền cấp cột ban đầu
            chkGrantColumnLevel.Visible = false;
            chkGrantColumnLevel.Checked = false;
            chkGrantColumnLevel.Enabled = false; // Disable cho đến khi SELECT/UPDATE được chọn
            clbGrantColumns.Visible = false;

            chkRevokeColumnLevel.Visible = false;
            chkRevokeColumnLevel.Checked = false;
            chkRevokeColumnLevel.Enabled = false; // Disable cho đến khi SELECT/UPDATE được chọn
            clbRevokeColumns.Visible = false;
        }
    }
}
