using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Page
    {
        private MainWindow _mainWindow;
        public Autorization(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String log = login.Text;
                String passwor = password.Password;
                if (string.IsNullOrEmpty(log) && string.IsNullOrEmpty(passwor))
                {
                    MessageBox.Show("Введите логин и пароль");
                    return;
                }
                if (string.IsNullOrEmpty(log))
                {
                    MessageBox.Show("Введите логин");
                    return;
                }
                if (string.IsNullOrEmpty(passwor))
                {
                    MessageBox.Show("Введите пароль");
                    return;
                }
                String pass = GetHash(passwor);
                using (var db = new StudyGroupEntities())
                {
                    var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Username == log && u.Password == pass);
                    if (user != null)
                    {
                        App.IdUser = user.UserID;
                        App.RoleId = user.RoleID;
                        if (user.RoleID == 2)
                        {
                            var teacher = db.Teachers.FirstOrDefault(t => t.UserID == user.UserID);
                            if (teacher != null)
                            {
                                App.IdTeacher = teacher.TeacherID;
                            }
                        }
                        else if (user.RoleID == 3)
                        {
                            var student = db.Students.FirstOrDefault(s => s.UserID == user.UserID);
                            if (student != null)
                            {
                                App.IdStudent = student.StudentID;
                            }
                        }
                        String stringRole = null;
                        switch (user.RoleID)
                        {
                            case 1:
                                AdminWindow adminWindow = new AdminWindow();
                                adminWindow.Show();
                                stringRole = "админ";
                                break;
                            case 2:
                                TeacherWindow teacherWindow = new TeacherWindow();
                                teacherWindow.Show();
                                stringRole = "преподаватель";
                                break;
                            case 3:
                                StudentWindow studenWindow = new StudentWindow();
                                studenWindow.Show();
                                stringRole = "студент";
                                break;
                        }
                        MessageBox.Show($"Успешный вход, {stringRole}!");
                        _mainWindow.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль");
                        return;
                    }
                }
            }
            catch (System.Data.Entity.Core.EntityException ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
                return;
            }
        }
    }
}
