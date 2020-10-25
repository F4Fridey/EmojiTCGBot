using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.Modules
{
    public class ShopPage
    {
        public ShopCategory category { get; set; }
        public List<ShopSlot> slots { get; set; }
        public List<ShopSlot> topThreeSlots { get; set; }
        public string text { get; set; }
        public uint id { get; set; }
    }
}
