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
    /// Логика взаимодействия для TeacherSubjectsPage.xaml
    /// </summary>
    public partial class TeacherSubjectsPage : Page
    {
        private StudyGroupEntities _context = StudyGroupEntities.GetContext();
        private int _teacherId;
        public TeacherSubjectsPage()
        {
            InitializeComponent();
            _teacherId = (int)App.IdTeacher;
            LoadSubjects();
            LoadTeacherSubjects();
        }
        private void LoadSubjects()
        {
            try
            {
                var subjects = _context.GroupSubjectTeacher.Where(gst => gst.TeacherID == _teacherId).Select(gst => gst.Subjects).Distinct().OrderBy(s => s.SubjectName).ToList();
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
        private void LoadTeacherSubjects()
        {
            try
            {
                IQueryable<GroupSubjectTeacher> query = _context.GroupSubjectTeacher.Where(gst => gst.TeacherID == _teacherId);
                if (SubjectFilterComboBox.SelectedItem is Subjects selectedSubject && selectedSubject.SubjectID > 0)
                {
                    query = query.Where(gst => gst.SubjectID == selectedSubject.SubjectID);
                }
                var teacherSubjects = query.Select(gst => new
                {
                    SubjectName = gst.Subjects.SubjectName,
                    Groups = gst.Groups.GroupName,
                    Hours = gst.HoursPerWeek
                }).ToList();
                TeacherSubjectsDataGrid.ItemsSource = teacherSubjects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTeacherSubjects();
        }
    }   
}
