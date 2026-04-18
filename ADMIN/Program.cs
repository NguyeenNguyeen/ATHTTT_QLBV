<<<<<<< HEAD
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

            // Hiển thị form đăng nhập trước tiên
            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu đăng nhập thành công, thì mở Form chính (Form1)
                    Application.Run(new Form1());
                }
                else
                {
                    // Nếu người dùng ấn Thoát / Tắt form, đóng ứng dụng
                    Application.Exit();
                }
            }
        }
    }
=======
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

            // Hiển thị form đăng nhập trước tiên
            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Nếu đăng nhập thành công, thì mở Form chính (Form1)
                    Application.Run(new Form1());
                }
                else
                {
                    // Nếu người dùng ấn Thoát / Tắt form, đóng ứng dụng
                    Application.Exit();
                }
            }
        }
    }
>>>>>>> 9718279 (Feature Role)
}