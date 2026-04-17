namespace ADMIN
{
    partial class AddUserForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblAccountType = new System.Windows.Forms.Label();
            this.cbAccountType = new System.Windows.Forms.ComboBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.txtGender = new System.Windows.Forms.TextBox();
            this.lblDob = new System.Windows.Forms.Label();
            this.dtpDob = new System.Windows.Forms.DateTimePicker();
            this.lblCccd = new System.Windows.Forms.Label();
            this.txtCccd = new System.Windows.Forms.TextBox();
            
            // Employee specific
            this.lblHometown = new System.Windows.Forms.Label();
            this.txtHometown = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.txtRole = new System.Windows.Forms.TextBox();
            this.lblSpecialty = new System.Windows.Forms.Label();
            this.txtSpecialty = new System.Windows.Forms.TextBox();
            this.lblFacility = new System.Windows.Forms.Label();
            this.txtFacility = new System.Windows.Forms.TextBox();

            // Patient specific
            this.lblHouseNumber = new System.Windows.Forms.Label();
            this.txtHouseNumber = new System.Windows.Forms.TextBox();
            this.lblStreet = new System.Windows.Forms.Label();
            this.txtStreet = new System.Windows.Forms.TextBox();
            this.lblDistrict = new System.Windows.Forms.Label();
            this.txtDistrict = new System.Windows.Forms.TextBox();
            this.lblProvince = new System.Windows.Forms.Label();
            this.txtProvince = new System.Windows.Forms.TextBox();
            this.lblMedicalHistory = new System.Windows.Forms.Label();
            this.txtMedicalHistory = new System.Windows.Forms.TextBox();
            this.lblFamilyMedicalHistory = new System.Windows.Forms.Label();
            this.txtFamilyMedicalHistory = new System.Windows.Forms.TextBox();
            this.lblDrugAllergy = new System.Windows.Forms.Label();
            this.txtDrugAllergy = new System.Windows.Forms.TextBox();

            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            
            int yPos = 15;
            int lblX = 12;
            int txtX = 150;
            int w = 222;
            int h = 20;
            int spacing = 30;

            // cbAccountType
            this.lblAccountType.AutoSize = true;
            this.lblAccountType.Location = new System.Drawing.Point(lblX, yPos + 3);
            this.lblAccountType.Size = new System.Drawing.Size(77, 13);
            this.lblAccountType.Text = "Loại tài khoản:";
            
            this.cbAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAccountType.FormattingEnabled = true;
            this.cbAccountType.Location = new System.Drawing.Point(txtX, yPos);
            this.cbAccountType.Size = new System.Drawing.Size(w, 21);
            yPos += spacing;

            // txtPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(lblX, yPos + 3);
            this.lblPassword.Text = "Mật khẩu:";
            
            this.txtPassword.Location = new System.Drawing.Point(txtX, yPos);
            this.txtPassword.Size = new System.Drawing.Size(w, h);
            yPos += spacing;

            // txtFullName
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(lblX, yPos + 3);
            this.lblFullName.Text = "Họ tên:";
            
            this.txtFullName.Location = new System.Drawing.Point(txtX, yPos);
            this.txtFullName.Size = new System.Drawing.Size(w, h);
            yPos += spacing;

            // txtGender
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point(lblX, yPos + 3);
            this.lblGender.Text = "Giới tính (Phái):";
            
            this.txtGender.Location = new System.Drawing.Point(txtX, yPos);
            this.txtGender.Size = new System.Drawing.Size(w, h);
            yPos += spacing;

            // dtpDob
            this.lblDob.AutoSize = true;
            this.lblDob.Location = new System.Drawing.Point(lblX, yPos + 3);
            this.lblDob.Text = "Ngày sinh:";
            
            this.dtpDob.Location = new System.Drawing.Point(txtX, yPos);
            this.dtpDob.Size = new System.Drawing.Size(w, h);
            yPos += spacing;

            // txtCccd
            this.lblCccd.AutoSize = true;
            this.lblCccd.Location = new System.Drawing.Point(lblX, yPos + 3);
            this.lblCccd.Text = "CMND/CCCD:";
            
            this.txtCccd.Location = new System.Drawing.Point(txtX, yPos);
            this.txtCccd.Size = new System.Drawing.Size(w, h);
            yPos += spacing;

            int empY = yPos;
            int patY = yPos;

            // Employee fields
            this.lblHometown.AutoSize = true;
            this.lblHometown.Location = new System.Drawing.Point(lblX, empY + 3);
            this.lblHometown.Text = "Quê quán:";
            this.txtHometown.Location = new System.Drawing.Point(txtX, empY);
            this.txtHometown.Size = new System.Drawing.Size(w, h);
            empY += spacing;

            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(lblX, empY + 3);
            this.lblPhone.Text = "Số ĐT:";
            this.txtPhone.Location = new System.Drawing.Point(txtX, empY);
            this.txtPhone.Size = new System.Drawing.Size(w, h);
            empY += spacing;

            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(lblX, empY + 3);
            this.lblRole.Text = "Vai trò:";
            this.txtRole.Location = new System.Drawing.Point(txtX, empY);
            this.txtRole.Size = new System.Drawing.Size(w, h);
            empY += spacing;

            this.lblSpecialty.AutoSize = true;
            this.lblSpecialty.Location = new System.Drawing.Point(lblX, empY + 3);
            this.lblSpecialty.Text = "Chuyên khoa:";
            this.txtSpecialty.Location = new System.Drawing.Point(txtX, empY);
            this.txtSpecialty.Size = new System.Drawing.Size(w, h);
            empY += spacing;

            this.lblFacility.AutoSize = true;
            this.lblFacility.Location = new System.Drawing.Point(lblX, empY + 3);
            this.lblFacility.Text = "Cơ sở:";
            this.txtFacility.Location = new System.Drawing.Point(txtX, empY);
            this.txtFacility.Size = new System.Drawing.Size(w, h);
            empY += spacing;

            // Patient fields
            this.lblHouseNumber.AutoSize = true;
            this.lblHouseNumber.Location = new System.Drawing.Point(lblX, patY + 3);
            this.lblHouseNumber.Text = "Số nhà:";
            this.txtHouseNumber.Location = new System.Drawing.Point(txtX, patY);
            this.txtHouseNumber.Size = new System.Drawing.Size(w, h);
            patY += spacing;

            this.lblStreet.AutoSize = true;
            this.lblStreet.Location = new System.Drawing.Point(lblX, patY + 3);
            this.lblStreet.Text = "Tên đường:";
            this.txtStreet.Location = new System.Drawing.Point(txtX, patY);
            this.txtStreet.Size = new System.Drawing.Size(w, h);
            patY += spacing;

            this.lblDistrict.AutoSize = true;
            this.lblDistrict.Location = new System.Drawing.Point(lblX, patY + 3);
            this.lblDistrict.Text = "Quận/huyện:";
            this.txtDistrict.Location = new System.Drawing.Point(txtX, patY);
            this.txtDistrict.Size = new System.Drawing.Size(w, h);
            patY += spacing;

            this.lblProvince.AutoSize = true;
            this.lblProvince.Location = new System.Drawing.Point(lblX, patY + 3);
            this.lblProvince.Text = "Tỉnh/TP:";
            this.txtProvince.Location = new System.Drawing.Point(txtX, patY);
            this.txtProvince.Size = new System.Drawing.Size(w, h);
            patY += spacing;

            this.lblMedicalHistory.AutoSize = true;
            this.lblMedicalHistory.Location = new System.Drawing.Point(lblX, patY + 3);
            this.lblMedicalHistory.Text = "Tiền sử bệnh:";
            this.txtMedicalHistory.Location = new System.Drawing.Point(txtX, patY);
            this.txtMedicalHistory.Size = new System.Drawing.Size(w, h);
            patY += spacing;

            this.lblFamilyMedicalHistory.AutoSize = true;
            this.lblFamilyMedicalHistory.Location = new System.Drawing.Point(lblX, patY + 3);
            this.lblFamilyMedicalHistory.Text = "TS bệnh gia đình:";
            this.txtFamilyMedicalHistory.Location = new System.Drawing.Point(txtX, patY);
            this.txtFamilyMedicalHistory.Size = new System.Drawing.Size(w, h);
            patY += spacing;

            this.lblDrugAllergy.AutoSize = true;
            this.lblDrugAllergy.Location = new System.Drawing.Point(lblX, patY + 3);
            this.lblDrugAllergy.Text = "Dị ứng thuốc:";
            this.txtDrugAllergy.Location = new System.Drawing.Point(txtX, patY);
            this.txtDrugAllergy.Size = new System.Drawing.Size(w, h);
            patY += spacing;

            int finalY = Math.Max(empY, patY) + 10;

            this.btnSave.Location = new System.Drawing.Point(150, finalY);
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Text = "Lưu";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnCancel.Location = new System.Drawing.Point(272, finalY);
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            this.ClientSize = new System.Drawing.Size(420, finalY + 60);
            this.Controls.Add(this.lblAccountType);
            this.Controls.Add(this.cbAccountType);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblFullName);
            this.Controls.Add(this.txtFullName);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.txtGender);
            this.Controls.Add(this.lblDob);
            this.Controls.Add(this.dtpDob);
            this.Controls.Add(this.lblCccd);
            this.Controls.Add(this.txtCccd);
            
            this.Controls.Add(this.lblHometown);
            this.Controls.Add(this.txtHometown);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.txtRole);
            this.Controls.Add(this.lblSpecialty);
            this.Controls.Add(this.txtSpecialty);
            this.Controls.Add(this.lblFacility);
            this.Controls.Add(this.txtFacility);

            this.Controls.Add(this.lblHouseNumber);
            this.Controls.Add(this.txtHouseNumber);
            this.Controls.Add(this.lblStreet);
            this.Controls.Add(this.txtStreet);
            this.Controls.Add(this.lblDistrict);
            this.Controls.Add(this.txtDistrict);
            this.Controls.Add(this.lblProvince);
            this.Controls.Add(this.txtProvince);
            this.Controls.Add(this.lblMedicalHistory);
            this.Controls.Add(this.txtMedicalHistory);
            this.Controls.Add(this.lblFamilyMedicalHistory);
            this.Controls.Add(this.txtFamilyMedicalHistory);
            this.Controls.Add(this.lblDrugAllergy);
            this.Controls.Add(this.txtDrugAllergy);

            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            
            this.Name = "AddUserForm";
            this.Text = "Thêm Người Dùng";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // Common
        private System.Windows.Forms.Label lblAccountType;
        private System.Windows.Forms.ComboBox cbAccountType;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.TextBox txtGender;
        private System.Windows.Forms.Label lblDob;
        private System.Windows.Forms.DateTimePicker dtpDob;
        private System.Windows.Forms.Label lblCccd;
        private System.Windows.Forms.TextBox txtCccd;

        // Employee
        private System.Windows.Forms.Label lblHometown;
        private System.Windows.Forms.TextBox txtHometown;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.TextBox txtRole;
        private System.Windows.Forms.Label lblSpecialty;
        private System.Windows.Forms.TextBox txtSpecialty;
        private System.Windows.Forms.Label lblFacility;
        private System.Windows.Forms.TextBox txtFacility;

        // Patient
        private System.Windows.Forms.Label lblHouseNumber;
        private System.Windows.Forms.TextBox txtHouseNumber;
        private System.Windows.Forms.Label lblStreet;
        private System.Windows.Forms.TextBox txtStreet;
        private System.Windows.Forms.Label lblDistrict;
        private System.Windows.Forms.TextBox txtDistrict;
        private System.Windows.Forms.Label lblProvince;
        private System.Windows.Forms.TextBox txtProvince;
        private System.Windows.Forms.Label lblMedicalHistory;
        private System.Windows.Forms.TextBox txtMedicalHistory;
        private System.Windows.Forms.Label lblFamilyMedicalHistory;
        private System.Windows.Forms.TextBox txtFamilyMedicalHistory;
        private System.Windows.Forms.Label lblDrugAllergy;
        private System.Windows.Forms.TextBox txtDrugAllergy;

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
