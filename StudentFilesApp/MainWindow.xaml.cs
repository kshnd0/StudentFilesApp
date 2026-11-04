using System.Windows;
using System.Windows.Controls;

namespace StudentFilesApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Клик по кнопке "Студенты"
        private void BtnStudents_Click(object sender, RoutedEventArgs e)
        {
            // Загружаем наш UserControl со списком групп и студентов
            MainContent.Content = new StudentsView();
        }

        // Клик по кнопке "Главная"
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем область (или выводим приветствие)
            MainContent.Content = null;
        }
    }
}