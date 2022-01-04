using Monopoly.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Monopoly.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private ICommand botModeClicked;
        public ICommand BotModeClicked
        {
            get
            {
                if (botModeClicked == null)
                    botModeClicked = new ButtonPressedWithParam((param) => botClick(param));
                return botModeClicked;
            }
        }

        private void botClick(object param)
        {
            BotGame game = new BotGame();
            Options option = param as Options;
            option.Close();
            game.ShowDialog();
        }

        private ICommand pvpModeClicked;
        public ICommand PvpModeClicked
        {
            get
            {
                if (pvpModeClicked == null)
                    pvpModeClicked = new ButtonPressed(() => pvpClick());
                return pvpModeClicked;
            }
        }

        private void pvpClick()
        {
            //Lobby lobby = new Lobby();
        }
    }
    
}
