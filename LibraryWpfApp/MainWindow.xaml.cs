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
            LoadSubscribers();
            LoadBooks();
        }

        private void LoadSubscribers()
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
                    SubscriberComboBox.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки абонентов: " + ex.Message);
                }
            }
        }

        private void LoadBooks()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT КнигаID, Название FROM Книги WHERE ДоступноЭкземпляров > 0", conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    BookComboBox.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки книг: " + ex.Message);
                }
            }
        }

        private void IssueBook_Click(object sender, RoutedEventArgs e)
        {
            if (SubscriberComboBox.SelectedValue == null || BookComboBox.SelectedValue == null ||
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
                    cmd.Parameters.AddWithValue("@АбонентID", SubscriberComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@КнигаID", BookComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@ДатаВыдачи", IssueDatePicker.SelectedDate);
                    cmd.Parameters.AddWithValue("@СрокВозврата", DueDatePicker.SelectedDate);
                    cmd.ExecuteNonQuery();
                    StatusTextBlock.Text = "Книга успешно выдана!";
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                    MessageBox.Show("Книга успешно выдана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBooks();
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

        private void OpenAddSubscriber_Click(object sender, RoutedEventArgs e)
        {
            AddSubscriberWindow addSubscriberWindow = new AddSubscriberWindow();
            addSubscriberWindow.Show();
            this.Close();
        }
    }
}
