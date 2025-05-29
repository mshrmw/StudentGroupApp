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
    /// Главное окно приложения, обеспечивающее навигацию между страницами авторизации и регистрации
    /// </summary>
    /// <remarks>
    /// Содержит Frame для навигации и кнопки для перехода между основными страницами приложения
    /// </remarks>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Инициализирует новый экземпляр главного окна приложения
        /// </summary>
        /// <remarks>
        /// При создании автоматически перенаправляет на страницу авторизации
        /// </remarks>
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new Autorization(this));
        }
        /// <summary>
        /// Обработчик нажатия кнопки перехода на страницу авторизации
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие (кнопка Auth)</param>
        /// <param name="e">Аргументы события нажатия</param>
        private void Auth_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new Autorization(this));
        }
        /// <summary>
        /// Обработчик нажатия кнопки перехода на страницу регистрации преподавателей
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие (кнопка RegForTeacher)</param>
        /// <param name="e">Аргументы события нажатия</param>
        private void RegForTeacher_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new RegForTeacher(this));
        }
        /// <summary>
        /// Обработчик нажатия кнопки перехода на страницу регистрации студентов
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие (кнопка RegForStudents)</param>
        /// <param name="e">Аргументы события нажатия</param>
        private void RegForStudents_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new RegForStudents(this));

        }
    }
}
