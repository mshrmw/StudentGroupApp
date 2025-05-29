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
    /// Логика взаимодействия для TeacherProfilePage.xaml
    /// </summary>
    public partial class TeacherProfilePage : Page
    {
        private int _teacherId;
        public TeacherProfilePage()
        {
            InitializeComponent();
            _teacherId = (int)App.IdTeacher;
            LoadTeacherProfile();
        }
        private void LoadTeacherProfile()
        {
            var db = StudyGroupEntities.GetContext();
            var teacher = db.Teachers.Find(_teacherId);
            if (teacher != null)
            {
                LastNameTextBox.Text = teacher.LastName;
                FirstNameTextBox.Text = teacher.FirstName;
                MiddleNameTextBox.Text = teacher.MiddleName;
                if (teacher.BirthDate.HasValue)
                {
                    BirthDatePicker.SelectedDate = teacher.BirthDate.Value;
                }
                SpecializationTextBox.Text = teacher.Specialization;
                EmailTextBox.Text = teacher.Email;
                PhoneTextBox.Text = teacher.Phone;
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
            var teacher = db.Teachers.Find(_teacherId);
            if (teacher != null)
            {
                if (db.Teachers.Any(s => s.Email == email && s.TeacherID != _teacherId))
                {
                    MessageBox.Show("Пользователь с таким email уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                teacher.LastName = lastName;
                teacher.FirstName = firstName;
                teacher.MiddleName = middleName;
                teacher.BirthDate = birthDate;
                teacher.Email = email;
                teacher.Phone = phone;
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
