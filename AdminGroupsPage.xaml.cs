using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudentGroup
{
    /// <summary>
    /// Логика взаимодействия для AdminGroupsPage.xaml
    /// </summary>
    public partial class AdminGroupsPage : Page
    {
        private StudyGroupEntities _context = StudyGroupEntities.GetContext();
        public AdminGroupsPage()
        {
            InitializeComponent();
            LoadGroups();
        }
        private void LoadGroups()
        {
            GroupsDataGrid.ItemsSource = _context.Groups.ToList();
        }
        private bool IsEnglishText(string text)
        {
            return Regex.IsMatch(text, @"^[a-zA-Z\s-]+$");
        }
        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            String groupName = GroupNameTextBox.Text;
            String specialization = GroupSpecializationTextBox.Text;
            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(specialization))
            {
                MessageBox.Show("Введите название и специальность группы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (IsEnglishText(groupName) || IsEnglishText(specialization))
            {
                MessageBox.Show("Не используйте английские буквы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_context.Groups.Any(g => g.GroupName == groupName))
            {
                MessageBox.Show("Группа с таким названием уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newGroup = new Groups
            {
                GroupName = GroupNameTextBox.Text,
                Specialization = GroupSpecializationTextBox.Text
            };
            _context.Groups.Add(newGroup);
            _context.SaveChanges();
            LoadGroups();
            GroupNameTextBox.Clear();
            GroupSpecializationTextBox.Clear();
        }
        private void DeleteGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (GroupsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selectedGroup = (Groups)GroupsDataGrid.SelectedItem;
            var groupName = selectedGroup.GroupName;
            var result = MessageBox.Show($"Вы действительно хотите удалить группу '{groupName}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var gradesToDelete = _context.Grades.Where(g => g.Students.GroupID == selectedGroup.GroupID).ToList();
                        _context.Grades.RemoveRange(gradesToDelete);
                        var groupLinksToDelete = _context.GroupSubjectTeacher.Where(gst => gst.GroupID == selectedGroup.GroupID).ToList();
                        _context.GroupSubjectTeacher.RemoveRange(groupLinksToDelete);
                        var studentsToDelete = _context.Students.Where(s => s.GroupID == selectedGroup.GroupID).ToList();
                        _context.Students.RemoveRange(studentsToDelete);
                        _context.Groups.Remove(selectedGroup);
                        _context.SaveChanges();
                        transaction.Commit();
                        LoadGroups();
                        MessageBox.Show($"Группа '{groupName}' удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении группы: {ex.Message}\n\n", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
