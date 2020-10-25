using EmojiTCG.Cards;
using EmojiTCG.ServerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.Modules
{
    public class Battle
    {
        public ulong player1 { get; set; }
        public ulong player2 { get; set; }
        public uint turn { get; set; }
        public BattleDeck player1deck { get; set; }
        public BattleDeck player2deck { get; set; }
        public bool player2accepted { get; set; }
        public int player1activeCard { get; set; }
        public int player2activeCard { get; set; }
        public int[] player1activeSupportCard { get; set; }
        public int[] player2activeSupportCard { get; set; }
        public List<int> player1deadCards { get; set; }
        public List<int> player2deadCards { get; set; }
    }

    public class BattleDeck
    {
        public List<BattleCard> battleCards { get; set; }
        public List<ItemOtherCard> itemOtherCards { get; set; }
    }

    public class BattleCard
    {
        public Card card { get; set; }
        public int health { get; set; }
        public List<string> activeEffects { get; set; }
    }

    public class ItemOtherCard
    {
        public Card card { get; set; }
    }

    public class LeaderBoardPlace
    {
        public ulong userid { get; set; }
        public string name { get; set; }
        public uint wins { get; set; }
    }
}
