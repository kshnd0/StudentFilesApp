using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StudentFilesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentFilesApp
{
    public partial class StudentsView : UserControl
    {
        public StudentsView()
        {
            InitializeComponent();
            LoadTreeView(); // Строим дерево при запуске
            LoadStudents(); // Загружаем всех студентов
        }

        // 1. Метод построения дерева из базы данных
        private void LoadTreeView()
        {
            GroupsTreeView.Items.Clear();

            using (var db = new AppDbContext())
            {
                // Получаем все группы из базы, сортируем по курсу
                var groups = db.Groups.OrderBy(g => g.Course).ToList();

                // Группируем их по курсу (1 курс, 2 курс и т.д.)
                var groupsByCourse = groups.GroupBy(g => g.Course);

                foreach (var courseGroup in groupsByCourse)
                {
                    // Создаем ветку для курса (например, "1 курс")
                    TreeViewItem courseNode = new TreeViewItem
                    {
                        Header = $"{courseGroup.Key} курс",
                        IsExpanded = true,
                        FontWeight = FontWeights.Bold,
                        Foreground = (System.Windows.Media.Brush)Application.Current.Resources["DarkestBlue"]
                    };

                    // Добавляем внутрь группы
                    foreach (var group in courseGroup)
                    {
                        TreeViewItem groupNode = new TreeViewItem
                        {
                            Header = group.GroupName, // Название группы (например, ИПсп-126)
                            Foreground = (System.Windows.Media.Brush)Application.Current.Resources["MidBlue"]
                        };
                        courseNode.Items.Add(groupNode);
                    }

                    GroupsTreeView.Items.Add(courseNode);
                }
            }
        }

        // 2. Метод загрузки студентов (с фильтром или без)
        private void LoadStudents(string groupName = null)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Students.Include(s => s.Group).AsQueryable();

                // Если передано имя группы, фильтруем
                if (!string.IsNullOrEmpty(groupName))
                {
                    query = query.Where(s => s.Group.GroupName == groupName);
                }

                StudentsDataGrid.ItemsSource = query.ToList();
            }
        }

        // 3. Обработка клика по дереву
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem selectedItem)
            {
                string headerText = selectedItem.Header.ToString();

                // Если кликнули на название группы (в нем есть дефис, например ИПсп-123)
                if (headerText.Contains("-"))
                {
                    LoadStudents(headerText);
                }
                else
                {
                    // Если кликнули на "1 курс", "2 курс" и т.д. - показываем всех
                    LoadStudents();
                }
            }
        }
    }
}