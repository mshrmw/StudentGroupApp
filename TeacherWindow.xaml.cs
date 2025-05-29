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
using System.Windows.Shapes;

namespace StudentGroup
{
    /// <summary>
    /// Логика взаимодействия для TeacherWindow.xaml
    /// </summary>
    public partial class TeacherWindow : Window
    {
        private int _teacherId;
        public TeacherWindow()
        {
            InitializeComponent();
            _teacherId = (int)App.IdTeacher;
            LoadTeacherData();
        }
        private void LoadTeacherData()
        {
            var db = StudyGroupEntities.GetContext();
            var teacher = db.Teachers.Find(_teacherId);
            if (teacher != null)
            {
                TeacherNameText.Text = $"{teacher.LastName} {teacher.FirstName} {teacher.MiddleName}";
            }
        }
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new TeacherProfilePage());
        }

        private void GradesButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new TeacherGradesPage());
        }

        private void SubjectsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new TeacherSubjectsPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                mainWindow.mainFrame.Navigate(new Autorization(mainWindow));
                this.Close();
            }
        }
    }
}
