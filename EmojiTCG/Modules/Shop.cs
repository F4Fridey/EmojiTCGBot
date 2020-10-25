using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiTCG.Modules
{
    public enum ShopCategory
    {
        PINNED,
        NORMAL,
        LIMMITED,
        TIMMED,
        SEASONAL
    }

    public static class Shop
    {
        public static List<ShopPage> shopPages = new List<ShopPage>(); 
    }
}
