using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows;
using Monopoly.Views;

namespace Monopoly.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        string username;
        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnChanged();
            }
        }
        string password;
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnChanged();
            }
        }
        private ICommand loginPressed;
        public ICommand LoginPressed
        {
            get
            {
                if (loginPressed == null)
                    loginPressed = new ButtonPressedWithParam((param)=>onClick(param));
                return loginPressed;
            }
        }
        private async void onClick(object param)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = await client.GetStringAsync(Instances.Url + "users/authentication/" + Username + "/" + Password);
                bool check = JsonConvert.DeserializeObject<bool>(content);
                if (!check)
                {
                    MessageBox.Show("Wrong Username or Password!", "Notification!~");
                }
                else
                {
                    App.username = Username;
                    App.Authentication = true;
                    Login login = param as Login;
                    login.Close();
                    Options option = new Options();
                    option.ShowDialog();
                }
                
            }
        }
    }
}
