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
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ПерсоналID FROM Персонал WHERE Логин = @Login AND Пароль = @Password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        MainMenuWindow mainMenu = new MainMenuWindow();
                        mainMenu.Show();
                        this.Close();
                    }
                    else
                    {
                        StatusTextBlock.Text = "Неверный логин или пароль!";
                    }
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = "Ошибка подключения: " + ex.Message;
                }
            }
        }
    }
}