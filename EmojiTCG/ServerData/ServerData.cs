using Discord;
using EmojiTCG.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.ServerData
{
    public class ServerData
    {
        public ulong serverId { get; set; }
        public string prefix { get; set; }
        public ulong linkedToServerId { get; set; }
        public List<ulong> linkedToThisServerIds { get; set; }
        public List<UserData> userData { get; set; }
        public List<TradeRequest> tradeRequests { get; set; }
        public List<Battle> battles { get; set; }
        public List<LeaderBoardPlace> leaderboard { get; set; }
        public bool setup { get; set; }
        public bool allowAnnouncements { get; set; }
        public bool allowNotifications { get; set; }
    }
}
