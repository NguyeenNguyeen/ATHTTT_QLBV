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

                bool shouldShowLogin = true;
                while (shouldShowLogin)
                {
                    shouldShowLogin = false;
                    System.IO.File.AppendAllText("debug_log.txt", "Showing LoginForm...\n");

                    using LoginForm loginForm = new LoginForm();
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        System.IO.File.AppendAllText("debug_log.txt", "Login cancelled or closed.\n");
                        break;
                    }

                    System.IO.File.AppendAllText("debug_log.txt", $"Login OK. Role: '{LoginForm.UserRole}'\n");

                    using Form mainForm = CreateMainForm();
                    Application.Run(mainForm);

                    if (mainForm is ILogoutAwareForm logoutAware && logoutAware.LogoutRequested)
                    {
                        System.IO.File.AppendAllText("debug_log.txt", "Logout requested. Returning to LoginForm...\n");
                        CloseCurrentLoginConnection();
                        shouldShowLogin = true;
                    }
                    else
                    {
                        CloseCurrentLoginConnection();
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("debug_log.txt", $"Exception: {ex.Message}\n{ex.StackTrace}\n");
                MessageBox.Show($"Lỗi nghiêm trọng: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}", "Crash", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Form CreateMainForm()
        {
            return LoginForm.UserRole switch
            {
                "DOCTOR" => new FormBacSi(
                    LoginForm.LoggedInUsername,
                    LoginForm.DoctorName,
                    LoginForm.DoctorConnection),
                "DISPATCHER" => new FormDieuPhoiVien(
                    LoginForm.LoggedInUsername,
                    LoginForm.DoctorName,
                    LoginForm.DoctorConnection),
                "TECHNICIAN" => new FormKyThuatVien(
                    LoginForm.LoggedInUsername,
                    LoginForm.DoctorName,
                    LoginForm.DoctorConnection),
                "PATIENT" => new FormBenhNhan(
                    LoginForm.LoggedInUsername,
                    LoginForm.DoctorName,
                    LoginForm.DoctorConnection),
                "OLS_USER" => new FormThongBaoOLS(
                    LoginForm.LoggedInUsername,
                    LoginForm.DoctorName,
                    LoginForm.DoctorConnection),
                _ => new Form1()
            };
        }

        private static void CloseCurrentLoginConnection()
        {
            try
            {
                LoginForm.DoctorConnection?.Close();
                LoginForm.DoctorConnection?.Dispose();
            }
            catch
            {
            }
        }
    }
}
