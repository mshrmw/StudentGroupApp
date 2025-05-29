using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для RegForStudents.xaml
    /// </summary>
    public partial class RegForStudents : Page
    {
        private MainWindow _mainWindow;
        public RegForStudents(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadGroups();
        }
        private void LoadGroups()
        {
            try
            {
                using (var db = new StudyGroupEntities())
                {
                    Grup.ItemsSource = db.Groups.ToList();
                    Grup.DisplayMemberPath = "GroupName";
                    Grup.SelectedValuePath = "GroupID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}");
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private bool IsRussianText(string text)
        {
            foreach (char c in text)
            {
                if (!((c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я') || c == 'ё' || c == 'Ё' || c == ' ' || c == '-'))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool ValidatePassword(string password)
        {
            bool hasLetter = false;
            bool hasDigit = false;
            bool hasInvalidChar = false;
            foreach (char c in password)
            {
                if (char.IsLetter(c))
                {
                    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    {
                        hasLetter = true;
                    }
                    else
                    {
                        hasInvalidChar = true;
                    }
                }
                else if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else
                {
                    hasInvalidChar = true;
                }
            }
            if (hasInvalidChar)
            {
                MessageBox.Show("Пароль может содержать только английские буквы и цифры");
                return false;
            }
            if (!hasLetter)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну английскую букву");
                return false;
            }
            if (!hasDigit)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру");
                return false;
            }
            return true;
        }
        private bool IsValidPhoneNumber(string phone)
        {
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            return digitsOnly.Length == 11;
        }
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            String lastName = LastName.Text;
            String firstName = FirstName.Text;
            String middleName = MiddleName.Text;
            String email = Email.Text;
            String phone = Phone.Text;
            DateTime? dateOfBirth = DateOfBirth.SelectedDate;
            String log = Login.Text;
            String pass = Password.Password;
            String passProverka = PasswordTest.Password;
            if (string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(firstName) ||
                Grup.SelectedItem == null ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(log) ||
                string.IsNullOrWhiteSpace(pass) ||
                string.IsNullOrWhiteSpace(passProverka) ||
                dateOfBirth == null)
            {
                MessageBox.Show("Все обязательные поля должны быть заполнены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (log.Length > 50 || log.Length < 2)
            {
                MessageBox.Show("Логин должен быть от 2 до 50 символов");
                return;
            }
            if (pass.Length < 8 || pass.Length > 25)
            {
                MessageBox.Show("Пароль должен быть от 8 до 25 символов");
                return;
            }
            if (!ValidatePassword(pass))
            {
                return;
            }
            if (pass != passProverka)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }
            if (dateOfBirth >= DateTime.Today)
            {
                MessageBox.Show("Дата рождения должна быть раньше текущей даты");
                return;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Неверный формат email");
                return;
            }
            if (!IsRussianText(lastName) || !IsRussianText(firstName) || (!string.IsNullOrWhiteSpace(middleName) && !IsRussianText(middleName)))
            {
                MessageBox.Show("ФИО должно содержать только русские буквы");
                return;
            }
            if (!IsValidPhoneNumber(phone))
            {
                MessageBox.Show("Номер телефона должен содержать 11 цифр и начинаться с 8");
                return;
            }
            try
            {
                using (var db = new StudyGroupEntities())
                {
                    var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Username == log);
                    if (user != null)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует");
                        return;
                    }
                    var emaildb = db.Students.AsNoTracking().FirstOrDefault(s => s.Email == email);
                    if (emaildb != null)
                    {
                        MessageBox.Show("Пользователь с таким email уже существует");
                        return;
                    }
                    var newUser = new Users
                    {
                        Username = Login.Text,
                        Password = GetHash(pass),
                        RoleID = 3 
                    };
                    var newStudent = new Students
                    {
                        UserID = newUser.UserID,
                        LastName = LastName.Text,
                        FirstName = FirstName.Text,
                        MiddleName = MiddleName.Text,
                        Email = Email.Text,
                        Phone = Phone.Text,
                        BirthDate = DateOfBirth.SelectedDate.Value,
                        GroupID = (int)Grup.SelectedValue
                    };
                    db.Users.Add(newUser);
                    db.Students.Add(newStudent);
                    db.SaveChanges();
                    MessageBox.Show("Регистрация прошла успешно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _mainWindow.mainFrame.Navigate(new Autorization(_mainWindow));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
