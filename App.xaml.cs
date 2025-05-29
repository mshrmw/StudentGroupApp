using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StudentGroup
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int IdUser { get; set; }
        public static int? IdStudent { get; set; }
        public static int? IdTeacher { get; set; }
        public static int RoleId { get; set; }
    }
}
