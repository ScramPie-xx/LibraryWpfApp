using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LibraryWpfApp
{
    public partial class MainWindow : Window
    {
        private const string connStr = "Server=WIN-4C2OD1FPDPQ\\SQLEXPRESS;Database=УправлениеБиблиотекой;Trusted_Connection=True;";
        private int currentStaffId; 

        public MainWindow()
        {
            InitializeComponent();
           
            currentStaffId = 1; 
            LoadAbonements();
            LoadBooks();
        }

        private void LoadAbonements()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT АбонементID, ФИО FROM Абонементы", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                AbonementComboBox.ItemsSource = dt.DefaultView;
                AbonementComboBox.DisplayMemberPath = "ФИО";
                AbonementComboBox.SelectedValuePath = "АбонементID";
            }
        }

        private void LoadBooks()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT КнигаID, Название FROM Книги WHERE ДоступноЭкземпляров > 0", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                BookComboBox.ItemsSource = dt.DefaultView;
                BookComboBox.DisplayMemberPath = "Название";
                BookComboBox.SelectedValuePath = "КнигаID";
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

            int abonementId = (int)AbonementComboBox.SelectedValue;
            int bookId = (int)BookComboBox.SelectedValue;
            DateTime issueDate = IssueDatePicker.SelectedDate.Value;
            DateTime dueDate = DueDatePicker.SelectedDate.Value;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Выдачи (АбонементID, КнигаID, ПерсоналID, ДатаВыдачи, СрокВозврата) VALUES (@AbonementID, @BookID, @StaffID, @IssueDate, @DueDate)", conn);
                    cmd.Parameters.AddWithValue("@AbonementID", abonementId);
                    cmd.Parameters.AddWithValue("@BookID", bookId);
                    cmd.Parameters.AddWithValue("@StaffID", currentStaffId);
                    cmd.Parameters.AddWithValue("@IssueDate", issueDate);
                    cmd.Parameters.AddWithValue("@DueDate", dueDate);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("UPDATE Книги SET ДоступноЭкземпляров = ДоступноЭкземпляров - 1 WHERE КнигаID = @BookID", conn);
                    cmd.Parameters.AddWithValue("@BookID", bookId);
                    cmd.ExecuteNonQuery();

                    StatusTextBlock.Text = "Книга успешно выдана!";
                    LoadBooks(); 
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = "Ошибка: " + ex.Message;
                }
            }
        }

     
        public void ShowMain()
        {
            this.Show();
            LoadAbonements();
        }

        private void BackToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenuWindow mainMenu = new MainMenuWindow();
            mainMenu.Show();
            this.Close();
        }
    }
}
