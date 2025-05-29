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
    /// Страница администратора для управления группами студентов
    /// </summary>
    /// <remarks>
    /// Предоставляет функционал для просмотра, добавления и удаления учебных групп,
    /// включая каскадное удаление связанных данных (студентов, оценок и связей с предметами)
    /// </remarks>
    public partial class AdminGroupsPage : Page
    {
        private StudyGroupEntities _context = StudyGroupEntities.GetContext();
        /// <summary>
        /// Инициализирует новую страницу управления группами
        /// </summary>
        /// <remarks>
        /// При создании автоматически загружает список всех групп из базы данных
        /// </remarks>
        public AdminGroupsPage()
        {
            InitializeComponent();
            LoadGroups();
        }
        /// <summary>
        /// Загружает список групп из базы данных в DataGrid
        /// </summary>
        private void LoadGroups()
        {
            GroupsDataGrid.ItemsSource = _context.Groups.ToList();
        }
        /// <summary>
        /// Проверяет, содержит ли текст только английские буквы, пробелы и дефисы
        /// </summary>
        /// <param name="text">Текст для проверки</param>
        /// <returns>True если текст содержит только разрешенные символы, иначе False</returns>
        private bool IsEnglishText(string text)
        {
            return Regex.IsMatch(text, @"^[a-zA-Z\s-]+$");
        }
        /// <summary>
        /// Обрабатывает событие нажатия кнопки добавления новой группы
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Аргументы события</param>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        /// Может возникнуть при ошибках сохранения в базу данных
        /// </exception>
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
        /// <summary>
        /// Обрабатывает событие нажатия кнопки удаления группы
        /// </summary>
        /// <remarks>
        /// Выполняет каскадное удаление всех связанных данных:
        /// 1) оценок студентов группы;
        /// 2) связей группы с предметами;
        /// 3) самих студентов группы;
        /// 4) группы.
        /// </remarks>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Аргументы события</param>
        /// <exception cref="System.Exception">
        /// Может возникнуть при ошибках работы с базой данных
        /// </exception>
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
