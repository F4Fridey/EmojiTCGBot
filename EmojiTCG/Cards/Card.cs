using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.Cards
{
    public enum CardType
    {
        COMMON,
        UNCOMMON,
        RARE,
        ULTRARARE,
        LENGENDARY,
        SPECIAL,
        OTHER,
    }

    public static class CardTypes
    {
        public static List<CardType> cardTypes = new List<CardType>() { CardType.COMMON, CardType.UNCOMMON, CardType.RARE, CardType.ULTRARARE, CardType.LENGENDARY, CardType.SPECIAL, CardType.OTHER };
    }

    public class Card
    {
        public string name { get; set; }
        public CardType type { get; set; }
        public string emoji { get; set; }
        public uint id { get; set; }
        public string fullEmoji { get; set; }
        public string desc { get; set; }
        public string notes { get; set; }
    }
}
