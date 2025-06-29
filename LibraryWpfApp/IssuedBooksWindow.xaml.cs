using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryWpfApp
{
    public partial class IssuedBooksWindow : Window
    {
        private const string connStr = "Server=WIN-4C2OD1FPDPQ\\SQLEXPRESS;Database=УправлениеБиблиотекой;Trusted_Connection=True;";

        public IssuedBooksWindow()
        {
            InitializeComponent();
            LoadIssuedBooks();
        }

        private void LoadIssuedBooks()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT v.ВыдачаID, a.ФИО AS АбонементФИО, k.Название, v.ДатаВыдачи, v.СрокВозврата, v.ДатаВозврата, p.ФИО AS ПерсоналФИО FROM Выдачи v JOIN Абонементы a ON v.АбонементID = a.АбонементID JOIN Книги k ON v.КнигаID = k.КнигаID JOIN Персонал p ON v.ПерсоналID = p.ПерсоналID", conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    MessageBox.Show($"Загружено записей: {dt.Rows.Count}", "Отладка");
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Нет выданных книг для отображения.", "Информация");
                    }

                    IssuedBooksDataGrid.ItemsSource = dt.DefaultView;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка SQL: {ex.Message}", "Ошибка базы данных");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Общая ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private void IssuedBooksDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Логика выбора строки, если нужна
        }

        private void ReturnBook_Click(object sender, RoutedEventArgs e)
        {
            if (IssuedBooksDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись для возврата!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (IssuedBooksDataGrid.SelectedItem as DataRowView).Row;
            int выдачаID = Convert.ToInt32(row["ВыдачаID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("RETURN_BOOK", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ВыдачаID", выдачаID);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Книга успешно возвращена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadIssuedBooks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка возврата книги: " + ex.Message);
                }
            }
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            MainMenuWindow mainWindow = new MainMenuWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}