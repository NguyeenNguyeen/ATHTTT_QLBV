using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public class FormDieuPhoiVien : Form
    {
        private OracleConnection _conn;
        private string _username;
        private string _displayName;

        // UI Controls
        private TabControl tabControl;
        private TabPage tabBenhNhan;
        private TabPage tabHSBA;
        private TabPage tabKTV;

        // GridViews
        private DataGridView dgvBenhNhan;
        private DataGridView dgvHSBA;
        private DataGridView dgvKTV;

        public FormDieuPhoiVien(string username, string displayName, OracleConnection? conn)
        {
            _username = username;
            _displayName = displayName;
            _conn = conn ?? throw new ArgumentNullException(nameof(conn), "Kết nối cơ sở dữ liệu không hợp lệ.");

            InitializeComponent();
            SetupUI();
            
            // Tải dữ liệu ban đầu
            LoadBenhNhanData();
            LoadHSBAData();
            LoadKTVData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Hệ thống Điều phối viên | Schema: ADMIN_PHANHE1";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            // Font mặc định
            Font fontHeader = new Font("Segoe UI", 11, FontStyle.Bold);
            Font fontRegular = new Font("Segoe UI", 9, FontStyle.Regular);
            Font fontTitle = new Font("Segoe UI", 14, FontStyle.Bold);

            // Panel Header
            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(41, 128, 185) // Blue color
            };
            this.Controls.Add(panelHeader);

            Label lblStatus = new Label
            {
                Text = $"Xin chào: {_displayName} ({_username}) | Vai trò: Điều phối viên",
                ForeColor = Color.White,
                Font = fontHeader,
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelHeader.Controls.Add(lblStatus);

            // TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = fontRegular
            };
            this.Controls.Add(tabControl);
            tabControl.BringToFront();

            // Tab 1: QUẢN LÝ BỆNH NHÂN
            tabBenhNhan = new TabPage("QUẢN LÝ BỆNH NHÂN") { Padding = new Padding(10) };
            tabControl.TabPages.Add(tabBenhNhan);
            SetupTabBenhNhan();

            // Tab 2: TẠO HỒ SƠ BỆNH ÁN
            tabHSBA = new TabPage("TẠO HỒ SƠ BỆNH ÁN") { Padding = new Padding(10) };
            tabControl.TabPages.Add(tabHSBA);
            SetupTabHSBA();

            // Tab 3: ĐIỀU PHỐI KỸ THUẬT VIÊN
            tabKTV = new TabPage("ĐIỀU PHỐI KỸ THUẬT VIÊN") { Padding = new Padding(10) };
            tabControl.TabPages.Add(tabKTV);
            SetupTabKTV();
        }

        #region TAB 1: QUẢN LÝ BỆNH NHÂN
        private void SetupTabBenhNhan()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            Label lblTitle = new Label
            {
                Text = "DANH SÁCH BỆNH NHÂN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(5, 12),
                AutoSize = true
            };
            panelTop.Controls.Add(lblTitle);

            // Button Thêm
            Button btnAdd = new Button
            {
                Text = "Thêm Bệnh Nhân",
                Location = new Point(400, 10),
                Width = 140,
                Height = 30,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAddBenhNhan_Click;
            panelTop.Controls.Add(btnAdd);

            // Button Sửa
            Button btnEdit = new Button
            {
                Text = "Sửa Thông Tin",
                Location = new Point(550, 10),
                Width = 120,
                Height = 30,
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEditBenhNhan_Click;
            panelTop.Controls.Add(btnEdit);

            // Button Làm mới
            Button btnReload = new Button
            {
                Text = "Làm Mới",
                Location = new Point(680, 10),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnReload.FlatAppearance.BorderSize = 0;
            btnReload.Click += (s, e) => LoadBenhNhanData();
            panelTop.Controls.Add(btnReload);

            tabBenhNhan.Controls.Add(panelTop);

            // DataGridView
            dgvBenhNhan = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tabBenhNhan.Controls.Add(dgvBenhNhan);
            dgvBenhNhan.BringToFront();
        }

        private void LoadBenhNhanData()
        {
            try
            {
                string query = "SELECT MABN, TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC FROM ADMIN_PHANHE1.BENHNHAN ORDER BY MABN DESC";
                DataTable dt = new DataTable();
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dgvBenhNhan.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách bệnh nhân: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddBenhNhan_Click(object sender, EventArgs e)
        {
            using (FormBenhNhanDetail form = new FormBenhNhanDetail(_conn))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBenhNhanData();
                }
            }
        }

        private void BtnEditBenhNhan_Click(object sender, EventArgs e)
        {
            if (dgvBenhNhan.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn bệnh nhân cần sửa trên danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRowView rowView = (DataRowView)dgvBenhNhan.CurrentRow.DataBoundItem;
            DataRow row = rowView.Row;

            using (FormBenhNhanDetail form = new FormBenhNhanDetail(_conn, row))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBenhNhanData();
                }
            }
        }
        #endregion

        #region TAB 2: TẠO HỒ SƠ BỆNH ÁN
        private void SetupTabHSBA()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            Label lblTitle = new Label
            {
                Text = "DANH SÁCH HỒ SƠ BỆNH ÁN (HSBA)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(5, 12),
                AutoSize = true
            };
            panelTop.Controls.Add(lblTitle);

            // Button Tạo HSBA mới
            Button btnCreate = new Button
            {
                Text = "Tạo HSBA Mới",
                Location = new Point(400, 10),
                Width = 130,
                Height = 30,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.Click += BtnCreateHSBA_Click;
            panelTop.Controls.Add(btnCreate);

            // Button Cập nhật phân công
            Button btnAssign = new Button
            {
                Text = "Cập Nhật Phân Công",
                Location = new Point(540, 10),
                Width = 160,
                Height = 30,
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAssign.FlatAppearance.BorderSize = 0;
            btnAssign.Click += BtnAssignHSBA_Click;
            panelTop.Controls.Add(btnAssign);

            // Button Làm mới
            Button btnReload = new Button
            {
                Text = "Làm Mới",
                Location = new Point(710, 10),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnReload.FlatAppearance.BorderSize = 0;
            btnReload.Click += (s, e) => LoadHSBAData();
            panelTop.Controls.Add(btnReload);

            tabHSBA.Controls.Add(panelTop);

            // DataGridView
            dgvHSBA = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tabHSBA.Controls.Add(dgvHSBA);
            dgvHSBA.BringToFront();
        }

        private void LoadHSBAData()
        {
            try
            {
                string query = "SELECT MAHSBA, MABN, NGAY, CHANDOAN, DIEUTRI, MABS, MAKHOA, KETLUAN FROM ADMIN_PHANHE1.HSBA ORDER BY NGAY DESC, MAHSBA DESC";
                DataTable dt = new DataTable();
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dgvHSBA.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách hồ sơ bệnh án: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCreateHSBA_Click(object sender, EventArgs e)
        {
            using (FormHSBACreate form = new FormHSBACreate(_conn))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadHSBAData();
                }
            }
        }

        private void BtnAssignHSBA_Click(object sender, EventArgs e)
        {
            if (dgvHSBA.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn HSBA cần cập nhật phân công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mahsba = dgvHSBA.CurrentRow.Cells["MAHSBA"].Value?.ToString() ?? "";
            string mabs = dgvHSBA.CurrentRow.Cells["MABS"].Value?.ToString() ?? "";
            string makhoa = dgvHSBA.CurrentRow.Cells["MAKHOA"].Value?.ToString() ?? "";

            using (FormHSBAAssign form = new FormHSBAAssign(_conn, mahsba, mabs, makhoa))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadHSBAData();
                }
            }
        }
        #endregion

        #region TAB 3: ĐIỀU PHỐI KỸ THUẬT VIÊN
        private void SetupTabKTV()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            Label lblTitle = new Label
            {
                Text = "DANH SÁCH DỊCH VỤ HSBA (ĐIỀU PHỐI KTV)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(5, 12),
                AutoSize = true
            };
            panelTop.Controls.Add(lblTitle);

            // Button Phân công KTV
            Button btnAssignKTV = new Button
            {
                Text = "Phân công KTV",
                Location = new Point(400, 10),
                Width = 140,
                Height = 30,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAssignKTV.FlatAppearance.BorderSize = 0;
            btnAssignKTV.Click += BtnAssignKTV_Click;
            panelTop.Controls.Add(btnAssignKTV);

            // Button Làm mới
            Button btnReload = new Button
            {
                Text = "Làm Mới",
                Location = new Point(550, 10),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnReload.FlatAppearance.BorderSize = 0;
            btnReload.Click += (s, e) => LoadKTVData();
            panelTop.Controls.Add(btnReload);

            tabKTV.Controls.Add(panelTop);

            // DataGridView
            dgvKTV = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tabKTV.Controls.Add(dgvKTV);
            dgvKTV.BringToFront();
        }

        private void LoadKTVData()
        {
            try
            {
                string query = "SELECT MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA FROM ADMIN_PHANHE1.HSBA_DV ORDER BY NGAYDV DESC, MAHSBA DESC";
                DataTable dt = new DataTable();
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dgvKTV.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách kỹ thuật dịch vụ: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAssignKTV_Click(object sender, EventArgs e)
        {
            if (dgvKTV.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần phân công kỹ thuật viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mahsba = dgvKTV.CurrentRow.Cells["MAHSBA"].Value?.ToString() ?? "";
            string loaidv = dgvKTV.CurrentRow.Cells["LOAIDV"].Value?.ToString() ?? "";
            DateTime ngaydv = Convert.ToDateTime(dgvKTV.CurrentRow.Cells["NGAYDV"].Value);
            string maktv = dgvKTV.CurrentRow.Cells["MAKTV"].Value?.ToString() ?? "";

            using (FormKTVAssign form = new FormKTVAssign(_conn, mahsba, loaidv, ngaydv, maktv))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadKTVData();
                }
            }
        }
        #endregion
    }

    #region FORM CON TAB 1: FORM THÊM / SỬA BỆNH NHÂN
    public class FormBenhNhanDetail : Form
    {
        private OracleConnection _conn;
        private DataRow _editRow;
        private bool _isEditMode => _editRow != null;

        // Controls
        private TextBox txtTenBN;
        private ComboBox cboPhai;
        private DateTimePicker dtpNgaySinh;
        private TextBox txtCCCD;
        private TextBox txtSoNha;
        private TextBox txtTenDuong;
        private TextBox txtQuanHuyen;
        private TextBox txtTinhTP;
        private TextBox txtTienSuBenh;
        private TextBox txtTienSuBenhGD;
        private TextBox txtDiUngThuoc;
        private TextBox txtMatKhau;

        public FormBenhNhanDetail(OracleConnection conn, DataRow editRow = null)
        {
            _conn = conn;
            _editRow = editRow;
            InitializeComponent();
            SetupUI();
            if (_isEditMode)
            {
                FillData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = _isEditMode ? "Cập Nhật Thông Tin Bệnh Nhân" : "Thêm Bệnh Nhân Mới";
            this.Size = new Size(500, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            Font labelFont = new Font("Segoe UI", 9, FontStyle.Bold);
            Font textFont = new Font("Segoe UI", 9, FontStyle.Regular);

            int y = 20;

            // Tên bệnh nhân
            Label lblTen = new Label { Text = "Họ và tên *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtTenBN = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont };
            this.Controls.Add(lblTen); this.Controls.Add(txtTenBN);
            y += 40;

            // Phái
            Label lblPhai = new Label { Text = "Giới tính *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            cboPhai = new ComboBox { Location = new Point(160, y), Size = new Size(100, 23), Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPhai.Items.AddRange(new object[] { "Nam", "Nữ" });
            cboPhai.SelectedIndex = 0;
            this.Controls.Add(lblPhai); this.Controls.Add(cboPhai);
            y += 40;

            // Ngày sinh
            Label lblNgaySinh = new Label { Text = "Ngày sinh *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            dtpNgaySinh = new DateTimePicker { Location = new Point(160, y), Size = new Size(150, 23), Font = textFont, Format = DateTimePickerFormat.Short };
            this.Controls.Add(lblNgaySinh); this.Controls.Add(dtpNgaySinh);
            y += 40;

            // CCCD
            Label lblCCCD = new Label { Text = "Số CMND/CCCD *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtCCCD = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont };
            this.Controls.Add(lblCCCD); this.Controls.Add(txtCCCD);
            y += 40;

            // Số nhà
            Label lblSoNha = new Label { Text = "Số nhà:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtSoNha = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont };
            this.Controls.Add(lblSoNha); this.Controls.Add(txtSoNha);
            y += 40;

            // Tên đường
            Label lblDuong = new Label { Text = "Tên đường:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtTenDuong = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont };
            this.Controls.Add(lblDuong); this.Controls.Add(txtTenDuong);
            y += 40;

            // Quận/Huyện
            Label lblQuan = new Label { Text = "Quận/Huyện:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtQuanHuyen = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont };
            this.Controls.Add(lblQuan); this.Controls.Add(txtQuanHuyen);
            y += 40;

            // Tỉnh/TP
            Label lblTinh = new Label { Text = "Tỉnh/Thành phố:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtTinhTP = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont };
            this.Controls.Add(lblTinh); this.Controls.Add(txtTinhTP);
            y += 40;

            // Tiền sử bệnh
            Label lblTS = new Label { Text = "Tiền sử bệnh:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtTienSuBenh = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont };
            this.Controls.Add(lblTS); this.Controls.Add(txtTienSuBenh);
            y += 40;

            // Tiền sử bệnh GD
            Label lblTSGD = new Label { Text = "Tiền sử gia đình:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtTienSuBenhGD = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont };
            this.Controls.Add(lblTSGD); this.Controls.Add(txtTienSuBenhGD);
            y += 40;

            // Dị ứng thuốc
            Label lblDiUng = new Label { Text = "Dị ứng thuốc:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtDiUngThuoc = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont };
            this.Controls.Add(lblDiUng); this.Controls.Add(txtDiUngThuoc);
            y += 40;

            // Mật khẩu Oracle
            Label lblMk = new Label { Text = _isEditMode ? "Mật khẩu mới (nếu đổi):" : "Mật khẩu Oracle *:", Location = new Point(25, y), Size = new Size(130, 20), Font = labelFont };
            txtMatKhau = new TextBox { Location = new Point(160, y), Size = new Size(280, 23), Font = textFont, UseSystemPasswordChar = true };
            this.Controls.Add(lblMk); this.Controls.Add(txtMatKhau);
            y += 50;

            // Buttons
            Button btnSave = new Button
            {
                Text = "Lưu lại",
                Location = new Point(160, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = labelFont
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnCancel = new Button
            {
                Text = "Hủy bỏ",
                Location = new Point(280, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(189, 195, 199),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = labelFont
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void FillData()
        {
            txtTenBN.Text = _editRow["TENBN"]?.ToString() ?? "";
            cboPhai.SelectedItem = _editRow["PHAI"]?.ToString() == "Nữ" ? "Nữ" : "Nam";
            if (_editRow["NGAYSINH"] != DBNull.Value)
                dtpNgaySinh.Value = Convert.ToDateTime(_editRow["NGAYSINH"]);
            txtCCCD.Text = _editRow["CCCD"]?.ToString() ?? "";
            txtSoNha.Text = _editRow["SONHA"]?.ToString() ?? "";
            txtTenDuong.Text = _editRow["TENDUONG"]?.ToString() ?? "";
            txtQuanHuyen.Text = _editRow["QUANHUYEN"]?.ToString() ?? "";
            txtTinhTP.Text = _editRow["TINHTP"]?.ToString() ?? "";
            txtTienSuBenh.Text = _editRow["TIENSUBENH"]?.ToString() ?? "";
            txtTienSuBenhGD.Text = _editRow["TIENSUBENHGD"]?.ToString() ?? "";
            txtDiUngThuoc.Text = _editRow["DIUNGTHUOC"]?.ToString() ?? "";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrEmpty(txtTenBN.Text.Trim()) ||
                string.IsNullOrEmpty(txtCCCD.Text.Trim()) ||
                (!_isEditMode && string.IsNullOrEmpty(txtMatKhau.Text)))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các thông tin bắt buộc (*).", "Thông tin thiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!_isEditMode)
                {
                    // Gọi SP_TAO_BENHNHAN
                    using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_TAO_BENHNHAN", _conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;

                        cmd.Parameters.Add("p_TENBN", OracleDbType.NVarchar2).Value = txtTenBN.Text.Trim();
                        cmd.Parameters.Add("p_PHAI", OracleDbType.NVarchar2).Value = cboPhai.SelectedItem.ToString();
                        cmd.Parameters.Add("p_NGAYSINH", OracleDbType.Date).Value = dtpNgaySinh.Value.Date;
                        cmd.Parameters.Add("p_CCCD", OracleDbType.Varchar2).Value = txtCCCD.Text.Trim();
                        cmd.Parameters.Add("p_SONHA", OracleDbType.NVarchar2).Value = txtSoNha.Text.Trim();
                        cmd.Parameters.Add("p_TENDUONG", OracleDbType.NVarchar2).Value = txtTenDuong.Text.Trim();
                        cmd.Parameters.Add("p_QUANHUYEN", OracleDbType.NVarchar2).Value = txtQuanHuyen.Text.Trim();
                        cmd.Parameters.Add("p_TINHTP", OracleDbType.NVarchar2).Value = txtTinhTP.Text.Trim();
                        cmd.Parameters.Add("p_TIENSUBENH", OracleDbType.NVarchar2).Value = txtTienSuBenh.Text.Trim();
                        cmd.Parameters.Add("p_TIENSUBENHGD", OracleDbType.NVarchar2).Value = txtTienSuBenhGD.Text.Trim();
                        cmd.Parameters.Add("p_DIUNGTHUOC", OracleDbType.NVarchar2).Value = txtDiUngThuoc.Text.Trim();
                        cmd.Parameters.Add("p_MATKHAU", OracleDbType.Varchar2).Value = txtMatKhau.Text;

                        // Output parameter
                        OracleParameter outParam = new OracleParameter("p_MABN_OUT", OracleDbType.Varchar2, 20);
                        outParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outParam);

                        cmd.ExecuteNonQuery();

                        string newMaBN = outParam.Value.ToString();
                        MessageBox.Show($"Tạo thành công bệnh nhân mới!\nMã bệnh nhân: {newMaBN}\nTài khoản Oracle: C##{newMaBN}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Gọi SP_SUA_BENHNHAN
                    string mabn = _editRow["MABN"].ToString();
                    using (OracleCommand cmd = new OracleCommand("ADMIN_PHANHE1.SP_SUA_BENHNHAN", _conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;

                        cmd.Parameters.Add("p_MABN", OracleDbType.Varchar2).Value = mabn;
                        cmd.Parameters.Add("p_TENBN", OracleDbType.NVarchar2).Value = txtTenBN.Text.Trim();
                        cmd.Parameters.Add("p_PHAI", OracleDbType.NVarchar2).Value = cboPhai.SelectedItem.ToString();
                        cmd.Parameters.Add("p_NGAYSINH", OracleDbType.Date).Value = dtpNgaySinh.Value.Date;
                        cmd.Parameters.Add("p_CCCD", OracleDbType.Varchar2).Value = txtCCCD.Text.Trim();
                        cmd.Parameters.Add("p_SONHA", OracleDbType.NVarchar2).Value = txtSoNha.Text.Trim();
                        cmd.Parameters.Add("p_TENDUONG", OracleDbType.NVarchar2).Value = txtTenDuong.Text.Trim();
                        cmd.Parameters.Add("p_QUANHUYEN", OracleDbType.NVarchar2).Value = txtQuanHuyen.Text.Trim();
                        cmd.Parameters.Add("p_TINHTP", OracleDbType.NVarchar2).Value = txtTinhTP.Text.Trim();
                        cmd.Parameters.Add("p_TIENSUBENH", OracleDbType.NVarchar2).Value = txtTienSuBenh.Text.Trim();
                        cmd.Parameters.Add("p_TIENSUBENHGD", OracleDbType.NVarchar2).Value = txtTienSuBenhGD.Text.Trim();
                        cmd.Parameters.Add("p_DIUNGTHUOC", OracleDbType.NVarchar2).Value = txtDiUngThuoc.Text.Trim();
                        
                        // Nếu sửa, cho phép mật khẩu rỗng (không đổi)
                        cmd.Parameters.Add("p_MATKHAU", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtMatKhau.Text) ? (object)DBNull.Value : txtMatKhau.Text;

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Cập nhật thông tin bệnh nhân thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Thao tác thất bại: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    #endregion

    #region FORM CON TAB 2: TẠO HỒ SƠ BỆNH ÁN (HSBA)
    public class FormHSBACreate : Form
    {
        private OracleConnection _conn;

        // Controls
        private ComboBox cboMABN;
        private ComboBox cboMABS;
        private ComboBox cboMAKHOA;

        public FormHSBACreate(OracleConnection conn)
        {
            _conn = conn;
            InitializeComponent();
            SetupUI();
            LoadComboboxData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Tạo Hồ Sơ Bệnh Án (HSBA) Mới";
            this.Size = new Size(450, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            Font labelFont = new Font("Segoe UI", 9, FontStyle.Bold);
            Font textFont = new Font("Segoe UI", 9, FontStyle.Regular);

            int y = 20;

            // Bệnh nhân
            Label lblBN = new Label { Text = "Bệnh nhân *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            cboMABN = new ComboBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(lblBN); this.Controls.Add(cboMABN);
            y += 40;

            // Bác sĩ
            Label lblBS = new Label { Text = "Bác sĩ điều trị *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            cboMABS = new ComboBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(lblBS); this.Controls.Add(cboMABS);
            y += 40;

            // Khoa
            Label lblKhoa = new Label { Text = "Khoa bệnh *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            cboMAKHOA = new ComboBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(lblKhoa); this.Controls.Add(cboMAKHOA);
            y += 50;

            // Buttons
            Button btnCreate = new Button
            {
                Text = "Tạo HSBA",
                Location = new Point(160, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = labelFont
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.Click += BtnCreate_Click;
            this.Controls.Add(btnCreate);

            Button btnCancel = new Button
            {
                Text = "Hủy bỏ",
                Location = new Point(280, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(189, 195, 199),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = labelFont
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void LoadComboboxData()
        {
            try
            {
                // 1. Load Bệnh nhân
                string queryBN = "SELECT MABN, TENBN FROM ADMIN_PHANHE1.BENHNHAN ORDER BY MABN";
                DataTable dtBN = new DataTable();
                using (OracleCommand cmd = new OracleCommand(queryBN, _conn))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dtBN);
                    }
                }
                
                // Trộn MABN và TENBN để dễ chọn
                dtBN.Columns.Add("DISPLAY", typeof(string), "MABN + ' - ' + TENBN");
                cboMABN.DataSource = dtBN;
                cboMABN.DisplayMember = "DISPLAY";
                cboMABN.ValueMember = "MABN";

                // 2. Load Bác sĩ/Y sĩ
                string queryBS = "SELECT MANV, HOTEN FROM ADMIN_PHANHE1.NHANVIEN WHERE VAITRO = 'Bác sĩ/Y sĩ' ORDER BY MANV";
                DataTable dtBS = new DataTable();
                using (OracleCommand cmd = new OracleCommand(queryBS, _conn))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dtBS);
                    }
                }
                dtBS.Columns.Add("DISPLAY", typeof(string), "MANV + ' - ' + HOTEN");
                cboMABS.DataSource = dtBS;
                cboMABS.DisplayMember = "DISPLAY";
                cboMABS.ValueMember = "MANV";

                // 3. Load Khoa
                string queryKhoa = "SELECT MAKHOA, TENKHOA FROM ADMIN_PHANHE1.KHOA ORDER BY MAKHOA";
                DataTable dtKhoa = new DataTable();
                using (OracleCommand cmd = new OracleCommand(queryKhoa, _conn))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dtKhoa);
                    }
                }
                dtKhoa.Columns.Add("DISPLAY", typeof(string), "MAKHOA + ' - ' + TENKHOA");
                cboMAKHOA.DataSource = dtKhoa;
                cboMAKHOA.DisplayMember = "DISPLAY";
                cboMAKHOA.ValueMember = "MAKHOA";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục: {ex.Message}", "Lỗi tải dropdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (cboMABN.SelectedValue == null || cboMABS.SelectedValue == null || cboMAKHOA.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Tự sinh MAHSBA: HS + timestamp (yyMMddHHmmss)
                string mahsba = "HS" + DateTime.Now.ToString("yyMMddHHmmss");

                string query = "INSERT INTO ADMIN_PHANHE1.HSBA (MAHSBA, MABN, NGAY, MABS, MAKHOA) VALUES (:mahsba, :mabn, SYSDATE, :mabs, :makhoa)";
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = mahsba;
                    cmd.Parameters.Add("mabn", OracleDbType.Varchar2).Value = cboMABN.SelectedValue.ToString();
                    cmd.Parameters.Add("mabs", OracleDbType.Varchar2).Value = cboMABS.SelectedValue.ToString();
                    cmd.Parameters.Add("makhoa", OracleDbType.Varchar2).Value = cboMAKHOA.SelectedValue.ToString();

                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Tạo hồ sơ bệnh án thành công!\nMã HSBA: {mahsba}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo hồ sơ bệnh án: {ex.Message}", "Lỗi thực thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class FormHSBAAssign : Form
    {
        private OracleConnection _conn;
        private string _mahsba;
        private string _currentMabs;
        private string _currentMakhoa;

        // Controls
        private ComboBox cboMABS;
        private ComboBox cboMAKHOA;

        public FormHSBAAssign(OracleConnection conn, string mahsba, string mabs, string makhoa)
        {
            _conn = conn;
            _mahsba = mahsba;
            _currentMabs = mabs;
            _currentMakhoa = makhoa;

            InitializeComponent();
            SetupUI();
            LoadComboboxData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Cập Nhật Phân Công HSBA";
            this.Size = new Size(450, 260);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            Font labelFont = new Font("Segoe UI", 9, FontStyle.Bold);
            Font textFont = new Font("Segoe UI", 9, FontStyle.Regular);

            int y = 20;

            // Mã HSBA
            Label lblHSBA = new Label { Text = "Mã HSBA:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            Label lblHSBAVal = new Label { Text = _mahsba, Location = new Point(160, y), Size = new Size(240, 20), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Blue };
            this.Controls.Add(lblHSBA); this.Controls.Add(lblHSBAVal);
            y += 40;

            // Bác sĩ
            Label lblBS = new Label { Text = "Bác sĩ mới *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            cboMABS = new ComboBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(lblBS); this.Controls.Add(cboMABS);
            y += 40;

            // Khoa
            Label lblKhoa = new Label { Text = "Khoa mới *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            cboMAKHOA = new ComboBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(lblKhoa); this.Controls.Add(cboMAKHOA);
            y += 50;

            // Buttons
            Button btnUpdate = new Button
            {
                Text = "Cập Nhật",
                Location = new Point(160, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = labelFont
            };
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Click += BtnUpdate_Click;
            this.Controls.Add(btnUpdate);

            Button btnCancel = new Button
            {
                Text = "Hủy bỏ",
                Location = new Point(280, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(189, 195, 199),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = labelFont
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void LoadComboboxData()
        {
            try
            {
                // 1. Load Bác sĩ/Y sĩ
                string queryBS = "SELECT MANV, HOTEN FROM ADMIN_PHANHE1.NHANVIEN WHERE VAITRO = 'Bác sĩ/Y sĩ' ORDER BY MANV";
                DataTable dtBS = new DataTable();
                using (OracleCommand cmd = new OracleCommand(queryBS, _conn))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dtBS);
                    }
                }
                dtBS.Columns.Add("DISPLAY", typeof(string), "MANV + ' - ' + HOTEN");
                cboMABS.DataSource = dtBS;
                cboMABS.DisplayMember = "DISPLAY";
                cboMABS.ValueMember = "MANV";

                // Chọn bác sĩ hiện tại
                if (!string.IsNullOrEmpty(_currentMabs))
                    cboMABS.SelectedValue = _currentMabs;

                // 2. Load Khoa
                string queryKhoa = "SELECT MAKHOA, TENKHOA FROM ADMIN_PHANHE1.KHOA ORDER BY MAKHOA";
                DataTable dtKhoa = new DataTable();
                using (OracleCommand cmd = new OracleCommand(queryKhoa, _conn))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dtKhoa);
                    }
                }
                dtKhoa.Columns.Add("DISPLAY", typeof(string), "MAKHOA + ' - ' + TENKHOA");
                cboMAKHOA.DataSource = dtKhoa;
                cboMAKHOA.DisplayMember = "DISPLAY";
                cboMAKHOA.ValueMember = "MAKHOA";

                // Chọn khoa hiện tại
                if (!string.IsNullOrEmpty(_currentMakhoa))
                    cboMAKHOA.SelectedValue = _currentMakhoa;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục: {ex.Message}", "Lỗi tải dropdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (cboMABS.SelectedValue == null || cboMAKHOA.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ bác sĩ và khoa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "UPDATE ADMIN_PHANHE1.HSBA SET MABS = :mabs, MAKHOA = :makhoa WHERE MAHSBA = :mahsba";
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("mabs", OracleDbType.Varchar2).Value = cboMABS.SelectedValue.ToString();
                    cmd.Parameters.Add("makhoa", OracleDbType.Varchar2).Value = cboMAKHOA.SelectedValue.ToString();
                    cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = _mahsba;

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật phân công bác sĩ & khoa thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật phân công: {ex.Message}", "Lỗi thực thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    #endregion

    #region FORM CON TAB 3: ĐIỀU PHỐI KỸ THUẬT VIÊN (KTV)
    public class FormKTVAssign : Form
    {
        private OracleConnection _conn;
        private string _mahsba;
        private string _loaidv;
        private DateTime _ngaydv;
        private string _currentMaktv;

        // Controls
        private ComboBox cboKTV;

        public FormKTVAssign(OracleConnection conn, string mahsba, string loaidv, DateTime ngaydv, string maktv)
        {
            _conn = conn;
            _mahsba = mahsba;
            _loaidv = loaidv;
            _ngaydv = ngaydv;
            _currentMaktv = maktv;

            InitializeComponent();
            SetupUI();
            LoadKTVData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Phân Công Kỹ Thuật Viên";
            this.Size = new Size(450, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            Font labelFont = new Font("Segoe UI", 9, FontStyle.Bold);
            Font textFont = new Font("Segoe UI", 9, FontStyle.Regular);

            int y = 20;

            // Mã HSBA & Loại DV
            Label lblHSBA = new Label { Text = "HSBA & Dịch vụ:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            Label lblVal = new Label { Text = $"{_mahsba} | {_loaidv}", Location = new Point(160, y), Size = new Size(260, 20), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Blue };
            this.Controls.Add(lblHSBA); this.Controls.Add(lblVal);
            y += 40;

            // Ngày thực hiện
            Label lblNgay = new Label { Text = "Ngày chỉ định:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            Label lblNgayVal = new Label { Text = _ngaydv.ToString("dd/MM/yyyy HH:mm:ss"), Location = new Point(160, y), Size = new Size(260, 20), Font = textFont };
            this.Controls.Add(lblNgay); this.Controls.Add(lblNgayVal);
            y += 40;

            // Kỹ thuật viên
            Label lblKTV = new Label { Text = "Kỹ thuật viên *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            cboKTV = new ComboBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(lblKTV); this.Controls.Add(cboKTV);
            y += 50;

            // Buttons
            Button btnUpdate = new Button
            {
                Text = "Phân Công",
                Location = new Point(160, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = labelFont
            };
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Click += BtnUpdate_Click;
            this.Controls.Add(btnUpdate);

            Button btnCancel = new Button
            {
                Text = "Hủy bỏ",
                Location = new Point(280, y),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(189, 195, 199),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = labelFont
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void LoadKTVData()
        {
            try
            {
                string query = "SELECT MANV, HOTEN FROM ADMIN_PHANHE1.NHANVIEN WHERE VAITRO = 'Kỹ thuật viên' ORDER BY MANV";
                DataTable dt = new DataTable();
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dt.Columns.Add("DISPLAY", typeof(string), "MANV + ' - ' + HOTEN");
                cboKTV.DataSource = dt;
                cboKTV.DisplayMember = "DISPLAY";
                cboKTV.ValueMember = "MANV";

                if (!string.IsNullOrEmpty(_currentMaktv))
                    cboKTV.SelectedValue = _currentMaktv;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục: {ex.Message}", "Lỗi tải dropdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (cboKTV.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn kỹ thuật viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "UPDATE ADMIN_PHANHE1.HSBA_DV SET MAKTV = :maktv WHERE MAHSBA = :mahsba AND LOAIDV = :loaidv AND NGAYDV = :ngaydv";
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("maktv", OracleDbType.Varchar2).Value = cboKTV.SelectedValue.ToString();
                    cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = _mahsba;
                    cmd.Parameters.Add("loaidv", OracleDbType.NVarchar2).Value = _loaidv;
                    cmd.Parameters.Add("ngaydv", OracleDbType.Date).Value = _ngaydv;

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Phân công kỹ thuật viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi phân công kỹ thuật viên: {ex.Message}", "Lỗi thực thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    #endregion
}
