using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryWpfApp
{
    public partial class MainWindow : Window
    {
        private const string connStr = "Server=WIN-4C2OD1FPDPQ\\SQLEXPRESS;Database=УправлениеБиблиотекой;Trusted_Connection=True;";

        public MainWindow()
        {
            InitializeComponent();
            LoadAbonements();
            LoadBooks();
        }

        private void LoadAbonements()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT АбонентID, ФИО FROM Абоненты WHERE Статус = 'Активен'", conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    AbonementComboBox.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки абонементов: " + ex.Message);
                }
            }
        }

        private void LoadBooks()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    // Проверка подключения
                    conn.Open();
                    MessageBox.Show("Подключение к БД успешно установлено.", "Отладка");

                    // Упрощенный запрос для тестирования
                    SqlCommand cmd = new SqlCommand("SELECT КнигаID, Название FROM Книги", conn); 
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    
                    MessageBox.Show($"Загружено книг: {dt.Rows.Count}\nПервая книга: {(dt.Rows.Count > 0 ? dt.Rows[0]["Название"] : "Нет данных")}", "Отладка");
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Таблица Книги пуста или не содержит данных.", "Информация");
                    }

                    BookComboBox.ItemsSource = dt.DefaultView;
                    BookComboBox.DisplayMemberPath = "Название";
                    BookComboBox.SelectedValuePath = "КнигаID";
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка SQL: {ex.Message}\nНомер ошибки: {ex.Number}\nИсточник: {ex.Source}", "Ошибка подключения");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Общая ошибка загрузки книг: {ex.Message}\nПодробности: {ex.StackTrace}", "Ошибка");
                }
            }
        }

        private void IssueBook_Click(object sender, RoutedEventArgs e)
        {
            if (AbonementComboBox.SelectedValue == null || BookComboBox.SelectedValue == null ||
                IssueDatePicker.SelectedDate == null || DueDatePicker.SelectedDate == null)
            {
                StatusTextBlock.Text = "Заполните все поля!";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("ВыдатьКнигу", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@АбонентID", AbonementComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@КнигаID", BookComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@ДатаВыдачи", IssueDatePicker.SelectedDate);
                    cmd.Parameters.AddWithValue("@СрокВозврата", DueDatePicker.SelectedDate);
                    cmd.ExecuteNonQuery();
                    StatusTextBlock.Text = "Книга успешно выдана!";
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                    MessageBox.Show("Книга успешно выдана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBooks(); // Обновляем список книг после выдачи
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = "Ошибка: " + ex.Message;
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
        }

        private void OpenIssuedBooks_Click(object sender, RoutedEventArgs e)
        {
            IssuedBooksWindow issuedBooksWindow = new IssuedBooksWindow();
            issuedBooksWindow.Show();
            this.Close();
        }

        private void OpenAddAbonement_Click(object sender, RoutedEventArgs e)
        {
            AddAbonementWindow addAbonementWindow = new AddAbonementWindow();
            addAbonementWindow.Show();
            this.Close();
        }
    }
}