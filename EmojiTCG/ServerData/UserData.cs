using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.ServerData
{
    public class UserData
    {
        public ulong userId { get; set; }
        public List<uint> inventoryCards { get; set; }
        public List<uint> inventoryBoosters { get; set; }
        public List<uint> inventoryBadges { get; set; }
        public List<Deck> decks { get; set; }
        public double coins { get; set; }
        public int dailyStreak { get; set; }
        public int dailyMonthStreak { get; set; }
        public DateTime lastDaily { get; set; }
        public int xp { get; set; }
        public int xpForCoin { get; set; }
        public int lastMin { get; set; }
        public long lastJoinVCTime { get; set; }
        public bool importantAnnouncements { get; set; }
        public bool notifyAnnouncements { get; set; }
        public uint battleWins { get; set; }
        public bool dmRecieved { get; set; }
    }

    public class Deck
    {
        public string name { get; set; }
        public List<uint> cards { get; set; }
    }
}
