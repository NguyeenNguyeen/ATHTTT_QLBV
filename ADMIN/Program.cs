namespace ADMIN
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Hiển thị form đăng nhập cũ
            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Kiểm tra role người dùng - nếu là bác sĩ/y sĩ, mở FormBacSi
                    if (LoginForm.UserRole == "DOCTOR")
                    {
                        FormBacSi formBacSi = new FormBacSi(
                            LoginForm.LoggedInUsername,
                            LoginForm.DoctorName,
                            LoginForm.DoctorConnection
                        );
                        Application.Run(formBacSi);
                    }
                    else if (LoginForm.UserRole == "DISPATCHER")
                    {
                        FormDieuPhoiVien formDieuPhoiVien = new FormDieuPhoiVien(
                            LoginForm.LoggedInUsername,
                            LoginForm.DoctorName,
                            LoginForm.DoctorConnection
                        );
                        Application.Run(formDieuPhoiVien);
                    }
                    else if (LoginForm.UserRole == "TECHNICIAN")
                    {
                        FormKyThuatVien formKyThuatVien = new FormKyThuatVien(
                            LoginForm.LoggedInUsername,
                            LoginForm.DoctorName,
                            LoginForm.DoctorConnection
                        );
                        Application.Run(formKyThuatVien);
                    }
                    else if (LoginForm.UserRole == "PATIENT")
                    {
                        FormBenhNhan formBenhNhan = new FormBenhNhan(
                            LoginForm.LoggedInUsername,
                            LoginForm.DoctorName,
                            LoginForm.DoctorConnection
                        );
                        Application.Run(formBenhNhan);
                    }
                    else
                    {
                        // Mở form chính cho admin/users khác
                        Application.Run(new Form1());
                    }
                }
                else
                {
                    // Nếu người dùng ấn Thoát / Tắt form, đóng ứng dụng
                    Application.Exit();
                }
            }
        }
    }
}
