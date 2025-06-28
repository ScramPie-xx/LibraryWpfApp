using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT v.ВыдачаID, a.ФИО, k.Название, v.ДатаВыдачи, v.СрокВозврата, v.ДатаВозврата
                        FROM ВыданныеКниги v
                        JOIN Абоненты a ON v.АбонентID = a.АбонентID
                        JOIN Книги k ON v.КнигаID = k.КнигаID
                        WHERE v.ДатаВозврата IS NULL", conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Нет выданных книг для отображения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    IssuedBooksDataGrid.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки выданных книг: " + ex.Message);
                }
            }
        }

        private void IssuedBooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Этот метод оставляем пустым, так как IsEnabled управляется конвертером
        }

        private void ReturnBook_Click(object sender, RoutedEventArgs e)
        {
            if (IssuedBooksDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int issueId = (int)selectedRow["ВыдачаID"];
                if (ReturnDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату возврата!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime returnDate = ReturnDatePicker.SelectedDate.Value;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("ВернутьКнигу", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ВыдачаID", issueId);
                        cmd.Parameters.AddWithValue("@ДатаВозврата", returnDate);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Книга успешно возвращена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadIssuedBooks();
                        ReturnDatePicker.SelectedDate = null; // Сброс даты после возврата
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка возврата книги: " + ex.Message);
                    }
                }
            }
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }

    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}