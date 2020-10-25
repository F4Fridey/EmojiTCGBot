using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.Modules
{
    public class DailyMonthPrizes
    {
        public string name { get; set; }
        public List<Prize> prizesPerDay { get; set; }
        public int month { get; set; }
    }

    public class Prize
    {
        public PrizeType prizeType { get; set; }
        public string name { get; set; }
        public string emoji { get; set; }
        public uint itemId { get; set; }
        public double coinAmount { get; set; }
    }

    public enum PrizeType
    {
        NOTHING,
        COINS,
        CARD,
        BOOSTER,
        BADGE
    }
}
