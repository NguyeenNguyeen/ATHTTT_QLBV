using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public partial class Form1 : Form
    {
        private static readonly Regex OracleIdentifierRegex = new("^[A-Za-z][A-Za-z0-9_$#]*$", RegexOptions.Compiled);
        private PermissionManager _permManager = new PermissionManager();
        private TabPage tabTask5;
        private DataGridView dgvAuditLog;
        private TextBox txtAuditUser;
        private TextBox txtAuditObject;
        private ComboBox cbAuditType;
        private NumericUpDown nudAuditLimit;
        private Button btnLoadAudit;
        private Button btnTestConnection;
        private Button btnBackupDataPump;
        private TextBox txtRestoreFile;
        private TextBox txtRestoreTable;
        private Button btnRestoreDataPump;
        private ComboBox cbFlashbackTable;
        private DateTimePicker dtpFlashbackDate;
        private TextBox txtFlashbackTime;
        private Button btnRestoreFlashback;
        private Button btnRunRmanBackup;
        private Button btnRunRmanRestore;
        private TextBox txtTask5Log;

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
            string connectionString = OracleConnectionConfig.BuildAdminConnectionString();

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
                string connectionString = OracleConnectionConfig.BuildAdminConnectionString();

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

            string connectionString = OracleConnectionConfig.BuildAdminConnectionString();

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
                    if (clbGrantPrivileges.GetItemChecked(i))
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
            LoadGranteeList();
            InitializePermissionLevelControls();
            InitializeTask5Controls();
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

        private void InitializeTask5Controls()
        {
            tabTask5 = new TabPage("Audit + Backup/Recover");
            tabMain.Controls.Add(tabTask5);

            var topPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 210,
                ColumnCount = 6,
                RowCount = 4,
                Padding = new Padding(11),
            };
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44));

            cbAuditType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            cbAuditType.Items.AddRange(new object[] { "ALL", "STANDARD", "FINE-GRAINED" });
            cbAuditType.SelectedIndex = 0;
            txtAuditUser = new TextBox { PlaceholderText = "User, VD: C##NV001", Dock = DockStyle.Fill };
            txtAuditObject = new TextBox { PlaceholderText = "Object, VD: HSBA", Dock = DockStyle.Fill };
            nudAuditLimit = new NumericUpDown { Minimum = 10, Maximum = 1000, Value = 100, Increment = 10, Dock = DockStyle.Fill };
            btnLoadAudit = new Button { Text = "Tai audit log", BackColor = Color.LightSkyBlue, Dock = DockStyle.Fill };
            btnLoadAudit.Click += btnLoadAudit_Click;
            btnTestConnection = new Button { Text = "Test connection", BackColor = Color.LightGreen, Dock = DockStyle.Fill };
            btnTestConnection.Click += btnTestConnection_Click;

            txtRestoreFile = new TextBox { PlaceholderText = "BV_PHANHE1_YYYYMMDD_HH24MISS.dmp", Dock = DockStyle.Fill };
            txtRestoreTable = new TextBox { PlaceholderText = "Table optional, VD: HSBA", Dock = DockStyle.Fill };
            btnBackupDataPump = new Button { Text = "Backup Data Pump", BackColor = Color.LightGreen, Dock = DockStyle.Fill };
            btnBackupDataPump.Click += btnBackupDataPump_Click;
            btnRestoreDataPump = new Button { Text = "Restore Data Pump", BackColor = Color.LightCoral, Dock = DockStyle.Fill };
            btnRestoreDataPump.Click += btnRestoreDataPump_Click;

            cbFlashbackTable = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            cbFlashbackTable.Items.AddRange(new object[] { "ADMIN_PHANHE1.HSBA", "ADMIN_PHANHE1.HSBA_DV", "ADMIN_PHANHE1.DONTHUOC" });
            cbFlashbackTable.SelectedIndex = 0;
            dtpFlashbackDate = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Dock = DockStyle.Fill };
            txtFlashbackTime = new TextBox { Text = DateTime.Now.ToString("HH:mm:ss"), Dock = DockStyle.Fill };
            btnRestoreFlashback = new Button { Text = "Flashback restore", BackColor = Color.LightCoral, Dock = DockStyle.Fill };
            btnRestoreFlashback.Click += btnRestoreFlashback_Click;
            btnRunRmanBackup = new Button { Text = "RMAN backup .bat", Dock = DockStyle.Fill };
            btnRunRmanBackup.Click += (s, e) => RunBatchScript("run_rman_backup.bat");
            btnRunRmanRestore = new Button { Text = "RMAN restore .bat", BackColor = Color.LightCoral, Dock = DockStyle.Fill };
            btnRunRmanRestore.Click += (s, e) => RunBatchScript("run_rman_restore.bat");

            AddTask5Row(topPanel, 0, "Loai audit", cbAuditType, "Nguoi dung", txtAuditUser, "Ket noi", btnTestConnection);
            AddTask5Row(topPanel, 1, "Doi tuong", txtAuditObject, "So dong", nudAuditLimit, "Audit", btnLoadAudit);
            AddTask5Row(topPanel, 2, "File restore", txtRestoreFile, "Bang restore", txtRestoreTable, "Data Pump", CreateTask5Flow(btnBackupDataPump, btnRestoreDataPump));
            AddTask5Row(topPanel, 3, "Bang flashback", cbFlashbackTable, "Ngay gio an toan", CreateTask5Flow(dtpFlashbackDate, txtFlashbackTime), "Recover", CreateTask5Flow(btnRestoreFlashback, btnRunRmanBackup, btnRunRmanRestore));

            dgvAuditLog = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersWidth = 51,
            };

            txtTask5Log = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
            };

            tabTask5.Controls.Add(dgvAuditLog);
            tabTask5.Controls.Add(txtTask5Log);
            tabTask5.Controls.Add(topPanel);
        }

        private static void AddTask5Row(TableLayoutPanel panel, int row, string label1, Control control1, string label2, Control control2, string label3, Control control3)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            panel.Controls.Add(new Label { Text = label1, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, row);
            panel.Controls.Add(control1, 1, row);
            panel.Controls.Add(new Label { Text = label2, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 2, row);
            panel.Controls.Add(control2, 3, row);
            panel.Controls.Add(new Label { Text = label3, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 4, row);
            panel.Controls.Add(control3, 5, row);
        }

        private static FlowLayoutPanel CreateTask5Flow(params Control[] controls)
        {
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = false, WrapContents = false };
            foreach (Control control in controls)
            {
                control.Width = 150;
                control.Height = 32;
                flow.Controls.Add(control);
            }
            return flow;
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                OracleHelper.TestAdminConnection();
                WriteTask5Log("Ket noi ADMIN_PHANHE1 thanh cong.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi ket noi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WriteTask5Log("Ket noi that bai: " + ex.Message);
            }
        }

        private void btnLoadAudit_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"
SELECT *
FROM (
    SELECT ""LOAI_AUDIT"", ""NGUOI_DUNG"", ""THOI_GIAN"", ""HANH_DONG"",
           ""DOI_TUONG"", ""CAU_LENH_SQL"", ""CHI_TIET_TRANG_THAI""
    FROM ADMIN_PHANHE1.V_ALL_AUDIT_LOG
    WHERE (:audit_type = 'ALL' OR ""LOAI_AUDIT"" = :audit_type)
      AND (:audit_user IS NULL OR UPPER(""NGUOI_DUNG"") LIKE '%' || UPPER(:audit_user) || '%')
      AND (:audit_object IS NULL OR UPPER(""DOI_TUONG"") LIKE '%' || UPPER(:audit_object) || '%')
)
WHERE ROWNUM <= :row_limit";

                DataTable dt = new DataTable();
                using (OracleConnection conn = new OracleConnection(OracleHelper.AdminConnectionString))
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("audit_type", OracleDbType.Varchar2).Value = cbAuditType.SelectedItem?.ToString() ?? "ALL";
                    cmd.Parameters.Add("audit_user", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(txtAuditUser.Text) ? DBNull.Value : txtAuditUser.Text.Trim();
                    cmd.Parameters.Add("audit_object", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(txtAuditObject.Text) ? DBNull.Value : txtAuditObject.Text.Trim();
                    cmd.Parameters.Add("row_limit", OracleDbType.Int32).Value = (int)nudAuditLimit.Value;
                    conn.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                dgvAuditLog.DataSource = dt;
                WriteTask5Log($"Da tai {dt.Rows.Count} dong audit log.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi tai audit log", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WriteTask5Log("Tai audit log that bai: " + ex.Message);
            }
        }

        private void btnBackupDataPump_Click(object sender, EventArgs e)
        {
            try
            {
                OracleHelper.ExecuteNonQuery("ADMIN_PHANHE1.SP_BACKUP_DATAPUMP", commandType: CommandType.StoredProcedure);
                WriteTask5Log("Da gui job backup Data Pump. File .dmp nam trong Oracle DIRECTORY BACKUP_DIR.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi backup Data Pump", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WriteTask5Log("Backup Data Pump that bai: " + ex.Message);
            }
        }

        private void btnRestoreDataPump_Click(object sender, EventArgs e)
        {
            string fileName = txtRestoreFile.Text.Trim();
            string tableName = txtRestoreTable.Text.Trim();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Nhap ten file .dmp can restore.", "Thieu thong tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Restore tu file {fileName}?", "Xac nhan restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                OracleHelper.ExecuteNonQuery(
                    "ADMIN_PHANHE1.SP_RESTORE_DATAPUMP",
                    new Dictionary<string, object>
                    {
                        ["p_filename"] = fileName,
                        ["p_table_name"] = string.IsNullOrWhiteSpace(tableName) ? DBNull.Value : tableName
                    },
                    CommandType.StoredProcedure);
                WriteTask5Log("Da gui job restore Data Pump.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi restore Data Pump", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WriteTask5Log("Restore Data Pump that bai: " + ex.Message);
            }
        }

        private void btnRestoreFlashback_Click(object sender, EventArgs e)
        {
            string tableName = cbFlashbackTable.SelectedItem?.ToString() ?? "";
            string safeTime = $"{dtpFlashbackDate.Value:yyyy-MM-dd} {txtFlashbackTime.Text.Trim()}";
            if (!DateTime.TryParseExact(safeTime, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out _))
            {
                MessageBox.Show("Nhap gio theo dinh dang HH:mm:ss.", "Sai dinh dang", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Flashback {tableName} ve {safeTime}?", "Xac nhan flashback", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                OracleHelper.ExecuteNonQuery(
                    "ADMIN_PHANHE1.SP_RESTORE_FLASHBACK",
                    new Dictionary<string, object>
                    {
                        ["p_table_name"] = tableName,
                        ["p_safe_time"] = safeTime
                    },
                    CommandType.StoredProcedure);
                WriteTask5Log($"Da flashback {tableName} ve {safeTime}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi flashback", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WriteTask5Log("Flashback that bai: " + ex.Message);
            }
        }

        private void RunBatchScript(string scriptName)
        {
            string scriptPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, scriptName));
            if (!File.Exists(scriptPath))
                scriptPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "backup_restore", "window", scriptName));

            if (!File.Exists(scriptPath))
            {
                MessageBox.Show($"Khong tim thay {scriptName}. Hay copy file .bat vao thu muc chua ADMIN.exe hoac giu dung cau truc repo.", "Thieu file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = scriptPath,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(scriptPath) ?? AppContext.BaseDirectory
            });
            WriteTask5Log("Da mo script: " + scriptPath);
        }

        private void WriteTask5Log(string message)
        {
            txtTask5Log.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }
    }
}
