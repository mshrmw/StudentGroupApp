using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для AdminTeacherAssignmentPage.xaml
    /// </summary>
    public partial class AdminTeacherAssignmentPage : Page
    {
        private StudyGroupEntities _context = StudyGroupEntities.GetContext();
        public AdminTeacherAssignmentPage()
        {
            InitializeComponent();
            LoadData();
            LoadAssignments();
        }
        private void LoadData()
        {
            try
            {
                TeachersComboBox.ItemsSource = _context.Teachers.Select(t => new
                {
                    t.TeacherID,
                    FullName = t.LastName + " " + t.FirstName + (t.MiddleName != null ? " " + t.MiddleName : "")
                }).OrderBy(t => t.FullName).ToList();
                SubjectsComboBox.ItemsSource = _context.Subjects.OrderBy(s => s.SubjectName).ToList();
                GroupsComboBox.ItemsSource = _context.Groups.OrderBy(g => g.GroupName).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadAssignments()
        {
            try
            {
                var assignments = _context.GroupSubjectTeacher.Include("Groups").Include("Subjects").Include("Teachers").OrderBy(gst => gst.Groups.GroupName).ThenBy(gst => gst.Subjects.SubjectName).ToList();
                AssignmentsDataGrid.ItemsSource = assignments;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке назначений: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddAssignmentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string hoursText = HoursTextBox.Text;
                if (TeachersComboBox.SelectedItem == null || SubjectsComboBox.SelectedItem == null || GroupsComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(hoursText))
                {
                    MessageBox.Show("Выберите преподавателя, предмет, группу и количество часов в неделю", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!int.TryParse(hoursText, out int hours) || hours <= 0)
                {
                    MessageBox.Show("Введите корректное количество часов (положительное число)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                dynamic teacher = TeachersComboBox.SelectedItem;
                int teacherId = teacher.TeacherID;
                var subject = (Subjects)SubjectsComboBox.SelectedItem;
                int subjectId = subject.SubjectID;
                var group = (Groups)GroupsComboBox.SelectedItem;
                int groupId = group.GroupID;
                if (_context.GroupSubjectTeacher.Any(gst => gst.TeacherID == teacherId && gst.SubjectID == subjectId && gst.GroupID == groupId))
                {
                    MessageBox.Show("Такое назначение уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var newAssignment = new GroupSubjectTeacher
                {
                    TeacherID = teacherId,
                    SubjectID = subjectId,
                    GroupID = groupId,
                    HoursPerWeek = hours
                };
                _context.GroupSubjectTeacher.Add(newAssignment);
                _context.SaveChanges();
                LoadAssignments();
                HoursTextBox.Clear();
                MessageBox.Show("Назначение успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении назначения: {ex.Message}", "Ошибка",  MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RemoveAssignmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (AssignmentsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите назначение для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selectedAssignment = AssignmentsDataGrid.SelectedItem as GroupSubjectTeacher;
            if (selectedAssignment == null)
            {
                MessageBox.Show("Выберите назначение для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var assignmentDescription = selectedAssignment.Subjects.SubjectName + " для группы " + selectedAssignment.Groups.GroupName + " (преподаватель: " + selectedAssignment.Teachers.LastName + ")";
            var result = MessageBox.Show("Вы действительно хотите удалить назначение '" + assignmentDescription + "'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var grades = _context.Grades.Where(g => g.GroupSubjectTeacherID == selectedAssignment.GroupSubjectTeacherID).ToList();
                        _context.Grades.RemoveRange(grades);
                        _context.GroupSubjectTeacher.Remove(selectedAssignment);
                        _context.SaveChanges();
                        transaction.Commit();
                        LoadAssignments();
                        MessageBox.Show("Назначение успешно удалено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("Ошибка при удалении назначения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
