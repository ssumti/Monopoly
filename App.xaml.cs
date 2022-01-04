using Monopoly.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Monopoly
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool Authentication = false;
        private static string Username;
        public static string username
        {
            get => Username;
            set
            {
                Username = value;
            }
        }
    }
}
