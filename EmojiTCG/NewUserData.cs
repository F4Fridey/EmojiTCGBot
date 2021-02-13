﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG
{
    public class NewUserData
    {
        public ulong userId { get; set; }
        public List<uint> inventoryCards { get; set; }
        public List<uint> inventoryBoosters { get; set; }
        public List<uint> inventoryBadges { get; set; }
        public List<ServerData.Deck> decks { get; set; }
        public double coins { get; set; }
        public int dailyStreak { get; set; }
        public int dailyMonthStreak { get; set; }
        public DateTime lastDaily { get; set; }
        public int xp { get; set; }
        public int xpForCoin { get; set; }
        public int lastMin { get; set; }
        public long lastJoinVCTime { get; set; }
        public uint battleWins { get; set; }
    }
}
