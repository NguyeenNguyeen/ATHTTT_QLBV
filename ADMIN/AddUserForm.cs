using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public partial class AddUserForm : Form
    {
        private bool isEditMode = false;
        private string editUserId = "";

        public AddUserForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private string GetCellValue(DataGridViewRow row, params string[] possibleColumnNames)
        {
            if (row == null || row.DataGridView == null) return "";

            foreach (string colName in possibleColumnNames)
            {
                if (row.DataGridView.Columns.Contains(colName))
                    return row.Cells[colName].Value?.ToString();
            }

            foreach (DataGridViewColumn col in row.DataGridView.Columns)
            {
                string name = col.HeaderText ?? col.Name;
                foreach (string possible in possibleColumnNames)
                {
                    if (name.Replace(" ", "").Equals(possible.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
                        return row.Cells[col.Index].Value?.ToString();
                }
            }

            return "";
        }

        public void SetupEditMode(string userType, string userId, DataGridViewRow row)
        {
            isEditMode = true;
            editUserId = userId;

            // Set form title and button text
            this.Text = "Cập nhật thông tin " + userType;
            btnSave.Text = "Cập nhật";
            // Set combobox and disable
            cbAccountType.SelectedItem = userType;
            cbAccountType.Enabled = false;

            lblPassword.Text = "Mật khẩu:";
            txtPassword.PlaceholderText = "Để trống nếu không đổi";

            // Fill data depending on the type
            if (userType == "Bệnh nhân")
            {
                txtFullName.Text = GetCellValue(row, "Tên Bệnh Nhân", "TENBN");
                txtGender.Text = GetCellValue(row, "Phái", "PHAI");

                string dobStr = GetCellValue(row, "Ngày Sinh", "NGAYSINH");
                if (DateTime.TryParse(dobStr, out DateTime dob))
                    dtpDob.Value = dob;
                else if (DateTime.TryParseExact(dobStr?.Split(' ')[0], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dobExact))
                    dtpDob.Value = dobExact;

                txtCccd.Text = GetCellValue(row, "CCCD", "CMND", "CMND/CCCD");

                string diaChi = GetCellValue(row, "Địa Chỉ", "SONHA", "Địa Chỉ Cụ Thể");
                if (string.IsNullOrEmpty(diaChi))
                {
                    string soNha = GetCellValue(row, "Số Nhà", "SONHA");
                    string tenDuong = GetCellValue(row, "Tên Đường", "TENDUONG");
                    if (!string.IsNullOrEmpty(soNha))
                    {
                        txtHouseNumber.Text = soNha;
                        txtStreet.Text = tenDuong;
                    }
                }
                else
                {
                    string[] parts = diaChi.Split(new string[] { ", " }, 2, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        txtHouseNumber.Text = parts[0];
                        txtStreet.Text = parts[1];
                    }
                    else
                    {
                        txtHouseNumber.Text = diaChi;
                    }
                }

                txtDistrict.Text = GetCellValue(row, "Quận/Huyện", "QUANHUYEN");
                txtProvince.Text = GetCellValue(row, "Tỉnh/TP", "TINHTP");
                txtMedicalHistory.Text = GetCellValue(row, "Tiền Sử Bệnh", "TIENSUBENH");
                txtFamilyMedicalHistory.Text = GetCellValue(row, "TS Bệnh Gia Đình", "TIENSUBENHGD");
                txtDrugAllergy.Text = GetCellValue(row, "Dị Ứng Thuốc", "DIUNGTHUOC");
            }
            else if (userType == "Nhân viên")
            {
                txtFullName.Text = GetCellValue(row, "Họ Tên", "HOTEN");
                txtGender.Text = GetCellValue(row, "Phái", "PHAI");

                string dobStr = GetCellValue(row, "Ngày Sinh", "NGAYSINH");
                if (DateTime.TryParse(dobStr, out DateTime dob))
                    dtpDob.Value = dob;
                else if (DateTime.TryParseExact(dobStr?.Split(' ')[0], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dobExact2))
                    dtpDob.Value = dobExact2;

                txtCccd.Text = GetCellValue(row, "CMND/CCCD", "CMND", "CCCD", "Chứng minh nhân dân");
                txtHometown.Text = GetCellValue(row, "Quê Quán", "QUEQUAN");
                txtPhone.Text = GetCellValue(row, "Số ĐT", "SODT", "Số điện thoại");
                txtFacility.Text = GetCellValue(row, "Cơ Sở", "CS Y Tế", "Cơ Sở Y Tế", "COSO");
                txtRole.Text = GetCellValue(row, "Vai Trò", "VAITRO");
                txtSpecialty.Text = GetCellValue(row, "Chuyên Khoa", "CHUYENKHOA");
            }
        }

        private void InitializeCustomComponents()
        {
            cbAccountType.Items.Clear();
            cbAccountType.Items.AddRange(new string[] { "Nhân viên", "Bệnh nhân" });
            cbAccountType.SelectedIndex = -1;
            cbAccountType.SelectedIndexChanged += new System.EventHandler(this.cbAccountType_SelectedIndexChanged);

            // Initially hide all specific fields until selected
            SetPatientFieldsVisibility(false);
            SetEmployeeFieldsVisibility(false);
            SetCommonFieldsVisibility(false);
        }

        private void cbAccountType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string? selected = cbAccountType.SelectedItem?.ToString();

            if (selected == "Nhân viên")
            {
                SetCommonFieldsVisibility(true);
                SetPatientFieldsVisibility(false);
                SetEmployeeFieldsVisibility(true);
            }
            else if (selected == "Bệnh nhân")
            {
                SetCommonFieldsVisibility(true);
                SetEmployeeFieldsVisibility(false);
                SetPatientFieldsVisibility(true);
            }
            else
            {
                SetCommonFieldsVisibility(false);
                SetEmployeeFieldsVisibility(false);
                SetPatientFieldsVisibility(false);
            }
        }

        private void SetCommonFieldsVisibility(bool isVisible)
        {
            lblPassword.Visible = isVisible;
            txtPassword.Visible = isVisible;
            lblFullName.Visible = isVisible;
            txtFullName.Visible = isVisible;
            lblGender.Visible = isVisible;
            txtGender.Visible = isVisible;
            lblDob.Visible = isVisible;
            dtpDob.Visible = isVisible;
            lblCccd.Visible = isVisible;
            txtCccd.Visible = isVisible;
        }

        private void SetEmployeeFieldsVisibility(bool isVisible)
        {
            lblHometown.Visible = isVisible;
            txtHometown.Visible = isVisible;
            lblPhone.Visible = isVisible;
            txtPhone.Visible = isVisible;
            lblRole.Visible = isVisible;
            txtRole.Visible = isVisible;
            lblSpecialty.Visible = isVisible;
            txtSpecialty.Visible = isVisible;
            lblFacility.Visible = isVisible;
            txtFacility.Visible = isVisible;
        }

        private void SetPatientFieldsVisibility(bool isVisible)
        {
            lblHouseNumber.Visible = isVisible;
            txtHouseNumber.Visible = isVisible;
            lblStreet.Visible = isVisible;
            txtStreet.Visible = isVisible;
            lblDistrict.Visible = isVisible;
            txtDistrict.Visible = isVisible;
            lblProvince.Visible = isVisible;
            txtProvince.Visible = isVisible;
            lblMedicalHistory.Visible = isVisible;
            txtMedicalHistory.Visible = isVisible;
            lblFamilyMedicalHistory.Visible = isVisible;
            txtFamilyMedicalHistory.Visible = isVisible;
            lblDrugAllergy.Visible = isVisible;
            txtDrugAllergy.Visible = isVisible;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string? selectedType = cbAccountType.SelectedItem?.ToString();

            if (selectedType == "Bệnh nhân")
            {
                // Thay đổi Connection String. Quan trọng:
                // 1. Phải dùng địa chỉ IP 127.0.0.1 (chuẩn IPv4) thay vì "localhost" hay "DESKTOP-E7U6Q38"
                // 2. Chắc chắn sử dụng 'orcl21' làm Service Name.
                string connectionString = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";

                try
                {
                    using (OracleConnection conn = new OracleConnection(connectionString))
                    {
                        conn.Open();

                        if (isEditMode)
                        {
                            using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_SUA_BENHNHAN", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Trích xuất thông tin từ các TextBox UI
                                string tenBN = txtFullName.Text.Trim();
                                string phai = txtGender.Text.Trim();
                                DateTime ngaySinh = dtpDob.Value.Date;
                                string cccd = txtCccd.Text.Trim();
                                string soNha = txtHouseNumber.Text.Trim();
                                string tenDuong = txtStreet.Text.Trim();
                                string quanHuyen = txtDistrict.Text.Trim();
                                string tinhTP = txtProvince.Text.Trim();
                                string tienSuBenh = txtMedicalHistory.Text.Trim();
                                string tsBenhGD = txtFamilyMedicalHistory.Text.Trim();
                                string diUngThuoc = txtDrugAllergy.Text.Trim();
                                string matKhau = txtPassword.Text.Trim();

                                // Tham số SP_SUA_BENHNHAN
                                cmd.Parameters.Add("p_MABN", OracleDbType.Varchar2).Value = editUserId;
                                cmd.Parameters.Add("p_TENBN", OracleDbType.NVarchar2).Value = tenBN;
                                cmd.Parameters.Add("p_PHAI", OracleDbType.NVarchar2).Value = phai;
                                cmd.Parameters.Add("p_NGAYSINH", OracleDbType.Date).Value = ngaySinh;
                                cmd.Parameters.Add("p_CCCD", OracleDbType.Varchar2).Value = cccd;
                                cmd.Parameters.Add("p_SONHA", OracleDbType.NVarchar2).Value = soNha;
                                cmd.Parameters.Add("p_TENDUONG", OracleDbType.NVarchar2).Value = tenDuong;
                                cmd.Parameters.Add("p_QUANHUYEN", OracleDbType.NVarchar2).Value = quanHuyen;
                                cmd.Parameters.Add("p_TINHTP", OracleDbType.NVarchar2).Value = tinhTP;
                                cmd.Parameters.Add("p_TIENSUBENH", OracleDbType.NVarchar2).Value = tienSuBenh;
                                cmd.Parameters.Add("p_TIENSUBENHGD", OracleDbType.NVarchar2).Value = tsBenhGD;
                                cmd.Parameters.Add("p_DIUNGTHUOC", OracleDbType.NVarchar2).Value = diUngThuoc;
                                cmd.Parameters.Add("p_MATKHAU", OracleDbType.Varchar2).Value = matKhau;

                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Cập nhật thông tin bệnh nhân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close();
                            }
                        }
                        else
                        {
                            using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_TAO_BENHNHAN", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Trích xuất thông tin từ các TextBox UI
                                string tenBN = txtFullName.Text.Trim();
                                string phai = txtGender.Text.Trim();
                                DateTime ngaySinh = dtpDob.Value.Date;
                                string cccd = txtCccd.Text.Trim();
                                string soNha = txtHouseNumber.Text.Trim();
                                string tenDuong = txtStreet.Text.Trim();
                                string quanHuyen = txtDistrict.Text.Trim();
                                string tinhTP = txtProvince.Text.Trim();
                                string tienSuBenh = txtMedicalHistory.Text.Trim();
                                string tsBenhGD = txtFamilyMedicalHistory.Text.Trim();
                                string diUngThuoc = txtDrugAllergy.Text.Trim();
                                string matKhau = txtPassword.Text.Trim();

                                // Bổ sung các Tham số theo đúng thứ tự trong khai báo Procedure của Oracle
                                cmd.Parameters.Add("p_TENBN", OracleDbType.NVarchar2).Value = tenBN;
                                cmd.Parameters.Add("p_PHAI", OracleDbType.NVarchar2).Value = phai;
                                cmd.Parameters.Add("p_NGAYSINH", OracleDbType.Date).Value = ngaySinh;
                                cmd.Parameters.Add("p_CCCD", OracleDbType.Varchar2).Value = cccd;
                                cmd.Parameters.Add("p_SONHA", OracleDbType.NVarchar2).Value = soNha;
                                cmd.Parameters.Add("p_TENDUONG", OracleDbType.NVarchar2).Value = tenDuong;
                                cmd.Parameters.Add("p_QUANHUYEN", OracleDbType.NVarchar2).Value = quanHuyen;
                                cmd.Parameters.Add("p_TINHTP", OracleDbType.NVarchar2).Value = tinhTP;
                                cmd.Parameters.Add("p_TIENSUBENH", OracleDbType.NVarchar2).Value = tienSuBenh;
                                cmd.Parameters.Add("p_TIENSUBENHGD", OracleDbType.NVarchar2).Value = tsBenhGD;
                                cmd.Parameters.Add("p_DIUNGTHUOC", OracleDbType.NVarchar2).Value = diUngThuoc;
                                cmd.Parameters.Add("p_MATKHAU", OracleDbType.Varchar2).Value = matKhau;

                                // Tham số OUT để nhận mã bệnh nhân Oracle tự sinh
                                OracleParameter pMaBnOut = new OracleParameter("p_MABN_OUT", OracleDbType.Varchar2, 20);
                                pMaBnOut.Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(pMaBnOut);

                                // Thực thi Procedure
                                cmd.ExecuteNonQuery();

                                // Lấy mã bệnh nhân trả về
                                string newMaBN = pMaBnOut.Value.ToString();

                                MessageBox.Show($"Thêm bệnh nhân thành công!\n\nMã bệnh nhân mới: {newMaBN}\nTên tài khoản đăng nhập (Username): C##{newMaBN}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close(); // Đóng form sau khi thành công
                            }
                        }
                    }
                }
                catch (OracleException oex)
                {
                    string logFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Oracle_Error_Log.txt");
                    string fullError = $"--- ORACLE EXCEPTION ---\nThời gian: {DateTime.Now}\nMã lỗi: {oex.Number}\nThông điệp: {oex.Message}\nSource: {oex.Source}\n\nStackTrace:\n{oex.StackTrace}\n\nConnection String Used:\n{connectionString}";
                    System.IO.File.WriteAllText(logFile, fullError);
                    MessageBox.Show($"Lỗi Oracle ({oex.Number}): {oex.Message}\n\nĐã ghi chi tiết lỗi ra file Desktop\\Oracle_Error_Log.txt\nBạn hãy mở thư mục Desktop để xem file log này và copy gửi cho tôi nhé!", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    string logFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "System_Error_Log.txt");
                    string fullError = $"--- SYSTEM EXCEPTION ---\nThời gian: {DateTime.Now}\nThông điệp: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}";
                    System.IO.File.WriteAllText(logFile, fullError);
                    MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}\n\nĐã ghi chi tiết lỗi ra file {logFile}\nBạn hãy copy file đó cho tôi xem nhé!", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (selectedType == "Nhân viên")
            {
                string connectionString = @"User Id=ADMIN_PHANHE1;Password=Admin@123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl21)));";

                try
                {
                    using (OracleConnection conn = new OracleConnection(connectionString))
                    {
                        conn.Open();

                        if (isEditMode)
                        {
                            using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_SUA_NHANVIEN", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                string hoTen = txtFullName.Text.Trim();
                                string phai = txtGender.Text.Trim();
                                DateTime ngaySinh = dtpDob.Value.Date;
                                string cmnd = txtCccd.Text.Trim();
                                string queQuan = txtHometown.Text.Trim();
                                string soDT = txtPhone.Text.Trim();
                                string vaiTro = txtRole.Text.Trim();
                                string chuyenKhoa = txtSpecialty.Text.Trim();
                                string coSo = txtFacility.Text.Trim();
                                string matKhau = txtPassword.Text.Trim();

                                cmd.Parameters.Add("p_MANV", OracleDbType.Varchar2).Value = editUserId;
                                cmd.Parameters.Add("p_HOTEN", OracleDbType.NVarchar2).Value = hoTen;
                                cmd.Parameters.Add("p_PHAI", OracleDbType.NVarchar2).Value = phai;
                                cmd.Parameters.Add("p_NGAYSINH", OracleDbType.Date).Value = ngaySinh;
                                cmd.Parameters.Add("p_CMND", OracleDbType.Varchar2).Value = cmnd;
                                cmd.Parameters.Add("p_QUEQUAN", OracleDbType.NVarchar2).Value = queQuan;
                                cmd.Parameters.Add("p_SODT", OracleDbType.Varchar2).Value = soDT;
                                cmd.Parameters.Add("p_VAITRO", OracleDbType.NVarchar2).Value = vaiTro;
                                cmd.Parameters.Add("p_CHUYENKHOA", OracleDbType.NVarchar2).Value = chuyenKhoa;
                                cmd.Parameters.Add("p_COSO", OracleDbType.NVarchar2).Value = coSo;
                                cmd.Parameters.Add("p_MATKHAU", OracleDbType.Varchar2).Value = matKhau;

                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close();
                            }
                        }
                        else
                        {
                            using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_TAO_NHANVIEN", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Trích xuất thông tin
                                string hoTen = txtFullName.Text.Trim();
                                string phai = txtGender.Text.Trim();
                                DateTime ngaySinh = dtpDob.Value.Date;
                                string cmnd = txtCccd.Text.Trim(); // CMND lấy từ textBox CCCD
                                string queQuan = txtHometown.Text.Trim();
                                string soDT = txtPhone.Text.Trim();
                                string vaiTro = txtRole.Text.Trim();
                                string chuyenKhoa = txtSpecialty.Text.Trim();
                                string coSo = txtFacility.Text.Trim();
                                string matKhau = txtPassword.Text.Trim();

                                // Tham số đầu vào (phải đúng cấu trúc của SP_TAO_NHANVIEN)
                                cmd.Parameters.Add("p_HOTEN", OracleDbType.NVarchar2).Value = hoTen;
                                cmd.Parameters.Add("p_PHAI", OracleDbType.NVarchar2).Value = phai;
                                cmd.Parameters.Add("p_NGAYSINH", OracleDbType.Date).Value = ngaySinh;
                                cmd.Parameters.Add("p_CMND", OracleDbType.Varchar2).Value = cmnd;
                                cmd.Parameters.Add("p_QUEQUAN", OracleDbType.NVarchar2).Value = queQuan;
                                cmd.Parameters.Add("p_SODT", OracleDbType.Varchar2).Value = soDT;
                                cmd.Parameters.Add("p_VAITRO", OracleDbType.NVarchar2).Value = vaiTro;
                                cmd.Parameters.Add("p_CHUYENKHOA", OracleDbType.NVarchar2).Value = chuyenKhoa;
                                cmd.Parameters.Add("p_COSO", OracleDbType.NVarchar2).Value = coSo;
                                cmd.Parameters.Add("p_MATKHAU", OracleDbType.Varchar2).Value = matKhau;

                                // Tham số OUT hứng mã nhân viên sinh tự động
                                OracleParameter pMaNvOut = new OracleParameter("p_MANV_OUT", OracleDbType.Varchar2, 20);
                                pMaNvOut.Direction = ParameterDirection.Output;
                                cmd.Parameters.Add(pMaNvOut);

                                // Thực thi Procedure
                                cmd.ExecuteNonQuery();

                                // Lấy mã trả về
                                string newMaNV = pMaNvOut.Value.ToString();

                                MessageBox.Show($"Thêm nhân viên thành công!\n\nMã nhân viên mới: {newMaNV}\nTên tài khoản đăng nhập (Username): C##{newMaNV}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close();
                            }
                        }
                    }
                }
                catch (OracleException oex)
                {
                    string logFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Oracle_Error_Log.txt");
                    System.IO.File.WriteAllText(logFile, $"Lỗi tạo NV\nMã: {oex.Number}\nLỗi: {oex.Message}");
                    MessageBox.Show($"Lỗi Oracle ({oex.Number}): {oex.Message}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi Hệ Thống: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn loại tài khoản!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
