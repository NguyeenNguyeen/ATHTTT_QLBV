# -*- coding: utf-8 -*-
with open(r"c:\Users\Acer\Desktop\ATHTTT_QLBV\ADMIN\AddUserForm.cs", "w", encoding="utf-8") as f:
    f.write("""using System;
using System.Windows.Forms;

namespace ADMIN
{
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            cbAccountType.Items.Clear();
            cbAccountType.Items.AddRange(new string[] { "Nhân viên", "B?nh nhân" });
            cbAccountType.SelectedIndex = -1;
            cbAccountType.SelectedIndexChanged += new System.EventHandler(this.cbAccountType_SelectedIndexChanged);
            
            // Initially hide all specific fields until selected
            SetPatientFieldsVisibility(false);
            SetEmployeeFieldsVisibility(false);
            SetCommonFieldsVisibility(false);
        }

        private void cbAccountType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string? selected = cbAccountType.SelectedItem?.ToString();
            
            if (selected == "Nhân viên")
            {
                SetCommonFieldsVisibility(true);
                SetPatientFieldsVisibility(false);
                SetEmployeeFieldsVisibility(true);
            }
            else if (selected == "B?nh nhân")
            {
                SetCommonFieldsVisibility(true);
                SetEmployeeFieldsVisibility(false);
                SetPatientFieldsVisibility(true);
            }
            else
            {
                SetCommonFieldsVisibility(false);
                SetEmployeeFieldsVisibility(false);
                SetPatientFieldsVisibility(false);
            }
        }
        
        private void SetCommonFieldsVisibility(bool isVisible)
        {
            lblPassword.Visible = isVisible;
            txtPassword.Visible = isVisible;
            lblFullName.Visible = isVisible;
            txtFullName.Visible = isVisible;
            lblGender.Visible = isVisible;
            txtGender.Visible = isVisible;
            lblDob.Visible = isVisible;
            dtpDob.Visible = isVisible;
            lblCccd.Visible = isVisible;
            txtCccd.Visible = isVisible;
        }

        private void SetEmployeeFieldsVisibility(bool isVisible)
        {
            lblHometown.Visible = isVisible;
            txtHometown.Visible = isVisible;
            lblPhone.Visible = isVisible;
            txtPhone.Visible = isVisible;
            lblRole.Visible = isVisible;
            txtRole.Visible = isVisible;
            lblSpecialty.Visible = isVisible;
            txtSpecialty.Visible = isVisible;
            lblFacility.Visible = isVisible;
            txtFacility.Visible = isVisible;
        }
        
        private void SetPatientFieldsVisibility(bool isVisible)
        {
            lblHouseNumber.Visible = isVisible;
            txtHouseNumber.Visible = isVisible;
            lblStreet.Visible = isVisible;
            txtStreet.Visible = isVisible;
            lblDistrict.Visible = isVisible;
            txtDistrict.Visible = isVisible;
            lblProvince.Visible = isVisible;
            txtProvince.Visible = isVisible;
            lblMedicalHistory.Visible = isVisible;
            txtMedicalHistory.Visible = isVisible;
            lblFamilyMedicalHistory.Visible = isVisible;
            txtFamilyMedicalHistory.Visible = isVisible;
            lblDrugAllergy.Visible = isVisible;
            txtDrugAllergy.Visible = isVisible;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Logic to save the new user will be implemented here
            MessageBox.Show("Luu thành công!");
            this.Close();
        }
    }
}
""")
