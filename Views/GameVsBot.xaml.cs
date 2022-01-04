using Monopoly.Models;
using Monopoly.ViewModels;
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

namespace Monopoly.Views
{
    /// <summary>
    /// Interaction logic for GameVsBot.xaml
    /// </summary>
    public partial class GameVsBot : Window
    {
        private GameWithBotViewModel vm;
        public GameVsBot(List<Player> players)
        {
            InitializeComponent();
            DataContext = vm = new GameWithBotViewModel(players);
        }
    }
}
