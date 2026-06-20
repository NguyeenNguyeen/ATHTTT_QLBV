using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public class FormBenhNhan : Form
    {
        private readonly OracleConnection _conn;
        private readonly string _username;
        private readonly string _displayName;

        private DataGridView dgvProfile = null!;

        public FormBenhNhan(string username, string displayName, OracleConnection? conn)
        {
            _username = username;
            _displayName = displayName;
            _conn = conn ?? throw new ArgumentNullException(nameof(conn), "Kết nối cơ sở dữ liệu không hợp lệ.");

            InitializeComponent();
            SetupUI();
            LoadProfile();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "Cổng thông tin Bệnh nhân | RBAC";
            Size = new Size(1050, 600);
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
        }

        private void SetupUI()
        {
            Font fontHeader = new Font("Segoe UI", 11, FontStyle.Bold);

            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.FromArgb(142, 68, 173)
            };
            Controls.Add(panelHeader);

            Label lblStatus = new Label
            {
                Text = $"Xin chào bệnh nhân: {_displayName} ({_username}) | RBAC chỉ mở hồ sơ của chính bạn",
                ForeColor = Color.White,
                Font = fontHeader,
                Location = new Point(18, 20),
                AutoSize = true
            };
            panelHeader.Controls.Add(lblStatus);

            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 54 };
            Controls.Add(panelTop);
            panelTop.BringToFront();

            Button btnRefresh = CreateButton("Tải lại", new Point(10, 10), Color.FromArgb(52, 152, 219));
            btnRefresh.Click += (s, e) => LoadProfile();
            panelTop.Controls.Add(btnRefresh);

            Button btnEdit = CreateButton("Cập nhật thông tin", new Point(130, 10), Color.FromArgb(46, 204, 113), 160);
            btnEdit.Click += BtnEditProfile_Click;
            panelTop.Controls.Add(btnEdit);

            dgvProfile = new DataGridView
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
            Controls.Add(dgvProfile);
            dgvProfile.BringToFront();
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

        private void LoadProfile()
        {
            try
            {
                string query = @"SELECT MABN, TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG, QUANHUYEN, TINHTP,
                                        TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC
                                 FROM ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN";
                using OracleCommand cmd = new OracleCommand(query, _conn);
                using OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvProfile.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải hồ sơ bệnh nhân: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditProfile_Click(object? sender, EventArgs e)
        {
            if (dgvProfile.CurrentRow == null)
            {
                MessageBox.Show("Không có hồ sơ để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using FormBenhNhanProfileEdit form = new FormBenhNhanProfileEdit(dgvProfile.CurrentRow);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string query = @"UPDATE ADMIN_PHANHE1.V_RBAC_BENHNHAN_THONGTIN
                                     SET SONHA = :sonha,
                                         TENDUONG = :tenduong,
                                         QUANHUYEN = :quanhuyen,
                                         TINHTP = :tinhtp,
                                         TIENSUBENH = :tiensubenh,
                                         TIENSUBENHGD = :tiensubenhgd,
                                         DIUNGTHUOC = :diungthuoc";

                    using OracleCommand cmd = new OracleCommand(query, _conn);
                    cmd.BindByName = true;
                    cmd.Parameters.Add("sonha", OracleDbType.NVarchar2).Value = form.SoNha;
                    cmd.Parameters.Add("tenduong", OracleDbType.NVarchar2).Value = form.TenDuong;
                    cmd.Parameters.Add("quanhuyen", OracleDbType.NVarchar2).Value = form.QuanHuyen;
                    cmd.Parameters.Add("tinhtp", OracleDbType.NVarchar2).Value = form.TinhTp;
                    cmd.Parameters.Add("tiensubenh", OracleDbType.NVarchar2).Value = form.TienSuBenh;
                    cmd.Parameters.Add("tiensubenhgd", OracleDbType.NVarchar2).Value = form.TienSuBenhGd;
                    cmd.Parameters.Add("diungthuoc", OracleDbType.NVarchar2).Value = form.DiUngThuoc;
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Cập nhật hồ sơ bệnh nhân thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProfile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi cập nhật hồ sơ bệnh nhân: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    internal class FormBenhNhanProfileEdit : Form
    {
        private readonly TextBox txtSoNha = null!;
        private readonly TextBox txtTenDuong = null!;
        private readonly TextBox txtQuanHuyen = null!;
        private readonly TextBox txtTinhTp = null!;
        private readonly TextBox txtTienSuBenh = null!;
        private readonly TextBox txtTienSuBenhGd = null!;
        private readonly TextBox txtDiUngThuoc = null!;

        public string SoNha => txtSoNha.Text.Trim();
        public string TenDuong => txtTenDuong.Text.Trim();
        public string QuanHuyen => txtQuanHuyen.Text.Trim();
        public string TinhTp => txtTinhTp.Text.Trim();
        public string TienSuBenh => txtTienSuBenh.Text.Trim();
        public string TienSuBenhGd => txtTienSuBenhGd.Text.Trim();
        public string DiUngThuoc => txtDiUngThuoc.Text.Trim();

        public FormBenhNhanProfileEdit(DataGridViewRow row)
        {
            Text = "Cập nhật hồ sơ bệnh nhân";
            Size = new Size(560, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            txtSoNha = AddField("Số nhà:", GetCell(row, "SONHA"), 22);
            txtTenDuong = AddField("Tên đường:", GetCell(row, "TENDUONG"), 62);
            txtQuanHuyen = AddField("Quận/Huyện:", GetCell(row, "QUANHUYEN"), 102);
            txtTinhTp = AddField("Tỉnh/TP:", GetCell(row, "TINHTP"), 142);
            txtTienSuBenh = AddTextArea("Tiền sử bệnh:", GetCell(row, "TIENSUBENH"), 190);
            txtTienSuBenhGd = AddTextArea("TS bệnh GĐ:", GetCell(row, "TIENSUBENHGD"), 275);
            txtDiUngThuoc = AddTextArea("Dị ứng thuốc:", GetCell(row, "DIUNGTHUOC"), 360);

            Button btnSave = new Button { Text = "Lưu", Location = new Point(350, 440), Size = new Size(80, 30), DialogResult = DialogResult.OK };
            Button btnCancel = new Button { Text = "Hủy", Location = new Point(440, 440), Size = new Size(80, 30), DialogResult = DialogResult.Cancel };
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            AcceptButton = btnSave;
            CancelButton = btnCancel;
        }

        private TextBox AddField(string label, string value, int y)
        {
            Controls.Add(new Label { Text = label, Location = new Point(20, y + 4), Size = new Size(115, 22) });
            TextBox textBox = new TextBox { Text = value, Location = new Point(150, y), Size = new Size(370, 27) };
            Controls.Add(textBox);
            return textBox;
        }

        private TextBox AddTextArea(string label, string value, int y)
        {
            Controls.Add(new Label { Text = label, Location = new Point(20, y + 4), Size = new Size(115, 22) });
            TextBox textBox = new TextBox
            {
                Text = value,
                Location = new Point(150, y),
                Size = new Size(370, 65),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
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
}
