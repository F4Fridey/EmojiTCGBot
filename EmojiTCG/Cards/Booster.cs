using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.Cards
{
    public class Booster
    {
        public string name { get; set; }
        public uint id { get; set; }
        public string emoji { get; set; }
        public uint amount { get; set; }
        public uint maxLegends { get; set; }
        public uint maxURares { get; set; }
        public uint maxRares { get; set; }
        public uint maxUncommons { get; set; }
        public uint maxCommons { get; set; }
        public List<uint> cardsUcanGet { get; set; }
        public List<BuildUp> possibleBuildup { get; set; }

        public string notes { get; set; }
    }

    public class BuildUp
    {
        public string types { get; set; }
        public string chances { get; set; }
    }
}
