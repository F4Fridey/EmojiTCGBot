using EmojiTCG.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.Modules
{
    public enum ShopSlotType
    {
        BOOSTER,
        BADGE,
        CARD
    }

    public class ShopSlot
    {
        public uint itemId { get; set; }
        public ShopSlotType type { get; set; }
        public uint stock { get; set; }
        public double price { get; set; }
        public string name { get; set; }
        public uint id { get; set; }
    }
}
