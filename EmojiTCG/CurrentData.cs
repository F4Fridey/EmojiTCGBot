using Discord.Commands;
using Discord.WebSocket;
using EmojiTCG.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG
{
    public static class CurrentData
    {
        //Status
        public static bool maintenance = false;
        public static string statusText = "";
        public static bool commandsEnabled = true;

        //bot
        public static DiscordSocketClient client { get; set; }

        //Bot Data
        public static Settings settings;
        public static List<ServerData.ServerData> serverData = new List<ServerData.ServerData>();
        public static List<Cards.Card> cards = new List<Cards.Card>();
        public static List<Cards.Booster> boosters = new List<Cards.Booster>();
        public static List<Cards.Badge> badges = new List<Cards.Badge>();

        //saving
        public static int numToSave = 0;

        //Set Up
        public static List<SettingUp> settingUpList = new List<SettingUp>();

        //daily
        public static List<DailyMonthPrizes> dailyMonthPrizes = new List<DailyMonthPrizes>();
    }

    public class SettingUp
    {
        public ulong serverId { get; set; }
        public uint state { get; set; }
        public Discord.WebSocket.SocketUser user { get; set; }
        public Discord.WebSocket.ISocketMessageChannel channel { get; set; }
        public bool settingUp { get; set; }

    }
}
