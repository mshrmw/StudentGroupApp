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
    /// Логика взаимодействия для StudentSubjectsPage.xaml
    /// </summary>
    public partial class StudentSubjectsPage : Page
    {
        private int _studentId;
        public StudentSubjectsPage()
        {
            InitializeComponent();
            _studentId = (int)App.IdStudent;
            LoadSubjects();
        }
        private void LoadSubjects()
        {
            try
            {
                var db = StudyGroupEntities.GetContext();
                var student = db.Students.Find(_studentId);
                if (student?.GroupID == null)
                {
                    MessageBox.Show("Студент не принадлежит ни к одной группе", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                var subjects = (from gst in db.GroupSubjectTeacher join s in db.Subjects on gst.SubjectID equals s.SubjectID where gst.GroupID == student.GroupID select new
                {
                    SubjectName = s.SubjectName,
                    Hours = gst.HoursPerWeek,
                    Teacher = gst.Teachers
                }).ToList();
                var result = subjects.Select(x => new
                {
                    x.SubjectName,
                    Hours = x.Hours,
                    TeacherFullName = x.Teacher != null ? $"{x.Teacher.LastName} {x.Teacher.FirstName} {x.Teacher.MiddleName}" : "Не назначен"
                }).ToList();
                SubjectsDataGrid.ItemsSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке предметов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

