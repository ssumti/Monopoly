using Monopoly.Models;
using Monopoly.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Monopoly.ViewModels
{
    public class GameWithBotViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #region Map positioning
        public double PlayerDisplayWidth
        {
            get => width - PlayerDisplay.LeftMap;
        }
        public MapPosition ChancesBox
        {
            get => new MapPosition()
            {
                TopMap = TopLeft.TopMap + 120,
                LeftMap = TopLeft.LeftMap + 120,
            };
        }
        public MapPosition RewardBox
        {
            get => new MapPosition()
            {
                TopMap = BottomRight.TopMap - 180,
                LeftMap = BottomRight.LeftMap - 180,
            };
        }
        public MapPosition Dice
        {
            get => new MapPosition()
            {
                TopMap = (height / 2) - 60,
                LeftMap = (width / 2) - 60,
            };
        }
        public MapPosition PlayerDisplay
        {
            get => new MapPosition()
            {
                TopMap = TopRight.TopMap,
                LeftMap = TopRight.LeftMap + 120,
            };
        }
        public MapPosition TopLeft
        {
            get => new MapPosition()
            {
                TopMap = ((height - 830) / 2),
                LeftMap = ((width - 830) / 2) - 50,
            };
        }

        public MapPosition TopRight
        {
            get => new MapPosition()
            {
                TopMap = TopLeft.TopMap,
                LeftMap = TopLeft.LeftMap + 100 + 70 * 9,
            };
        }
        public MapPosition BottomRight
        {
            get => new MapPosition()
            {
                TopMap = TopRight.TopMap + 100 + 70 * 9,
                LeftMap = TopRight.LeftMap,
            };
        }
        public MapPosition BottomLeft
        {
            get => new MapPosition()
            {
                TopMap = BottomRight.TopMap,
                LeftMap = TopLeft.LeftMap,
            };
        }

        private void InitiateMap()
        {
            InitiateTop();
            InitiateRight();
            InitiateBottom();
            InitiateLeft();
        }

        private void InitiateBottom()
        {
            map.Add(BottomRight);
            map.Add(new MapPosition()
            {
                TopMap = BottomRight.TopMap,
                LeftMap = BottomRight.LeftMap - 70,
            });
            for (int i = 22; i < 30; i++)
            {
                map.Add(new MapPosition()
                {
                    TopMap = BottomRight.TopMap,
                    LeftMap = map[i - 1].LeftMap - 70,
                });
            }
        }

        private void InitiateRight()
        {
            map.Add(TopRight);
            map.Add(new MapPosition()
            {
                TopMap = TopRight.TopMap + 100,
                LeftMap = TopRight.LeftMap,
            });
            for (int i = 12; i <= 19; i++)
            {
                map.Add(new MapPosition()
                {
                    TopMap = map[i - 1].TopMap + 70,
                    LeftMap = TopRight.LeftMap,
                });
            }
        }

        private void InitiateLeft()
        {
            map.Add(BottomLeft);
            map.Add(new MapPosition()
            {
                TopMap = BottomLeft.TopMap - 70,
                LeftMap = BottomLeft.LeftMap,
            });
            for (int i = 32; i < 40; i++)
            {
                map.Add(new MapPosition()
                {
                    TopMap = map[i - 1].TopMap - 70,
                    LeftMap = BottomLeft.LeftMap,
                });
            }
        }

        private void InitiateTop()
        {
            map.Add(TopLeft);
            map.Add(new MapPosition()
            {
                TopMap = TopLeft.TopMap,
                LeftMap = TopLeft.LeftMap + 100,
            });
            for (int i = 2; i <= 9; i++)
            {
                map.Add(new MapPosition()
                {
                    TopMap = TopLeft.TopMap,
                    LeftMap = map[i - 1].LeftMap + 70,
                });
            }
        }
        #endregion

        private double width = SystemParameters.PrimaryScreenWidth;
        private double height = SystemParameters.PrimaryScreenHeight;
        private Thread gameThread;
        private int steps;
        private int outPlayer = 0;
        public int Steps
        {
            get => steps;
            set
            {
                steps = value;
                OnChanged();
            }
        }

        private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static DispatcherTimer countdownTimer = new DispatcherTimer();
        //private static DispatcherTimer gameTimer = new DispatcherTimer();
        private static DispatcherTimer moveTimer = new DispatcherTimer();
        private static DispatcherTimer stopTimer = new DispatcherTimer();
        private List<Player> players;
        private Random random = new Random();
        private InGamePlayer CurrentPlayer;
        public InGamePlayer currentPlayer 
        { 
            get => CurrentPlayer; 
            set
            {
                CurrentPlayer = value;
                OnChanged();
            } 
        }
        private int playerClickInTurn;
        private string diceResult;
        public string DiceResult
        {
            get => diceResult;
            set
            {
                diceResult = value;
                OnChanged();
            }
        }
        private int countdown;
        public int CountDown
        {
            get => countdown;
            set
            {
                countdown = value;
                OnChanged();
            }
        }
        private string playerTurn;
        public string PlayerTurn
        {
            get => playerTurn;
            set
            {
                playerTurn = value;
                OnChanged();
            }
        }
        private ObservableCollection<InGamePlayer> ingames ;
        public ObservableCollection<InGamePlayer> InGames
        {
            get => ingames;
            set
            {
                ingames = value;
                OnChanged();
            }
        }

        private ObservableCollection<MapTiles> mapTiles;
        public ObservableCollection<MapTiles> MapTile
        {
            get => mapTiles;
            set
            {
                mapTiles = value;
                OnChanged();
            }
        }
        private ObservableCollection<string> gameMessages;
        public ObservableCollection<string> GameMessages
        {
            get => gameMessages;
            set
            {
                gameMessages = value;
                OnChanged();
            }
        }
        private ObservableCollection<Chance> chanceCards;
        public ObservableCollection<Chance> ChanceCards
        {
            get => chanceCards;
            set
            {
                chanceCards = value;
                OnChanged();
            }
        }
        private ObservableCollection<Reward> rewardCards;
        public ObservableCollection<Reward> RewardCards
        {
            get => rewardCards;
            set
            {
                rewardCards = value;
                OnChanged();
            }
        }
        private void CreateCards()
        {

            RewardCards.Add(new Reward()
            {
                RewardPrice = 300,
                Content = "300$ just fly down from the sky",
            });

            RewardCards.Add(new Reward()
            {
                RewardPrice = 400,
                Content = "400$ just fly down from the sky",
            });

            RewardCards.Add(new Reward()
            {
                RewardPrice = 100,
                Content = "100$ just fly down from the sky",
            });
            RewardCards.Add(new Reward()
            {
                RewardPrice = 300,
                Content = "300$ just fly down from the sky",
            });

            RewardCards.Add(new Reward()
            {
                RewardPrice = 400,
                Content = "400$ just fly down from the sky",
            });

            RewardCards.Add(new Reward()
            {
                RewardPrice = 100,
                Content = "100$ just fly down from the sky",
            });

            RewardCards.Add(new Reward()
            {
                RewardPrice = 500,
                Content = "A little bit generous from Game Master!",
            });

            RewardCards.Add(new Reward()
            {
                RewardPrice = 800,
                Content = "A bit generous from Game Master!",
            });

            RewardCards.Add(new Reward()
            {
                RewardPrice = 1500,
                Content = "Generous from Game Master!",
            });


            ChanceCards.Add(new Chance()
            {
                EventPrice = 200,
                Content = "Today is your birthday, and you get 200$",
            });
            ChanceCards.Add(new Chance()
            {
                EventPrice = 200,
                Content = "You got a promotion, and you get 200$",
            });
            ChanceCards.Add(new Chance()
            {
                EventPrice = 500,
                Content = "OH wow, you won a lottery ticket, you earn 500",
            });
            ChanceCards.Add(new Chance()
            {
                EventPrice = 1000,
                Content = "You help your boss's wife... , she gave you 1000$",
            });
            ChanceCards.Add(new Chance()
            {
                EventPrice = 1000,
                Content = "A robber gave you 1000$ for not telling the cops",
            });
            ChanceCards.Add(new Chance()
            {
                EventPrice = -800,
                Content = "Your boss find out you have done dirty thing to his wife",
            });
            ChanceCards.Add(new Chance()
            {
                EventPrice = -200,
                Content = "You got caught by a cop",
            });
            ChanceCards.Add(new Chance()
            {
                EventPrice = -100,
                Content = "Your friend invite to his birthday party, 100$ as celebrated money",
            });
        }

        private void ShuffleCards()
        {
            Random rng = new Random();
            int n = ChanceCards.Count;
            for (int i = n - 1; i >= 0; --i)
            {
                var temp = ChanceCards[i];
                var rNumber = rng.Next(0, n - 1);
                ChanceCards[i] = ChanceCards[rNumber];
                ChanceCards[rNumber] = temp;
            }
            n = RewardCards.Count;
            for (int i = n - 1; i >= 0; --i)
            {
                var temp = RewardCards[i];
                var rNumber = rng.Next(0, n - 1);
                RewardCards[i] = RewardCards[rNumber];
                RewardCards[rNumber] = temp;
            }
        }

        private void CreateRealEstate()
        {
            MapTile.Add(new MapTiles()
            {
                Type = "Begin",
            });     //0
            MapTile.Add(new MapTiles()  //1
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 60,
                    GuestRented = new ObservableCollection<double>()
                    {
                        2,
                        10,
                        30,
                        90,
                        160,
                        250,
                    },
                    Level = 0,
                    Name = "Saint St",
                    
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Reward",
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 60,
                    GuestRented = new ObservableCollection<double>()
                    {
                        4,
                        20  ,
                        660 ,
                        180 ,
                        320 ,
                        450 ,
                    },
                    Level = 0,
                    Name = "2Women St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Tax",
                tax = new Tax()
                {
                    Content = "Income Tax",
                    TaxMoney = 200,
                },
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Station",
                realEstate = new RealEstate()
                {
                    BuyPrice = 200,
                    Level = 0,
                    Name = "City Station",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 100,
                    GuestRented = new ObservableCollection<double>()
                    {
                        6   ,
                        30  ,
                        90  ,
                        270 ,
                        400 ,
                        550 ,
                    },
                    Level = 0,
                    Name = "Billton St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Chance",
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 100,
                    GuestRented = new ObservableCollection<double>()
                    {
                        6   ,
                        30  ,
                        90  ,
                        270 ,
                        400 ,
                        550 ,
                    },
                    Level = 0,
                    Name = "SungSa St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 120,
                    GuestRented = new ObservableCollection<double>()
                    {
                        8   ,
                        40  ,
                        100 ,
                        300 ,
                        450 ,
                        600 ,

                    },
                    Level = 0,
                    Name = "Millt St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Jail",
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 140,
                    GuestRented = new ObservableCollection<double>()
                    {
                        10  ,
                        50  ,
                        150 ,
                        450 ,
                        625 ,
                        750 ,

                    },
                    Level = 0,
                    Name = "Pall St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Tax",
                tax = new Tax()
                {
                    Content = "Electric Bill",
                    TaxMoney = 150,
                },
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 140,
                    GuestRented = new ObservableCollection<double>()
                    {
                        10  ,
                        50  ,
                        150 ,
                        450 ,
                        625 ,
                        750 ,

                    },
                    Level = 0,
                    Name = "Black St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 160,
                    GuestRented = new ObservableCollection<double>()
                    {
                        12  ,
                        60  ,
                        180 ,
                        500 ,
                        700 ,
                        900 ,

                    },
                    Level = 0,
                    Name = "Black Revenue",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Station",
                realEstate = new RealEstate()
                {
                    BuyPrice = 200,
                    Level = 0,
                    Name = "Black Station",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 180,
                    GuestRented = new ObservableCollection<double>()
                    {
                        14  ,
                        70  ,
                        200 ,
                        550 ,
                        750 ,
                        950 ,

                    },
                    Level = 0,
                    Name = "Boiz St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Reward",
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 180,
                    GuestRented = new ObservableCollection<double>()
                    {
                       14   ,
                        70  ,
                        200 ,
                        550 ,
                        750 ,
                        950 ,

                    },
                    Level = 0,
                    Name = "Girly St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 200,
                    GuestRented = new ObservableCollection<double>()
                    {
                        16  ,
                        80  ,
                        220 ,
                        600 ,
                        800 ,
                        1000    ,

                    },
                    Level = 0,
                    Name = "Mom St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Tax",
                tax = new Tax()
                {
                    Content = "Free Parking",
                    TaxMoney = 0,
                },
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 240,
                    GuestRented = new ObservableCollection<double>()
                    {
                        18  ,
                        90  ,
                        250 ,
                        700 ,
                        875 ,
                        1050    ,

                    },
                    Level = 0,
                    Name = "Stand St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles() //22
            {
                Type = "Chance",
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 240,
                    GuestRented = new ObservableCollection<double>()
                    {
                        18  ,
                        90  ,
                        250 ,
                        700 ,
                        875 ,
                        1050    ,

                    },
                    Level = 0,
                    Name = "Sit St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 240,
                    GuestRented = new ObservableCollection<double>()
                    {
                       20   ,
                        100 ,
                        300 ,
                        750 ,
                        925 ,
                        1100    ,

                    },
                    Level = 0,
                    Name = "Mor St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Station",
                realEstate = new RealEstate()
                {
                    BuyPrice = 200,
                    Level = 0,
                    Name = "White Station",
                },
                Owner = "",
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 260,
                    GuestRented = new ObservableCollection<double>()
                    {
                       22   ,
                        110 ,
                        330 ,
                        800 ,
                        975 ,
                        1150    ,

                    },
                    Level = 0,
                    Name = "Sleep St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 260,
                    GuestRented = new ObservableCollection<double>()
                    {
                        22  ,
                        110 ,
                        330 ,
                        800 ,
                        975 ,
                        1150    ,

                    },
                    Level = 0,
                    Name = "Awake St",
                },
                Owner = "",
                OwnerPos = -1,
            });

            MapTile.Add(new MapTiles()
            {
                Type = "Tax",
                tax = new Tax()
                {
                    Content = "Water Bill",
                    TaxMoney = 180,
                },
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 280,
                    GuestRented = new ObservableCollection<double>()
                    {
                        24  ,
                        120 ,
                        360 ,
                        850 ,
                        1025    ,
                        1200    ,

                    },
                    Level = 0,
                    Name = "ABC St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Jail",
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 300,
                    GuestRented = new ObservableCollection<double>()
                    {
                        26  ,
                        130 ,
                        390 ,
                        900 ,
                        1100    ,
                        1275    ,

                    },
                    Level = 0,
                    Name = "XYZ St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 300,
                    GuestRented = new ObservableCollection<double>()
                    {
                        26  ,
                        130 ,
                        390 ,
                        900 ,
                        1100    ,
                        1275    ,

                    },
                    Level = 0,
                    Name = "Alpha St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Reward",
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 320,
                    GuestRented = new ObservableCollection<double>()
                    {
                        28  ,
                        150 ,
                        450 ,
                        1000    ,
                        1200    ,
                        1400    ,

                    },
                    Level = 0,
                    Name = "Beta St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Station",
                realEstate = new RealEstate()
                {
                    BuyPrice = 200,
                    Level = 0,
                    Name = "Real Station",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Chance",
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 350,
                    GuestRented = new ObservableCollection<double>()
                    {
                        35  ,
                        175 ,
                        500 ,
                        1100    ,
                        1300    ,
                        1500    ,

                    },
                    Level = 0,
                    Name = "Omnicron St",
                },
                Owner = "",
                OwnerPos = -1,
            });
            MapTile.Add(new MapTiles()
            {
                Type = "Tax",
                tax = new Tax()
                {
                    Content = "Super Tax",
                    TaxMoney = 100,
                },
            });
            MapTile.Add(new MapTiles()
            {
                Type = "RealEstate",
                realEstate = new RealEstate()
                {
                    BuyPrice = 400,
                    GuestRented = new ObservableCollection<double>()
                    {
                       50   ,
                        200 ,
                        600 ,
                        1400    ,
                        1700    ,
                        2000    ,

                    },
                    Level = 0,
                    Name = "Mega St",
                },
                Owner = "",
                OwnerPos = -1,
            });
        }

        public ObservableCollection<MapPosition> map { get; set; }
        public string ChancesBoxImg
        {
            get => Path.GetFullPath("Resources/chances.png");
        }
        public string RewardBoxImg
        {
            get => Path.GetFullPath("Resources/reward.png");
        }

        public string DiceImg
        {
            get => Path.GetFullPath("Resources/dice.png");
        }
        public GameWithBotViewModel(List<Player> players)
        {
            InGames = new ObservableCollection<InGamePlayer>();
            map = new ObservableCollection<MapPosition>();
            MapTile = new ObservableCollection<MapTiles>();
            RewardCards = new ObservableCollection<Reward>();
            ChanceCards = new ObservableCollection<Chance>();
            GameMessages = new ObservableCollection<string>()
            {
                "Let the game STARTED",
            };

            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            
            countdownTimer.Tick += TimerColapse;
            InitiateMap();
            CreateRealEstate();
            CreateCards();
            ShuffleCards();
            this.players = players;
            Transfer();
            Shuffle();
            ThreadStart ts = new ThreadStart(StartGameThread);
            gameThread = new Thread(ts);
            gameThread.Start();
        }
        #region Transfer player to ingame
        private void Transfer()
        {
            InGames.Add(new InGamePlayer()
            {
                Avatar = players[0].Avatar,
                CurrentPosition = 0,
                IsPlayer = true,
                Name = players[0].Username,
                NumberOfHotel = 0,
                Property = 1000,
                ListPosition = -1,
                TopMap = map[0].TopMap,
                LeftMap = map[0].LeftMap,
                OnGame = true,
                IsInJail = false,
                NumberOfStation = 0
            });
            for (int i = 1; i <= 3; ++i) 
            {
                InGames.Add(new InGamePlayer()
                {
                    Name = players[i].Username,
                    Avatar = players[i].Avatar,
                    CurrentPosition = 0,
                    Property = 1000,
                    NumberOfHotel = 0,
                    IsPlayer = false,
                    OnGame = true,
                    ListPosition = -1,
                    TopMap = map[0].TopMap,
                    LeftMap = map[0].LeftMap,
                    IsInJail = false,
                    NumberOfStation = 0
                });
            }
        }
        #endregion

        #region Shuffle Players
        private void Shuffle()
        {
            Random rng = new Random();
            int n = InGames.Count;
            for (int i = n-1; i >= 0; --i)
            {
                var temp = InGames[i];
                var rNumber = rng.Next(0, n-1);
                InGames[i] = InGames[rNumber];
                InGames[rNumber] = temp;
            }
            InGames[0].ListPosition = 0;
            InGames[1].ListPosition = 1;
            InGames[2].ListPosition = 2;
            InGames[3].ListPosition = 3;
        }
        #endregion
        #region Game region
        private void StartGameThread()
        {
            while (!WinCondition())
            {
                for (int i = 0; i < 4; i++)
                {
                    if (InGames[i].OnGame)
                    {
                        if (!InGames[i].IsInJail)
                        {
                            if (InGames[i].IsPlayer)
                            {
                                CountDown = 10;
                                countdownTimer.Start();
                                PlayerTurn = InGames[i].Name;
                                currentPlayer = InGames[i];
                                playerClickInTurn = 0;
                                ewh.WaitOne();
                                InGames[i] = currentPlayer;
                            }
                            else
                            {
                                CountDown = 10;
                                countdownTimer.Start();
                                PlayerTurn = InGames[i].Name;
                                currentPlayer = InGames[i];
                                playerClickInTurn = 0;
                                throwingDice();
                                ewh.WaitOne();
                                InGames[i] = currentPlayer;
                            }
                            Thread.Sleep(2000);
                            Steps = 0;
                        }
                        else
                        {
                            InGames[i].IsInJail = false;
                        }
                    }
                }
            }
        }
        private void TimerColapse(object sender, EventArgs e)
        {
            --CountDown;
            if (CountDown == -1)
            {
                countdownTimer.Stop();
                throwingDice();
                //ewh.Set();
            }
                
        }
        private ICommand throwDice;
        public ICommand ThrowDice
        {
            get
            {
                if (throwDice == null)
                {
                    throwDice = new ButtonPressed(() => throwingDice());
                }
                return throwDice;
            }
        }
        private void throwingDice()
        {
            ++playerClickInTurn;

            if (playerClickInTurn == 1)
            {
                int stepTemp = random.Next(1, 6);
                Thread.Sleep(500);
                Steps = stepTemp;
                #region Move
                if (currentPlayer.CurrentPosition + Steps >= 40)
                {
                    currentPlayer.CurrentPosition = (currentPlayer.CurrentPosition + Steps) % 10;
                    currentPlayer.TopMap = map[currentPlayer.CurrentPosition].TopMap;
                    currentPlayer.LeftMap = map[currentPlayer.CurrentPosition].LeftMap;
                }
                else
                {
                    currentPlayer.CurrentPosition = currentPlayer.CurrentPosition + Steps;
                    currentPlayer.TopMap = map[currentPlayer.CurrentPosition].TopMap;
                    currentPlayer.LeftMap = map[currentPlayer.CurrentPosition].LeftMap;
                }
                #endregion
                #region Process Type

                if (currentPlayer.IsPlayer)
                {
                    switch (MapTile[currentPlayer.CurrentPosition].Type)
                    {
                        case "RealEstate":
                            if (MapTile[currentPlayer.CurrentPosition].OwnerPos == -1)
                            {
                                if (currentPlayer.Property >= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice)
                                {
                                    var result = MessageBox.Show("Would you want to buy" + MapTile[currentPlayer.CurrentPosition].realEstate.Name + " ?", "Game Master", MessageBoxButton.YesNo);
                                    switch (result)
                                    {
                                        case MessageBoxResult.None:
                                            break;
                                        case MessageBoxResult.Yes:
                                            currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice;
                                            MapTiles tile = MapTile[currentPlayer.CurrentPosition];
                                            tile.OwnerPos = currentPlayer.ListPosition;
                                            tile.Owner = currentPlayer.Name;
                                            MapTile[currentPlayer.CurrentPosition] = tile;
                                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had bought " + MapTile[currentPlayer.CurrentPosition].realEstate.Name)));
                                            if (currentPlayer.Property <= 0)
                                            {
                                                currentPlayer.OnGame = false;
                                                ++outPlayer;
                                                Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                                            }
                                            break;
                                        case MessageBoxResult.No:
                                            break;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("You cannot buy" + MapTile[currentPlayer.CurrentPosition].realEstate.Name + " !", "Game Master", MessageBoxButton.OK);
                                }               
                            }
                            else
                            {
                                if (MapTile[currentPlayer.CurrentPosition].OwnerPos == currentPlayer.ListPosition)
                                {
                                    if (MapTile[currentPlayer.CurrentPosition].realEstate.Level < 5)
                                    {
                                        MapTile[currentPlayer.CurrentPosition].realEstate.Level++;
                                        Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had upgrade " + MapTile[currentPlayer.CurrentPosition].realEstate.Name + " to level " + MapTile[currentPlayer.CurrentPosition].realEstate.Level.ToString())));                                        
                                    }
                                }
                                else
                                {
                                    currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].realEstate.GuestRented[MapTile[currentPlayer.CurrentPosition].realEstate.Level];
                                    InGames[MapTile[currentPlayer.CurrentPosition].OwnerPos].Property += MapTile[currentPlayer.CurrentPosition].realEstate.GuestRented[MapTile[currentPlayer.CurrentPosition].realEstate.Level];
                                    Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had paid " + InGames[MapTile[currentPlayer.CurrentPosition].OwnerPos].Name + " " + MapTile[currentPlayer.CurrentPosition].realEstate.GuestRented[MapTile[currentPlayer.CurrentPosition].realEstate.Level].ToString() + "$")));
                                    if (currentPlayer.Property <= 0)
                                    {
                                        currentPlayer.OnGame = false;
                                        ++outPlayer;
                                        Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                                    }
                                }
                            }
                            break;
                        case "Tax":
                            currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].tax.TaxMoney;
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " have to pay " + MapTile[currentPlayer.CurrentPosition].tax.TaxMoney + "$ for tax")));
                            if (currentPlayer.Property <= 0)
                            {
                                currentPlayer.OnGame = false;
                                ++outPlayer;
                                Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                            }
                            break;
                        case "Jail":
                            currentPlayer.TopMap = map[10].TopMap;
                            currentPlayer.LeftMap = map[10].LeftMap;
                            currentPlayer.IsInJail = true;
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " is in jail")));
                            break;
                        case "Chance":
                            var playerChance = ChanceCards.First();
                            currentPlayer.Property += playerChance.EventPrice;
                            ChanceCards.Remove(playerChance);
                            ChanceCards.Add(playerChance);
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " : " + playerChance.Content)));
                            if (currentPlayer.Property <= 0)
                            {
                                currentPlayer.OnGame = false;
                                ++outPlayer;
                                Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                            }
                            break;
                        case "Reward":
                            var playerReward = RewardCards.First();
                            currentPlayer.Property += playerReward.RewardPrice;
                            RewardCards.Remove(playerReward);
                            RewardCards.Add(playerReward);
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " : " + playerReward.Content)));
                            break;
                        case "Begin":
                            currentPlayer.Property += 1000;
                            break;
                        case "Station":
                            if (MapTile[currentPlayer.CurrentPosition].OwnerPos == -1)
                            {
                                if (currentPlayer.Property >= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice)
                                {
                                    var result = MessageBox.Show("Would you want to buy" + MapTile[currentPlayer.CurrentPosition].realEstate.Name + " ?", "Game Master", MessageBoxButton.YesNo);
                                    switch (result)
                                    {
                                        case MessageBoxResult.None:
                                            break;
                                        case MessageBoxResult.Yes:
                                            currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice;
                                            MapTiles tile = MapTile[currentPlayer.CurrentPosition];
                                            tile.OwnerPos = currentPlayer.ListPosition;
                                            tile.Owner = currentPlayer.Name;
                                            MapTile[currentPlayer.CurrentPosition] = tile;
                                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>GameMessages.Add(currentPlayer.Name + " had bought " + MapTile[currentPlayer.CurrentPosition].realEstate.Name)));
                                            if (currentPlayer.Property <= 0)
                                            {
                                                currentPlayer.OnGame = false;
                                                ++outPlayer;
                                                Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                                            }
                                            break;
                                        case MessageBoxResult.No:
                                            break;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("You cannot buy" + MapTile[currentPlayer.CurrentPosition].realEstate.Name + " !", "Game Master", MessageBoxButton.OK);
                                }
                            }
                            else
                            {
                                currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice * InGames[MapTile[currentPlayer.CurrentPosition].OwnerPos].NumberOfStation;
                                Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had paid " + InGames[MapTile[currentPlayer.CurrentPosition].OwnerPos].Name + " " + MapTile[currentPlayer.CurrentPosition].realEstate.GuestRented[MapTile[currentPlayer.CurrentPosition].realEstate.Level].ToString() + "$")));
                                if (currentPlayer.Property <= 0)
                                {
                                    currentPlayer.OnGame = false;
                                    ++outPlayer;
                                    Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                                }
                            }
                            break;
                    } 
                }
                else
                {
                    switch (MapTile[currentPlayer.CurrentPosition].Type)
                    {
                        case "RealEstate":
                            if (MapTile[currentPlayer.CurrentPosition].OwnerPos == -1)
                            {
                                if (currentPlayer.Property >= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice)
                                {
                                    currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice;
                                    MapTiles tile = MapTile[currentPlayer.CurrentPosition];
                                    tile.OwnerPos = currentPlayer.ListPosition;
                                    tile.Owner = currentPlayer.Name;
                                    MapTile[currentPlayer.CurrentPosition] = tile;
                                    Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had bought " + MapTile[currentPlayer.CurrentPosition].realEstate.Name)));
                                    if (currentPlayer.Property <= 0)
                                    {
                                        currentPlayer.OnGame = false;
                                        ++outPlayer;
                                        Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                                    }
                                }
                            }
                            else
                            {
                                if (MapTile[currentPlayer.CurrentPosition].OwnerPos == currentPlayer.ListPosition)
                                {
                                    if (MapTile[currentPlayer.CurrentPosition].realEstate.Level < 5)
                                    {
                                        MapTile[currentPlayer.CurrentPosition].realEstate.Level++;
                                        Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had upgrade " + MapTile[currentPlayer.CurrentPosition].realEstate.Name + " to level " + MapTile[currentPlayer.CurrentPosition].realEstate.Level.ToString())));                                        
                                    }
                                }
                                else
                                {
                                    currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].realEstate.GuestRented[MapTile[currentPlayer.CurrentPosition].realEstate.Level];
                                    InGames[MapTile[currentPlayer.CurrentPosition].OwnerPos].Property += MapTile[currentPlayer.CurrentPosition].realEstate.GuestRented[MapTile[currentPlayer.CurrentPosition].realEstate.Level];
                                    Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had paid " + InGames[MapTile[currentPlayer.CurrentPosition].OwnerPos].Name + " " + MapTile[currentPlayer.CurrentPosition].realEstate.GuestRented[MapTile[currentPlayer.CurrentPosition].realEstate.Level].ToString() + "$")));
                                    if (currentPlayer.Property <= 0)
                                    {
                                        currentPlayer.OnGame = false;
                                        ++outPlayer;
                                        Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                                    }
                                }
                            }
                            break;
                        case "Tax":
                            currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].tax.TaxMoney;
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " have to pay " + MapTile[currentPlayer.CurrentPosition].tax.TaxMoney + "$ for tax")));
                            if (currentPlayer.Property <= 0)
                            {
                                currentPlayer.OnGame = false;
                                ++outPlayer;
                                Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                            }

                            break;
                        case "Jail":
                            currentPlayer.TopMap = map[10].TopMap;
                            currentPlayer.LeftMap = map[10].LeftMap;
                            currentPlayer.IsInJail = true;
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " is in jail")));
                            break;
                        case "Chance":
                            var playerChance = ChanceCards.First();
                            currentPlayer.Property += playerChance.EventPrice;
                            ChanceCards.Remove(playerChance);
                            ChanceCards.Add(playerChance);
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " : " + playerChance.Content)));
                            if (currentPlayer.Property <= 0)
                            {
                                currentPlayer.OnGame = false;
                                ++outPlayer;
                                Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                            }
                            break;
                        case "Reward":
                            var playerReward = RewardCards.First();
                            currentPlayer.Property += playerReward.RewardPrice;
                            RewardCards.Remove(playerReward);
                            RewardCards.Add(playerReward);
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " : " + playerReward.Content)));
                            break;
                        case "Begin":
                            currentPlayer.Property += 1000;
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " got a month salary")));
                            break;
                        case "Station":
                            if (MapTile[currentPlayer.CurrentPosition].OwnerPos == -1)
                            {
                                if (currentPlayer.Property >= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice)
                                {
                                    currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice;
                                    MapTiles tile = MapTile[currentPlayer.CurrentPosition];
                                    tile.OwnerPos = currentPlayer.ListPosition;
                                    tile.Owner = currentPlayer.Name;
                                    MapTile[currentPlayer.CurrentPosition] = tile;
                                    Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had bought " + MapTile[currentPlayer.CurrentPosition].realEstate.Name)));
                                }
                            }
                            else
                            {
                                currentPlayer.Property -= MapTile[currentPlayer.CurrentPosition].realEstate.BuyPrice * InGames[MapTile[currentPlayer.CurrentPosition].OwnerPos].NumberOfStation;
                                Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had paid " + InGames[MapTile[currentPlayer.CurrentPosition].OwnerPos].Name + " " + MapTile[currentPlayer.CurrentPosition].realEstate.GuestRented[MapTile[currentPlayer.CurrentPosition].realEstate.Level].ToString() + "$")));
                                if (currentPlayer.Property <=0 )
                                {
                                    currentPlayer.OnGame = false;
                                    ++outPlayer;
                                    Application.Current.Dispatcher.BeginInvoke(new Action(() => GameMessages.Add(currentPlayer.Name + " had been eliminate ")));
                                }
                            }
                            break;
                    }
                }
                #endregion
                ewh.Set();
            }
        }
        private bool WinCondition()
        {
            if (outPlayer!=3)
                return false;
            else
            {
                return true;
            }
        }
        #endregion
    }
}
