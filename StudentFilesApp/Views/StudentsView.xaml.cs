using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using StudentFilesApp.Data;
using StudentFilesApp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentFilesApp
{
    public partial class StudentsView : UserControl
    {
        private User _currentUser;

        public StudentsView(User user)
        {
            InitializeComponent();
            _currentUser = user;

            if (_currentUser.RoleId == 2)
            {
                BtnAddStudent.Visibility = Visibility.Collapsed;
            }

            LoadTreeView();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStudents();
        }

        private void LoadTreeView()
        {
            GroupsTreeView.Items.Clear();
            using (var db = new AppDbContext())
            {
                var groups = db.Groups.OrderBy(g => g.Course).ToList();
                var groupsByCourse = groups.GroupBy(g => g.Course);

                foreach (var courseGroup in groupsByCourse)
                {
                    TreeViewItem courseNode = new TreeViewItem
                    {
                        Header = $"{courseGroup.Key} курс",
                        IsExpanded = true,
                        FontWeight = FontWeights.Bold,
                        Foreground = (System.Windows.Media.Brush)Application.Current.Resources["DarkestBlue"]
                    };

                    foreach (var group in courseGroup)
                    {
                        TreeViewItem groupNode = new TreeViewItem
                        {
                            Header = group.GroupName,
                            Foreground = (System.Windows.Media.Brush)Application.Current.Resources["MidBlue"]
                        };
                        courseNode.Items.Add(groupNode);
                    }
                    GroupsTreeView.Items.Add(courseNode);
                }
            }
        }

        private void LoadStudents(string groupNameFromTree = null)
        {
            if (StudentsDataGrid == null) return;

            using (var db = new AppDbContext())
            {
                var query = db.Students.Include(s => s.Group).AsQueryable();

                if (!string.IsNullOrEmpty(groupNameFromTree))
                {
                    query = query.Where(s => s.Group.GroupName == groupNameFromTree);
                }

                string searchText = TxtSearch.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(s => s.LastName.ToLower().Contains(searchText) ||
                                             s.FirstName.ToLower().Contains(searchText) ||
                                             s.MiddleName.ToLower().Contains(searchText));
                }

                if (CmbCourse.SelectedIndex > 0)
                {
                    int course = CmbCourse.SelectedIndex;
                    query = query.Where(s => s.Group.Course == course);
                }

                if (CmbForm.SelectedIndex > 0)
                {
                    string form = ((ComboBoxItem)CmbForm.SelectedItem).Content.ToString();
                    query = query.Where(s => s.Group.FormOfStudy == form);
                }

                if (CmbStatus.SelectedIndex > 0)
                {
                    string status = ((ComboBoxItem)CmbStatus.SelectedItem).Content.ToString().ToLower();
                    query = query.Where(s => s.Status.ToLower() == status);
                }

                StudentsDataGrid.ItemsSource = query.ToList();
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem selectedItem)
            {
                string headerText = selectedItem.Header.ToString();
                if (headerText.Contains("-"))
                    LoadStudents(headerText);
                else
                    LoadStudents();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadStudents();
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadStudents();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StudentsDataGrid.SelectedItem is Student selectedStudent)
            {
                StudentCardWindow card = new StudentCardWindow(selectedStudent, _currentUser);
                card.ShowDialog();
                LoadStudents();
            }
        }

        private void BtnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            AddStudentWindow addWindow = new AddStudentWindow();
            addWindow.ShowDialog();
            LoadStudents();
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsDataGrid.ItemsSource == null) return;

            ExportSettingsWindow settingsWindow = new ExportSettingsWindow();
            if (settingsWindow.ShowDialog() == true)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = "Студенты_выгрузка.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        var students = (System.Collections.Generic.List<Student>)StudentsDataGrid.ItemsSource;

                        if (settingsWindow.SelectedGroup != "Все группы" && !string.IsNullOrEmpty(settingsWindow.SelectedGroup))
                        {
                            students = students.Where(s => s.Group?.GroupName == settingsWindow.SelectedGroup).ToList();
                        }

                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Студенты");
                            int col = 1;

                            if (settingsWindow.ExportFullName) worksheet.Cell(1, col++).Value = "ФИО";
                            if (settingsWindow.ExportGroup) worksheet.Cell(1, col++).Value = "Группа";
                            if (settingsWindow.ExportGender) worksheet.Cell(1, col++).Value = "Пол";
                            if (settingsWindow.ExportStatus) worksheet.Cell(1, col++).Value = "Статус";
                            if (settingsWindow.ExportBirthDate) worksheet.Cell(1, col++).Value = "Дата рождения";
                            if (settingsWindow.ExportPhone) worksheet.Cell(1, col++).Value = "Телефон";

                            var headerRange = worksheet.Range(1, 1, 1, col - 1);
                            headerRange.Style.Font.Bold = true;
                            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1B263B");
                            headerRange.Style.Font.FontColor = XLColor.White;
                            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            for (int i = 0; i < students.Count; i++)
                            {
                                col = 1;
                                if (settingsWindow.ExportFullName) worksheet.Cell(i + 2, col++).Value = students[i].FullName;
                                if (settingsWindow.ExportGroup) worksheet.Cell(i + 2, col++).Value = students[i].Group?.GroupName;
                                if (settingsWindow.ExportGender) worksheet.Cell(i + 2, col++).Value = students[i].Gender == "М" ? "Мужской" : "Женский";
                                if (settingsWindow.ExportStatus) worksheet.Cell(i + 2, col++).Value = students[i].Status;
                                if (settingsWindow.ExportBirthDate) worksheet.Cell(i + 2, col++).Value = students[i].BirthDate.ToString("dd.MM.yyyy");
                                if (settingsWindow.ExportPhone) worksheet.Cell(i + 2, col++).Value = students[i].Phone;
                            }

                            var dataRange = worksheet.Range(1, 1, students.Count + 1, col - 1);
                            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                            worksheet.Columns().AdjustToContents();
                            workbook.SaveAs(saveFileDialog.FileName);
                        }
                        MessageBox.Show($"Данные успешно выгружены!\nСтудентов в файле: {students.Count}", "Успех");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                    }
                }
            }
        }
    }
}