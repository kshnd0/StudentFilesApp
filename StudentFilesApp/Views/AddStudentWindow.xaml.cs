using System;
using System.Linq;
using System.Windows;
using StudentFilesApp.Models;

namespace StudentFilesApp
{
    public partial class AddStudentWindow : Window
    {
        public AddStudentWindow()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void LoadGroups()
        {
            using (var db = new AppDbContext())
            {
                CmbGroup.ItemsSource = db.Groups.ToList();
                CmbGroup.DisplayMemberPath = "GroupName";
                CmbGroup.SelectedValuePath = "GroupId";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) || (CmbGroup.SelectedValue == null && string.IsNullOrWhiteSpace(TxtNewGroup.Text)))
            {
                MessageBox.Show("Заполните фамилию и выберите (или введите) группу!", "Ошибка");
                return;
            }

            using (var db = new AppDbContext())
            {
                int groupId = 0;

                // Если введена новая группа
                if (!string.IsNullOrWhiteSpace(TxtNewGroup.Text))
                {
                    var newGroup = new Group
                    {
                        GroupName = TxtNewGroup.Text.Trim(),
                        Course = 1,
                        FormOfStudy = "Очная",
                        SpecialtyId = 1
                    };
                    db.Groups.Add(newGroup);
                    db.SaveChanges();
                    groupId = newGroup.GroupId;
                }
                else if (CmbGroup.SelectedValue != null)
                {
                    groupId = (int)CmbGroup.SelectedValue;
                }

                var newStudent = new Student
                {
                    LastName = TxtLastName.Text,
                    FirstName = TxtFirstName.Text,
                    MiddleName = TxtMiddleName.Text,
                    BirthDate = DpBirthDate.SelectedDate ?? DateTime.Now,
                    Gender = "М",
                    Citizenship = "РФ",
                    Status = "обучается",
                    EnrollmentDate = DateTime.Now,
                    GroupId = groupId,
                    Phone = TxtPhone.Text
                };

                db.Students.Add(newStudent);
                db.SaveChanges();
            }

            MessageBox.Show("Студент успешно добавлен!", "Успех");
            this.Close();
        }
    }
}