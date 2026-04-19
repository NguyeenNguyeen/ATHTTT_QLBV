# -*- coding: utf-8 -*-
import sys

designer_path = r'c:\Users\Acer\Desktop\ATHTTT_QLBV\ADMIN\AddUserForm.Designer.cs'
with open(designer_path, 'r', encoding='utf-8') as f:
    content = f.read()

new_instances = """            this.lblCCCD = new System.Windows.Forms.Label();
            this.txtCCCD = new System.Windows.Forms.TextBox();
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
"""
content = content.replace('            this.btnSave = new System.Windows.Forms.Button();', new_instances + '            this.btnSave = new System.Windows.Forms.Button();')

new_props = """            // 
            // lblCCCD
            // 
            this.lblCCCD.AutoSize = true;
            this.lblCCCD.Location = new System.Drawing.Point(400, 75);
            this.lblCCCD.Name = "lblCCCD";
            this.lblCCCD.Size = new System.Drawing.Size(39, 13);
            this.lblCCCD.TabIndex = 40;
            this.lblCCCD.Text = "CCCD:";
            // 
            // txtCCCD
            // 
            this.txtCCCD.Location = new System.Drawing.Point(500, 72);
            this.txtCCCD.Name = "txtCCCD";
            this.txtCCCD.Size = new System.Drawing.Size(222, 20);
            this.txtCCCD.TabIndex = 41;
            // 
            // lblHouseNumber
            // 
            this.lblHouseNumber.AutoSize = true;
            this.lblHouseNumber.Location = new System.Drawing.Point(400, 105);
            this.lblHouseNumber.Name = "lblHouseNumber";
            this.lblHouseNumber.Size = new System.Drawing.Size(44, 13);
            this.lblHouseNumber.TabIndex = 42;
            this.lblHouseNumber.Text = "S? nhà:";
            // 
            // txtHouseNumber
            // 
            this.txtHouseNumber.Location = new System.Drawing.Point(500, 102);
            this.txtHouseNumber.Name = "txtHouseNumber";
            this.txtHouseNumber.Size = new System.Drawing.Size(222, 20);
            this.txtHouseNumber.TabIndex = 43;
            // 
            // lblStreet
            // 
            this.lblStreet.AutoSize = true;
            this.lblStreet.Location = new System.Drawing.Point(400, 135);
            this.lblStreet.Name = "lblStreet";
            this.lblStreet.Size = new System.Drawing.Size(63, 13);
            this.lblStreet.TabIndex = 44;
            this.lblStreet.Text = "Tên du?ng:";
            // 
            // txtStreet
            // 
            this.txtStreet.Location = new System.Drawing.Point(500, 132);
            this.txtStreet.Name = "txtStreet";
            this.txtStreet.Size = new System.Drawing.Size(222, 20);
            this.txtStreet.TabIndex = 45;
            // 
            // lblDistrict
            // 
            this.lblDistrict.AutoSize = true;
            this.lblDistrict.Location = new System.Drawing.Point(400, 165);
            this.lblDistrict.Name = "lblDistrict";
            this.lblDistrict.Size = new System.Drawing.Size(72, 13);
            this.lblDistrict.TabIndex = 46;
            this.lblDistrict.Text = "Qu?n/huy?n:";
            // 
            // txtDistrict
            // 
            this.txtDistrict.Location = new System.Drawing.Point(500, 162);
            this.txtDistrict.Name = "txtDistrict";
            this.txtDistrict.Size = new System.Drawing.Size(222, 20);
            this.txtDistrict.TabIndex = 47;
            // 
            // lblProvince
            // 
            this.lblProvince.AutoSize = true;
            this.lblProvince.Location = new System.Drawing.Point(400, 195);
            this.lblProvince.Name = "lblProvince";
            this.lblProvince.Size = new System.Drawing.Size(46, 13);
            this.lblProvince.TabIndex = 48;
            this.lblProvince.Text = "T?nhThành:";
            // 
            // txtProvince
            // 
            this.txtProvince.Location = new System.Drawing.Point(500, 192);
            this.txtProvince.Name = "txtProvince";
            this.txtProvince.Size = new System.Drawing.Size(222, 20);
            this.txtProvince.TabIndex = 49;
            // 
            // lblMedicalHistory
            // 
            this.lblMedicalHistory.AutoSize = true;
            this.lblMedicalHistory.Location = new System.Drawing.Point(400, 225);
            this.lblMedicalHistory.Name = "lblMedicalHistory";
            this.lblMedicalHistory.Size = new System.Drawing.Size(73, 13);
            this.lblMedicalHistory.TabIndex = 50;
            this.lblMedicalHistory.Text = "TS b?nh:";
            // 
            // txtMedicalHistory
            // 
            this.txtMedicalHistory.Location = new System.Drawing.Point(500, 222);
            this.txtMedicalHistory.Name = "txtMedicalHistory";
            this.txtMedicalHistory.Size = new System.Drawing.Size(222, 20);
            this.txtMedicalHistory.TabIndex = 51;
            // 
            // lblFamilyMedicalHistory
            // 
            this.lblFamilyMedicalHistory.AutoSize = true;
            this.lblFamilyMedicalHistory.Location = new System.Drawing.Point(400, 255);
            this.lblFamilyMedicalHistory.Name = "lblFamilyMedicalHistory";
            this.lblFamilyMedicalHistory.Size = new System.Drawing.Size(84, 13);
            this.lblFamilyMedicalHistory.TabIndex = 52;
            this.lblFamilyMedicalHistory.Text = "TS b?nh GD:";
            // 
            // txtFamilyMedicalHistory
            // 
            this.txtFamilyMedicalHistory.Location = new System.Drawing.Point(500, 252);
            this.txtFamilyMedicalHistory.Name = "txtFamilyMedicalHistory";
            this.txtFamilyMedicalHistory.Size = new System.Drawing.Size(222, 20);
            this.txtFamilyMedicalHistory.TabIndex = 53;
            // 
            // lblDrugAllergy
            // 
            this.lblDrugAllergy.AutoSize = true;
            this.lblDrugAllergy.Location = new System.Drawing.Point(400, 285);
            this.lblDrugAllergy.Name = "lblDrugAllergy";
            this.lblDrugAllergy.Size = new System.Drawing.Size(75, 13);
            this.lblDrugAllergy.TabIndex = 54;
            this.lblDrugAllergy.Text = "D? ?ng thu?c:";
            // 
            // txtDrugAllergy
            // 
            this.txtDrugAllergy.Location = new System.Drawing.Point(500, 282);
            this.txtDrugAllergy.Name = "txtDrugAllergy";
            this.txtDrugAllergy.Size = new System.Drawing.Size(222, 20);
            this.txtDrugAllergy.TabIndex = 55;
"""
content = content.replace('            // \n            // btnSave', new_props + '            // \n            // btnSave')

new_controls = """            this.Controls.Add(this.txtDrugAllergy);
            this.Controls.Add(this.lblDrugAllergy);
            this.Controls.Add(this.txtFamilyMedicalHistory);
            this.Controls.Add(this.lblFamilyMedicalHistory);
            this.Controls.Add(this.txtMedicalHistory);
            this.Controls.Add(this.lblMedicalHistory);
            this.Controls.Add(this.txtProvince);
            this.Controls.Add(this.lblProvince);
            this.Controls.Add(this.txtDistrict);
            this.Controls.Add(this.lblDistrict);
            this.Controls.Add(this.txtStreet);
            this.Controls.Add(this.lblStreet);
            this.Controls.Add(this.txtHouseNumber);
            this.Controls.Add(this.lblHouseNumber);
            this.Controls.Add(this.txtCCCD);
            this.Controls.Add(this.lblCCCD);
"""
content = content.replace('            this.Name = "AddUserForm";', new_controls + '            this.Name = "AddUserForm";')

new_vars = """        private System.Windows.Forms.Label lblCCCD;
        private System.Windows.Forms.TextBox txtCCCD;
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
"""
content = content.replace('        private System.Windows.Forms.Button btnCancel;\n    }\n}', '        private System.Windows.Forms.Button btnCancel;\n' + new_vars + '    }\n}')

with open(designer_path, 'w', encoding='utf-8') as f:
    f.write(content)
