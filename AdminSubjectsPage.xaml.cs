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
    /// Логика взаимодействия для AdminSubjectsPage.xaml
    /// </summary>
    public partial class AdminSubjectsPage : Page
    {
        private StudyGroupEntities _context = StudyGroupEntities.GetContext();
        public AdminSubjectsPage()
        {
            InitializeComponent();
            LoadSubjects();
        }
        private void LoadSubjects()
        {
            SubjectsDataGrid.ItemsSource = _context.Subjects.OrderBy(s => s.SubjectName).ToList();
        }
        private void AddSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SubjectNameTextBox.Text) || string.IsNullOrWhiteSpace(HoursTextBox.Text))
            {
                MessageBox.Show("Введите название и часы предмета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(HoursTextBox.Text, out int hours) || hours <= 0)
            {
                MessageBox.Show("Введите корректное количество часов (положительное число)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_context.Subjects.Any(s => s.SubjectName == SubjectNameTextBox.Text))
            {
                MessageBox.Show("Предмет с таким названием уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newSubject = new Subjects
            {
                SubjectName = SubjectNameTextBox.Text,
                Hours = hours
            };
            _context.Subjects.Add(newSubject);
            _context.SaveChanges();
            LoadSubjects();
            SubjectNameTextBox.Clear();
            HoursTextBox.Clear();
        }

        private void DeleteSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedSubject = SubjectsDataGrid.SelectedItem as Subjects;
            if (selectedSubject == null)
            {
                MessageBox.Show("Выберите предмет для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Удалить предмет '{selectedSubject.SubjectName}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var groupSubjectTeachers = _context.GroupSubjectTeacher.Where(gst => gst.SubjectID == selectedSubject.SubjectID).ToList();
                    foreach (var gst in groupSubjectTeachers)
                    {
                        var grades = _context.Grades.Where(g => g.GroupSubjectTeacherID == gst.GroupSubjectTeacherID).ToList();
                        _context.Grades.RemoveRange(grades);
                    }
                    _context.GroupSubjectTeacher.RemoveRange(groupSubjectTeachers);
                    _context.Subjects.Remove(selectedSubject);
                    _context.SaveChanges();
                    LoadSubjects();
                    MessageBox.Show("Предмет успешно удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении предмета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
