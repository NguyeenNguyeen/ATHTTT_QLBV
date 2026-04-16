namespace ADMIN
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private TabControl tabMain;
        private TabPage tabUsers;
        private TabPage tabRoles;
        private TabPage tabGrant;
        private TabPage tabRevoke;
        private TabPage tabPrivInfo;

        private DataGridView dgvUsers;
        private TextBox txtUserName;
        private Button btnAddUser;
        private Button btnEditUser;
        private Button btnDeleteUser;

        private DataGridView dgvRoles;
        private TextBox txtRoleName;
        private Button btnAddRole;
        private Button btnEditRole;
        private Button btnDeleteRole;

        private Label lblGrantGrantee;
        private Label lblGrantObjectType;
        private Label lblGrantObjectName;
        private ComboBox cbGrantGrantee;
        private ComboBox cbGrantObjectType;
        private ComboBox cbGrantObjectName;
        private CheckedListBox clbGrantPrivileges;
        private CheckedListBox clbGrantColumns;
        private CheckBox chkGrantWithOption;
        private Button btnGrantExecute;

        private Label lblRevokeGrantee;
        private Label lblRevokeObjectType;
        private Label lblRevokeObjectName;
        private ComboBox cbRevokeGrantee;
        private ComboBox cbRevokeObjectType;
        private ComboBox cbRevokeObjectName;
        private CheckedListBox clbRevokePrivileges;
        private CheckedListBox clbRevokeColumns;
        private Button btnRevokeExecute;

        private Label lblPrivInfoGrantee;
        private ComboBox cbPrivInfoGrantee;
        private Button btnLoadPrivInfo;
        private DataGridView dgvPrivInfo;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabMain = new TabControl();
            tabUsers = new TabPage();
            dgvUsers = new DataGridView();
            txtUserName = new TextBox();
            btnAddUser = new Button();
            btnEditUser = new Button();
            btnDeleteUser = new Button();
            tabRoles = new TabPage();
            dgvRoles = new DataGridView();
            txtRoleName = new TextBox();
            btnAddRole = new Button();
            btnEditRole = new Button();
            btnDeleteRole = new Button();
            tabGrant = new TabPage();
            cbGrantGrantee = new ComboBox();
            cbGrantObjectType = new ComboBox();
            cbGrantObjectName = new ComboBox();
            clbGrantPrivileges = new CheckedListBox();
            clbGrantColumns = new CheckedListBox();
            chkGrantWithOption = new CheckBox();
            btnGrantExecute = new Button();
            tabRevoke = new TabPage();
            cbRevokeGrantee = new ComboBox();
            cbRevokeObjectType = new ComboBox();
            cbRevokeObjectName = new ComboBox();
            clbRevokePrivileges = new CheckedListBox();
            clbRevokeColumns = new CheckedListBox();
            btnRevokeExecute = new Button();
            tabPrivInfo = new TabPage();
            cbPrivInfoGrantee = new ComboBox();
            btnLoadPrivInfo = new Button();
            dgvPrivInfo = new DataGridView();
            btnSearch = new Button();
            tabMain.SuspendLayout();
            tabUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
            tabRoles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRoles).BeginInit();
            tabGrant.SuspendLayout();
            tabRevoke.SuspendLayout();
            tabPrivInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPrivInfo).BeginInit();
            SuspendLayout();
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabUsers);
            tabMain.Controls.Add(tabRoles);
            tabMain.Controls.Add(tabGrant);
            tabMain.Controls.Add(tabRevoke);
            tabMain.Controls.Add(tabPrivInfo);
            tabMain.Dock = DockStyle.Fill;
            tabMain.Location = new Point(0, 0);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1100, 700);
            tabMain.TabIndex = 0;
            // 
            // tabUsers
            // 
            tabUsers.Controls.Add(dgvUsers);
            tabUsers.Controls.Add(txtUserName);
            tabUsers.Controls.Add(btnSearch);
            tabUsers.Controls.Add(btnAddUser);
            tabUsers.Controls.Add(btnEditUser);
            tabUsers.Controls.Add(btnDeleteUser);
            tabUsers.Location = new Point(4, 24);
            tabUsers.Name = "tabUsers";
            tabUsers.Padding = new Padding(10);
            tabUsers.Size = new Size(1092, 672);
            tabUsers.TabIndex = 0;
            tabUsers.Text = "User";
            tabUsers.UseVisualStyleBackColor = true;
            // 
            // dgvUsers
            // 
            dgvUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.Location = new Point(10, 120);
            dgvUsers.Name = "dgvUsers";
            dgvUsers.ReadOnly = true;
            dgvUsers.Size = new Size(1072, 542);
            dgvUsers.TabIndex = 0;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(13, 21);
            txtUserName.Name = "txtUserName";
            txtUserName.PlaceholderText = "Mã user";
            txtUserName.Size = new Size(200, 23);
            txtUserName.TabIndex = 1;
            // 
            // btnAddUser
            // 
            btnAddUser.BackColor = Color.LightGreen;
            btnAddUser.Location = new Point(923, 17);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(90, 27);
            btnAddUser.TabIndex = 3;
            btnAddUser.Text = "Thêm";
            btnAddUser.UseVisualStyleBackColor = false;
            // 
            // btnEditUser
            // 
            btnEditUser.BackColor = Color.LightGoldenrodYellow;
            btnEditUser.Location = new Point(450, 18);
            btnEditUser.Name = "btnEditUser";
            btnEditUser.Size = new Size(90, 27);
            btnEditUser.TabIndex = 4;
            btnEditUser.Text = "Sửa";
            btnEditUser.UseVisualStyleBackColor = false;
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.BackColor = Color.LightCoral;
            btnDeleteUser.Location = new Point(578, 18);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new Size(90, 27);
            btnDeleteUser.TabIndex = 5;
            btnDeleteUser.Text = "Xóa";
            btnDeleteUser.UseVisualStyleBackColor = false;
            // 
            // tabRoles
            // 
            tabRoles.Controls.Add(dgvRoles);
            tabRoles.Controls.Add(txtRoleName);
            tabRoles.Controls.Add(btnAddRole);
            tabRoles.Controls.Add(btnEditRole);
            tabRoles.Controls.Add(btnDeleteRole);
            tabRoles.Location = new Point(4, 24);
            tabRoles.Name = "tabRoles";
            tabRoles.Padding = new Padding(10);
            tabRoles.Size = new Size(1092, 672);
            tabRoles.TabIndex = 1;
            tabRoles.Text = "Role";
            tabRoles.UseVisualStyleBackColor = true;
            // 
            // dgvRoles
            // 
            dgvRoles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvRoles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRoles.Location = new Point(10, 120);
            dgvRoles.Name = "dgvRoles";
            dgvRoles.ReadOnly = true;
            dgvRoles.Size = new Size(1072, 542);
            dgvRoles.TabIndex = 0;
            // 
            // txtRoleName
            // 
            txtRoleName.Location = new Point(10, 20);
            txtRoleName.Name = "txtRoleName";
            txtRoleName.PlaceholderText = "Tên role";
            txtRoleName.Size = new Size(200, 23);
            txtRoleName.TabIndex = 1;
            // 
            // btnAddRole
            // 
            btnAddRole.BackColor = Color.LightGreen;
            btnAddRole.Location = new Point(220, 18);
            btnAddRole.Name = "btnAddRole";
            btnAddRole.Size = new Size(90, 27);
            btnAddRole.TabIndex = 2;
            btnAddRole.Text = "Thêm";
            btnAddRole.UseVisualStyleBackColor = false;
            // 
            // btnEditRole
            // 
            btnEditRole.BackColor = Color.LightGoldenrodYellow;
            btnEditRole.Location = new Point(320, 18);
            btnEditRole.Name = "btnEditRole";
            btnEditRole.Size = new Size(90, 27);
            btnEditRole.TabIndex = 3;
            btnEditRole.Text = "Sửa";
            btnEditRole.UseVisualStyleBackColor = false;
            // 
            // btnDeleteRole
            // 
            btnDeleteRole.BackColor = Color.LightCoral;
            btnDeleteRole.Location = new Point(420, 18);
            btnDeleteRole.Name = "btnDeleteRole";
            btnDeleteRole.Size = new Size(90, 27);
            btnDeleteRole.TabIndex = 4;
            btnDeleteRole.Text = "Xóa";
            btnDeleteRole.UseVisualStyleBackColor = false;
            // 
            // tabGrant
            // 
            tabGrant.Controls.Add(lblGrantGrantee);
            tabGrant.Controls.Add(lblGrantObjectType);
            tabGrant.Controls.Add(lblGrantObjectName);
            tabGrant.Controls.Add(cbGrantGrantee);
            tabGrant.Controls.Add(cbGrantObjectType);
            tabGrant.Controls.Add(cbGrantObjectName);
            tabGrant.Controls.Add(clbGrantPrivileges);
            tabGrant.Controls.Add(clbGrantColumns);
            tabGrant.Controls.Add(chkGrantWithOption);
            tabGrant.Controls.Add(btnGrantExecute);
            tabGrant.Location = new Point(4, 24);
            tabGrant.Name = "tabGrant";
            tabGrant.Padding = new Padding(10);
            tabGrant.Size = new Size(1092, 672);
            tabGrant.TabIndex = 2;
            tabGrant.Text = "Grant";
            tabGrant.UseVisualStyleBackColor = true;
            // 
            // lblGrantGrantee
            // 
            lblGrantGrantee.AutoSize = true;
            lblGrantGrantee.Location = new Point(10, 5);
            lblGrantGrantee.Name = "lblGrantGrantee";
            lblGrantGrantee.Size = new Size(68, 15);
            lblGrantGrantee.TabIndex = 0;
            lblGrantGrantee.Text = "Chọn Grantee:";
            // 
            // lblGrantObjectType
            // 
            lblGrantObjectType.AutoSize = true;
            lblGrantObjectType.Location = new Point(250, 5);
            lblGrantObjectType.Name = "lblGrantObjectType";
            lblGrantObjectType.Size = new Size(83, 15);
            lblGrantObjectType.TabIndex = 1;
            lblGrantObjectType.Text = "Loại Đối tượng:";
            // 
            // lblGrantObjectName
            // 
            lblGrantObjectName.AutoSize = true;
            lblGrantObjectName.Location = new Point(450, 5);
            lblGrantObjectName.Name = "lblGrantObjectName";
            lblGrantObjectName.Size = new Size(69, 15);
            lblGrantObjectName.TabIndex = 2;
            lblGrantObjectName.Text = "Tên đối tượng:";
            // 
            // 
            // cbGrantGrantee
            // 
            cbGrantGrantee.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGrantGrantee.Location = new Point(10, 20);
            cbGrantGrantee.Name = "cbGrantGrantee";
            cbGrantGrantee.Size = new Size(220, 23);
            cbGrantGrantee.TabIndex = 0;
            // 
            // cbGrantObjectType
            // 
            cbGrantObjectType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGrantObjectType.Items.AddRange(new object[] { "TABLE", "VIEW", "PROCEDURE", "FUNCTION" });
            cbGrantObjectType.Location = new Point(250, 20);
            cbGrantObjectType.Name = "cbGrantObjectType";
            cbGrantObjectType.Size = new Size(180, 23);
            cbGrantObjectType.TabIndex = 1;
            // 
            // cbGrantObjectName
            // 
            cbGrantObjectName.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGrantObjectName.Location = new Point(450, 20);
            cbGrantObjectName.Name = "cbGrantObjectName";
            cbGrantObjectName.Size = new Size(220, 23);
            cbGrantObjectName.TabIndex = 2;
            // 
            // clbGrantPrivileges
            // 
            clbGrantPrivileges.CheckOnClick = true;
            clbGrantPrivileges.Items.AddRange(new object[] { "SELECT", "INSERT", "UPDATE", "DELETE", "EXECUTE" });
            clbGrantPrivileges.Location = new Point(10, 70);
            clbGrantPrivileges.Name = "clbGrantPrivileges";
            clbGrantPrivileges.Size = new Size(180, 130);
            clbGrantPrivileges.TabIndex = 3;
            // 
            // clbGrantColumns
            // 
            clbGrantColumns.CheckOnClick = true;
            clbGrantColumns.Location = new Point(210, 70);
            clbGrantColumns.Name = "clbGrantColumns";
            clbGrantColumns.Size = new Size(220, 130);
            clbGrantColumns.TabIndex = 4;
            clbGrantColumns.Visible = false;
            // 
            // chkGrantWithOption
            // 
            chkGrantWithOption.Location = new Point(450, 70);
            chkGrantWithOption.Name = "chkGrantWithOption";
            chkGrantWithOption.Size = new Size(180, 24);
            chkGrantWithOption.TabIndex = 5;
            chkGrantWithOption.Text = "WITH GRANT OPTION";
            chkGrantWithOption.UseVisualStyleBackColor = true;
            // 
            // btnGrantExecute
            // 
            btnGrantExecute.BackColor = Color.LightGreen;
            btnGrantExecute.Location = new Point(450, 110);
            btnGrantExecute.Name = "btnGrantExecute";
            btnGrantExecute.Size = new Size(180, 30);
            btnGrantExecute.TabIndex = 6;
            btnGrantExecute.Text = "Thực thi GRANT";
            btnGrantExecute.UseVisualStyleBackColor = false;
            // 
            // tabRevoke
            // 
            tabRevoke.Controls.Add(lblRevokeGrantee);
            tabRevoke.Controls.Add(lblRevokeObjectType);
            tabRevoke.Controls.Add(lblRevokeObjectName);
            tabRevoke.Controls.Add(cbRevokeGrantee);
            tabRevoke.Controls.Add(cbRevokeObjectType);
            tabRevoke.Controls.Add(cbRevokeObjectName);
            tabRevoke.Controls.Add(clbRevokePrivileges);
            tabRevoke.Controls.Add(clbRevokeColumns);
            tabRevoke.Controls.Add(btnRevokeExecute);
            tabRevoke.Location = new Point(4, 24);
            tabRevoke.Name = "tabRevoke";
            tabRevoke.Padding = new Padding(10);
            tabRevoke.Size = new Size(1092, 672);
            tabRevoke.TabIndex = 3;
            tabRevoke.Text = "Revoke";
            tabRevoke.UseVisualStyleBackColor = true;
            // 
            // lblRevokeGrantee
            // 
            lblRevokeGrantee.AutoSize = true;
            lblRevokeGrantee.Location = new Point(10, 5);
            lblRevokeGrantee.Name = "lblRevokeGrantee";
            lblRevokeGrantee.Size = new Size(68, 15);
            lblRevokeGrantee.TabIndex = 0;
            lblRevokeGrantee.Text = "Chọn Grantee:";
            // 
            // lblRevokeObjectType
            // 
            lblRevokeObjectType.AutoSize = true;
            lblRevokeObjectType.Location = new Point(250, 5);
            lblRevokeObjectType.Name = "lblRevokeObjectType";
            lblRevokeObjectType.Size = new Size(83, 15);
            lblRevokeObjectType.TabIndex = 1;
            lblRevokeObjectType.Text = "Loại Đối tượng:";
            // 
            // lblRevokeObjectName
            // 
            lblRevokeObjectName.AutoSize = true;
            lblRevokeObjectName.Location = new Point(450, 5);
            lblRevokeObjectName.Name = "lblRevokeObjectName";
            lblRevokeObjectName.Size = new Size(69, 15);
            lblRevokeObjectName.Text = "Tên đối tượng:";
            // 
            // 
            // cbRevokeGrantee
            // 
            cbRevokeGrantee.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRevokeGrantee.Location = new Point(10, 20);
            cbRevokeGrantee.Name = "cbRevokeGrantee";
            cbRevokeGrantee.Size = new Size(220, 23);
            cbRevokeGrantee.TabIndex = 0;
            // 
            // cbRevokeObjectType
            // 
            cbRevokeObjectType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRevokeObjectType.Items.AddRange(new object[] { "TABLE", "VIEW", "PROCEDURE", "FUNCTION" });
            cbRevokeObjectType.Location = new Point(250, 20);
            cbRevokeObjectType.Name = "cbRevokeObjectType";
            cbRevokeObjectType.Size = new Size(180, 23);
            cbRevokeObjectType.TabIndex = 1;
            // 
            // cbRevokeObjectName
            // 
            cbRevokeObjectName.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRevokeObjectName.Location = new Point(450, 20);
            cbRevokeObjectName.Name = "cbRevokeObjectName";
            cbRevokeObjectName.Size = new Size(220, 23);
            cbRevokeObjectName.TabIndex = 2;
            // 
            // clbRevokePrivileges
            // 
            clbRevokePrivileges.CheckOnClick = true;
            clbRevokePrivileges.Items.AddRange(new object[] { "SELECT", "INSERT", "UPDATE", "DELETE", "EXECUTE" });
            clbRevokePrivileges.Location = new Point(10, 70);
            clbRevokePrivileges.Name = "clbRevokePrivileges";
            clbRevokePrivileges.Size = new Size(180, 130);
            clbRevokePrivileges.TabIndex = 3;
            // 
            // clbRevokeColumns
            // 
            clbRevokeColumns.CheckOnClick = true;
            clbRevokeColumns.Location = new Point(210, 70);
            clbRevokeColumns.Name = "clbRevokeColumns";
            clbRevokeColumns.Size = new Size(220, 130);
            clbRevokeColumns.TabIndex = 4;
            clbRevokeColumns.Visible = false;
            // 
            // btnRevokeExecute
            // 
            btnRevokeExecute.BackColor = Color.LightCoral;
            btnRevokeExecute.Location = new Point(450, 110);
            btnRevokeExecute.Name = "btnRevokeExecute";
            btnRevokeExecute.Size = new Size(180, 30);
            btnRevokeExecute.TabIndex = 5;
            btnRevokeExecute.Text = "Thực thi REVOKE";
            btnRevokeExecute.UseVisualStyleBackColor = false;
            // 
            // tabPrivInfo
            // 
            tabPrivInfo.Controls.Add(lblPrivInfoGrantee);
            tabPrivInfo.Controls.Add(cbPrivInfoGrantee);
            tabPrivInfo.Controls.Add(btnLoadPrivInfo);
            tabPrivInfo.Controls.Add(dgvPrivInfo);
            tabPrivInfo.Location = new Point(4, 24);
            tabPrivInfo.Name = "tabPrivInfo";
            tabPrivInfo.Padding = new Padding(10);
            tabPrivInfo.Size = new Size(1092, 672);
            tabPrivInfo.TabIndex = 4;
            tabPrivInfo.Text = "Thông tin quyền";
            tabPrivInfo.UseVisualStyleBackColor = true;
            // 
            // lblPrivInfoGrantee
            // 
            lblPrivInfoGrantee.AutoSize = true;
            lblPrivInfoGrantee.Location = new Point(10, 5);
            lblPrivInfoGrantee.Name = "lblPrivInfoGrantee";
            lblPrivInfoGrantee.Size = new Size(68, 15);
            lblPrivInfoGrantee.TabIndex = 0;
            lblPrivInfoGrantee.Text = "Chọn Grantee:";
            // 
            // 
            // cbPrivInfoGrantee
            // 
            cbPrivInfoGrantee.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPrivInfoGrantee.Location = new Point(10, 20);
            cbPrivInfoGrantee.Name = "cbPrivInfoGrantee";
            cbPrivInfoGrantee.Size = new Size(220, 23);
            cbPrivInfoGrantee.TabIndex = 0;
            // 
            // btnLoadPrivInfo
            // 
            btnLoadPrivInfo.BackColor = Color.LightSkyBlue;
            btnLoadPrivInfo.Location = new Point(250, 18);
            btnLoadPrivInfo.Name = "btnLoadPrivInfo";
            btnLoadPrivInfo.Size = new Size(140, 27);
            btnLoadPrivInfo.TabIndex = 1;
            btnLoadPrivInfo.Text = "Xem quyền";
            btnLoadPrivInfo.UseVisualStyleBackColor = false;
            // 
            // dgvPrivInfo
            // 
            dgvPrivInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPrivInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPrivInfo.Location = new Point(10, 70);
            dgvPrivInfo.Name = "dgvPrivInfo";
            dgvPrivInfo.ReadOnly = true;
            dgvPrivInfo.Size = new Size(1072, 592);
            dgvPrivInfo.TabIndex = 2;
            // 
            // btnSearch
            // 
            btnSearch.BackColor = Color.LightGreen;
            btnSearch.Location = new Point(330, 18);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(90, 27);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 700);
            Controls.Add(tabMain);
            Name = "Form1";
            Text = "ADMIN - Quản lý bệnh viện";
            tabMain.ResumeLayout(false);
            tabUsers.ResumeLayout(false);
            tabUsers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            tabRoles.ResumeLayout(false);
            tabRoles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRoles).EndInit();
            tabGrant.ResumeLayout(false);
            tabRevoke.ResumeLayout(false);
            tabPrivInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPrivInfo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnSearch;
    }
}
