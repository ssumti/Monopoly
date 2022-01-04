using Monopoly.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Monopoly
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private ICommand playPressed;
        public ICommand PlayPressed
        {
            get
            {
                if (playPressed == null)
                    playPressed = new ButtonPressed(() => onClick());
                return playPressed;
            }
        }
        private void onClick()
        {
            Login login = new Login();
            login.ShowDialog();
        }
    }
}
