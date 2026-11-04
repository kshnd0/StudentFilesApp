using System.Linq;
using System.Windows.Controls;
using StudentFilesApp.Models;

namespace StudentFilesApp
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            using (var db = new AppDbContext())
            {
                
                TxtTotalStudents.Text = db.Students.Count().ToString();

                
                TxtActiveStudents.Text = db.Students.Count(s => s.Status == "обучается").ToString();
                TxtAкадем.Text = db.Students.Count(s => s.Status == "академ").ToString();
                TxtExpelled.Text = db.Students.Count(s => s.Status == "отчислен").ToString();
            }
        }
    }
}