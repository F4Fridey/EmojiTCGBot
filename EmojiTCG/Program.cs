using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using EmojiTCG.Cards;
using EmojiTCG.Modules;
using System.Runtime.InteropServices;

namespace EmojiTCG
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            float tps = 5;
            tps = 1 / tps;
            bool succesLaunch = true;
            do
            {
                try
                {
                    new Program().RunBotAsync().GetAwaiter().GetResult();
                }
                catch
                {
                    succesLaunch = false;
                }
            } while (!succesLaunch);
        }

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        /*
        public Settings settings;
        public List<ServerData.ServerData> serverData = new List<ServerData.ServerData>();
        public List<Cards.Card> cards = new List<Card>();
        public List<Cards.Booster> boosters = new List<Booster>();
        public List<Badge> badges = new List<Badge>();
        */
        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            bool fileExists = File.Exists(@"Bot/Settings/settings.json");
            string _token = "";
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/Settings");
                File.Create(@"Bot/Settings/settings.json");
                Console.WriteLine("Error: settings.json file did not exist. Please close the bot and fill in info in settings.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/Settings/settings.json");
                if (json == "")
                {
                    CurrentData.settings = CreateNewSettings();
                    json = JsonConvert.SerializeObject(CurrentData.settings, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/settings.json", json);
                }
                else
                {
                    CurrentData.settings = JsonConvert.DeserializeObject<Settings>(json);
                }
                _token = CurrentData.settings.token;
            }
            fileExists = File.Exists(@"Bot/ServerData/serverData.json");
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/ServerData");
                File.Create(@"Bot/ServerData/serverData.json");
                Console.WriteLine("Error: serverData.json file did not exist. Please close the bot and fill in info in serverData.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/ServerData/serverData.json");
                if (json == "")
                {
                    CurrentData.serverData.Add(CreateNewServerData());
                    json = JsonConvert.SerializeObject(CurrentData.serverData, Formatting.Indented);
                    File.WriteAllText(@"Bot/ServerData/serverData.json", json);
                }
                else
                {
                    CurrentData.serverData = JsonConvert.DeserializeObject<List<ServerData.ServerData>>(json);
                }
            }
            fileExists = File.Exists(@"Bot/Settings/cards.json");
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/Settings");
                File.Create(@"Bot/Settings/cards.json");
                Console.WriteLine("Error: cards.json file did not exist. Please close the bot and fill in info in cards.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/Settings/cards.json");
                if (json == "")
                {
                    CurrentData.cards = CreateNewCards();
                    json = JsonConvert.SerializeObject(CurrentData.cards, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/cards.json", json);
                }
                else
                {
                    CurrentData.cards = JsonConvert.DeserializeObject<List<Cards.Card>>(json);
                }
            }
            fileExists = File.Exists(@"Bot/Settings/boosters.json");
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/Settings");
                File.Create(@"Bot/Settings/boosters.json");
                Console.WriteLine("Error: boosters.json file did not exist. Please close the bot and fill in info in boosters.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/Settings/boosters.json");
                if (json == "")
                {
                    CurrentData.boosters.Add(CreateNewBooster());
                    json = JsonConvert.SerializeObject(CurrentData.boosters, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/boosters.json", json);
                }
                else
                {
                    CurrentData.boosters = JsonConvert.DeserializeObject<List<Cards.Booster>>(json);
                }
            }
            fileExists = File.Exists(@"Bot/Settings/badge.json");
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/Settings");
                File.Create(@"Bot/Settings/badge.json");
                Console.WriteLine("Error: badge.json file did not exist. Please close the bot and fill in info in badge.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/Settings/badge.json");
                if (json == "")
                {
                    CurrentData.badges.Add(CreateNewBadge());
                    json = JsonConvert.SerializeObject(CurrentData.badges, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/badge.json", json);
                }
                else
                {
                    CurrentData.badges = JsonConvert.DeserializeObject<List<Badge>>(json);
                }
            }
            fileExists = File.Exists(@"Bot/Settings/shoppages.json");
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/Settings");
                File.Create(@"Bot/Settings/shoppages.json");
                Console.WriteLine("Error: shoppages.json file did not exist. Please close the bot and fill in info in shoppages.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/Settings/shoppages.json");
                if (json == "")
                {
                    Shop.shopPages.Add(CreateNewShopPage(CurrentData.boosters[0], CurrentData.cards[0], CurrentData.badges[0]));
                    json = JsonConvert.SerializeObject(Shop.shopPages, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/shoppages.json", json);
                }
                else
                {
                    Shop.shopPages = JsonConvert.DeserializeObject<List<ShopPage>>(json);
                }
            }
            fileExists = File.Exists(@"Bot/Settings/dailymonthprizes.json");
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/Settings");
                File.Create(@"Bot/Settings/dailymonthprizes.json");
                Console.WriteLine("Error: shopdailymonthprizespages.json file did not exist. Please close the bot and fill in info in dailymonthprizes.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/Settings/dailymonthprizes.json");
                if (json == "")
                {
                    CurrentData.dailyMonthPrizes.Add(CreateNewDailyMonthPrize());
                    json = JsonConvert.SerializeObject(CurrentData.dailyMonthPrizes, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/dailymonthprizes.json", json);
                }
                else
                {
                    CurrentData.dailyMonthPrizes = JsonConvert.DeserializeObject<List<DailyMonthPrizes>>(json);
                }
            }

            string token = _token;

            _client.Log += _client_Log;

            await RegisterCommandsAsync();

            _client.UserVoiceStateUpdated += HandleVoiceEventsAsync;

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            Functions.LoadAllData();

            await _client.SetStatusAsync(UserStatus.Online);
            await _client.SetGameAsync("for =help", null, ActivityType.Watching);

            CurrentData.client = _client;

            for (int i = 0; i < CurrentData.serverData.Count; i++)
            {
                for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                {
                    CurrentData.serverData[i].userData[j].lastJoinVCTime = DateTime.Now.Ticks;
                }
            }

            await Task.Delay(-1);
        }

        DailyMonthPrizes CreateNewDailyMonthPrize()
        {
            DailyMonthPrizes dailyMonthPrizes = new DailyMonthPrizes()
            {
                name = "Test Month Prizes",
                prizesPerDay = new List<Prize>()
                {
                    new Prize()
                    {
                        name = "Day 1",
                        prizeType = PrizeType.NOTHING,
                        emoji = ":black_circle:",
                        itemId = 0,
                        coinAmount = 0
                    }
                },
                month = 0
            };
            return dailyMonthPrizes;
        }

        ShopPage CreateNewShopPage(Booster booster, Card card, Badge badge)
        {
            ShopPage shopPage = new ShopPage()
            {
                category = ShopCategory.PINNED,
                topThreeSlots = new List<ShopSlot>()
                {
                    new ShopSlot
                    {
                        itemId = booster.id,
                        type = ShopSlotType.BOOSTER,
                        stock = 4000000000,
                        price = 10,
                        name = "Test Booster Pack",
                        id = 0
                    },
                    new ShopSlot
                    {
                        itemId = card.id,
                        type = ShopSlotType.CARD,
                        stock = 4000000000,
                        price = 15,
                        name = "Testitator",
                        id = 1
                    },
                    new ShopSlot
                    {
                        itemId = badge.id,
                        type = ShopSlotType.BADGE,
                        stock = 4000000000,
                        price = 20,
                        name = "Test Badge",
                        id = 2
                    }
                },
                slots = new List<ShopSlot>()
                {
                    new ShopSlot
                    {
                        itemId = booster.id,
                        type = ShopSlotType.BOOSTER,
                        stock = 4000000000,
                        price = 10,
                        name = "Test Booster Pack",
                        id = 0
                    },
                    new ShopSlot
                    {
                        itemId = card.id,
                        type = ShopSlotType.CARD,
                        stock = 4000000000,
                        price = 15,
                        name = "Testitator",
                        id = 1
                    },
                    new ShopSlot
                    {
                        itemId = badge.id,
                        type = ShopSlotType.BADGE,
                        stock = 4000000000,
                        price = 20,
                        name = "Test Badge",
                        id = 2
                    }
                },
                text = "**__SHOP__**\n*To buy do `=shop buy id`*\n*Do `=shop category` to see other categories*\n**Category: __Pinned__**\n<:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>     <:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>     <:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>\n<:normall:766791680376307714>:tools:<:normalr:766791680791281674>     <:normall:766791680376307714>:apple:<:normalr:766791680791281674>     <:normall:766791680376307714>:grinning:<:normalr:766791680791281674>\n<:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>     <:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>     <:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>\nID:  0                     1                      2\n0: Test Booster Pack 10 coins\n1: Testitator Card 15 coins\n2: Test Badge 20 coins\n<:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>     <:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>     <:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>\n<:normall:766791680376307714>:tools:<:normalr:766791680791281674>     <:normall:766791680376307714>:apple:<:normalr:766791680791281674>     <:normall:766791680376307714>:grinning:<:normalr:766791680791281674>\n<:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>     <:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>     <:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>\nID:  0                     1                      2\n0: Test Booster Pack 10 coins\n1: Testitator Card 15 coins\n2: Test Badge 20 coins\n*Page: 1 Do `=shop pinned 2`*§**__SHOP__**\n*To buy do `=shop buy id`*\n*Do `=shop category` to see other categories*\n**Category: __Pinned__**\n<:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>     <:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>     <:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>\n<:normall:766791680376307714>:tools:<:normalr:766791680791281674>     <:normall:766791680376307714>:apple:<:normalr:766791680791281674>     <:normall:766791680376307714>:grinning:<:normalr:766791680791281674>\n<:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>     <:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>     <:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>\nID:  0                     1                      2\n0: Test Booster Pack 10 coins\n1: Testitator Card 15 coins\n2: Test Badge 20 coins\n<:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>     <:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>     <:normaltl:766791680778436628><:normalt:766791680782499850><:normaltr:766791680795344896>\n<:normall:766791680376307714>:tools:<:normalr:766791680791281674>     <:normall:766791680376307714>:apple:<:normalr:766791680791281674>     <:normall:766791680376307714>:grinning:<:normalr:766791680791281674>\n<:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>     <:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>     <:normalbl:766791680762183730><:normalb:766791680765853736><:normalbr:766791680816185345>\nID:  0                     1                      2\n0: Test Booster Pack 100 coins\n1: Testitator Card 150 coins\n2: Test Badge 200 coins\n*Page: 2*",
                id = 0
            };
            return shopPage;
        }

        Booster CreateNewBooster()
        {
            Booster booster = new Booster
            {
                name = "Test Pack",
                id = 0,
                emoji = ":warning:",
                amount = 5,
                maxCommons = 4,
                maxUncommons = 3,
                maxRares = 2,
                maxURares = 1,
                maxLegends = 1,
                cardsUcanGet = new List<uint>() { 1, 2, 3, 4, 5 },
                possibleBuildup = new List<BuildUp>()
                {
                    new BuildUp() { types="1",chances="100"},
                    new BuildUp() { types="12",chances="75-25"},
                    new BuildUp() { types="12",chances="50-50"},
                    new BuildUp() { types="1234",chances="40-30-20-10"},
                    new BuildUp() { types="345",chances="50-30-20"}
                },
                notes = "This is just a tester pack."
            };
            return booster;
        }

        Badge CreateNewBadge()
        {
            Badge badge = new Badge
            {
                name = "The Tester",
                emoji = ":tools:",
                notes = "This is a test badge.",
                id = 0
            };
            return badge;
        }

        List<Card> CreateNewCards()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card
            {
                name = "Testitator",
                type = CardType.COMMON,
                emoji = ":tools:",
                id = 0,
                notes = "canBattle§type§health§attackType$name$dmg$effect$target#attackType$name$dmg$effect$target§fullEmoji§description"
            });
            return cards;
        }

        ServerData.ServerData CreateNewServerData()
        {
            ServerData.ServerData data = new ServerData.ServerData()
            {
                serverId = 1,
                prefix = "=",
                linkedToServerId = 0,
                linkedToThisServerIds = new List<ulong>()
                {
                    0
                },
                userData = new List<ServerData.UserData>()
                {
                    new ServerData.UserData
                    {
                        userId = 0,
                        inventoryCards = new List<uint>(),
                        inventoryBoosters = new List<uint>(),
                        inventoryBadges = new List<uint>(),
                        coins = 0
                    }
                },
                tradeRequests = new List<TradeRequest>()
                {
                    new TradeRequest()
                    {
                        offererId = 0,
                        accepterId = 0,
                        offeredCoins = 0,
                        acceptedCoins = 0,
                        offererCardIds = new List<uint>(),
                        accepterCardIds = new List<uint>()
                    }
                },
                battles = new List<Battle>(),
                leaderboard = new List<LeaderBoardPlace>(),
                setup = false,
                allowAnnouncements = true,
                allowNotifications = true
            };
            return data;
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            
            try
            {
                var message = arg as SocketUserMessage;
                var context = new SocketCommandContext(_client, message);
                if (message.Author.IsBot) return;
                int argPos = 0;
                bool userExistsInData = false;
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                }
                                else
                                {
                                    contextGuildId = context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = context.Guild.Id;
                        }
                    }
                    if (_serverData.serverId == contextGuildId)
                    {
                        foreach (ServerData.UserData _userdata in _serverData.userData)
                        {
                            if (_userdata.userId == context.User.Id)
                            {
                                userExistsInData = true;
                                break;
                            }
                        }
                        break;
                    }
                }
                if (!userExistsInData)
                {
                    ServerData.UserData userdata = new ServerData.UserData()
                    {
                        userId = context.User.Id,
                        coins = 100,
                        inventoryBadges = new List<uint>(),
                        inventoryBoosters = new List<uint>(),
                        inventoryCards = new List<uint>(),
                        decks = new List<ServerData.Deck>(),
                        xp = 0,
                        xpForCoin = 10,
                        lastMin = DateTime.Now.Minute,
                        lastJoinVCTime = DateTime.Now.Ticks,
                        importantAnnouncements = true,
                        notifyAnnouncements = true
                    };
                    for (int i = 0; i < CurrentData.serverData.Count; i++)
                    {
                        if (CurrentData.serverData[i].serverId == contextGuildId)
                        {
                            CurrentData.serverData[i].userData.Add(userdata);
                            userExistsInData = true;
                        }
                    }
                }
                if (userExistsInData)
                {
                    //halloween 2020
                    /*for (int i = 0; i < CurrentData.serverData.Count; i++)
                    {
                        if (CurrentData.serverData[i].serverId == contextGuildId)
                        {
                            for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                            {
                                if (CurrentData.serverData[i].userData[j].userId == context.User.Id)
                                {
                                    if (!CurrentData.serverData[i].userData[j].inventoryBadges.Contains(1) && CurrentData.serverData[i].userData[j].inventoryCards.Contains(31) && CurrentData.serverData[i].userData[j].inventoryCards.Contains(40) && DateTime.Now.Month == 10 && DateTime.Now.Year == 2020)
                                    {
                                        CurrentData.serverData[i].userData[j].inventoryBadges.Add(1);
                                        IEmote emote = new Emoji("\U0001f383");
                                        await context.Message.AddReactionAsync(emote);
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }*/
                    //-----------
                    if (context.User.Id == 350279720144863244)
                    {
                        if (!HasAdminBadge(context, contextGuildId))
                        {
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == contextGuildId)
                                {
                                    for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                    {
                                        if (CurrentData.serverData[i].userData[j].userId == context.User.Id)
                                        {
                                            CurrentData.serverData[i].userData[j].inventoryBadges.Add(CurrentData.badges[0].id);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            IEmote emote = new Emoji("\U00002611");
                            await context.Message.AddReactionAsync(emote);
                        }
                    }
                    //coin command
                    if (context.Message.Content == "coin" && contextGuildId == CurrentData.settings.adminServerID)
                    {
                        for (int i = 0; i < CurrentData.serverData.Count; i++)
                        {
                            if (CurrentData.serverData[i].serverId == contextGuildId)
                            {
                                for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                {
                                    if (CurrentData.serverData[i].userData[j].userId == context.User.Id)
                                    {
                                        Random rnd = new Random();
                                        int coins = rnd.Next(1, 21);
                                        CurrentData.serverData[i].userData[j].coins += coins;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        IEmote emote = new Emoji("\U0001FA99");
                        await context.Message.AddReactionAsync(emote);
                    }
                    //--------
                    for (int i = 0; i < CurrentData.serverData.Count; i++)
                    {
                        if (CurrentData.serverData[i].serverId == contextGuildId)
                        {
                            for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                            {
                                if (CurrentData.serverData[i].userData[j].userId == context.User.Id)
                                {
                                    if (CurrentData.serverData[i].userData[j].lastMin != DateTime.Now.Minute)
                                    {
                                        CurrentData.serverData[i].userData[j].lastMin = DateTime.Now.Minute;
                                        CurrentData.serverData[i].userData[j].xp += 1 * CurrentData.settings.chatXPmultiplier;
                                        if (CurrentData.serverData[i].userData[j].xp >= CurrentData.serverData[i].userData[j].xpForCoin)
                                        {
                                            CurrentData.serverData[i].userData[j].xp = 0;
                                            CurrentData.serverData[i].userData[j].coins += 1;
                                            Random rnd = new Random();
                                            CurrentData.serverData[i].userData[j].xpForCoin = rnd.Next(CurrentData.settings.minXPreq, CurrentData.settings.maxXPreq);
                                            IEmote emote = new Emoji("\U0001FA99");
                                            await context.Message.AddReactionAsync(emote);
                                        }
                                        
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                if (message.HasStringPrefix("=", ref argPos))
                {
                    var result = await _commands.ExecuteAsync(context, argPos, _services);
                    if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                }
            }
            catch { Console.WriteLine("Joined Server or DM recieved: Nominal Exception detected"); }
        }

        private static Task HandleVoiceEventsAsync(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState)
        {
            if (CurrentData.commandsEnabled)
            {
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (newVoiceState.VoiceChannel != null)
                    {
                        if (_serverData.serverId == newVoiceState.VoiceChannel.Guild.Id)
                        {
                            if (_serverData.linkedToServerId != 0)
                            {
                                foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                                {
                                    if (__serverData.linkedToThisServerIds.Contains(newVoiceState.VoiceChannel.Guild.Id))
                                    {

                                        contextGuildId = _serverData.linkedToServerId;
                                    }
                                    else
                                    {
                                        contextGuildId = newVoiceState.VoiceChannel.Guild.Id;
                                    }
                                }
                            }
                            else
                            {
                                contextGuildId = newVoiceState.VoiceChannel.Guild.Id;
                            }
                        }
                    }
                    else if (oldVoiceState.VoiceChannel != null)
                    {
                        if (_serverData.serverId == oldVoiceState.VoiceChannel.Guild.Id)
                        {
                            if (_serverData.linkedToServerId != 0)
                            {
                                foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                                {
                                    if (__serverData.linkedToThisServerIds.Contains(oldVoiceState.VoiceChannel.Guild.Id))
                                    {

                                        contextGuildId = _serverData.linkedToServerId;
                                    }
                                    else
                                    {
                                        contextGuildId = oldVoiceState.VoiceChannel.Guild.Id;
                                    }
                                }
                            }
                            else
                            {
                                contextGuildId = oldVoiceState.VoiceChannel.Guild.Id;
                            }
                        }
                    }
                }
                for (int i = 0; i < CurrentData.serverData.Count; i++)
                {
                    if (CurrentData.serverData[i].serverId == contextGuildId)
                    {
                        for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                        {
                            if (CurrentData.serverData[i].userData[j].userId == user.Id)
                            {
                                if (oldVoiceState.VoiceChannel == null && newVoiceState.VoiceChannel != null)
                                {
                                    //user joined
                                    CurrentData.serverData[i].userData[j].lastJoinVCTime = DateTime.Now.Ticks;
                                }
                                else if (oldVoiceState.VoiceChannel != null && newVoiceState.VoiceChannel == null)
                                {
                                    //User left
                                    long elapsedTicks = DateTime.Now.Ticks - CurrentData.serverData[i].userData[j].lastJoinVCTime;
                                    TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                                    double mins = elapsedSpan.TotalMinutes;
                                    Console.WriteLine(mins.ToString("0.000") + " " + elapsedTicks);
                                    Random rnd = new Random();
                                    int max = rnd.Next(CurrentData.settings.minXPvc, CurrentData.settings.maxXPvc);
                                    int pos = 0;
                                    while (mins >= 10 && pos < max)
                                    {
                                        pos += 1;
                                        mins -= 10;
                                        CurrentData.serverData[i].userData[j].coins += 1 * CurrentData.settings.voiceChatXPmultiplier;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        bool HasAdminBadge(SocketCommandContext context, ulong contextGuildId)
        {
            foreach (ServerData.ServerData _serverData in CurrentData.serverData)
            {
                if (_serverData.serverId == contextGuildId)
                {
                    foreach (ServerData.UserData _userData in _serverData.userData)
                    {
                        if (_userData.userId == context.User.Id)
                        {
                            foreach (uint _badgeId in _userData.inventoryBadges)
                            {
                                if (_badgeId == 0)
                                {
                                    return true;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return false;
        }

        Settings CreateNewSettings()
        {
            Settings settings = new Settings()
            {
                token = "place_token_here",
                adminServerID = 0,
                minXPreq = 5,
                maxXPreq = 16,
                chatXPmultiplier = 1,
                voiceChatXPmultiplier = 1,
                minXPvc = 3,
                maxXPvc = 13,
                announcement = "Announcements here",
                notification = "Notifications here"
            };
            return settings;
        }
    }

    public class Settings
    {
        public string token { get; set; }
        public ulong adminServerID { get; set; }
        public int minXPreq { get; set; }
        public int maxXPreq { get; set; }
        public int chatXPmultiplier { get; set; }
        public int voiceChatXPmultiplier { get; set; }
        public int minXPvc { get; set; }
        public int maxXPvc { get; set; }
        public string announcement { get; set; }
        public string notification { get; set; }
    }
}
