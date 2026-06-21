using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ADMIN
{
    public class FormChiTietThongBao : Form
    {
        private OracleConnection _conn;
        private DataGridView dgvAnnouncements;
        private Button btnLayDuLieu;
        private Button btnDong;

        public FormChiTietThongBao(OracleConnection conn)
        {
            _conn = conn;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "BẢNG \"THÔNG BÁO\"";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Title Label
            Label lblTitle = new Label
            {
                Text = "DANH SÁCH THÔNG BÁO CỦA BẠN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185), // Đẹp hơn
                AutoSize = false,
                Size = new Size(1000, 40),
                Location = new Point(0, 15),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(lblTitle);

            // Button ĐÓNG
            btnDong = new Button
            {
                Text = "ĐÓNG",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(120, 45), 
                Location = new Point((1000 - 120) / 2, 500), // Căn giữa
                Anchor = AnchorStyles.Bottom,
                BackColor = Color.FromArgb(231, 76, 60), 
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.Click += (s, e) => this.Close();
            this.Controls.Add(btnDong);

            // DataGridView
            dgvAnnouncements = new DataGridView
            {
                Location = new Point(15, 65),
                Size = new Size(955, 415),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White, 
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToResizeRows = false,
                GridColor = Color.LightGray,
                RowTemplate = { Height = 40 }
            };
            dgvAnnouncements.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvAnnouncements.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvAnnouncements.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAnnouncements.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvAnnouncements.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Căn giữa Header
            dgvAnnouncements.ColumnHeadersHeight = 45;
            dgvAnnouncements.EnableHeadersVisualStyles = false;
            
            dgvAnnouncements.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvAnnouncements.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Căn giữa Nội dung

            this.Controls.Add(dgvAnnouncements);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Chỉ lấy các trường cần thiết, bỏ qua OLS_LABEL để tối ưu.
                string query = "SELECT MATB, NOIDUNG, NGAYGIO AS THOIGIAN, DIADIEM FROM ADMIN_PHANHE1.THONGBAO ORDER BY MATB";
                DataTable dt = new DataTable();
                using (OracleCommand cmd = new OracleCommand(query, _conn))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                dgvAnnouncements.DataSource = dt;
                
                // Cấu hình hiển thị các cột
                if (dgvAnnouncements.Columns["MATB"] != null) 
                {
                    dgvAnnouncements.Columns["MATB"].Visible = false; // Ẩn cột MATB
                }
                
                if (dgvAnnouncements.Columns["NOIDUNG"] != null) 
                {
                    dgvAnnouncements.Columns["NOIDUNG"].HeaderText = "NỘI DUNG";
                    dgvAnnouncements.Columns["NOIDUNG"].FillWeight = 50; 
                }
                
                if (dgvAnnouncements.Columns["THOIGIAN"] != null) 
                {
                    dgvAnnouncements.Columns["THOIGIAN"].HeaderText = "THỜI GIAN";
                    dgvAnnouncements.Columns["THOIGIAN"].FillWeight = 25;
                }
                
                if (dgvAnnouncements.Columns["DIADIEM"] != null) 
                {
                    dgvAnnouncements.Columns["DIADIEM"].HeaderText = "ĐỊA ĐIỂM";
                    dgvAnnouncements.Columns["DIADIEM"].FillWeight = 25;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi truy xuất dữ liệu: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
