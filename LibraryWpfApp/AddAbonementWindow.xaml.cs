using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibraryWpfApp
{
    public partial class AddAbonementWindow : Window
    {
        private const string connStr = "Server=WIN-4C2OD1FPDPQ\\SQLEXPRESS;Database=УправлениеБиблиотекой;Trusted_Connection=True;";

        public AddAbonementWindow()
        {
            InitializeComponent();
        }

        private void AddAbonement_Click(object sender, RoutedEventArgs e)
        {
            string fullName = AbonementFullNameTextBox.Text.Trim();
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
                    SqlCommand cmd = new SqlCommand("INSERT INTO Абоненты (ФИО, Статус, ДатаРегистрации) VALUES (@ФИО, @Статус, @ДатаРегистрации)", conn);
                    cmd.Parameters.AddWithValue("@ФИО", fullName);
                    cmd.Parameters.AddWithValue("@Статус", status);
                    cmd.Parameters.AddWithValue("@ДатаРегистрации", DateTime.Now); // Устанавливаем текущую дату
                    int rowsAffected = cmd.ExecuteNonQuery(); // Проверяем, сколько строк изменено
                    MessageBox.Show($"Rows affected: {rowsAffected}", "Отладка");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Абонемент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        AbonementFullNameTextBox.Text = "";
                        StatusComboBox.SelectedIndex = -1;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить абонемент. Проверьте данные или права доступа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка SQL: {ex.Message}\nНомер ошибки: {ex.Number}\nИсточник: {ex.Source}\nСтек: {ex.StackTrace}", "Ошибка базы данных");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Общая ошибка: {ex.Message}\nПодробности: {ex.StackTrace}", "Ошибка");
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