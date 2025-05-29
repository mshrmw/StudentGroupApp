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
    /// Логика взаимодействия для RegForTeacher.xaml
    /// </summary>
    public partial class RegForTeacher : Page
    {
        private MainWindow _mainWindow;
        public RegForTeacher(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
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
        private bool IsValidPhoneNumber(string phone)
        {
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            return (digitsOnly.Length == 11 && (digitsOnly[0] == '7' || digitsOnly[0] == '8'));
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
            string lastName = LastName.Text;
            string firstName = FirstName.Text;
            string middleName = MiddleName.Text;
            string email = Email.Text;
            string phone = Phone.Text;
            string specialization = Specialization.Text;
            DateTime? dateOfBirth = DateOfBirth.SelectedDate;
            string login = Login.Text;
            string password = Password.Password;
            string passwordConfirm = PasswordTest.Password;
            if (string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(specialization) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(passwordConfirm) ||
                dateOfBirth == null)
            {
                MessageBox.Show("Все обязательные поля должны быть заполнены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (login.Length < 2 || login.Length > 50)
            {
                MessageBox.Show("Логин должен быть от 2 до 50 символов");
                return;
            }
            if (password.Length < 8 || password.Length > 25)
            {
                MessageBox.Show("Пароль должен быть от 8 до 25 символов");
                return;
            }
            if (!ValidatePassword(password))
            {
                return;
            }
            if (password != passwordConfirm)
            {
                MessageBox.Show("Пароли не совпадают");
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
                MessageBox.Show("Номер телефона должен содержать 11 цифр");
                return;
            }
            if (dateOfBirth >= DateTime.Today)
            {
                MessageBox.Show("Дата рождения должна быть раньше текущей даты");
                return;
            }
            if (!IsRussianText(specialization))
            {
                MessageBox.Show("Специальность должна содержать только русские буквы");
                return;
            }
            try
            {
                using (var db = new StudyGroupEntities())
                {
                    if (db.Users.Any(u => u.Username == login))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует");
                        return;
                    }
                    if (db.Teachers.Any(t => t.Email == email))
                    {
                        MessageBox.Show("Преподаватель с таким email уже существует");
                        return;
                    }
                    var newUser = new Users
                    {
                        Username = login,
                        Password = GetHash(password), 
                        RoleID = 2
                    };
                    var newTeacher = new Teachers
                    {
                        UserID = newUser.UserID,
                        LastName = lastName,
                        FirstName = firstName,
                        MiddleName = middleName,
                        Email = email,
                        Phone = phone,
                        Specialization = specialization,
                        BirthDate = dateOfBirth.Value
                    };
                    db.Users.Add(newUser);
                    db.Teachers.Add(newTeacher);
                    db.SaveChanges();
                    MessageBox.Show("Регистрация преподавателя прошла успешно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
