import sys

designer_path = r'c:\Users\Acer\Desktop\ATHTTT_QLBV\ADMIN\AddUserForm.Designer.cs'
with open(designer_path, 'r', encoding='utf-8') as f:
    content = f.read()

new_instances = """            this.lblHometown = new System.Windows.Forms.Label();
            this.txtHometown = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.txtRole = new System.Windows.Forms.TextBox();
            this.lblSpecialty = new System.Windows.Forms.Label();
            this.txtSpecialty = new System.Windows.Forms.TextBox();
            this.lblFacility = new System.Windows.Forms.Label();
            this.txtFacility = new System.Windows.Forms.TextBox();
"""
content = content.replace('            this.btnSave = new System.Windows.Forms.Button();', new_instances + '            this.btnSave = new System.Windows.Forms.Button();')

new_props = """            // 
            // lblHometown
            // 
            this.lblHometown.AutoSize = true;
            this.lblHometown.Location = new System.Drawing.Point(12, 195);
            this.lblHometown.Name = "lblHometown";
            this.lblHometown.Size = new System.Drawing.Size(59, 13);
            this.lblHometown.TabIndex = 28;
            this.lblHometown.Text = "Quê quán:";
            // 
            // txtHometown
            // 
            this.txtHometown.Location = new System.Drawing.Point(150, 192);
            this.txtHometown.Name = "txtHometown";
            this.txtHometown.Size = new System.Drawing.Size(222, 20);
            this.txtHometown.TabIndex = 29;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(12, 225);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(41, 13);
            this.lblPhone.TabIndex = 30;
            this.lblPhone.Text = "S? ÐT:";
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(150, 222);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(222, 20);
            this.txtPhone.TabIndex = 31;
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(12, 255);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(40, 13);
            this.lblRole.TabIndex = 32;
            this.lblRole.Text = "Vai trò:";
            // 
            // txtRole
            // 
            this.txtRole.Location = new System.Drawing.Point(150, 252);
            this.txtRole.Name = "txtRole";
            this.txtRole.Size = new System.Drawing.Size(222, 20);
            this.txtRole.TabIndex = 33;
            // 
            // lblSpecialty
            // 
            this.lblSpecialty.AutoSize = true;
            this.lblSpecialty.Location = new System.Drawing.Point(12, 285);
            this.lblSpecialty.Name = "lblSpecialty";
            this.lblSpecialty.Size = new System.Drawing.Size(73, 13);
            this.lblSpecialty.TabIndex = 34;
            this.lblSpecialty.Text = "Chuyên khoa:";
            // 
            // txtSpecialty
            // 
            this.txtSpecialty.Location = new System.Drawing.Point(150, 282);
            this.txtSpecialty.Name = "txtSpecialty";
            this.txtSpecialty.Size = new System.Drawing.Size(222, 20);
            this.txtSpecialty.TabIndex = 35;
            // 
            // lblFacility
            // 
            this.lblFacility.AutoSize = true;
            this.lblFacility.Location = new System.Drawing.Point(12, 315);
            this.lblFacility.Name = "lblFacility";
            this.lblFacility.Size = new System.Drawing.Size(37, 13);
            this.lblFacility.TabIndex = 36;
            this.lblFacility.Text = "Co s?:";
            // 
            // txtFacility
            // 
            this.txtFacility.Location = new System.Drawing.Point(150, 312);
            this.txtFacility.Name = "txtFacility";
            this.txtFacility.Size = new System.Drawing.Size(222, 20);
            this.txtFacility.TabIndex = 37;
"""
content = content.replace('            // \n            // btnSave', new_props + '            // \n            // btnSave')

new_controls = """            this.Controls.Add(this.txtFacility);
            this.Controls.Add(this.lblFacility);
            this.Controls.Add(this.txtSpecialty);
            this.Controls.Add(this.lblSpecialty);
            this.Controls.Add(this.txtRole);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtHometown);
            this.Controls.Add(this.lblHometown);
"""
content = content.replace('            this.Name = "AddUserForm";', new_controls + '            this.Name = "AddUserForm";')

new_vars = """        private System.Windows.Forms.Label lblHometown;
        private System.Windows.Forms.TextBox txtHometown;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.TextBox txtRole;
        private System.Windows.Forms.Label lblSpecialty;
        private System.Windows.Forms.TextBox txtSpecialty;
        private System.Windows.Forms.Label lblFacility;
        private System.Windows.Forms.TextBox txtFacility;
"""
content = content.replace('        private System.Windows.Forms.Button btnCancel;\n    }\n}', '        private System.Windows.Forms.Button btnCancel;\n' + new_vars + '    }\n}')

with open(designer_path, 'w', encoding='utf-8') as f:
    f.write(content)
# -*- coding: utf-8 -*-
