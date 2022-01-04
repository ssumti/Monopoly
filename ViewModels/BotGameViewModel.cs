using Monopoly.Models;
using Monopoly.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Monopoly.ViewModels
{
    public class BotGameViewModel : INotifyPropertyChanged
    {
        Player pl = new Player();
        public BotGameViewModel()
        {
            // API get Player
            //Players.Add(new Player()
            //{
            //    Username = "ssumti",
            //    Password = "123456",
            //    Avatar = "Resources/dog.jpg"
            //});
            prepare(App.username);
        }
        private async void prepare(string user)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = await client.GetStringAsync(Instances.Url + "users/" + user);
                pl = JsonConvert.DeserializeObject<Player>(content);
            }
            Players.Add(pl);
            CreateBot();
            ProcessingList();
            ThisPlayer = Players.First();
            Bot1 = Players[1];
            Bot2 = Players[2];
            Bot3 = Players[3];
        }
        private void ProcessingList()
        {
            foreach (var player in Players)
            {
                player.Avatar = Path.GetFullPath(player.Avatar);
            }
        }

        private void CreateBot()
        {
            Players.Add(new Player()
            {
                Avatar = "Resources/bot.jpg",
                Username = "Bot 1",
            });
            Players.Add(new Player()
            {
                Avatar = "Resources/bot.jpg",
                Username = "Bot 2",
            });
            Players.Add(new Player()
            {
                Avatar = "Resources/bot.jpg",
                Username = "Bot 3",
            });

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        //private Message recentMessage;
        //public Message RecentMessage
        //{
        //    get => recentMessage;
        //    set
        //    {
        //        recentMessage = value;
        //        OnChanged();
        //    }
        //}
        private string modeName = "Bot Mode";
        public string ModeName
        {
            get => modeName;
            set
            {
                modeName = value;
                OnChanged();
            }
        }
        private List<Player> players = new List<Player>();
        public List<Player> Players
        {
            get => players;
            set
            {
                players = value;
                OnChanged();
            }
        }
        private Player thisPlayer;
        public Player ThisPlayer
        {
            get => thisPlayer;
            set
            {
                thisPlayer = value;
                OnChanged();
            }
        }
        private Player bot1;
        public Player Bot1
        {
            get => bot1;
            set
            {
                bot1 = value;
                OnChanged();
            }
        }
        private Player bot2;
        public Player Bot2
        {
            get => bot2;
            set
            {
                bot2 = value;
                OnChanged();
            }
        }
        private Player bot3;
        public Player Bot3
        {
            get => bot3;
            set
            {
                bot3 = value;
                OnChanged();
            }
        }
        private ICommand backPressed;
        public ICommand BackPressed
        {
            get
            {
                if (backPressed == null)
                {
                    backPressed = new ButtonPressedWithParam((param) => BackClick(param));
                }
                return backPressed;
            }
        }

        private void BackClick(object param)
        {
            BotGame game = param as BotGame;
            game.Close();
            Options op = new Options();
            op.ShowDialog();
        }

        private ICommand startPressed;
        public ICommand StartPressed
        {
            get
            {
                if (startPressed == null)
                {
                    startPressed = new ButtonPressedWithParam((param) => StartClick(param));
                }
                return startPressed;
            }
        }

        private void StartClick(object param)
        {
            BotGame lobby = param as BotGame;
            GameVsBot game = new GameVsBot(Players);
            game.ShowDialog();
            lobby.Close();
        }

        //private List<Message> messages = new List<Message>();
        //public List<Message> Messages
        //{
        //    get => messages;
        //    set
        //    {
        //        messages = value;
        //        OnChanged();
        //    }
        //}
    }
}
