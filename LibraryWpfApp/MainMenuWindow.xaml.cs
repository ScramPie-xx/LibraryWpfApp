using System.Windows;

namespace LibraryWpfApp
{
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void IssuedBooks_Click(object sender, RoutedEventArgs e)
        {
            IssuedBooksWindow issuedBooksWindow = new IssuedBooksWindow();
            issuedBooksWindow.Show();
            this.Close();
        }

        private void AddAbonement_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(); 
            AddAbonementWindow addAbonementWindow = new AddAbonementWindow(mainWindow);
            addAbonementWindow.Show();
            this.Close();
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            AddBookWindow addBookWindow = new AddBookWindow(this); 
            addBookWindow.Show();
            this.Hide(); 
        }
    }
}
