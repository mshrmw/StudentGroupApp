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
    /// Логика взаимодействия для StudentProfilePage.xaml
    /// </summary>
    public partial class StudentProfilePage : Page
    {
        private int _studentId;
        public StudentProfilePage()
        {
            InitializeComponent();
            _studentId = (int)App.IdStudent;
            LoadStudentProfile();
        }
        private void LoadStudentProfile()
        {
            var db = StudyGroupEntities.GetContext();
            var student = db.Students.Find(_studentId);
            if (student != null)
            {
                LastNameTextBox.Text = student.LastName;
                FirstNameTextBox.Text = student.FirstName;
                MiddleNameTextBox.Text = student.MiddleName;
                if (student.BirthDate.HasValue)
                {
                    BirthDatePicker.SelectedDate = student.BirthDate.Value;
                }
                if (student.GroupID != null)
                {
                    var group = db.Groups.Find(student.GroupID.Value);
                    GroupTextBox.Text = group?.GroupName ?? "Не указана";
                }
                else
                {
                    GroupTextBox.Text = "Не указана";
                }
                EmailTextBox.Text = student.Email;
                PhoneTextBox.Text = student.Phone;
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
        private bool IsValidPhoneNumber(string phone)
        {
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
            return digitsOnly.Length == 11;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string lastName = LastNameTextBox.Text;
            string firstName = FirstNameTextBox.Text;
            string middleName = MiddleNameTextBox.Text;
            DateTime? birthDate = BirthDatePicker.SelectedDate;
            string email = EmailTextBox.Text;
            string phone = PhoneTextBox.Text;
            if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show("Фамилия и имя должны быть заполнены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!IsRussianText(lastName) || !IsRussianText(firstName) || (!string.IsNullOrWhiteSpace(middleName) && !IsRussianText(middleName)))
            {
                MessageBox.Show("ФИО должно содержать только русские буквы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (birthDate > DateTime.Today)
            {
                MessageBox.Show("Дата рождения не может быть в будущем", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Неверный формат email", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!IsValidPhoneNumber(phone))
            {
                MessageBox.Show("Номер телефона должен содержать 11 цифр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var db = StudyGroupEntities.GetContext();
            var student = db.Students.Find(_studentId);
            if (student != null)
            {
                if (db.Students.Any(s => s.Email == email && s.StudentID != _studentId))
                {
                    MessageBox.Show("Пользователь с таким email уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                student.LastName = lastName;
                student.FirstName = firstName;
                student.MiddleName = middleName;
                student.BirthDate = birthDate;
                student.Email = email;
                student.Phone = phone;
                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
