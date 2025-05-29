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
    /// Логика взаимодействия для StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        private int _studentId;
        public StudentWindow()
        {
            InitializeComponent();
            _studentId = (int)App.IdStudent;
            LoadStudentData();
        }
        private void LoadStudentData()
        {
            var db = StudyGroupEntities.GetContext();
            var student = db.Students.Find(_studentId);
            if (student != null)
            {
                StudentNameText.Text = $"{student.LastName} {student.FirstName} {student.MiddleName}";
            }
        }
    
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new StudentProfilePage());
        }

        private void GradesButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new StudentGradesPage());
        }

        private void SubjectsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new StudentSubjectsPage());
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
