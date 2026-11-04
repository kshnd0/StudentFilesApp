using System;
using System.Windows;
using System.Windows.Media;
using StudentFilesApp.Data;
using StudentFilesApp.Models;

namespace StudentFilesApp
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

           
            using (var db = new AppDbContext())
            {
                DbInitializer.Initialize(db);
            }

            TxtUserName.Text = $"{user.LastName} {user.FirstName}";
            TxtUserRole.Text = user.RoleId == 1 ? "Сотрудник деканата" : "Сотрудник кафедры";
            TxtCurrentDate.Text = "Дата: " + DateTime.Now.ToString("dd.MM.yyyy");

            MainContent.Content = new HomeView();
            SetActiveButton(BtnHome);
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new HomeView();
            SetActiveButton(BtnHome);
        }

        private void BtnStudents_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new StudentsView(_currentUser);
            SetActiveButton(BtnStudents);
        }

        private void SetActiveButton(System.Windows.Controls.Button activeButton)
        {
            BtnHome.Background = Brushes.Transparent;
            BtnStudents.Background = Brushes.Transparent;
            activeButton.Background = (Brush)Application.Current.Resources["DarkBlue"];
        }
    }
}