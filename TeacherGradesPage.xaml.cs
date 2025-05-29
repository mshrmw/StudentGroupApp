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
    /// Логика взаимодействия для TeacherGradesPage.xaml
    /// </summary>
    public partial class TeacherGradesPage : Page
    {
        private StudyGroupEntities _context = StudyGroupEntities.GetContext();
        private int _teacherId;
        public TeacherGradesPage()
        {
            InitializeComponent();
            _teacherId = (int)App.IdTeacher;
            LoadSubjects();
            WireUpEvents();
        }
        private void WireUpEvents()
        {
            SubjectComboBox.SelectionChanged += SubjectComboBox_SelectionChanged;
            GroupComboBox.SelectionChanged += GroupComboBox_SelectionChanged;
            FilterButton.Click += FilterButton_Click;
            AddGradeButton.Click += AddGradeButton_Click;
        }
        private void LoadSubjects()
        {
            try
            {
                var subjects = _context.GroupSubjectTeacher.Where(gst => gst.TeacherID == _teacherId).Select(gst => gst.Subjects).Distinct().OrderBy(s => s.SubjectName).ToList();
                SubjectComboBox.ItemsSource = subjects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке предметов: {ex.Message}");
            }
        }
        private void SubjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubjectComboBox.SelectedItem is Subjects selectedSubject)
            {
                LoadGroups(selectedSubject.SubjectID);
            }
        }
        private void LoadGroups(int subjectId)
        {
            try
            {
                var groups = _context.GroupSubjectTeacher.Where(gst => gst.TeacherID == _teacherId && gst.SubjectID == subjectId).Select(gst => gst.Groups).Distinct().OrderBy(g => g.GroupName).ToList();
                GroupComboBox.ItemsSource = groups;
                GroupComboBox.SelectedIndex = -1;
                GradesDataGrid.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке групп: {ex.Message}");
            }
        }
        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupComboBox.SelectedItem is Groups selectedGroup)
            {
                LoadStudents(selectedGroup.GroupID);
            }
        }
        private void LoadStudents(int groupId)
        {
            try
            {
                var students = _context.Students.Where(s => s.GroupID == groupId).OrderBy(s => s.LastName).ToList().Select(s => new
                {
                    StudentID = s.StudentID,
                    FullName = $"{s.LastName} {s.FirstName} {s.MiddleName}"
                }).OrderBy(s => s.FullName).ToList();
                StudentComboBox.ItemsSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке студентов: {ex.Message}");
            }
        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubjectComboBox.SelectedItem == null || GroupComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите предмет и группу");
                return;
            }
            LoadGrades();
        }
        private void LoadGrades()
        {
            try
            {
                var subjectId = ((Subjects)SubjectComboBox.SelectedItem).SubjectID;
                var groupId = ((Groups)GroupComboBox.SelectedItem).GroupID;
                var grades = _context.Grades.Include("Students").Include("GroupSubjectTeacher").Where(g => g.GroupSubjectTeacher.TeacherID == _teacherId && g.GroupSubjectTeacher.SubjectID == subjectId && g.GroupSubjectTeacher.GroupID == groupId).Select(g => new
                {
                    ID = g.GradeID,
                    StudentFullName = g.Students.LastName + " " + g.Students.FirstName + " " + g.Students.MiddleName,
                    g.GradeValue,
                    g.GradeDate
                }).OrderBy(g => g.StudentFullName).ThenByDescending(g => g.GradeDate).ToList();
                GradesDataGrid.ItemsSource = grades;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке оценок: {ex.Message}");
            }
        }
        private void AddGradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubjectComboBox.SelectedItem == null || GroupComboBox.SelectedItem == null || StudentComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите предмет, группу и студента");
                return;
            }
            if (!int.TryParse(GradeTextBox.Text, out int gradeValue) || gradeValue < 1 || gradeValue > 5)
            {
                MessageBox.Show("Оценка должна быть числом от 1 до 5");
                return;
            }
            DateTime? dateGrade = GradeDatePicker.SelectedDate;
            if (dateGrade > DateTime.Today)
            {
                MessageBox.Show("Дата проставлении оценки не должна быть позже текущей даты");
                return;
            }
            try
            {
                var subjectId = ((Subjects)SubjectComboBox.SelectedItem).SubjectID;
                var groupId = ((Groups)GroupComboBox.SelectedItem).GroupID;
                var studentId = (StudentComboBox.SelectedItem as dynamic).StudentID;
                var gst = _context.GroupSubjectTeacher.FirstOrDefault(g => g.TeacherID == _teacherId && g.SubjectID == subjectId && g.GroupID == groupId);
                if (gst == null)
                {
                    MessageBox.Show("Не найдена связь предмета с группой");
                    return;
                }
                var newGrade = new Grades
                {
                    StudentID = studentId,
                    GroupSubjectTeacherID = gst.GroupSubjectTeacherID,
                    GradeValue = gradeValue,
                    GradeDate = GradeDatePicker.SelectedDate.Value
                };
                _context.Grades.Add(newGrade);
                _context.SaveChanges();
                LoadGrades();
                MessageBox.Show("Оценка успешно добавлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении оценки: {ex.Message}");
            }
        }
        private void DeleteGradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GradesDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите оценку для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show("Вы уверены, что хотите удалить выбранную оценку?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var selectedItem = GradesDataGrid.SelectedItem;
                    var gradeId = (int)selectedItem.GetType().GetProperty("ID").GetValue(selectedItem, null);
                    var gradeToDelete = _context.Grades.Find(gradeId);
                    if (gradeToDelete != null)
                    {
                        _context.Grades.Remove(gradeToDelete);
                        _context.SaveChanges();
                        LoadGrades();
                        MessageBox.Show("Оценка успешно удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении оценки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
