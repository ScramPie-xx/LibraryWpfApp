using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibraryWpfApp
{
    public partial class AddSubscriberWindow : Window
    {
        private const string connStr = "Server=WIN-4C2OD1FPDPQ\\SQLEXPRESS;Database=УправлениеБиблиотекой;Trusted_Connection=True;";

        public AddSubscriberWindow()
        {
            InitializeComponent();
        }

        private void AddSubscriber_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text.Trim();
            string status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Отладка: выводим введенные данные
            MessageBox.Show($"ФИО: {fullName}, Статус: {status}", "Отладка данных");

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Абоненты (ФИО, Статус) VALUES (@ФИО, @Статус)", conn);
                    cmd.Parameters.AddWithValue("@ФИО", fullName);
                    cmd.Parameters.AddWithValue("@Статус", status);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Абонент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        FullNameTextBox.Text = "";
                        StatusComboBox.SelectedIndex = -1;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить абонента. Проверьте данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления абонента: {ex.Message}\nПодробности: {ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
}