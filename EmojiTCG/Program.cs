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
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;

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
                    json = JsonConvert.SerializeObject(CurrentData.settings, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/settings.json", json);
                }
                else
                {
                    CurrentData.settings = JsonConvert.DeserializeObject<Settings>(json);
                }
                _token = CurrentData.settings.token;
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
                    json = JsonConvert.SerializeObject(CurrentData.badges, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/badge.json", json);
                }
                else
                {
                    CurrentData.badges = JsonConvert.DeserializeObject<List<Cards.Badge>>(json);
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
                    json = JsonConvert.SerializeObject(Modules.Shop.shopPages, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/shoppages.json", json);
                }
                else
                {
                    Modules.Shop.shopPages = JsonConvert.DeserializeObject<List<Modules.ShopPage>>(json);
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
                    json = JsonConvert.SerializeObject(CurrentData.dailyMonthPrizes, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/dailymonthprizes.json", json);
                }
                else
                {
                    CurrentData.dailyMonthPrizes = JsonConvert.DeserializeObject<List<Modules.DailyMonthPrizes>>(json);
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
                    json = JsonConvert.SerializeObject(CurrentData.cards, Formatting.Indented);
                    File.WriteAllText(@"Bot/Settings/cards.json", json);
                }
                else
                {
                    CurrentData.cards = JsonConvert.DeserializeObject<List<Cards.Card>>(json);
                }
            }

            fileExists = File.Exists(@"Bot/Data/userData.json");
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/Data");
                File.Create(@"Bot/Data/userData.json");
                Console.WriteLine("Error: userData.json file did not exist. Please close the bot and fill in info in userData.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/Data/userData.json");
                if (json == "")
                {
                    json = JsonConvert.SerializeObject(CurrentData.userData, Formatting.Indented);
                    File.WriteAllText(@"Bot/Data/userData.json", json);
                }
                else
                {
                    CurrentData.userData = JsonConvert.DeserializeObject<List<Data.UserData>>(json);
                }
            }

            fileExists = File.Exists(@"Bot/Data/serverData.json");
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/Data");
                File.Create(@"Bot/Data/serverData.json");
                Console.WriteLine("Error: serverData.json file did not exist. Please close the bot and fill in info in serverData.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/Data/serverData.json");
                if (json == "")
                {
                    json = JsonConvert.SerializeObject(CurrentData.serverData, Formatting.Indented);
                    File.WriteAllText(@"Bot/Data/serverData.json", json);
                }
                else
                {
                    CurrentData.serverData = JsonConvert.DeserializeObject<List<Data.ServerData>>(json);
                }
            }

            fileExists = File.Exists(@"Bot/Data/tradeReq.json");
            if (!fileExists)
            {
                Directory.CreateDirectory(@"Bot/Data");
                File.Create(@"Bot/Data/tradeReq.json");
                Console.WriteLine("Error: tradeReq.json file did not exist. Please close the bot and fill in info in tradeReq.json.");
            }
            else
            {
                string json = "";
                json += File.ReadAllText(@"Bot/Data/tradeReq.json");
                if (json == "")
                {
                    json = JsonConvert.SerializeObject(CurrentData.tradeRequests, Formatting.Indented);
                    File.WriteAllText(@"Bot/Data/tradeReq.json", json);
                }
                else
                {
                    CurrentData.tradeRequests = JsonConvert.DeserializeObject<List<Modules.TradeRequest>>(json);
                }
            }

            string token = _token;

            _client.Log += _client_Log;
            
            await RegisterCommandsAsync();
            
            _client.UserVoiceStateUpdated += HandleVoiceEventsAsync;

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.SetStatusAsync(UserStatus.Online);
            await _client.SetGameAsync("for =help or =tut", null, ActivityType.Watching);
            
            CurrentData.client = _client;

            for (int j = 0; j < CurrentData.userData.Count; j++)
            {
                CurrentData.userData[j].lastJoinVCTime = DateTime.Now.Ticks;
            }

            await _client.StartAsync();

            await Task.Delay(-1);
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
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;
            int argPos = 0;
            try
            {
                bool userExistsInData = false;
                
                foreach (Data.UserData _userdata in CurrentData.userData)
                {
                    if (_userdata.userId == context.User.Id)
                    {
                        userExistsInData = true;
                        break;
                    }
                }

                if (!userExistsInData)
                {
                    Data.UserData userdata = new Data.UserData()
                    {
                        userId = context.User.Id,
                        coins = 20,
                        inventoryBadges = new List<uint>(),
                        inventoryBoosters = new List<uint>(),
                        inventoryCards = new List<uint>(),
                        decks = new List<Data.Deck>(),
                        xp = 0,
                        xpForCoin = 10,
                        lastMin = DateTime.Now.Minute,
                        lastJoinVCTime = DateTime.Now.Ticks
                    };
                    CurrentData.userData.Add(userdata);
                    userExistsInData = true;
                }
                
                if (userExistsInData)
                {
                    for (int i = 0; i < CurrentData.userData.Count; i++)
                    {
                        if (CurrentData.userData[i].userId == context.User.Id)
                        {
                            //halloween 2020
                            /*
                            if (!CurrentData.serverData[sDIndex].userData[uDIndex].inventoryBadges.Contains(1) && CurrentData.serverData[sDIndex].userData[uDIndex].inventoryCards.Contains(31) && CurrentData.serverData[sDIndex].userData[uDIndex].inventoryCards.Contains(40) && DateTime.Now.Month == 10 && DateTime.Now.Year == 2020)
                                            {
                                                CurrentData.serverData[sDIndex].userData[uDIndex].inventoryBadges.Add(1);
                                                IEmote emote = new Emoji("\U0001f383");
                                                await context.Message.AddReactionAsync(emote);
                                            } 
                            */
                            //-----------
                            if (CurrentData.userData[i].lastMin != DateTime.Now.Minute)
                            {
                                CurrentData.userData[i].lastMin = DateTime.Now.Minute;
                                CurrentData.userData[i].xp += 1 * CurrentData.settings.chatXPmultiplier;
                                if (CurrentData.userData[i].xp >= CurrentData.userData[i].xpForCoin)
                                {
                                    CurrentData.userData[i].xp = 0;
                                    CurrentData.userData[i].coins += 1;
                                    Random rnd = new Random();
                                    CurrentData.userData[i].xpForCoin = rnd.Next(CurrentData.settings.minXPreq, CurrentData.settings.maxXPreq);
                                    IEmote emote = new Emoji("\U0001FA99");
                                    await context.Message.AddReactionAsync(emote);
                                    for (int j = 0; j < CurrentData.serverData.Count; j++)
                                    {
                                        if (CurrentData.serverData[j].serverId == context.Guild.Id)
                                        {
                                            if (CurrentData.serverData[j].coinboard != 0)
                                            {
                                                try
                                                {
                                                    var adminChannel = CurrentData.client.GetChannel(CurrentData.serverData[j].coinboard) as IMessageChannel;
                                                    await adminChannel.SendMessageAsync(":coin: \"" + context.Message.Content + "\" - <@" + context.User.Id + ">");
                                                }
                                                catch (Exception e)
                                                {
                                                    CurrentData.serverData[j].coinboard = 0;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    CurrentData.numToSave += 1;
                    if (CurrentData.numToSave > 5 && CurrentData.commandsEnabled)
                    {
                        CurrentData.numToSave = 0;
                        Functions.SaveAllData();
                    }
                }

                if (message.HasStringPrefix("+", ref argPos))
                {
                    var result = await _commands.ExecuteAsync(context, argPos, _services);
                    if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                }
            }
            catch { Console.WriteLine("Joined Server or DM recieved or Server doesnt exist yet: Nominal Exception detected"); }
        }

        private static Task HandleVoiceEventsAsync(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState)
        {
            if (CurrentData.commandsEnabled)
            {
                ulong contextGuildId = 0;
                if (newVoiceState.VoiceChannel != null)
                    contextGuildId = newVoiceState.VoiceChannel.Guild.Id;
                else if (oldVoiceState.VoiceChannel != null)
                    contextGuildId = oldVoiceState.VoiceChannel.Guild.Id;

                int uDIndex = -1;
                bool userExistsInData = false;
                for (int i = 0; i < CurrentData.userData.Count; i++)
                {
                    if (CurrentData.userData[i].userId == user.Id)
                    {
                        userExistsInData = true;
                        uDIndex = i;
                        break;
                    }
                }

                if (userExistsInData)
                {
                    if (oldVoiceState.VoiceChannel == null && newVoiceState.VoiceChannel != null)
                    {
                        //user joined
                        CurrentData.userData[uDIndex].lastJoinVCTime = DateTime.Now.Ticks;
                    }
                    else if (oldVoiceState.VoiceChannel != null && newVoiceState.VoiceChannel == null)
                    {
                        //User left
                        long elapsedTicks = DateTime.Now.Ticks - CurrentData.userData[uDIndex].lastJoinVCTime;
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
                            CurrentData.userData[uDIndex].coins += 1 * CurrentData.settings.voiceChatXPmultiplier;
                        }
                    }
                }
            }
            return Task.CompletedTask;
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
