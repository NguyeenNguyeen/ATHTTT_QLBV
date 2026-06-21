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
            System.IO.File.WriteAllText("debug_log.txt", "Application started.\n");
            try
            {
                ApplicationConfiguration.Initialize();

                // Hiển thị form đăng nhập cũ
                System.IO.File.AppendAllText("debug_log.txt", "Showing LoginForm...\n");
                using (LoginForm loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.AppendAllText("debug_log.txt", $"Login OK. Role: '{LoginForm.UserRole}'\n");
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
                        else if (LoginForm.UserRole == "OLS_USER")
                        {
                            System.IO.File.AppendAllText("debug_log.txt", "Creating FormThongBaoOLS...\n");
                            FormThongBaoOLS formOls = new FormThongBaoOLS(
                                LoginForm.LoggedInUsername,
                                LoginForm.DoctorName,
                                LoginForm.DoctorConnection
                            );
                            System.IO.File.AppendAllText("debug_log.txt", "Running FormThongBaoOLS...\n");
                            Application.Run(formOls);
                            System.IO.File.AppendAllText("debug_log.txt", "FormThongBaoOLS closed normally.\n");
                        }
                        else
                        {
                            System.IO.File.AppendAllText("debug_log.txt", "Creating Form1...\n");
                            // Mở form chính cho admin/users khác
                            Application.Run(new Form1());
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText("debug_log.txt", "Login cancelled or closed.\n");
                        // Nếu người dùng ấn Thoát / Tắt form, đóng ứng dụng
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("debug_log.txt", $"Exception: {ex.Message}\n{ex.StackTrace}\n");
                MessageBox.Show($"Lỗi nghiêm trọng: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}", "Crash", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}