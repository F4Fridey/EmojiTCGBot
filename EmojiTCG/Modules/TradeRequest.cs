using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.Modules
{
    public class TradeRequest
    {
        public ulong offererId { get; set; }
        public ulong accepterId { get; set; }
        public double offeredCoins { get; set; }
        public double acceptedCoins { get; set; }
        public List<uint> offererCardIds { get; set; }
        public List<uint> accepterCardIds { get; set; }
    }
}
