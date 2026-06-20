using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public class FormBacSi : Form
    {
        private OracleConnection _conn;
        private string _username;
        private string _displayName;

        // UI Controls
        private TabControl tabControl;
        private TabPage tabHSBA;
        private TabPage tabDichVu;
        private TabPage tabDonThuoc;
        private TabPage tabBenhNhan;

        // GridViews
        private DataGridView dgvHSBA;
        private DataGridView dgvDichVu;
        private DataGridView dgvDonThuoc;
        private DataGridView dgvBenhNhan;

        // Tab 2 Controls
        private ComboBox cboHSBADichVu;
        // Tab 3 Controls
        private ComboBox cboHSBADonThuoc;

        public FormBacSi(string username, string displayName, OracleConnection? conn)
        {
            _username = username;
            _displayName = displayName;
            _conn = conn ?? throw new ArgumentNullException(nameof(conn), "Kết nối cơ sở dữ liệu không hợp lệ.");

            InitializeComponent();
            SetupUI();

            // Load data
            LoadHSBAData();
            LoadHSBADropdowns();
            LoadBenhNhanData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Hệ thống Bác sĩ / Y sĩ | Schema: ADMIN_PHANHE1 (VPD Filter Active)";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void SetupUI()
        {
            Font fontHeader = new Font("Segoe UI", 11, FontStyle.Bold);
            Font fontRegular = new Font("Segoe UI", 9, FontStyle.Regular);

            // Panel Header
            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(26, 188, 156) // Teal / Green-Blue color
            };
            this.Controls.Add(panelHeader);

            Label lblStatus = new Label
            {
                Text = $"Xin chào bác sĩ: {_displayName} ({_username}) | VPD tự động lọc hồ sơ của bạn",
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

            // Tab 1: HỒ SƠ BỆNH ÁN CỦA TÔI
            tabHSBA = new TabPage("HỒ SƠ BỆNH ÁN CỦA TÔI") { Padding = new Padding(10) };
            tabControl.TabPages.Add(tabHSBA);
            SetupTabHSBA();

            // Tab 2: DỊCH VỤ HỖ TRỢ CHẨN ĐOÁN
            tabDichVu = new TabPage("DỊCH VỤ HỖ TRỢ CHẨN ĐOÁN") { Padding = new Padding(10) };
            tabControl.TabPages.Add(tabDichVu);
            SetupTabDichVu();

            // Tab 3: ĐƠN THUỐC
            tabDonThuoc = new TabPage("ĐƠN THUỐC") { Padding = new Padding(10) };
            tabControl.TabPages.Add(tabDonThuoc);
            SetupTabDonThuoc();

            // Tab 4: BỆNH NHÂN DO TÔI ĐIỀU TRỊ
            tabBenhNhan = new TabPage("BỆNH NHÂN DO TÔI ĐIỀU TRỊ") { Padding = new Padding(10) };
            tabControl.TabPages.Add(tabBenhNhan);
            SetupTabBenhNhan();
        }

        #region TAB 1: HỒ SƠ BỆNH ÁN CỦA TÔI
        private void SetupTabHSBA()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            Label lblTitle = new Label
            {
                Text = "DS HỒ SƠ BỆNH ÁN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 188, 156),
                Location = new Point(5, 12),
                AutoSize = true
            };
            panelTop.Controls.Add(lblTitle);

            // Button Cập nhật chẩn đoán
            Button btnUpdate = new Button
            {
                Text = "Cập Nhật Chẩn Đoán",
                Location = new Point(480, 10),
                Width = 160,
                Height = 30,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Click += BtnUpdateChanDoan_Click;
            panelTop.Controls.Add(btnUpdate);

            // Button Làm mới
            Button btnReload = new Button
            {
                Text = "Làm Mới",
                Location = new Point(650, 10),
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
                string query = "SELECT MAHSBA, MABN, NGAY, CHANDOAN, DIEUTRI, MABS, MAKHOA, KETLUAN FROM ADMIN_PHANHE1.HSBA ORDER BY NGAY DESC";
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
                MessageBox.Show($"Lỗi lấy HSBA của tôi: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdateChanDoan_Click(object sender, EventArgs e)
        {
            if (dgvHSBA.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn HSBA cần cập nhật chẩn đoán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mahsba = dgvHSBA.CurrentRow.Cells["MAHSBA"].Value?.ToString() ?? "";
            string chandoan = dgvHSBA.CurrentRow.Cells["CHANDOAN"].Value?.ToString() ?? "";
            string dieutri = dgvHSBA.CurrentRow.Cells["DIEUTRI"].Value?.ToString() ?? "";
            string ketluan = dgvHSBA.CurrentRow.Cells["KETLUAN"].Value?.ToString() ?? "";

            using (FormHSBAUpdateText form = new FormHSBAUpdateText(_conn, mahsba, chandoan, dieutri, ketluan))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadHSBAData();
                }
            }
        }
        #endregion

        #region TAB 2: DỊCH VỤ HỖ TRỢ CHẨN ĐOÁN
        private void SetupTabDichVu()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 60 };
            
            Label lblSelect = new Label
            {
                Text = "Chọn Hồ Sơ Bệnh Án:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(5, 20),
                AutoSize = true
            };
            panelTop.Controls.Add(lblSelect);

            cboHSBADichVu = new ComboBox
            {
                Location = new Point(180, 17),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboHSBADichVu.SelectedIndexChanged += CboHSBADichVu_SelectedIndexChanged;
            panelTop.Controls.Add(cboHSBADichVu);

            // Button Thêm dịch vụ
            Button btnAddDV = new Button
            {
                Text = "Thêm Dịch Vụ",
                Location = new Point(410, 13),
                Width = 120,
                Height = 30,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAddDV.FlatAppearance.BorderSize = 0;
            btnAddDV.Click += BtnAddDichVu_Click;
            panelTop.Controls.Add(btnAddDV);

            // Button Xóa dịch vụ
            Button btnDeleteDV = new Button
            {
                Text = "Xóa Dịch Vụ",
                Location = new Point(540, 13),
                Width = 110,
                Height = 30,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnDeleteDV.FlatAppearance.BorderSize = 0;
            btnDeleteDV.Click += BtnDeleteDichVu_Click;
            panelTop.Controls.Add(btnDeleteDV);

            // Button Làm mới
            Button btnReload = new Button
            {
                Text = "Làm Mới",
                Location = new Point(660, 13),
                Width = 90,
                Height = 30,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnReload.FlatAppearance.BorderSize = 0;
            btnReload.Click += (s, e) => {
                LoadHSBADropdowns();
                LoadDichVuData();
            };
            panelTop.Controls.Add(btnReload);

            tabDichVu.Controls.Add(panelTop);

            // DataGridView
            dgvDichVu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tabDichVu.Controls.Add(dgvDichVu);
            dgvDichVu.BringToFront();
        }

        private void LoadHSBADropdowns()
        {
            try
            {
                string query = "SELECT MAHSBA FROM ADMIN_PHANHE1.HSBA ORDER BY MAHSBA DESC";
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt1);
                    }
                }
                
                // Copy cho tab đơn thuốc
                dt2 = dt1.Copy();

                cboHSBADichVu.DataSource = dt1;
                cboHSBADichVu.DisplayMember = "MAHSBA";
                cboHSBADichVu.ValueMember = "MAHSBA";

                cboHSBADonThuoc.DataSource = dt2;
                cboHSBADonThuoc.DisplayMember = "MAHSBA";
                cboHSBADonThuoc.ValueMember = "MAHSBA";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục HSBA: {ex.Message}", "Lỗi tải dropdown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDichVuData()
        {
            if (cboHSBADichVu.SelectedValue == null)
            {
                dgvDichVu.DataSource = null;
                return;
            }

            string mahsba = cboHSBADichVu.SelectedValue.ToString();
            try
            {
                string query = "SELECT MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA FROM ADMIN_PHANHE1.HSBA_DV WHERE MAHSBA = :mahsba ORDER BY NGAYDV DESC";
                DataTable dt = new DataTable();
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = mahsba;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dgvDichVu.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lấy dịch vụ của HSBA {mahsba}: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboHSBADichVu_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDichVuData();
        }

        private void BtnAddDichVu_Click(object sender, EventArgs e)
        {
            if (cboHSBADichVu.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn hoặc tạo HSBA trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mahsba = cboHSBADichVu.SelectedValue.ToString();
            using (FormDichVuAdd form = new FormDichVuAdd(_conn, mahsba))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadDichVuData();
                }
            }
        }

        private void BtnDeleteDichVu_Click(object sender, EventArgs e)
        {
            if (dgvDichVu.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần xóa khỏi danh sách bên dưới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mahsba = dgvDichVu.CurrentRow.Cells["MAHSBA"].Value?.ToString() ?? "";
            string loaidv = dgvDichVu.CurrentRow.Cells["LOAIDV"].Value?.ToString() ?? "";
            DateTime ngaydv = Convert.ToDateTime(dgvDichVu.CurrentRow.Cells["NGAYDV"].Value);

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa dịch vụ '{loaidv}' chỉ định ngày {ngaydv:dd/MM/yyyy HH:mm:ss}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM ADMIN_PHANHE1.HSBA_DV WHERE MAHSBA = :mahsba AND LOAIDV = :loaidv AND NGAYDV = :ngaydv";
                    using (OracleCommand cmd = new OracleCommand(query, _conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = mahsba;
                        cmd.Parameters.Add("loaidv", OracleDbType.NVarchar2).Value = loaidv;
                        cmd.Parameters.Add("ngaydv", OracleDbType.Date).Value = ngaydv;

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Xóa dịch vụ hỗ trợ chẩn đoán thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    LoadDichVuData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa dịch vụ: {ex.Message}", "Lỗi thực thi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region TAB 3: ĐƠN THUỐC
        private void SetupTabDonThuoc()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 60 };

            Label lblSelect = new Label
            {
                Text = "Chọn Hồ Sơ Bệnh Án:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(5, 20),
                AutoSize = true
            };
            panelTop.Controls.Add(lblSelect);

            cboHSBADonThuoc = new ComboBox
            {
                Location = new Point(180, 17),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboHSBADonThuoc.SelectedIndexChanged += CboHSBADonThuoc_SelectedIndexChanged;
            panelTop.Controls.Add(cboHSBADonThuoc);

            // Button Thêm thuốc
            Button btnAddThuoc = new Button
            {
                Text = "Thêm Thuốc",
                Location = new Point(410, 13),
                Width = 110,
                Height = 30,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAddThuoc.FlatAppearance.BorderSize = 0;
            btnAddThuoc.Click += BtnAddThuoc_Click;
            panelTop.Controls.Add(btnAddThuoc);

            // Button Sửa thuốc
            Button btnEditThuoc = new Button
            {
                Text = "Sửa Thuốc",
                Location = new Point(530, 13),
                Width = 110,
                Height = 30,
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnEditThuoc.FlatAppearance.BorderSize = 0;
            btnEditThuoc.Click += BtnEditThuoc_Click;
            panelTop.Controls.Add(btnEditThuoc);

            // Button Xóa thuốc
            Button btnDeleteThuoc = new Button
            {
                Text = "Xóa Thuốc",
                Location = new Point(650, 13),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnDeleteThuoc.FlatAppearance.BorderSize = 0;
            btnDeleteThuoc.Click += BtnDeleteThuoc_Click;
            panelTop.Controls.Add(btnDeleteThuoc);

            // Button Làm mới
            Button btnReload = new Button
            {
                Text = "Làm Mới",
                Location = new Point(760, 13),
                Width = 80,
                Height = 30,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnReload.FlatAppearance.BorderSize = 0;
            btnReload.Click += (s, e) => {
                LoadHSBADropdowns();
                LoadDonThuocData();
            };
            panelTop.Controls.Add(btnReload);

            tabDonThuoc.Controls.Add(panelTop);

            // DataGridView
            dgvDonThuoc = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            tabDonThuoc.Controls.Add(dgvDonThuoc);
            dgvDonThuoc.BringToFront();
        }

        private void LoadDonThuocData()
        {
            if (cboHSBADonThuoc.SelectedValue == null)
            {
                dgvDonThuoc.DataSource = null;
                return;
            }

            string mahsba = cboHSBADonThuoc.SelectedValue.ToString();
            try
            {
                string query = "SELECT MAHSBA, TENTHUOC, NGAYDT, LIEUDUNG FROM ADMIN_PHANHE1.DONTHUOC WHERE MAHSBA = :mahsba ORDER BY NGAYDT DESC";
                DataTable dt = new DataTable();
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = mahsba;
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dgvDonThuoc.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lấy đơn thuốc: {ex.Message}", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboHSBADonThuoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDonThuocData();
        }

        private void BtnAddThuoc_Click(object sender, EventArgs e)
        {
            if (cboHSBADonThuoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn hoặc tạo HSBA trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mahsba = cboHSBADonThuoc.SelectedValue.ToString();
            using (FormThuocAdd form = new FormThuocAdd(_conn, mahsba))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadDonThuocData();
                }
            }
        }

        private void BtnEditThuoc_Click(object sender, EventArgs e)
        {
            if (dgvDonThuoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dòng thuốc cần chỉnh sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mahsba = dgvDonThuoc.CurrentRow.Cells["MAHSBA"].Value?.ToString() ?? "";
            string tenthuoc = dgvDonThuoc.CurrentRow.Cells["TENTHUOC"].Value?.ToString() ?? "";
            DateTime ngaydt = Convert.ToDateTime(dgvDonThuoc.CurrentRow.Cells["NGAYDT"].Value);
            string lieudung = dgvDonThuoc.CurrentRow.Cells["LIEUDUNG"].Value?.ToString() ?? "";

            using (FormThuocEdit form = new FormThuocEdit(_conn, mahsba, tenthuoc, ngaydt, lieudung))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadDonThuocData();
                }
            }
        }

        private void BtnDeleteThuoc_Click(object sender, EventArgs e)
        {
            if (dgvDonThuoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn dòng thuốc cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mahsba = dgvDonThuoc.CurrentRow.Cells["MAHSBA"].Value?.ToString() ?? "";
            string tenthuoc = dgvDonThuoc.CurrentRow.Cells["TENTHUOC"].Value?.ToString() ?? "";
            DateTime ngaydt = Convert.ToDateTime(dgvDonThuoc.CurrentRow.Cells["NGAYDT"].Value);

            DialogResult confirm = MessageBox.Show($"Bạn có chắc muốn xóa thuốc '{tenthuoc}' cấp ngày {ngaydt:dd/MM/yyyy}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM ADMIN_PHANHE1.DONTHUOC WHERE MAHSBA = :mahsba AND TENTHUOC = :tenthuoc AND NGAYDT = :ngaydt";
                    using (OracleCommand cmd = new OracleCommand(query, _conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = mahsba;
                        cmd.Parameters.Add("tenthuoc", OracleDbType.NVarchar2).Value = tenthuoc;
                        cmd.Parameters.Add("ngaydt", OracleDbType.Date).Value = ngaydt;

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Xóa thuốc thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    LoadDonThuocData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa thuốc: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region TAB 4: BỆNH NHÂN DO TÔI ĐIỀU TRỊ
        private void SetupTabBenhNhan()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            Label lblTitle = new Label
            {
                Text = "BN ĐƯỢC PHÂN CÔNG ĐIỀU TRỊ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 188, 156),
                Location = new Point(5, 12),
                AutoSize = true
            };
            panelTop.Controls.Add(lblTitle);

            // Button Cập nhật tiền sử bệnh án
            Button btnEdit = new Button
            {
                Text = "Cập Nhật Bệnh Án (Tiền Sử/Dị Ứng)",
                Location = new Point(480, 10),
                Width = 240,
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
                Location = new Point(730, 10),
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
                // VPD tự động filter chỉ lấy những BN mà bác sĩ này đang khám/điều trị
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
                MessageBox.Show($"Lỗi lấy danh sách bệnh nhân điều trị: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditBenhNhan_Click(object sender, EventArgs e)
        {
            if (dgvBenhNhan.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn bệnh nhân cần sửa bệnh án.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mabn = dgvBenhNhan.CurrentRow.Cells["MABN"].Value?.ToString() ?? "";
            string tenbn = dgvBenhNhan.CurrentRow.Cells["TENBN"].Value?.ToString() ?? "";
            string tiensubenh = dgvBenhNhan.CurrentRow.Cells["TIENSUBENH"].Value?.ToString() ?? "";
            string tiensubenhgd = dgvBenhNhan.CurrentRow.Cells["TIENSUBENHGD"].Value?.ToString() ?? "";
            string diungthuoc = dgvBenhNhan.CurrentRow.Cells["DIUNGTHUOC"].Value?.ToString() ?? "";

            using (FormBenhNhanMedicalUpdate form = new FormBenhNhanMedicalUpdate(_conn, mabn, tenbn, tiensubenh, tiensubenhgd, diungthuoc))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBenhNhanData();
                }
            }
        }
        #endregion
    }

    #region FORM CON TAB 1: FORM CẬP NHẬT CHẨN ĐOÁN / ĐIỀU TRỊ / KẾT LUẬN HSBA
    public class FormHSBAUpdateText : Form
    {
        private OracleConnection _conn;
        private string _mahsba;

        // Controls
        private TextBox txtChanDoan;
        private TextBox txtDieuTri;
        private TextBox txtKetLuan;

        public FormHSBAUpdateText(OracleConnection conn, string mahsba, string chandoan, string dieutri, string ketluan)
        {
            _conn = conn;
            _mahsba = mahsba;

            InitializeComponent();
            SetupUI();

            // Gán dữ liệu ban đầu
            txtChanDoan.Text = chandoan;
            txtDieuTri.Text = dieutri;
            txtKetLuan.Text = ketluan;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = $"Cập nhật chẩn đoán/điều trị - HSBA: {_mahsba}";
            this.Size = new Size(500, 360);
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

            // Chẩn đoán
            Label lblChanDoan = new Label { Text = "Chẩn đoán bệnh:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtChanDoan = new TextBox { Location = new Point(160, y), Size = new Size(280, 50), Multiline = true, Font = textFont };
            this.Controls.Add(lblChanDoan); this.Controls.Add(txtChanDoan);
            y += 70;

            // Điều trị
            Label lblDieuTri = new Label { Text = "Phương pháp điều trị:", Location = new Point(25, y), Size = new Size(130, 20), Font = labelFont };
            txtDieuTri = new TextBox { Location = new Point(160, y), Size = new Size(280, 50), Multiline = true, Font = textFont };
            this.Controls.Add(lblDieuTri); this.Controls.Add(txtDieuTri);
            y += 70;

            // Kết luận
            Label lblKetLuan = new Label { Text = "Kết luận y tế:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtKetLuan = new TextBox { Location = new Point(160, y), Size = new Size(280, 50), Multiline = true, Font = textFont };
            this.Controls.Add(lblKetLuan); this.Controls.Add(txtKetLuan);
            y += 70;

            // Buttons
            Button btnSave = new Button
            {
                Text = "Cập nhật",
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "UPDATE ADMIN_PHANHE1.HSBA SET CHANDOAN = :cd, DIEUTRI = :dt, KETLUAN = :kl WHERE MAHSBA = :id";
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("cd", OracleDbType.NVarchar2).Value = txtChanDoan.Text.Trim();
                    cmd.Parameters.Add("dt", OracleDbType.NVarchar2).Value = txtDieuTri.Text.Trim();
                    cmd.Parameters.Add("kl", OracleDbType.NVarchar2).Value = txtKetLuan.Text.Trim();
                    cmd.Parameters.Add("id", OracleDbType.Varchar2).Value = _mahsba;

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật chẩn đoán/điều trị hồ sơ bệnh án thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cập nhật thất bại: {ex.Message}", "Lỗi thực thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    #endregion

    #region FORM CON TAB 2: FORM THÊM DỊCH VỤ HỖ TRỢ CHẨN ĐOÁN
    public class FormDichVuAdd : Form
    {
        private OracleConnection _conn;
        private string _mahsba;

        // Controls
        private TextBox txtLoaiDV;
        private DateTimePicker dtpNgayDV;

        public FormDichVuAdd(OracleConnection conn, string mahsba)
        {
            _conn = conn;
            _mahsba = mahsba;

            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Chỉ định dịch vụ chẩn đoán mới";
            this.Size = new Size(450, 240);
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
            Label lblHS = new Label { Text = "Mã HSBA:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            Label lblHSVal = new Label { Text = _mahsba, Location = new Point(160, y), Size = new Size(240, 20), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Blue };
            this.Controls.Add(lblHS); this.Controls.Add(lblHSVal);
            y += 40;

            // Loại dịch vụ
            Label lblLoai = new Label { Text = "Loại dịch vụ *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtLoaiDV = new TextBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont };
            this.Controls.Add(lblLoai); this.Controls.Add(txtLoaiDV);
            y += 40;

            // Ngày thực hiện
            Label lblNgay = new Label { Text = "Ngày chỉ định *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            dtpNgayDV = new DateTimePicker
            {
                Location = new Point(160, y),
                Size = new Size(240, 23),
                Font = textFont,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm:ss"
            };
            this.Controls.Add(lblNgay); this.Controls.Add(dtpNgayDV);
            y += 50;

            // Buttons
            Button btnSave = new Button
            {
                Text = "Chỉ Định",
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLoaiDV.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập loại dịch vụ chỉ định.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO ADMIN_PHANHE1.HSBA_DV (MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA) VALUES (:mahsba, :loaidv, :ngaydv, NULL, NULL)";
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = _mahsba;
                    cmd.Parameters.Add("loaidv", OracleDbType.NVarchar2).Value = txtLoaiDV.Text.Trim();
                    cmd.Parameters.Add("ngaydv", OracleDbType.Date).Value = dtpNgayDV.Value;

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Chỉ định dịch vụ hỗ trợ chẩn đoán thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chỉ định dịch vụ thất bại: {ex.Message}", "Lỗi thực thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    #endregion

    #region FORM CON TAB 3: THÊM / SỬA THUỐC ĐƠN THUỐC
    public class FormThuocAdd : Form
    {
        private OracleConnection _conn;
        private string _mahsba;

        // Controls
        private TextBox txtTenThuoc;
        private DateTimePicker dtpNgayDT;
        private TextBox txtLieuDung;

        public FormThuocAdd(OracleConnection conn, string mahsba)
        {
            _conn = conn;
            _mahsba = mahsba;

            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Kê đơn thuốc mới";
            this.Size = new Size(450, 270);
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
            Label lblHS = new Label { Text = "Mã HSBA:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            Label lblHSVal = new Label { Text = _mahsba, Location = new Point(160, y), Size = new Size(240, 20), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Blue };
            this.Controls.Add(lblHS); this.Controls.Add(lblHSVal);
            y += 40;

            // Tên thuốc
            Label lblThuoc = new Label { Text = "Tên thuốc *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtTenThuoc = new TextBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont };
            this.Controls.Add(lblThuoc); this.Controls.Add(txtTenThuoc);
            y += 40;

            // Ngày kê đơn
            Label lblNgay = new Label { Text = "Ngày kê đơn *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            dtpNgayDT = new DateTimePicker
            {
                Location = new Point(160, y),
                Size = new Size(240, 23),
                Font = textFont,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm:ss"
            };
            this.Controls.Add(lblNgay); this.Controls.Add(dtpNgayDT);
            y += 40;

            // Liều dùng
            Label lblLieu = new Label { Text = "Liều dùng *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtLieuDung = new TextBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont };
            this.Controls.Add(lblLieu); this.Controls.Add(txtLieuDung);
            y += 50;

            // Buttons
            Button btnSave = new Button
            {
                Text = "Kê Đơn",
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenThuoc.Text.Trim()) || string.IsNullOrEmpty(txtLieuDung.Text.Trim()))
            {
                MessageBox.Show("Vui lòng điền đầy đủ Tên thuốc và Liều dùng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO ADMIN_PHANHE1.DONTHUOC (MAHSBA, TENTHUOC, NGAYDT, LIEUDUNG) VALUES (:mahsba, :tenthuoc, :ngaydt, :lieudung)";
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = _mahsba;
                    cmd.Parameters.Add("tenthuoc", OracleDbType.NVarchar2).Value = txtTenThuoc.Text.Trim();
                    cmd.Parameters.Add("ngaydt", OracleDbType.Date).Value = dtpNgayDT.Value;
                    cmd.Parameters.Add("lieudung", OracleDbType.NVarchar2).Value = txtLieuDung.Text.Trim();

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Kê thuốc vào đơn thuốc thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kê đơn thuốc thất bại: {ex.Message}", "Lỗi thực thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class FormThuocEdit : Form
    {
        private OracleConnection _conn;
        private string _mahsba;
        private string _oldTenthuoc;
        private DateTime _ngaydt;

        // Controls
        private TextBox txtTenThuoc;
        private TextBox txtLieuDung;

        public FormThuocEdit(OracleConnection conn, string mahsba, string oldTenthuoc, DateTime ngaydt, string lieudung)
        {
            _conn = conn;
            _mahsba = mahsba;
            _oldTenthuoc = oldTenthuoc;
            _ngaydt = ngaydt;

            InitializeComponent();
            SetupUI();

            // Set current
            txtTenThuoc.Text = oldTenthuoc;
            txtLieuDung.Text = lieudung;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "Chỉnh sửa đơn thuốc";
            this.Size = new Size(450, 240);
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

            // Mã HSBA & Ngày kê
            Label lblHS = new Label { Text = "Thông tin:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            Label lblHSVal = new Label { Text = $"{_mahsba} | Ngày: {_ngaydt:dd/MM/yyyy}", Location = new Point(160, y), Size = new Size(240, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Blue };
            this.Controls.Add(lblHS); this.Controls.Add(lblHSVal);
            y += 40;

            // Tên thuốc
            Label lblThuoc = new Label { Text = "Tên thuốc mới *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtTenThuoc = new TextBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont };
            this.Controls.Add(lblThuoc); this.Controls.Add(txtTenThuoc);
            y += 40;

            // Liều dùng
            Label lblLieu = new Label { Text = "Liều dùng mới *:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtLieuDung = new TextBox { Location = new Point(160, y), Size = new Size(240, 23), Font = textFont };
            this.Controls.Add(lblLieu); this.Controls.Add(txtLieuDung);
            y += 50;

            // Buttons
            Button btnSave = new Button
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenThuoc.Text.Trim()) || string.IsNullOrEmpty(txtLieuDung.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập Tên thuốc và Liều dùng mới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = @"UPDATE ADMIN_PHANHE1.DONTHUOC 
                                 SET TENTHUOC = :tenthuoc, LIEUDUNG = :lieudung 
                                 WHERE MAHSBA = :mahsba AND TENTHUOC = :oldtenthuoc AND NGAYDT = :ngaydt";
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("tenthuoc", OracleDbType.NVarchar2).Value = txtTenThuoc.Text.Trim();
                    cmd.Parameters.Add("lieudung", OracleDbType.NVarchar2).Value = txtLieuDung.Text.Trim();
                    cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = _mahsba;
                    cmd.Parameters.Add("oldtenthuoc", OracleDbType.NVarchar2).Value = _oldTenthuoc;
                    cmd.Parameters.Add("ngaydt", OracleDbType.Date).Value = _ngaydt;

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật đơn thuốc thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cập nhật thuốc thất bại: {ex.Message}", "Lỗi thực thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    #endregion

    #region FORM CON TAB 4: BỆNH NHÂN DO TÔI ĐIỀU TRỊ - CẬP NHẬT MEDICAL INFO
    public class FormBenhNhanMedicalUpdate : Form
    {
        private OracleConnection _conn;
        private string _mabn;
        private string _tenbn;

        // Controls
        private TextBox txtTienSu;
        private TextBox txtTienSuGD;
        private TextBox txtDiUng;

        public FormBenhNhanMedicalUpdate(OracleConnection conn, string mabn, string tenbn, string tiensu, string tiensugd, string diung)
        {
            _conn = conn;
            _mabn = mabn;
            _tenbn = tenbn;

            InitializeComponent();
            SetupUI();

            // Set current
            txtTienSu.Text = tiensu;
            txtTienSuGD.Text = tiensugd;
            txtDiUng.Text = diung;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = $"Cập nhật bệnh án - Bệnh nhân: {_tenbn} ({_mabn})";
            this.Size = new Size(500, 360);
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

            // Tiền sử bệnh
            Label lblTienSu = new Label { Text = "Tiền sử bệnh:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtTienSu = new TextBox { Location = new Point(160, y), Size = new Size(280, 50), Multiline = true, Font = textFont };
            this.Controls.Add(lblTienSu); this.Controls.Add(txtTienSu);
            y += 70;

            // Tiền sử gia đình
            Label lblTienSuGD = new Label { Text = "Tiền sử gia đình:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtTienSuGD = new TextBox { Location = new Point(160, y), Size = new Size(280, 50), Multiline = true, Font = textFont };
            this.Controls.Add(lblTienSuGD); this.Controls.Add(txtTienSuGD);
            y += 70;

            // Dị ứng thuốc
            Label lblDiUng = new Label { Text = "Dị ứng thuốc:", Location = new Point(25, y), Size = new Size(120, 20), Font = labelFont };
            txtDiUng = new TextBox { Location = new Point(160, y), Size = new Size(280, 50), Multiline = true, Font = textFont };
            this.Controls.Add(lblDiUng); this.Controls.Add(txtDiUng);
            y += 70;

            // Buttons
            Button btnSave = new Button
            {
                Text = "Cập nhật",
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"UPDATE ADMIN_PHANHE1.BENHNHAN 
                                 SET TIENSUBENH = :ts, TIENSUBENHGD = :tsgd, DIUNGTHUOC = :du 
                                 WHERE MABN = :mabn";
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add("ts", OracleDbType.NVarchar2).Value = txtTienSu.Text.Trim();
                    cmd.Parameters.Add("tsgd", OracleDbType.NVarchar2).Value = txtTienSuGD.Text.Trim();
                    cmd.Parameters.Add("du", OracleDbType.NVarchar2).Value = txtDiUng.Text.Trim();
                    cmd.Parameters.Add("mabn", OracleDbType.Varchar2).Value = _mabn;

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật bệnh án của bệnh nhân thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cập nhật thất bại: {ex.Message}", "Lỗi thực thi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    #endregion
}
