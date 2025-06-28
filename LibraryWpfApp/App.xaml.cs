using System.Windows;

namespace LibraryWpfApp
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
