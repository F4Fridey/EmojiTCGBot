using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using EmojiTCG.Modules;

namespace EmojiTCG
{
    public static class Functions
    {
        public static void SaveAllData()
        {
            string json = JsonConvert.SerializeObject(CurrentData.settings, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/settings.json", json);
            json = JsonConvert.SerializeObject(CurrentData.serverData, Formatting.Indented);
            File.WriteAllText(@"Bot/ServerData/serverData.json", json);
            json = JsonConvert.SerializeObject(CurrentData.cards, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/cards.json", json);
            json = JsonConvert.SerializeObject(CurrentData.boosters, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/boosters.json", json);
            json = JsonConvert.SerializeObject(CurrentData.badges, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/badge.json", json);
            json = JsonConvert.SerializeObject(Shop.shopPages, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/shoppages.json", json);
            json = JsonConvert.SerializeObject(CurrentData.dailyMonthPrizes, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/dailymonthprizes.json", json);
        }

        public static void LoadAllData()
        {
            string json = "";
            json = File.ReadAllText(@"Bot/Settings/settings.json");
            CurrentData.settings = JsonConvert.DeserializeObject<Settings>(json);
            json = File.ReadAllText(@"Bot/ServerData/serverData.json");
            CurrentData.serverData = JsonConvert.DeserializeObject<List<ServerData.ServerData>>(json);
            json = File.ReadAllText(@"Bot/Settings/cards.json");
            CurrentData.cards = JsonConvert.DeserializeObject<List<Cards.Card>>(json);
            json = File.ReadAllText(@"Bot/Settings/boosters.json");
            CurrentData.boosters = JsonConvert.DeserializeObject<List<Cards.Booster>>(json);
            json = File.ReadAllText(@"Bot/Settings/badge.json");
            CurrentData.badges = JsonConvert.DeserializeObject<List<Cards.Badge>>(json);
            json = File.ReadAllText(@"Bot/Settings/shoppages.json");
            Shop.shopPages = JsonConvert.DeserializeObject<List<ShopPage>>(json);
            json = File.ReadAllText(@"Bot/Settings/dailymonthprizes.json");
            CurrentData.dailyMonthPrizes = JsonConvert.DeserializeObject<List<DailyMonthPrizes>>(json);
        }
    }
}
