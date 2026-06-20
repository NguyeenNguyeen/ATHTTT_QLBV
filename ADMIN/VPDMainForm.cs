using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public partial class VPDMainForm : Form
    {
        private VPDDataManager _dataManager;
        private string _loggedInUsername;
        private string _doctorName;
        private OracleConnection _doctorConnection;

        public VPDMainForm(string username, string doctorName, OracleConnection? connection)
        {
            InitializeComponent();
            _loggedInUsername = username;
            _doctorName = doctorName;
            _doctorConnection = connection ?? throw new Exception("Lỗi: Không có kết nối cơ sở dữ liệu!");
            _dataManager = new VPDDataManager(_doctorConnection, username);
            SetupUI();
            LoadAllData();
        }

        private void SetupUI()
        {
            this.Text = "VPD - Hệ thống Quản lý Y tế (Bác sĩ/Y sĩ)";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Panel Header (VPD Status)
            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(0, 102, 204)
            };
            this.Controls.Add(panelHeader);

            Label lblStatus = new Label
            {
                Text = $"Đang đăng nhập: {_loggedInUsername} ({_doctorName}) | VPD đang hoạt động",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            panelHeader.Controls.Add(lblStatus);

            Button btnViewVPD = new Button
            {
                Text = "Xem WHERE VPD",
                Location = new Point(panelHeader.Width - 150, 15),
                Width = 130,
                Height = 30,
                BackColor = Color.FromArgb(255, 153, 0),
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnViewVPD.Click += (s, e) => ShowVPDInfo();
            panelHeader.Controls.Add(btnViewVPD);

            // Panel Sidebar (Tabs)
            Panel panelSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 180,
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelSidebar);

            int yPos = 20;
            string[] tabs = { "Hồ sơ bệnh án", "Bệnh nhân", "Đơn thuốc", "Dịch vụ chẩn đoán" };
            foreach (string tab in tabs)
            {
                Button btnTab = new Button
                {
                    Text = tab,
                    Location = new Point(10, yPos),
                    Width = 160,
                    Height = 40,
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 9)
                };
                btnTab.Click += (s, e) => SelectTab(tab, btnTab, panelSidebar);
                panelSidebar.Controls.Add(btnTab);
                yPos += 50;
            }

            // Panel Main Content
            Panel panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            this.Controls.Add(panelContent);
            panelContent.Name = "panelContent";

            // Mặc định load tab "Hồ sơ bệnh án"
            SelectTab("Hồ sơ bệnh án", panelSidebar.Controls[0] as Button, panelSidebar);
        }

        private void SelectTab(string tabName, Button btnTab, Panel panelSidebar)
        {
            // Cập nhật trạng thái button
            foreach (Control c in panelSidebar.Controls)
            {
                if (c is Button btn)
                    btn.BackColor = Color.White;
            }
            btnTab.BackColor = Color.FromArgb(0, 153, 102);
            btnTab.ForeColor = Color.White;

            // Cập nhật content
            Panel panelContent = this.Controls["panelContent"] as Panel;
            panelContent.Controls.Clear();

            switch (tabName)
            {
                case "Hồ sơ bệnh án":
                    LoadHSBATab(panelContent);
                    break;
                case "Bệnh nhân":
                    LoadBenhNhanTab(panelContent);
                    break;
                case "Đơn thuốc":
                    LoadDonThuocTab(panelContent);
                    break;
                case "Dịch vụ chẩn đoán":
                    LoadHSBA_DVTab(panelContent);
                    break;
            }
        }

        private void LoadHSBATab(Panel panelContent)
        {
            Label lblTitle = new Label
            {
                Text = "Hồ sơ bệnh án (HSBA)",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                Location = new Point(0, 0),
                AutoSize = true
            };
            panelContent.Controls.Add(lblTitle);

            DataGridView dgv = new DataGridView
            {
                Location = new Point(0, 30),
                Width = panelContent.Width - 20,
                Height = 350,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            panelContent.Controls.Add(dgv);

            DataTable dt = _dataManager.GetHSBA();
            dgv.DataSource = dt;
            dgv.AutoResizeColumns();

            // Nút Cập nhật
            Button btnUpdate = new Button
            {
                Text = "Cập nhật",
                Location = new Point(0, 400),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(0, 153, 102),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnUpdate.Click += (s, e) => EditHSBA(dgv);
            panelContent.Controls.Add(btnUpdate);

            Label lblSQL = new Label
            {
                Text = "SQL: SELECT * FROM HSBA (VPD tự thêm: WHERE MABS = '" + _loggedInUsername + "')",
                Font = new Font("Arial", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(0, 450),
                AutoSize = true,
                MaximumSize = new Size(panelContent.Width - 20, 0)
            };
            panelContent.Controls.Add(lblSQL);
        }

        private void EditHSBA(DataGridView dgv)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn 1 dòng để cập nhật.", "Thông báo");
                return;
            }

            string mahsba = dgv.CurrentRow.Cells[0].Value?.ToString();
            if (string.IsNullOrEmpty(mahsba))
            {
                MessageBox.Show("Không thể xác định MAHSBA.", "Lỗi");
                return;
            }

            Form editForm = new Form
            {
                Text = $"Cập nhật HSBA: {mahsba}",
                Width = 500,
                Height = 300,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false
            };

            int yPos = 20;

            // Chẩn đoán
            Label lbl1 = new Label { Text = "Chẩn đoán:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt1 = new TextBox { Location = new Point(150, yPos), Width = 300, Height = 25 };
            txt1.Text = dgv.CurrentRow.Cells[3].Value?.ToString() ?? "";
            editForm.Controls.Add(lbl1);
            editForm.Controls.Add(txt1);
            yPos += 40;

            // Điều trị
            Label lbl2 = new Label { Text = "Điều trị:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt2 = new TextBox { Location = new Point(150, yPos), Width = 300, Height = 25 };
            txt2.Text = dgv.CurrentRow.Cells[4].Value?.ToString() ?? "";
            editForm.Controls.Add(lbl2);
            editForm.Controls.Add(txt2);
            yPos += 40;

            // Kết luận
            Label lbl3 = new Label { Text = "Kết luận:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt3 = new TextBox { Location = new Point(150, yPos), Width = 300, Height = 25 };
            txt3.Text = dgv.CurrentRow.Cells[5].Value?.ToString() ?? "";
            editForm.Controls.Add(lbl3);
            editForm.Controls.Add(txt3);
            yPos += 40;

            // Nút Lưu
            Button btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(150, 200),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(0, 153, 102),
                ForeColor = Color.White
            };
            btnSave.Click += (s, e) =>
            {
                try
                {
                    _dataManager.UpdateHSBA(mahsba, txt1.Text, txt2.Text, txt3.Text);
                    MessageBox.Show("Cập nhật thành công!", "Thành công");
                    editForm.Close();
                    LoadAllData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
                }
            };
            editForm.Controls.Add(btnSave);

            editForm.ShowDialog();
        }

        private void LoadBenhNhanTab(Panel panelContent)
        {
            Label lblTitle = new Label
            {
                Text = "Danh sách bệnh nhân",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                Location = new Point(0, 0),
                AutoSize = true
            };
            panelContent.Controls.Add(lblTitle);

            DataGridView dgv = new DataGridView
            {
                Location = new Point(0, 30),
                Width = panelContent.Width - 20,
                Height = 350,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            panelContent.Controls.Add(dgv);

            DataTable dt = _dataManager.GetBenhNhan();
            dgv.DataSource = dt;
            dgv.AutoResizeColumns();

            Label lblSQL = new Label
            {
                Text = "SQL: SELECT * FROM BENHNHAN (VPD tự filter)",
                Font = new Font("Arial", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(0, 400),
                AutoSize = true
            };
            panelContent.Controls.Add(lblSQL);
        }

        private void LoadDonThuocTab(Panel panelContent)
        {
            Label lblTitle = new Label
            {
                Text = "Danh sách đơn thuốc",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                Location = new Point(0, 0),
                AutoSize = true
            };
            panelContent.Controls.Add(lblTitle);

            DataGridView dgv = new DataGridView
            {
                Location = new Point(0, 30),
                Width = panelContent.Width - 20,
                Height = 350,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            panelContent.Controls.Add(dgv);

            DataTable dt = _dataManager.GetDonThuoc();
            dgv.DataSource = dt;
            dgv.AutoResizeColumns();

            // Nút Thêm mới
            Button btnAdd = new Button
            {
                Text = "Thêm mới",
                Location = new Point(0, 400),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(0, 153, 102),
                ForeColor = Color.White
            };
            btnAdd.Click += (s, e) => AddDonThuoc();
            panelContent.Controls.Add(btnAdd);

            // Nút Xóa
            Button btnDelete = new Button
            {
                Text = "Xóa",
                Location = new Point(110, 400),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(204, 0, 0),
                ForeColor = Color.White
            };
            btnDelete.Click += (s, e) => DeleteDonThuoc(dgv);
            panelContent.Controls.Add(btnDelete);
        }

        private void AddDonThuoc()
        {
            Form addForm = new Form
            {
                Text = "Thêm đơn thuốc",
                Width = 500,
                Height = 300,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false
            };

            int yPos = 20;

            // Chọn HSBA
            Label lbl1 = new Label { Text = "HSBA:", Location = new Point(20, yPos), AutoSize = true };
            ComboBox cbo1 = new ComboBox { Location = new Point(150, yPos), Width = 300, Height = 25 };
            cbo1.DataSource = _dataManager.GetHSBAList();
            addForm.Controls.Add(lbl1);
            addForm.Controls.Add(cbo1);
            yPos += 40;

            // Ngày
            Label lbl2 = new Label { Text = "Ngày:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt2 = new TextBox { Location = new Point(150, yPos), Width = 300, Height = 25 };
            addForm.Controls.Add(lbl2);
            addForm.Controls.Add(txt2);
            yPos += 40;

            // Tên thuốc
            Label lbl3 = new Label { Text = "Tên thuốc:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt3 = new TextBox { Location = new Point(150, yPos), Width = 300, Height = 25 };
            addForm.Controls.Add(lbl3);
            addForm.Controls.Add(txt3);
            yPos += 40;

            // Liều dùng
            Label lbl4 = new Label { Text = "Liều dùng:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt4 = new TextBox { Location = new Point(150, yPos), Width = 300, Height = 25 };
            addForm.Controls.Add(lbl4);
            addForm.Controls.Add(txt4);

            // Nút Lưu
            Button btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(150, 240),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(0, 153, 102),
                ForeColor = Color.White
            };
            btnSave.Click += (s, e) =>
            {
                try
                {
                    if (cbo1.SelectedItem == null) throw new Exception("Chưa chọn HSBA!");
                    string mahsba = cbo1.SelectedItem?.ToString() ?? "";
                    if (string.IsNullOrEmpty(mahsba)) throw new Exception("HSBA không hợp lệ!");
                    _dataManager.InsertDonThuoc(mahsba, txt2.Text, txt3.Text, txt4.Text);
                    MessageBox.Show("Thêm thành công!", "Thành công");
                    addForm.Close();
                    LoadAllData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
                }
            };
            addForm.Controls.Add(btnSave);

            addForm.ShowDialog();
        }

        private void DeleteDonThuoc(DataGridView dgv)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn 1 dòng để xóa.", "Thông báo");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    string mahsba = dgv.CurrentRow.Cells[0].Value?.ToString() ?? "";
                    string ngaydt = dgv.CurrentRow.Cells[1].Value?.ToString() ?? "";
                    if (string.IsNullOrEmpty(mahsba) || string.IsNullOrEmpty(ngaydt))
                        throw new Exception("Dữ liệu không hợp lệ!");
                    _dataManager.DeleteDonThuoc(mahsba, ngaydt);
                    MessageBox.Show("Xóa thành công!", "Thành công");
                    LoadAllData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
                }
            }
        }

        private void LoadHSBA_DVTab(Panel panelContent)
        {
            Label lblTitle = new Label
            {
                Text = "Dịch vụ chẩn đoán",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                Location = new Point(0, 0),
                AutoSize = true
            };
            panelContent.Controls.Add(lblTitle);

            DataGridView dgv = new DataGridView
            {
                Location = new Point(0, 30),
                Width = panelContent.Width - 20,
                Height = 350,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            panelContent.Controls.Add(dgv);

            DataTable dt = _dataManager.GetHSBA_DV();
            dgv.DataSource = dt;
            dgv.AutoResizeColumns();

            // Nút Thêm
            Button btnAdd = new Button
            {
                Text = "Thêm mới",
                Location = new Point(0, 400),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(0, 153, 102),
                ForeColor = Color.White
            };
            btnAdd.Click += (s, e) => AddHSBA_DV();
            panelContent.Controls.Add(btnAdd);

            // Nút Xóa
            Button btnDelete = new Button
            {
                Text = "Xóa",
                Location = new Point(110, 400),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(204, 0, 0),
                ForeColor = Color.White
            };
            btnDelete.Click += (s, e) => DeleteHSBA_DV(dgv);
            panelContent.Controls.Add(btnDelete);
        }

        private void AddHSBA_DV()
        {
            Form addForm = new Form
            {
                Text = "Thêm dịch vụ chẩn đoán",
                Width = 500,
                Height = 350,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false
            };

            int yPos = 20;

            // HSBA
            Label lbl1 = new Label { Text = "HSBA:", Location = new Point(20, yPos), AutoSize = true };
            ComboBox cbo1 = new ComboBox { Location = new Point(150, yPos), Width = 300 };
            cbo1.DataSource = _dataManager.GetHSBAList();
            addForm.Controls.Add(lbl1);
            addForm.Controls.Add(cbo1);
            yPos += 40;

            // Loại DV
            Label lbl2 = new Label { Text = "Loại DV:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt2 = new TextBox { Location = new Point(150, yPos), Width = 300 };
            addForm.Controls.Add(lbl2);
            addForm.Controls.Add(txt2);
            yPos += 40;

            // Ngày
            Label lbl3 = new Label { Text = "Ngày DV:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt3 = new TextBox { Location = new Point(150, yPos), Width = 300 };
            addForm.Controls.Add(lbl3);
            addForm.Controls.Add(txt3);
            yPos += 40;

            // Mã KTV
            Label lbl4 = new Label { Text = "Mã KTV:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt4 = new TextBox { Location = new Point(150, yPos), Width = 300 };
            addForm.Controls.Add(lbl4);
            addForm.Controls.Add(txt4);
            yPos += 40;

            // Kết quả
            Label lbl5 = new Label { Text = "Kết quả:", Location = new Point(20, yPos), AutoSize = true };
            TextBox txt5 = new TextBox { Location = new Point(150, yPos), Width = 300 };
            addForm.Controls.Add(lbl5);
            addForm.Controls.Add(txt5);

            Button btnSave = new Button
            {
                Text = "Lưu",
                Location = new Point(150, 280),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(0, 153, 102),
                ForeColor = Color.White
            };
            btnSave.Click += (s, e) =>
            {
                try
                {
                    if (cbo1.SelectedItem == null) throw new Exception("Chưa chọn HSBA!");
                    string mahsba = cbo1.SelectedItem?.ToString() ?? "";
                    if (string.IsNullOrEmpty(mahsba)) throw new Exception("HSBA không hợp lệ!");
                    _dataManager.InsertHSBA_DV(mahsba, txt2.Text, txt3.Text, txt4.Text, txt5.Text);
                    MessageBox.Show("Thêm thành công!", "Thành công");
                    addForm.Close();
                    LoadAllData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
                }
            };
            addForm.Controls.Add(btnSave);

            addForm.ShowDialog();
        }

        private void DeleteHSBA_DV(DataGridView dgv)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn 1 dòng để xóa.", "Thông báo");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    string mahsba = dgv.CurrentRow.Cells[0].Value?.ToString() ?? "";
                    string loaidv = dgv.CurrentRow.Cells[1].Value?.ToString() ?? "";
                    string ngaydv = dgv.CurrentRow.Cells[2].Value?.ToString() ?? "";
                    if (string.IsNullOrEmpty(mahsba) || string.IsNullOrEmpty(loaidv) || string.IsNullOrEmpty(ngaydv))
                        throw new Exception("Dữ liệu không hợp lệ!");
                    _dataManager.DeleteHSBA_DV(mahsba, loaidv, ngaydv);
                    MessageBox.Show("Xóa thành công!", "Thành công");
                    LoadAllData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
                }
            }
        }

        private void LoadAllData()
        {
            // Reload lại tab hiện tại
            Panel panelContent = this.Controls["panelContent"] as Panel;
            Panel panelSidebar = this.Controls["panelSidebar"] as Panel;
            if (panelContent != null && panelSidebar != null)
            {
                // Tìm tab đang active
                foreach (Control c in panelSidebar.Controls)
                {
                    if (c is Button btn && btn.BackColor == Color.FromArgb(0, 153, 102))
                    {
                        SelectTab(btn.Text, btn, panelSidebar);
                        break;
                    }
                }
            }
        }

        private void ShowVPDInfo()
        {
            string info = $@"VPD - Virtual Private Database

Bác sĩ đăng nhập: {_loggedInUsername}
Tên: {_doctorName}

VPD Policies đang hoạt động:
1. HSBA: WHERE MABS = '{_loggedInUsername}'
2. BENHNHAN: WHERE MABN IN (SELECT MABN FROM HSBA WHERE MABS = '{_loggedInUsername}')
3. DONTHUOC: WHERE MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS = '{_loggedInUsername}')
4. HSBA_DV: WHERE MAHSBA IN (SELECT MAHSBA FROM HSBA WHERE MABS = '{_loggedInUsername}')

Lưu ý: Các truy vấn được thực thi mà không có WHERE clause,
VPD tự động áp dụng filter dựa trên SYS_CONTEXT('USERENV','SESSION_USER')";

            MessageBox.Show(info, "Thông tin VPD");
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
