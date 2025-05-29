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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AdminGroupsPage());
        }

        private void SubjectsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AdminSubjectsPage());
        }

        private void TeachersButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AdminTeacherAssignmentPage());
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
