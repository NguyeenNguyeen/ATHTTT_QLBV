using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public class FormKyThuatVien : Form, ILogoutAwareForm
    {
        private readonly OracleConnection _conn;
        private readonly string _username;
        private readonly string _displayName;
        public bool LogoutRequested { get; private set; }

        private TabControl tabControl = null!;
        private DataGridView dgvProfile = null!;
        private DataGridView dgvServices = null!;

        public FormKyThuatVien(string username, string displayName, OracleConnection? conn)
        {
            _username = username;
            _displayName = displayName;
            _conn = conn ?? throw new ArgumentNullException(nameof(conn), "Kết nối cơ sở dữ liệu không hợp lệ.");

            InitializeComponent();
            SetupUI();
            LoadProfile();
            LoadAssignedServices();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "Hệ thống Kỹ thuật viên | RBAC";
            Size = new Size(1050, 640);
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
        }

        private void SetupUI()
        {
            Font fontHeader = new Font("Segoe UI", 11, FontStyle.Bold);
            Font fontRegular = new Font("Segoe UI", 9, FontStyle.Regular);

            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.FromArgb(39, 174, 96)
            };
            Controls.Add(panelHeader);

            Label lblStatus = new Label
            {
                Text = $"Xin chào kỹ thuật viên: {_displayName} ({_username}) | RBAC chỉ mở dữ liệu được phân công",
                ForeColor = Color.White,
                Font = fontHeader,
                Location = new Point(18, 20),
                AutoSize = true
            };
            panelHeader.Controls.Add(lblStatus);

            Button btnLogout = new Button
            {
                Text = "Đăng xuất",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(192, 57, 43),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(110, 34),
                Location = new Point(panelHeader.Width - 130, 15),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;
            panelHeader.Controls.Add(btnLogout);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = fontRegular
            };
            Controls.Add(tabControl);
            tabControl.BringToFront();

            SetupProfileTab();
            SetupServiceTab();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            LogoutRequested = true;
            try
            {
                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch
            {
            }

            Close();
        }

        private void SetupProfileTab()
        {
            TabPage tabProfile = new TabPage("THÔNG TIN CÁ NHÂN") { Padding = new Padding(10) };
            tabControl.TabPages.Add(tabProfile);

            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 54 };
            tabProfile.Controls.Add(panelTop);

            Button btnRefresh = CreateButton("Tải lại", new Point(10, 10), Color.FromArgb(52, 152, 219));
            btnRefresh.Click += (s, e) => LoadProfile();
            panelTop.Controls.Add(btnRefresh);

            Button btnEdit = CreateButton("Cập nhật thông tin", new Point(130, 10), Color.FromArgb(46, 204, 113), 160);
            btnEdit.Click += BtnEditProfile_Click;
            panelTop.Controls.Add(btnEdit);

            dgvProfile = CreateGrid();
            tabProfile.Controls.Add(dgvProfile);
            dgvProfile.BringToFront();
        }

        private void SetupServiceTab()
        {
            TabPage tabServices = new TabPage("DỊCH VỤ ĐƯỢC PHÂN CÔNG") { Padding = new Padding(10) };
            tabControl.TabPages.Add(tabServices);

            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 54 };
            tabServices.Controls.Add(panelTop);

            Button btnRefresh = CreateButton("Tải lại", new Point(10, 10), Color.FromArgb(52, 152, 219));
            btnRefresh.Click += (s, e) => LoadAssignedServices();
            panelTop.Controls.Add(btnRefresh);

            Button btnUpdateResult = CreateButton("Ghi kết quả", new Point(130, 10), Color.FromArgb(230, 126, 34), 130);
            btnUpdateResult.Click += BtnUpdateResult_Click;
            panelTop.Controls.Add(btnUpdateResult);

            dgvServices = CreateGrid();
            tabServices.Controls.Add(dgvServices);
            dgvServices.BringToFront();
        }

        private Button CreateButton(string text, Point location, Color color, int width = 100)
        {
            Button button = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(width, 32),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private DataGridView CreateGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White
            };
        }

        private void LoadProfile()
        {
            try
            {
                string query = @"SELECT MANV, HOTEN, PHAI, NGAYSINH, CMND, QUEQUAN, SODT, VAITRO, CHUYENKHOA, COSO
                                 FROM ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN";
                dgvProfile.DataSource = ExecuteTable(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin cá nhân: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAssignedServices()
        {
            try
            {
                string query = @"SELECT MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA
                                 FROM ADMIN_PHANHE1.V_RBAC_KTV_DICHVU
                                 ORDER BY NGAYDV DESC, MAHSBA DESC";
                dgvServices.DataSource = ExecuteTable(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dịch vụ được phân công: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable ExecuteTable(string query)
        {
            using OracleCommand cmd = new OracleCommand(query, _conn);
            using OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        private void BtnEditProfile_Click(object? sender, EventArgs e)
        {
            if (dgvProfile.CurrentRow == null)
            {
                MessageBox.Show("Không có dữ liệu cá nhân để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using FormKtvProfileEdit form = new FormKtvProfileEdit(dgvProfile.CurrentRow);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string query = @"UPDATE ADMIN_PHANHE1.V_RBAC_KTV_THONGTIN
                                     SET QUEQUAN = :quequan, SODT = :sodt, COSO = :coso";
                    using OracleCommand cmd = new OracleCommand(query, _conn);
                    cmd.BindByName = true;
                    cmd.Parameters.Add("quequan", OracleDbType.NVarchar2).Value = form.QueQuan;
                    cmd.Parameters.Add("sodt", OracleDbType.Varchar2).Value = form.SoDt;
                    cmd.Parameters.Add("coso", OracleDbType.NVarchar2).Value = form.CoSo;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật thông tin cá nhân thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProfile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi cập nhật thông tin cá nhân: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnUpdateResult_Click(object? sender, EventArgs e)
        {
            if (dgvServices.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một dịch vụ để ghi kết quả.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow row = dgvServices.CurrentRow;
            string mahsba = GetCell(row, "MAHSBA");
            string loaidv = GetCell(row, "LOAIDV");
            DateTime ngaydv = Convert.ToDateTime(row.Cells["NGAYDV"].Value);
            string currentResult = GetCell(row, "KETQUA");

            using FormKetQuaEdit form = new FormKetQuaEdit(mahsba, loaidv, currentResult);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string query = @"UPDATE ADMIN_PHANHE1.V_RBAC_KTV_DICHVU
                                     SET KETQUA = :ketqua
                                     WHERE MAHSBA = :mahsba AND LOAIDV = :loaidv AND NGAYDV = :ngaydv";
                    using OracleCommand cmd = new OracleCommand(query, _conn);
                    cmd.BindByName = true;
                    cmd.Parameters.Add("ketqua", OracleDbType.NVarchar2).Value = form.KetQua;
                    cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = mahsba;
                    cmd.Parameters.Add("loaidv", OracleDbType.NVarchar2).Value = loaidv;
                    cmd.Parameters.Add("ngaydv", OracleDbType.Date).Value = ngaydv;
                    int updated = cmd.ExecuteNonQuery();
                    MessageBox.Show(updated > 0 ? "Ghi kết quả thành công." : "Không có dòng nào được cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAssignedServices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi ghi kết quả: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string GetCell(DataGridViewRow row, string columnName)
        {
            return row.DataGridView != null && row.DataGridView.Columns.Contains(columnName)
                ? row.Cells[columnName].Value?.ToString() ?? ""
                : "";
        }
    }

    internal class FormKtvProfileEdit : Form
    {
        private readonly TextBox txtQueQuan = null!;
        private readonly TextBox txtSoDt = null!;
        private readonly TextBox txtCoSo = null!;

        public string QueQuan => txtQueQuan.Text.Trim();
        public string SoDt => txtSoDt.Text.Trim();
        public string CoSo => txtCoSo.Text.Trim();

        public FormKtvProfileEdit(DataGridViewRow row)
        {
            Text = "Cập nhật thông tin cá nhân";
            Size = new Size(430, 240);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            txtQueQuan = AddField("Quê quán:", GetCell(row, "QUEQUAN"), 24);
            txtSoDt = AddField("Số điện thoại:", GetCell(row, "SODT"), 68);
            txtCoSo = AddField("Cơ sở:", GetCell(row, "COSO"), 112);

            Button btnSave = new Button { Text = "Lưu", Location = new Point(220, 160), Size = new Size(80, 30), DialogResult = DialogResult.OK };
            Button btnCancel = new Button { Text = "Hủy", Location = new Point(310, 160), Size = new Size(80, 30), DialogResult = DialogResult.Cancel };
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        private TextBox AddField(string label, string value, int y)
        {
            Controls.Add(new Label { Text = label, Location = new Point(20, y + 4), Size = new Size(110, 22) });
            TextBox textBox = new TextBox { Text = value, Location = new Point(140, y), Size = new Size(250, 27) };
            Controls.Add(textBox);
            return textBox;
        }

        private static string GetCell(DataGridViewRow row, string columnName)
        {
            return row.DataGridView != null && row.DataGridView.Columns.Contains(columnName)
                ? row.Cells[columnName].Value?.ToString() ?? ""
                : "";
        }
    }

    internal class FormKetQuaEdit : Form
    {
        private readonly TextBox txtKetQua = null!;

        public string KetQua => txtKetQua.Text.Trim();

        public FormKetQuaEdit(string mahsba, string loaidv, string currentResult)
        {
            Text = "Ghi kết quả dịch vụ";
            Size = new Size(520, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Controls.Add(new Label { Text = $"HSBA: {mahsba}", Location = new Point(20, 20), AutoSize = true });
            Controls.Add(new Label { Text = $"Dịch vụ: {loaidv}", Location = new Point(20, 48), AutoSize = true });
            Controls.Add(new Label { Text = "Kết quả:", Location = new Point(20, 82), AutoSize = true });

            txtKetQua = new TextBox
            {
                Text = currentResult,
                Location = new Point(20, 108),
                Size = new Size(460, 90),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            Controls.Add(txtKetQua);

            Button btnSave = new Button { Text = "Lưu", Location = new Point(310, 215), Size = new Size(80, 30), DialogResult = DialogResult.OK };
            Button btnCancel = new Button { Text = "Hủy", Location = new Point(400, 215), Size = new Size(80, 30), DialogResult = DialogResult.Cancel };
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }
    }
}
