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
        private ComboBox cbRolePrivilege;
        private TextBox txtRoleTableName;
        private Button btnAddRole;
        private Button btnDeleteRole;
        private Button btnLoadRoles;

        private Label lblGrantGrantee;
        private Label lblGrantObjectType;
        private Label lblGrantObjectName;
        private Label lblGrantPrivileges;
        private Label lblGrantColumnLevel;
        private ComboBox cbGrantGrantee;
        private ComboBox cbGrantObjectType;
        private ComboBox cbGrantObjectName;
        private CheckedListBox clbGrantPrivileges;
        private CheckedListBox clbGrantColumns;
        private CheckBox chkGrantWithOption;
        private CheckBox chkGrantColumnLevel;
        private Button btnGrantExecute;

        private Label lblRevokeGrantee;
        private Label lblRevokeObjectType;
        private Label lblRevokeObjectName;
        private Label lblRevokeColumnLevel;
        private ComboBox cbRevokeGrantee;
        private ComboBox cbRevokeObjectType;
        private ComboBox cbRevokeObjectName;
        private CheckedListBox clbRevokePrivileges;
        private CheckedListBox clbRevokeColumns;
        private CheckBox chkRevokeColumnLevel;
        private Button btnRevokeExecute;

        private Label lblPrivInfoGrantee;
        private ComboBox cbPrivilegeType;
        private TextBox txtSearchPrivUser;
        private Button btnLoadPrivInfo;
        private DataGridView dgvPrivInfo;

        private ComboBox cbUserType;

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
            cbUserType = new ComboBox();
            dgvUsers = new DataGridView();
            txtUserName = new TextBox();
            btnSearch = new Button();
            btnAddUser = new Button();
            btnEditUser = new Button();
            btnDeleteUser = new Button();
            tabRoles = new TabPage();
            dgvRoles = new DataGridView();
            txtRoleName = new TextBox();
            cbRolePrivilege = new ComboBox();
            txtRoleTableName = new TextBox();
            btnAddRole = new Button();
            btnDeleteRole = new Button();
            btnLoadRoles = new Button();
            tabGrant = new TabPage();
            lblGrantGrantee = new Label();
            lblGrantObjectType = new Label();
            lblGrantObjectName = new Label();
            lblGrantPrivileges = new Label();
            lblGrantColumnLevel = new Label();
            cbGrantGrantee = new ComboBox();
            cbGrantObjectType = new ComboBox();
            cbGrantObjectName = new ComboBox();
            clbGrantPrivileges = new CheckedListBox();
            clbGrantColumns = new CheckedListBox();
            chkGrantWithOption = new CheckBox();
            chkGrantColumnLevel = new CheckBox();
            btnGrantExecute = new Button();
            tabRevoke = new TabPage();
            lblRevokeGrantee = new Label();
            lblRevokeObjectType = new Label();
            lblRevokeObjectName = new Label();
            lblRevokeColumnLevel = new Label();
            cbRevokeGrantee = new ComboBox();
            cbRevokeObjectType = new ComboBox();
            cbRevokeObjectName = new ComboBox();
            clbRevokePrivileges = new CheckedListBox();
            clbRevokeColumns = new CheckedListBox();
            chkRevokeColumnLevel = new CheckBox();
            btnRevokeExecute = new Button();
            tabPrivInfo = new TabPage();
            cbPrivilegeType = new ComboBox();
            txtSearchPrivUser = new TextBox();
            btnLoadPrivInfo = new Button();
            dgvPrivInfo = new DataGridView();
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
            tabMain.Margin = new Padding(3, 4, 3, 4);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1257, 933);
            tabMain.TabIndex = 0;
            // 
            // tabUsers
            // 
            tabUsers.Controls.Add(cbUserType);
            tabUsers.Controls.Add(dgvUsers);
            tabUsers.Controls.Add(txtUserName);
            tabUsers.Controls.Add(btnSearch);
            tabUsers.Controls.Add(btnAddUser);
            tabUsers.Controls.Add(btnEditUser);
            tabUsers.Controls.Add(btnDeleteUser);
            tabUsers.Location = new Point(4, 29);
            tabUsers.Margin = new Padding(3, 4, 3, 4);
            tabUsers.Name = "tabUsers";
            tabUsers.Padding = new Padding(11, 13, 11, 13);
            tabUsers.Size = new Size(1249, 900);
            tabUsers.TabIndex = 0;
            tabUsers.Text = "User";
            tabUsers.UseVisualStyleBackColor = true;
            // 
            // cbUserType
            // 
            cbUserType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbUserType.FormattingEnabled = true;
            cbUserType.Items.AddRange(new object[] { "Nhân viên", "Bệnh nhân" });
            cbUserType.Location = new Point(251, 27);
            cbUserType.Margin = new Padding(3, 4, 3, 4);
            cbUserType.Name = "cbUserType";
            cbUserType.Size = new Size(148, 28);
            cbUserType.TabIndex = 2;
            // 
            // dgvUsers
            // 
            dgvUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.ColumnHeadersHeight = 29;
            dgvUsers.Location = new Point(11, 160);
            dgvUsers.Margin = new Padding(3, 4, 3, 4);
            dgvUsers.Name = "dgvUsers";
            dgvUsers.ReadOnly = true;
            dgvUsers.RowHeadersWidth = 51;
            dgvUsers.Size = new Size(1225, 723);
            dgvUsers.TabIndex = 0;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(11, 27);
            txtUserName.Margin = new Padding(3, 4, 3, 4);
            txtUserName.Name = "txtUserName";
            txtUserName.PlaceholderText = "Mã user";
            txtUserName.Size = new Size(228, 27);
            txtUserName.TabIndex = 1;
            // 
            // btnSearch
            // 
            btnSearch.BackColor = Color.LightGreen;
            btnSearch.Location = new Point(411, 24);
            btnSearch.Margin = new Padding(3, 4, 3, 4);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(103, 36);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += btnSearch_Click;
            // 
            // btnAddUser
            // 
            btnAddUser.BackColor = Color.LightGreen;
            btnAddUser.Location = new Point(526, 24);
            btnAddUser.Margin = new Padding(3, 4, 3, 4);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(103, 36);
            btnAddUser.TabIndex = 10;
            btnAddUser.Text = "Thêm";
            btnAddUser.UseVisualStyleBackColor = false;
            btnAddUser.Click += btnAddUser_Click;
            // 
            // btnEditUser
            // 
            btnEditUser.BackColor = Color.LightGoldenrodYellow;
            btnEditUser.Location = new Point(640, 24);
            btnEditUser.Margin = new Padding(3, 4, 3, 4);
            btnEditUser.Name = "btnEditUser";
            btnEditUser.Size = new Size(103, 36);
            btnEditUser.TabIndex = 4;
            btnEditUser.Text = "Sửa";
            btnEditUser.UseVisualStyleBackColor = false;
            btnEditUser.Click += btnEditUser_Click;
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.BackColor = Color.LightCoral;
            btnDeleteUser.Location = new Point(754, 24);
            btnDeleteUser.Margin = new Padding(3, 4, 3, 4);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new Size(103, 36);
            btnDeleteUser.TabIndex = 5;
            btnDeleteUser.Text = "Xóa";
            btnDeleteUser.UseVisualStyleBackColor = false;
            btnDeleteUser.Click += btnDeleteUser_Click;
            // 
            // tabRoles
            // 
            tabRoles.Controls.Add(dgvRoles);
            tabRoles.Controls.Add(txtRoleName);
            tabRoles.Controls.Add(cbRolePrivilege);
            tabRoles.Controls.Add(txtRoleTableName);
            tabRoles.Controls.Add(btnAddRole);
            tabRoles.Controls.Add(btnDeleteRole);
            tabRoles.Controls.Add(btnLoadRoles);
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
            dgvRoles.Location = new Point(10, 70);
            dgvRoles.Name = "dgvRoles";
            dgvRoles.ReadOnly = true;
            dgvRoles.Size = new Size(1072, 592);
            dgvRoles.TabIndex = 0;
            // 
            // txtRoleName
            // 
            txtRoleName.Location = new Point(10, 20);
            txtRoleName.Name = "txtRoleName";
            txtRoleName.PlaceholderText = "Tên role (p_role_name)";
            txtRoleName.Size = new Size(180, 23);
            txtRoleName.TabIndex = 1;
            // 
            // cbRolePrivilege
            // 
            cbRolePrivilege.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRolePrivilege.FormattingEnabled = true;
            cbRolePrivilege.Items.AddRange(new object[] { "SELECT", "INSERT", "UPDATE", "DELETE", "ALL" });
            cbRolePrivilege.Location = new Point(200, 20);
            cbRolePrivilege.Name = "cbRolePrivilege";
            cbRolePrivilege.Size = new Size(130, 23);
            cbRolePrivilege.TabIndex = 2;
            // 
            // txtRoleTableName
            // 
            txtRoleTableName.Location = new Point(340, 20);
            txtRoleTableName.Name = "txtRoleTableName";
            txtRoleTableName.PlaceholderText = "Tên bảng (p_table_name)";
            txtRoleTableName.Size = new Size(190, 23);
            txtRoleTableName.TabIndex = 3;
            // 
            // btnAddRole
            // 
            btnAddRole.BackColor = Color.LightGreen;
            btnAddRole.Location = new Point(540, 18);
            btnAddRole.Name = "btnAddRole";
            btnAddRole.Size = new Size(170, 27);
            btnAddRole.TabIndex = 4;
            btnAddRole.Text = "Tạo role + cấp quyền";
            btnAddRole.UseVisualStyleBackColor = false;
            btnAddRole.Click += btnAddRole_Click;
            // 
            // btnDeleteRole
            // 
            btnDeleteRole.BackColor = Color.LightCoral;
            btnDeleteRole.Location = new Point(720, 18);
            btnDeleteRole.Name = "btnDeleteRole";
            btnDeleteRole.Size = new Size(120, 27);
            btnDeleteRole.TabIndex = 5;
            btnDeleteRole.Text = "Xóa role";
            btnDeleteRole.UseVisualStyleBackColor = false;
            btnDeleteRole.Click += btnDeleteRole_Click;
            // 
            // btnLoadRoles
            // 
            btnLoadRoles.BackColor = Color.LightSkyBlue;
            btnLoadRoles.Location = new Point(850, 18);
            btnLoadRoles.Name = "btnLoadRoles";
            btnLoadRoles.Size = new Size(140, 27);
            btnLoadRoles.TabIndex = 6;
            btnLoadRoles.Text = "Xem tất cả Role";
            btnLoadRoles.UseVisualStyleBackColor = false;
            btnLoadRoles.Click += btnLoadRoles_Click;
            // 
            // tabGrant
            // 
            tabGrant.Controls.Add(lblGrantGrantee);
            tabGrant.Controls.Add(lblGrantObjectType);
            tabGrant.Controls.Add(lblGrantObjectName);
            tabGrant.Controls.Add(lblGrantPrivileges);
            tabGrant.Controls.Add(lblGrantColumnLevel);
            tabGrant.Controls.Add(cbGrantGrantee);
            tabGrant.Controls.Add(cbGrantObjectType);
            tabGrant.Controls.Add(cbGrantObjectName);
            tabGrant.Controls.Add(clbGrantPrivileges);
            tabGrant.Controls.Add(clbGrantColumns);
            tabGrant.Controls.Add(chkGrantWithOption);
            tabGrant.Controls.Add(chkGrantColumnLevel);
            tabGrant.Controls.Add(btnGrantExecute);
            tabGrant.Location = new Point(4, 29);
            tabGrant.Margin = new Padding(3, 4, 3, 4);
            tabGrant.Name = "tabGrant";
            tabGrant.Padding = new Padding(11, 13, 11, 13);
            tabGrant.Size = new Size(1249, 900);
            tabGrant.TabIndex = 2;
            tabGrant.Text = "Grant";
            tabGrant.UseVisualStyleBackColor = true;
            // 
            // lblGrantGrantee
            // 
            lblGrantGrantee.AutoSize = true;
            lblGrantGrantee.Location = new Point(11, 7);
            lblGrantGrantee.Name = "lblGrantGrantee";
            lblGrantGrantee.Size = new Size(143, 20);
            lblGrantGrantee.TabIndex = 0;
            lblGrantGrantee.Text = "Grantee (User/Role):";
            // 
            // lblGrantObjectType
            // 
            lblGrantObjectType.AutoSize = true;
            lblGrantObjectType.Location = new Point(286, 7);
            lblGrantObjectType.Name = "lblGrantObjectType";
            lblGrantObjectType.Size = new Size(110, 20);
            lblGrantObjectType.TabIndex = 1;
            lblGrantObjectType.Text = "Loại đối tượng:";
            // 
            // lblGrantObjectName
            // 
            lblGrantObjectName.AutoSize = true;
            lblGrantObjectName.Location = new Point(514, 7);
            lblGrantObjectName.Name = "lblGrantObjectName";
            lblGrantObjectName.Size = new Size(105, 20);
            lblGrantObjectName.TabIndex = 2;
            lblGrantObjectName.Text = "Tên đối tượng:";
            // 
            // lblGrantPrivileges
            // 
            lblGrantPrivileges.AutoSize = true;
            lblGrantPrivileges.Location = new Point(11, 67);
            lblGrantPrivileges.Name = "lblGrantPrivileges";
            lblGrantPrivileges.Size = new Size(54, 20);
            lblGrantPrivileges.TabIndex = 3;
            lblGrantPrivileges.Text = "Quyền:";
            // 
            // lblGrantColumnLevel
            // 
            lblGrantColumnLevel.AutoSize = true;
            lblGrantColumnLevel.Location = new Point(240, 67);
            lblGrantColumnLevel.Name = "lblGrantColumnLevel";
            lblGrantColumnLevel.Size = new Size(115, 20);
            lblGrantColumnLevel.TabIndex = 4;
            lblGrantColumnLevel.Text = "Cột (nếu muốn):";
            // 
            // cbGrantGrantee
            // 
            cbGrantGrantee.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGrantGrantee.Location = new Point(11, 27);
            cbGrantGrantee.Margin = new Padding(3, 4, 3, 4);
            cbGrantGrantee.Name = "cbGrantGrantee";
            cbGrantGrantee.Size = new Size(251, 28);
            cbGrantGrantee.TabIndex = 0;
            // 
            // cbGrantObjectType
            // 
            cbGrantObjectType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGrantObjectType.Items.AddRange(new object[] { "TABLE", "VIEW", "PROCEDURE", "FUNCTION" });
            cbGrantObjectType.Location = new Point(286, 27);
            cbGrantObjectType.Margin = new Padding(3, 4, 3, 4);
            cbGrantObjectType.Name = "cbGrantObjectType";
            cbGrantObjectType.Size = new Size(205, 28);
            cbGrantObjectType.TabIndex = 1;
            cbGrantObjectType.SelectedIndexChanged += cbGrantObjectType_SelectedIndexChanged;
            // 
            // cbGrantObjectName
            // 
            cbGrantObjectName.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGrantObjectName.Location = new Point(514, 27);
            cbGrantObjectName.Margin = new Padding(3, 4, 3, 4);
            cbGrantObjectName.Name = "cbGrantObjectName";
            cbGrantObjectName.Size = new Size(251, 28);
            cbGrantObjectName.TabIndex = 2;
            cbGrantObjectName.SelectedIndexChanged += cbGrantObjectName_SelectedIndexChanged;
            // 
            // clbGrantPrivileges
            // 
            clbGrantPrivileges.CheckOnClick = true;
            clbGrantPrivileges.Items.AddRange(new object[] { "SELECT", "INSERT", "UPDATE", "DELETE", "EXECUTE" });
            clbGrantPrivileges.Location = new Point(11, 93);
            clbGrantPrivileges.Margin = new Padding(3, 4, 3, 4);
            clbGrantPrivileges.Name = "clbGrantPrivileges";
            clbGrantPrivileges.Size = new Size(205, 158);
            clbGrantPrivileges.TabIndex = 3;
            clbGrantPrivileges.ItemCheck += clbGrantPrivileges_ItemCheck;
            // 
            // clbGrantColumns
            // 
            clbGrantColumns.CheckOnClick = true;
            clbGrantColumns.Location = new Point(240, 93);
            clbGrantColumns.Margin = new Padding(3, 4, 3, 4);
            clbGrantColumns.Name = "clbGrantColumns";
            clbGrantColumns.Size = new Size(251, 158);
            clbGrantColumns.TabIndex = 4;
            clbGrantColumns.Visible = false;
            // 
            // chkGrantWithOption
            // 
            chkGrantWithOption.Location = new Point(514, 93);
            chkGrantWithOption.Margin = new Padding(3, 4, 3, 4);
            chkGrantWithOption.Name = "chkGrantWithOption";
            chkGrantWithOption.Size = new Size(206, 32);
            chkGrantWithOption.TabIndex = 5;
            chkGrantWithOption.Text = "WITH GRANT OPTION";
            chkGrantWithOption.UseVisualStyleBackColor = true;
            // 
            // chkGrantColumnLevel
            // 
            chkGrantColumnLevel.Location = new Point(514, 127);
            chkGrantColumnLevel.Margin = new Padding(3, 4, 3, 4);
            chkGrantColumnLevel.Name = "chkGrantColumnLevel";
            chkGrantColumnLevel.Size = new Size(251, 32);
            chkGrantColumnLevel.TabIndex = 6;
            chkGrantColumnLevel.Text = "Phân quyền cấp cột (chọn cột bên trên)";
            chkGrantColumnLevel.UseVisualStyleBackColor = true;
            chkGrantColumnLevel.CheckedChanged += chkGrantColumnLevel_CheckedChanged;
            // 
            // btnGrantExecute
            // 
            btnGrantExecute.BackColor = Color.LightGreen;
            btnGrantExecute.Location = new Point(514, 147);
            btnGrantExecute.Margin = new Padding(3, 4, 3, 4);
            btnGrantExecute.Name = "btnGrantExecute";
            btnGrantExecute.Size = new Size(206, 40);
            btnGrantExecute.TabIndex = 6;
            btnGrantExecute.Text = "Thực thi GRANT";
            btnGrantExecute.UseVisualStyleBackColor = false;
            btnGrantExecute.Click += btnGrantExecute_Click;
            // 
            // tabRevoke
            // 
            tabRevoke.Controls.Add(lblRevokeGrantee);
            tabRevoke.Controls.Add(lblRevokeObjectType);
            tabRevoke.Controls.Add(lblRevokeObjectName);
            tabRevoke.Controls.Add(lblRevokeColumnLevel);
            tabRevoke.Controls.Add(cbRevokeGrantee);
            tabRevoke.Controls.Add(cbRevokeObjectType);
            tabRevoke.Controls.Add(cbRevokeObjectName);
            tabRevoke.Controls.Add(clbRevokePrivileges);
            tabRevoke.Controls.Add(clbRevokeColumns);
            tabRevoke.Controls.Add(chkRevokeColumnLevel);
            tabRevoke.Controls.Add(btnRevokeExecute);
            tabRevoke.Location = new Point(4, 29);
            tabRevoke.Margin = new Padding(3, 4, 3, 4);
            tabRevoke.Name = "tabRevoke";
            tabRevoke.Padding = new Padding(11, 13, 11, 13);
            tabRevoke.Size = new Size(1249, 900);
            tabRevoke.TabIndex = 3;
            tabRevoke.Text = "Revoke";
            tabRevoke.UseVisualStyleBackColor = true;
            // 
            // lblRevokeGrantee
            // 
            lblRevokeGrantee.AutoSize = true;
            lblRevokeGrantee.Location = new Point(11, 7);
            lblRevokeGrantee.Name = "lblRevokeGrantee";
            lblRevokeGrantee.Size = new Size(143, 20);
            lblRevokeGrantee.TabIndex = 0;
            lblRevokeGrantee.Text = "Grantee (User/Role):";
            // 
            // lblRevokeObjectType
            // 
            lblRevokeObjectType.AutoSize = true;
            lblRevokeObjectType.Location = new Point(286, 7);
            lblRevokeObjectType.Name = "lblRevokeObjectType";
            lblRevokeObjectType.Size = new Size(110, 20);
            lblRevokeObjectType.TabIndex = 1;
            lblRevokeObjectType.Text = "Loại đối tượng:";
            // 
            // lblRevokeObjectName
            // 
            lblRevokeObjectName.AutoSize = true;
            lblRevokeObjectName.Location = new Point(514, 7);
            lblRevokeObjectName.Name = "lblRevokeObjectName";
            lblRevokeObjectName.Size = new Size(105, 20);
            lblRevokeObjectName.TabIndex = 2;
            lblRevokeObjectName.Text = "Tên đối tượng:";
            // 
            // lblRevokeColumnLevel
            // 
            lblRevokeColumnLevel.AutoSize = true;
            lblRevokeColumnLevel.Location = new Point(240, 67);
            lblRevokeColumnLevel.Name = "lblRevokeColumnLevel";
            lblRevokeColumnLevel.Size = new Size(115, 20);
            lblRevokeColumnLevel.TabIndex = 3;
            lblRevokeColumnLevel.Text = "Cột (nếu muốn):";
            // 
            // cbRevokeGrantee
            // 
            cbRevokeGrantee.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRevokeGrantee.Location = new Point(11, 27);
            cbRevokeGrantee.Margin = new Padding(3, 4, 3, 4);
            cbRevokeGrantee.Name = "cbRevokeGrantee";
            cbRevokeGrantee.Size = new Size(251, 28);
            cbRevokeGrantee.TabIndex = 0;
            // 
            // cbRevokeObjectType
            // 
            cbRevokeObjectType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRevokeObjectType.Items.AddRange(new object[] { "TABLE", "VIEW", "PROCEDURE", "FUNCTION" });
            cbRevokeObjectType.Location = new Point(286, 27);
            cbRevokeObjectType.Margin = new Padding(3, 4, 3, 4);
            cbRevokeObjectType.Name = "cbRevokeObjectType";
            cbRevokeObjectType.Size = new Size(205, 28);
            cbRevokeObjectType.TabIndex = 1;
            cbRevokeObjectType.SelectedIndexChanged += cbRevokeObjectType_SelectedIndexChanged;
            // 
            // cbRevokeObjectName
            // 
            cbRevokeObjectName.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRevokeObjectName.Location = new Point(514, 27);
            cbRevokeObjectName.Margin = new Padding(3, 4, 3, 4);
            cbRevokeObjectName.Name = "cbRevokeObjectName";
            cbRevokeObjectName.Size = new Size(251, 28);
            cbRevokeObjectName.TabIndex = 2;
            cbRevokeObjectName.SelectedIndexChanged += cbRevokeObjectName_SelectedIndexChanged;
            // 
            // clbRevokePrivileges
            // 
            clbRevokePrivileges.CheckOnClick = true;
            clbRevokePrivileges.Items.AddRange(new object[] { "SELECT", "INSERT", "UPDATE", "DELETE", "EXECUTE" });
            clbRevokePrivileges.Location = new Point(11, 93);
            clbRevokePrivileges.Margin = new Padding(3, 4, 3, 4);
            clbRevokePrivileges.Name = "clbRevokePrivileges";
            clbRevokePrivileges.Size = new Size(205, 158);
            clbRevokePrivileges.TabIndex = 3;
            clbRevokePrivileges.ItemCheck += clbRevokePrivileges_ItemCheck;
            // 
            // clbRevokeColumns
            // 
            clbRevokeColumns.CheckOnClick = true;
            clbRevokeColumns.Location = new Point(240, 93);
            clbRevokeColumns.Margin = new Padding(3, 4, 3, 4);
            clbRevokeColumns.Name = "clbRevokeColumns";
            clbRevokeColumns.Size = new Size(251, 158);
            clbRevokeColumns.TabIndex = 4;
            clbRevokeColumns.Visible = false;
            // 
            // chkRevokeColumnLevel
            // 
            chkRevokeColumnLevel.Location = new Point(514, 127);
            chkRevokeColumnLevel.Margin = new Padding(3, 4, 3, 4);
            chkRevokeColumnLevel.Name = "chkRevokeColumnLevel";
            chkRevokeColumnLevel.Size = new Size(251, 32);
            chkRevokeColumnLevel.TabIndex = 6;
            chkRevokeColumnLevel.Text = "Phân quyền cấp cột (chọn cột bên trên)";
            chkRevokeColumnLevel.UseVisualStyleBackColor = true;
            chkRevokeColumnLevel.CheckedChanged += chkRevokeColumnLevel_CheckedChanged;
            // 
            // btnRevokeExecute
            // 
            btnRevokeExecute.BackColor = Color.LightCoral;
            btnRevokeExecute.Location = new Point(514, 147);
            btnRevokeExecute.Margin = new Padding(3, 4, 3, 4);
            btnRevokeExecute.Name = "btnRevokeExecute";
            btnRevokeExecute.Size = new Size(206, 40);
            btnRevokeExecute.TabIndex = 5;
            btnRevokeExecute.Text = "Thực thi REVOKE";
            btnRevokeExecute.UseVisualStyleBackColor = false;
            btnRevokeExecute.Click += btnRevokeExecute_Click;
            // 
            // tabPrivInfo
            // 
            tabPrivInfo.Controls.Add(cbPrivilegeType);
            tabPrivInfo.Controls.Add(txtSearchPrivUser);
            tabPrivInfo.Controls.Add(btnLoadPrivInfo);
            tabPrivInfo.Controls.Add(dgvPrivInfo);
            tabPrivInfo.Location = new Point(4, 29);
            tabPrivInfo.Margin = new Padding(3, 4, 3, 4);
            tabPrivInfo.Name = "tabPrivInfo";
            tabPrivInfo.Padding = new Padding(11, 13, 11, 13);
            tabPrivInfo.Size = new Size(1249, 900);
            tabPrivInfo.TabIndex = 4;
            tabPrivInfo.Text = "Thông tin quyền";
            tabPrivInfo.UseVisualStyleBackColor = true;
            // 
            // cbPrivilegeType
            // 
            cbPrivilegeType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPrivilegeType.FormattingEnabled = true;
            cbPrivilegeType.Items.AddRange(new object[] { "Xem quyền trên bảng", "Xem quyền trên cột", "Xem quyền trên view", "Xem quyền trên procedure/function" });
            cbPrivilegeType.Location = new Point(11, 27);
            cbPrivilegeType.Margin = new Padding(3, 4, 3, 4);
            cbPrivilegeType.Name = "cbPrivilegeType";
            cbPrivilegeType.Size = new Size(228, 28);
            cbPrivilegeType.TabIndex = 0;
            // 
            // txtSearchPrivUser
            // 
            txtSearchPrivUser.Location = new Point(251, 27);
            txtSearchPrivUser.Margin = new Padding(3, 4, 3, 4);
            txtSearchPrivUser.Name = "txtSearchPrivUser";
            txtSearchPrivUser.PlaceholderText = "Mã user / Role (tuỳ chọn)";
            txtSearchPrivUser.Size = new Size(228, 27);
            txtSearchPrivUser.TabIndex = 2;
            // 
            // btnLoadPrivInfo
            // 
            btnLoadPrivInfo.BackColor = Color.LightSkyBlue;
            btnLoadPrivInfo.Location = new Point(491, 24);
            btnLoadPrivInfo.Margin = new Padding(3, 4, 3, 4);
            btnLoadPrivInfo.Name = "btnLoadPrivInfo";
            btnLoadPrivInfo.Size = new Size(160, 36);
            btnLoadPrivInfo.TabIndex = 1;
            btnLoadPrivInfo.Text = "Xem quyền";
            btnLoadPrivInfo.UseVisualStyleBackColor = false;
            btnLoadPrivInfo.Click += btnLoadPrivInfo_Click;
            // 
            // dgvPrivInfo
            // 
            dgvPrivInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPrivInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPrivInfo.ColumnHeadersHeight = 29;
            dgvPrivInfo.Location = new Point(11, 93);
            dgvPrivInfo.Margin = new Padding(3, 4, 3, 4);
            dgvPrivInfo.Name = "dgvPrivInfo";
            dgvPrivInfo.ReadOnly = true;
            dgvPrivInfo.RowHeadersWidth = 51;
            dgvPrivInfo.Size = new Size(1225, 789);
            dgvPrivInfo.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1257, 933);
            Controls.Add(tabMain);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            tabMain.ResumeLayout(false);
            tabUsers.ResumeLayout(false);
            tabUsers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            tabRoles.ResumeLayout(false);
            tabRoles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRoles).EndInit();
            tabGrant.ResumeLayout(false);
            tabGrant.PerformLayout();
            tabRevoke.ResumeLayout(false);
            tabRevoke.PerformLayout();
            tabPrivInfo.ResumeLayout(false);
            tabPrivInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPrivInfo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnSearch;
    }
}
