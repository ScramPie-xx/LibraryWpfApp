using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibraryWpfApp
{
    public partial class AddAbonementWindow : Window
    {
        private const string connStr = "Server=WIN-4C2OD1FPDPQ\\SQLEXPRESS;Database=УправлениеБиблиотекой;Trusted_Connection=True;";
        private MainWindow mainWindow;

        public AddAbonementWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void AddAbonement_Click(object sender, RoutedEventArgs e)
        {
            string fullName = AbonementFullNameTextBox.Text;
            string status = StatusComboBox.SelectedItem != null ? (StatusComboBox.SelectedItem as ComboBoxItem).Content.ToString() : "Активен";

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Абонементы (ФИО, Статус) VALUES (@FullName, @Status)", conn);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Абонемент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                    mainWindow.ShowMain(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка добавления: " + ex.Message);
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
