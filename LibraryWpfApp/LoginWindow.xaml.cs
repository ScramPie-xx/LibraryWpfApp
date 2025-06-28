using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryWpfApp
{
    public partial class LoginWindow : Window
    {
        private const string connStr = "Server=WIN-4C2OD1FPDPQ\\SQLEXPRESS;Database=УправлениеБиблиотекой;Trusted_Connection=True;";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                StatusTextBlock.Text = "Введите логин и пароль!";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Роль FROM Персонал WHERE Логин = @u AND Пароль = @p", conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);
                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        StatusTextBlock.Text = "Неверный логин или пароль!";
                        MessageBox.Show("Неверный логин или пароль. Попробуйте еще раз.", "Ошибка авторизации",
                                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = $"Ошибка: {ex.Message}";
                }
            }
        }
    }
}
