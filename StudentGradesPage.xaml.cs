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
using System.Xml.Linq;

namespace StudentGroup
{
    /// <summary>
    /// Логика взаимодействия для StudentGradesPage.xaml
    /// </summary>
    public partial class StudentGradesPage : Page
    {
        private int _studentId;
        private StudyGroupEntities _context = StudyGroupEntities.GetContext();
        public StudentGradesPage()
        {
            InitializeComponent();
            _studentId = (int)App.IdStudent;
            LoadSubjects();
            LoadGrades();
        }
        private void LoadSubjects()
        {
            try
            {
                var subjects = _context.Grades.Where(g => g.StudentID == _studentId).Select(g => g.GroupSubjectTeacher.Subjects).Distinct().OrderBy(s => s.SubjectName).ToList();
                subjects.Insert(0, new Subjects
                {
                    SubjectID = -1,
                    SubjectName = "Все предметы"
                });
                SubjectFilterComboBox.ItemsSource = subjects;
                SubjectFilterComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке предметов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadGrades()
        {
            try
            {
                IQueryable<Grades> query = _context.Grades.Where(g => g.StudentID == _studentId);
                if (SubjectFilterComboBox.SelectedItem is Subjects selectedSubject && selectedSubject.SubjectID > 0)
                {
                    query = query.Where(g => g.GroupSubjectTeacher.SubjectID == selectedSubject.SubjectID);
                }
                var grades = query.Select(g => new
                {
                    Subject = g.GroupSubjectTeacher.Subjects,
                    Grade = g.GradeValue,
                    Date = g.GradeDate
                }).OrderByDescending(g => g.Date).ToList();
                GradesDataGrid.ItemsSource = grades;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке оценок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            LoadGrades();
        }
    }
}
