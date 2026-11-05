using System;
using System.IO;
using System.Linq;
using System.Windows;
using StudentFilesApp.Models;
using Xceed.Words.NET;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;

namespace StudentFilesApp
{
    public partial class StudentCardWindow : Window
    {
        private Student _currentStudent;
        private User _currentUser;

        public StudentCardWindow(Student student, User user)
        {
            InitializeComponent();
            _currentStudent = student;
            _currentUser = user;
            LoadStudentData();
        }

        private void LoadStudentData()
        {
            if (_currentStudent == null) return;

            TxtFullName.Text = _currentStudent.FullName;
            TxtStatus.Text = _currentStudent.Status;

            if (_currentStudent.Status == "обучается")
                TxtStatus.Foreground = System.Windows.Media.Brushes.Green;
            else if (_currentStudent.Status == "академ")
                TxtStatus.Foreground = System.Windows.Media.Brushes.Orange;
            else
                TxtStatus.Foreground = System.Windows.Media.Brushes.Red;

            if (_currentStudent.Group != null)
                TxtGroupInfo.Text = $"Группа: {_currentStudent.Group.GroupName} | Курс: {_currentStudent.Group.Course} | Форма: {_currentStudent.Group.FormOfStudy}";

            DpBirthDate.SelectedDate = _currentStudent.BirthDate;
            CmbGender.SelectedIndex = _currentStudent.Gender == "М" ? 0 : 1;

            TxtCitizenship.Text = string.IsNullOrEmpty(_currentStudent.Citizenship) ? "Не указано" : _currentStudent.Citizenship;
            TxtEnrollmentDate.Text = _currentStudent.EnrollmentDate.ToString("dd.MM.yyyy");

            int age = DateTime.Now.Year - _currentStudent.BirthDate.Year;
            if (DateTime.Now.DayOfYear < _currentStudent.BirthDate.DayOfYear) age--;
            TxtAge.Text = $"({age} лет)";

            TxtPhone.Text = string.IsNullOrEmpty(_currentStudent.Phone) ? "Не указан" : _currentStudent.Phone;
            TxtEmail.Text = string.IsNullOrEmpty(_currentStudent.Email) ? "Не указан" : _currentStudent.Email;
            TxtAddress.Text = string.IsNullOrEmpty(_currentStudent.Address) ? "Не указан" : _currentStudent.Address;


            if (_currentUser.RoleId == 2) 
            {
                TxtFullName.IsReadOnly = true;
                TxtFullName.Background = System.Windows.Media.Brushes.Transparent;
                TxtFullName.BorderThickness = new Thickness(0);
                DpBirthDate.IsEnabled = false;
                CmbGender.IsEnabled = false;
                BtnSaveChanges.Visibility = Visibility.Collapsed;
            }
            else 
            {
                TxtFullName.IsReadOnly = false;
                TxtFullName.Background = System.Windows.Media.Brushes.White;
                TxtFullName.BorderThickness = new Thickness(1);
                TxtFullName.BorderBrush = System.Windows.Media.Brushes.LightGray;
                DpBirthDate.IsEnabled = true;
                CmbGender.IsEnabled = true;
                BtnSaveChanges.Visibility = Visibility.Visible;
            }

            LoadTabsData();
        }

        private void LoadTabsData()
        {
            using (var db = new AppDbContext())
            {
                var grades = db.StudentDisciplines
                    .Include(sd => sd.Discipline)
                    .Where(sd => sd.StudentId == _currentStudent.StudentId)
                    .Select(sd => new
                    {
                        StudentDisciplineId = sd.StudentDisciplineId,
                        DisciplineName = sd.Discipline.Name,
                        TotalHours = sd.Discipline.TotalHours,
                        GradeValue = sd.GradeValue,
                        IsDebt = sd.IsDebt
                    }).ToList();
                GradesDataGrid.ItemsSource = grades;

                var orders = db.OrderStudents
                    .Include(os => os.Order)
                    .Where(os => os.StudentId == _currentStudent.StudentId)
                    .Select(os => new
                    {
                        OrderNumber = os.Order.OrderNumber,
                        OrderDate = os.Order.OrderDate.ToString("dd.MM.yyyy"),
                        Comment = os.Order.Comment
                    }).ToList();
                OrdersDataGrid.ItemsSource = orders;

                var parent = db.Parents.FirstOrDefault(p => p.StudentId == _currentStudent.StudentId);
                if (parent != null)
                {
                    TxtParentName.Text = $"{parent.LastName} {parent.FirstName} {parent.MiddleName}";
                    TxtParentRelation.Text = parent.Relation;
                    TxtParentPhone.Text = string.IsNullOrEmpty(parent.Phone) ? "Не указан" : parent.Phone;
                }
                else
                {
                    TxtParentName.Text = "Данные отсутствуют";
                    TxtParentRelation.Text = "";
                    TxtParentPhone.Text = "";
                }
            }
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var studentToUpdate = db.Students.Find(_currentStudent.StudentId);
                if (studentToUpdate != null)
                {
                    var fioParts = TxtFullName.Text.Split(' ');
                    if (fioParts.Length >= 2)
                    {
                        studentToUpdate.LastName = fioParts[0];
                        studentToUpdate.FirstName = fioParts[1];
                        if (fioParts.Length > 2) studentToUpdate.MiddleName = fioParts[2];
                    }
                    studentToUpdate.BirthDate = DpBirthDate.SelectedDate ?? studentToUpdate.BirthDate;
                    studentToUpdate.Gender = CmbGender.SelectedIndex == 0 ? "М" : "Ж";

                    db.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!", "Успех");
                }
            }
        }

        private void BtnCreateReport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document|*.docx",
                FileName = $"Справка_{_currentStudent.LastName}.docx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Spravka_Ob_Obuchenii.docx");
                    if (!File.Exists(templatePath))
                    {
                        MessageBox.Show($"Шаблон не найден: {templatePath}", "Ошибка");
                        return;
                    }

                    int enrollYear = _currentStudent.EnrollmentDate.Year;
                    string orderDate = new DateTime(enrollYear, 8, 25).ToString("dd.MM.yyyy");
                    string endDate = new DateTime(enrollYear + 4, 6, 30).ToString("dd.MM.yyyy");
                    string orderNumber = $"{new Random().Next(100, 1000)}-ст";

                    using (DocX document = DocX.Load(templatePath))
                    {
                        document.ReplaceText("{ФИО}", _currentStudent.FullName);
                        document.ReplaceText("{группа}", _currentStudent.Group?.GroupName ?? "Не указана");
                        document.ReplaceText("{курс}", _currentStudent.Group?.Course.ToString() ?? "?");
                        document.ReplaceText("{форма обучения}", _currentStudent.Group?.FormOfStudy ?? "Очная");
                        document.ReplaceText("{дата зачисления}", _currentStudent.EnrollmentDate.ToString("dd.MM.yyyy"));
                        document.ReplaceText("{дата_рождения}", _currentStudent.BirthDate.ToString("dd.MM.yyyy"));
                        document.ReplaceText("{дата_приказа}", orderDate);
                        document.ReplaceText("{дата окончания}", endDate);
                        document.ReplaceText("{номер приказа}", orderNumber);
                        document.ReplaceText("{номер}", new Random().Next(100000, 999999).ToString());
                        document.ReplaceText("{специальность}", "09.02.07 Информационные системы и программирование");
                        document.ReplaceText("{бюджет}", "Бюджет");

                        document.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Справка успешно создана!", "Успех");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private void BtnCreateDebtReport_Click(object sender, RoutedEventArgs e)
        {
            if (GradesDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите дисциплину в таблице!", "Внимание");
                return;
            }

            dynamic selectedRow = GradesDataGrid.SelectedItem;

            if (selectedRow.IsDebt == false)
            {
                MessageBox.Show("По этой дисциплине нет задолженности!", "Ошибка");
                return;
            }

            string disciplineName = selectedRow.DisciplineName;

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document|*.docx",
                FileName = $"Направление_{_currentStudent.LastName}_{disciplineName}.docx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Napravlenie_Na_Dolg.docx");
                    if (!File.Exists(templatePath))
                    {
                        MessageBox.Show($"Шаблон не найден: {templatePath}", "Ошибка");
                        return;
                    }

                    using (DocX document = DocX.Load(templatePath))
                    {
                        document.ReplaceText("{ФИО}", _currentStudent.FullName);
                        document.ReplaceText("{Группа}", _currentStudent.Group?.GroupName ?? "Не указана");
                        document.ReplaceText("{Дисциплина}", disciplineName);
                        document.ReplaceText("{Преподаватель}", "Смирнов А.А.");
                        document.ReplaceText("{Дата}", DateTime.Now.ToString("dd.MM.yyyy"));
                        document.ReplaceText("{Семестр}", "2");
                        document.ReplaceText("{Дата выдачи}", DateTime.Now.ToString("dd.MM.yyyy"));

                        document.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Направление успешно создано!", "Успех");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                }
            }
        }
    }
}