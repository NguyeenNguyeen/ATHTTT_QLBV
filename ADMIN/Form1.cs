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

        public Form1()
        {
            InitializeComponent();
        }
    }
}
