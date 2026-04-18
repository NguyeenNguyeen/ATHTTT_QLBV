using System.Data;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = "User Id=SYSTEM;Password=oracle;Data Source=localhost:1521/orcl21";
        private static readonly Regex OracleIdentifierRegex = new("^[A-Za-z][A-Za-z0-9_$#]*$", RegexOptions.Compiled);

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

        public Form1()
        {
            InitializeComponent();
            SetupGrantTab();
        }
        
        // --- CÁC THÀNH PHẦN GIAO DIỆN ĐỘNG CHO TAB GRANT ---
        private TextBox txtTargetGrantee;
        private Label lblTargetGrantee;
        private ComboBox cbGrantColumnName;
        private Label lblGrantColumnName;
        private TextBox txtRoleToGrant;
        private Label lblRoleToGrant;

        private void SetupGrantTab()
        {
            // 1. Thêm TextBox để nhập Grantee (Người nhận quyền / Role)
            lblTargetGrantee = new Label() { Text = "Đối tượng nhận (User/Role):", Location = new Point(10, 50), AutoSize = true };
            txtTargetGrantee = new TextBox() { Location = new Point(160, 48), Size = new Size(200, 23) };
            tabGrant.Controls.Add(lblTargetGrantee);
            tabGrant.Controls.Add(txtTargetGrantee);

            // 2. Thêm TextBox để nhập Role khi chọn Grant Role
            lblRoleToGrant = new Label() { Text = "Nhập Role cần cấp:", Location = new Point(10, 80), AutoSize = true, Visible = false };
            txtRoleToGrant = new TextBox() { Location = new Point(160, 78), Size = new Size(200, 23), Visible = false };
            tabGrant.Controls.Add(lblRoleToGrant);
            tabGrant.Controls.Add(txtRoleToGrant);

            // 3. Thêm ComboBox đổ danh sách cột (khi chọn Cột)
            lblGrantColumnName = new Label() { Text = "Chọn Cột:", Location = new Point(680, 20), AutoSize = true, Visible = false };
            cbGrantColumnName = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(740, 20), Size = new Size(180, 23), Visible = false };
            tabGrant.Controls.Add(lblGrantColumnName);
            tabGrant.Controls.Add(cbGrantColumnName);

            // Gán 2 options cho cbGrantGrantee
            cbGrantGrantee.Items.Clear();
            cbGrantGrantee.Items.AddRange(new object[] { "Grant Quyền", "Grant Role" });
            cbGrantGrantee.SelectedIndexChanged += cbGrantGrantee_SelectedIndexChanged;

            // Xóa Items mặc định của cbGrantObjectType (bảng, view, procedure/function) -> Thay bằng: Bảng, Cột, View, Procedure/Function
            cbGrantObjectType.Items.Clear();
            cbGrantObjectType.Items.AddRange(new object[] { "Bảng", "Cột", "View", "Procedure/Function" });
            cbGrantObjectType.SelectedIndexChanged += cbGrantObjectType_SelectedIndexChanged;

            cbGrantObjectName.SelectedIndexChanged += cbGrantObjectName_SelectedIndexChanged;
            btnGrantExecute.Click += btnGrantExecute_Click;
            
            // Đẩy các control cũ xuống dưới một chút cho khỏi đè lên txtTargetGrantee
            clbGrantPrivileges.Location = new Point(10, 110);
            chkGrantWithOption.Location = new Point(200, 110);
            btnGrantExecute.Location = new Point(200, 140);
        }

        private void cbGrantGrantee_SelectedIndexChanged(object sender, EventArgs e)
        {
            string option = cbGrantGrantee.SelectedItem?.ToString() ?? "";
            if (option == "Grant Quyền")
            {
                lblTargetGrantee.Text = "Đối tượng nhận (User/Role):";
                cbGrantObjectType.Visible = true;
                cbGrantObjectName.Visible = true;
                clbGrantPrivileges.Visible = true;
                chkGrantWithOption.Visible = true;
                chkGrantWithOption.Text = "WITH GRANT OPTION";
                
                lblRoleToGrant.Visible = false;
                txtRoleToGrant.Visible = false;

                if (cbGrantObjectType.SelectedItem != null && cbGrantObjectType.SelectedItem.ToString() == "Cột")
                {
                    lblGrantColumnName.Visible = true;
                    cbGrantColumnName.Visible = true;
                }
            }
            else if (option == "Grant Role")
            {
                lblTargetGrantee.Text = "Nhập User nhận Role:";
                cbGrantObjectType.Visible = false;
                cbGrantObjectName.Visible = false;
                clbGrantPrivileges.Visible = false;
                
                lblGrantColumnName.Visible = false;
                cbGrantColumnName.Visible = false;

                lblRoleToGrant.Visible = true;
                txtRoleToGrant.Visible = true;

                chkGrantWithOption.Visible = true;
                chkGrantWithOption.Text = "WITH ADMIN OPTION";
            }
        }

        private void cbGrantObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string objType = cbGrantObjectType.SelectedItem?.ToString() ?? "";
            cbGrantObjectName.Items.Clear();
            cbGrantColumnName.Items.Clear();
            
            if (objType == "Cột")
            {
                lblGrantColumnName.Visible = true;
                cbGrantColumnName.Visible = true;
                LoadDataToComboBox("ADMIN_PHANHE1.SP_GET_LIST_TABLES", cbGrantObjectName, "Tên Bảng");
            }
            else
            {
                lblGrantColumnName.Visible = false;
                cbGrantColumnName.Visible = false;
                
                if (objType == "Bảng")
                {
                    LoadDataToComboBox("ADMIN_PHANHE1.SP_GET_LIST_TABLES", cbGrantObjectName, "Tên Bảng");
                }
                else if (objType == "View")
                {
                    LoadDataToComboBox("ADMIN_PHANHE1.SP_GET_LIST_VIEWS", cbGrantObjectName, "Tên View");
                }
                else if (objType == "Procedure/Function")
                {
                    LoadDataToComboBox("ADMIN_PHANHE1.SP_GET_LIST_PROCS_FUNCS", cbGrantObjectName, "Tên Đối Tượng");
                }
            }
        }

        private void cbGrantObjectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string objType = cbGrantObjectType.SelectedItem?.ToString() ?? "";
            string objName = cbGrantObjectName.SelectedItem?.ToString() ?? "";
            
            if (objType == "Cột" && !string.IsNullOrEmpty(objName))
            {
                cbGrantColumnName.Items.Clear();
                string connStr = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";
                try
                {
                    using (OracleConnection conn = new OracleConnection(connStr))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_GET_COLUMNS", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("p_TABLE_NAME", OracleDbType.Varchar2).Value = objName;
                            
                            OracleParameter pCursor = new OracleParameter("p_CURSOR", OracleDbType.RefCursor);
                            pCursor.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(pCursor);

                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cbGrantColumnName.Items.Add(reader["COLUMN_NAME"].ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải danh sách cột: {ex.Message}");
                }
            }
        }

        private void LoadDataToComboBox(string procName, ComboBox cb, string columnName)
        {
            string connStr = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand(procName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        OracleParameter pCursor = new OracleParameter("p_CURSOR", OracleDbType.RefCursor);
                        pCursor.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pCursor);

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cb.Items.Add(reader[columnName].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load danh sách: {ex.Message}");
            }
        }

        private void btnGrantExecute_Click(object sender, EventArgs e)
        {
            string grantTarget = txtTargetGrantee.Text.Trim();
            if (string.IsNullOrEmpty(grantTarget))
            {
                MessageBox.Show("Vui lòng nhập đối tượng nhận!", "Cảnh báo");
                return;
            }

            string option = cbGrantGrantee.SelectedItem?.ToString() ?? "";
            
            if (option == "Grant Role")
            {
                string roleToGrant = txtRoleToGrant.Text.Trim();
                if (string.IsNullOrEmpty(roleToGrant))
                {
                    MessageBox.Show("Vui lòng nhập Role cần cấp!", "Cảnh báo");
                    return;
                }
                int adminOption = chkGrantWithOption.Checked ? 1 : 0;
                ExecuteGrantRole(roleToGrant, grantTarget, adminOption);
            }
            else if (option == "Grant Quyền")
            {
                string objType = cbGrantObjectType.SelectedItem?.ToString() ?? "";
                string objName = cbGrantObjectName.SelectedItem?.ToString() ?? "";
                
                if (string.IsNullOrEmpty(objType) || string.IsNullOrEmpty(objName))
                {
                    MessageBox.Show("Vui lòng chọn loại đối tượng và tên đối tượng!", "Cảnh báo");
                    return;
                }

                if (clbGrantPrivileges.CheckedItems.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một quyền!", "Cảnh báo");
                    return;
                }

                if (objType == "View")
                {
                    foreach (var priv in clbGrantPrivileges.CheckedItems)
                    {
                        ExecuteGrantView(objName, grantTarget, priv.ToString());
                    }
                }
                else if (objType == "Procedure/Function")
                {
                    if (!clbGrantPrivileges.CheckedItems.Contains("EXECUTE"))
                    {
                        MessageBox.Show("Phân quyền cho Proc/Func chỉ hỗ trợ quyền EXECUTE!", "Cảnh báo");
                        return;
                    }
                    ExecuteGrantProcFunc(objName, grantTarget);
                }
                else // Bảng và Cột chung Procedure SP_GRANT_PRIVILEGE
                {
                    string columns = "";
                    if (objType == "Cột")
                    {
                        if (cbGrantColumnName.SelectedItem == null)
                        {
                            MessageBox.Show("Vui lòng chọn cột cần cấp quyền!", "Cảnh báo");
                            return;
                        }
                        columns = cbGrantColumnName.SelectedItem.ToString();
                    }

                    int grantOption = chkGrantWithOption.Checked ? 1 : 0;

                    foreach (var priv in clbGrantPrivileges.CheckedItems)
                    {
                        ExecuteGrantTable(grantTarget, priv.ToString(), objName, columns, grantOption);
                    }
                }
            }
        }

        private void ExecuteGrantRole(string roleName, string userName, int adminOption)
        {
            string connStr = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_GRANT_ROLE_TO_USER", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_ROLE_NAME", OracleDbType.Varchar2).Value = roleName;
                        cmd.Parameters.Add("p_USER_NAME", OracleDbType.Varchar2).Value = userName;
                        cmd.Parameters.Add("p_ADMIN_OPTION", OracleDbType.Int32).Value = adminOption;
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Cấp Role thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi cấp Role: {ex.Message}", "Lỗi"); }
        }

        private void ExecuteGrantView(string viewName, string grantee, string privilege)
        {
            string connStr = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_GRANT_VIEW_PRIV", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_VIEW_NAME", OracleDbType.Varchar2).Value = viewName;
                        cmd.Parameters.Add("p_GRANTEE", OracleDbType.Varchar2).Value = grantee;
                        cmd.Parameters.Add("p_PRIVILEGE", OracleDbType.Varchar2).Value = privilege;
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"Cấp quyền {privilege} trên View {viewName} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi cấp quyền View: {ex.Message}", "Lỗi"); }
        }

        private void ExecuteGrantProcFunc(string objName, string grantee)
        {
            string connStr = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_GRANT_PROC_FUNC_PRIV", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_OBJECT_NAME", OracleDbType.Varchar2).Value = objName;
                        cmd.Parameters.Add("p_GRANTEE", OracleDbType.Varchar2).Value = grantee;
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"Cấp quyền EXECUTE trên Proc/Func {objName} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi cấp quyền Proc/Func: {ex.Message}", "Lỗi"); }
        }

        private void ExecuteGrantTable(string grantee, string privilege, string objName, string columns, int grantOption)
        {
            string connStr = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_GRANT_PRIVILEGE", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_GRANTEE", OracleDbType.Varchar2).Value = grantee;
                        cmd.Parameters.Add("p_PRIVILEGE", OracleDbType.Varchar2).Value = privilege;
                        cmd.Parameters.Add("p_OBJECT_NAME", OracleDbType.Varchar2).Value = objName;
                        cmd.Parameters.Add("p_COLUMNS", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(columns) ? DBNull.Value : (object)columns;
                        cmd.Parameters.Add("p_GRANT_OPTION", OracleDbType.Int32).Value = grantOption;
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"Cấp quyền {privilege} trên {objName}{(string.IsNullOrEmpty(columns) ? "" : $" cột {columns}")} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi cấp quyền: {ex.Message}", "Lỗi"); }
        }

    }
}
