﻿using System;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(new Autorization(this));
        }

        private void Auth_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new Autorization(this));
        }

        private void RegForTeacher_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new RegForTeacher(this));
        }

        private void RegForStudents_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new RegForStudents(this));

        }
    }
}
