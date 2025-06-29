using System;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryWpfApp
{
    public partial class AddBookWindow : Window
    {
        private const string connStr = "Server=WIN-4C2OD1FPDPQ\\SQLEXPRESS;Database=УправлениеБиблиотекой;Trusted_Connection=True;";
        private MainMenuWindow mainMenu;

        public AddBookWindow(MainMenuWindow mainMenu)
        {
            InitializeComponent();
            this.mainMenu = mainMenu;
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            string title = BookTitleTextBox.Text;
            string author = BookAuthorTextBox.Text;
            if (!int.TryParse(TotalCopiesTextBox.Text, out int totalCopies) || !int.TryParse(AvailableCopiesTextBox.Text, out int availableCopies))
            {
                MessageBox.Show("Введите корректное число экземпляров!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || totalCopies < availableCopies)
            {
                MessageBox.Show("Заполните все поля корректно! Доступно не может превышать всего.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    MessageBox.Show("Подключение к базе данных...", "Отладка");
                    conn.Open();
                    MessageBox.Show("Подключение успешно, выполнение SQL...", "Отладка");
                    SqlCommand cmd = new SqlCommand("INSERT INTO Книги (Название, Автор, ДоступноЭкземпляров, ВсегоЭкземпляров) VALUES (@Title, @Author, @Available, @Total)", conn);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Author", author);
                    cmd.Parameters.AddWithValue("@Available", availableCopies);
                    cmd.Parameters.AddWithValue("@Total", totalCopies);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Книга успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    
                    this.Close();
                    mainMenu.Show();
                    mainMenu.Activate(); 
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка SQL: {ex.Message}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Общая ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            mainMenu.Show();
            mainMenu.Activate(); 
        }
    }
}
