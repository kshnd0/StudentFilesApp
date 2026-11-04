using System.Linq;
using System.Windows;
using StudentFilesApp.Models;

namespace StudentFilesApp
{
    public partial class ExportSettingsWindow : Window
    {
        public bool ExportFullName => ChkFullName.IsChecked == true;
        public bool ExportGroup => ChkGroup.IsChecked == true;
        public bool ExportGender => ChkGender.IsChecked == true;
        public bool ExportStatus => ChkStatus.IsChecked == true;
        public bool ExportBirthDate => ChkBirthDate.IsChecked == true;
        public bool ExportPhone => ChkPhone.IsChecked == true;

        public string SelectedGroup => CmbExportGroup.SelectedItem?.ToString();

        public ExportSettingsWindow()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void LoadGroups()
        {
            using (var db = new AppDbContext())
            {
                var groups = db.Groups.OrderBy(g => g.GroupName).Select(g => g.GroupName).ToList();
                groups.Insert(0, "Все группы");
                CmbExportGroup.ItemsSource = groups;
                CmbExportGroup.SelectedIndex = 0;
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}