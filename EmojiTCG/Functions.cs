using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace EmojiTCG
{
    public static class Functions
    {
        public static void SaveAllData()
        {
            string json = JsonConvert.SerializeObject(CurrentData.settings, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/settings.json", json);
            json = JsonConvert.SerializeObject(CurrentData.serverData, Formatting.Indented);
            File.WriteAllText(@"Bot/Data/serverData.json", json);
            json = JsonConvert.SerializeObject(CurrentData.userData, Formatting.Indented);
            File.WriteAllText(@"Bot/Data/userData.json", json);
            json = JsonConvert.SerializeObject(CurrentData.cards, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/cards.json", json);
            json = JsonConvert.SerializeObject(CurrentData.boosters, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/boosters.json", json);
            json = JsonConvert.SerializeObject(CurrentData.badges, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/badge.json", json);
            json = JsonConvert.SerializeObject(Modules.Shop.shopPages, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/shoppages.json", json);
            json = JsonConvert.SerializeObject(CurrentData.dailyMonthPrizes, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/dailymonthprizes.json", json);
            json = JsonConvert.SerializeObject(CurrentData.tradeRequests, Formatting.Indented);
            File.WriteAllText(@"Bot/Settings/tradeReq.json", json);
        }

        public static void LoadAllData()
        {
            string json = "";
            json = File.ReadAllText(@"Bot/Settings/settings.json");
            CurrentData.settings = JsonConvert.DeserializeObject<Settings>(json);
            json = File.ReadAllText(@"Bot/Data/serverData.json");
            CurrentData.serverData = JsonConvert.DeserializeObject<List<Data.ServerData>>(json);
            json = File.ReadAllText(@"Bot/Data/userData.json");
            CurrentData.userData = JsonConvert.DeserializeObject<List<Data.UserData>>(json);
            json = File.ReadAllText(@"Bot/Settings/cards.json");
            CurrentData.cards = JsonConvert.DeserializeObject<List<Cards.Card>>(json);
            json = File.ReadAllText(@"Bot/Settings/boosters.json");
            CurrentData.boosters = JsonConvert.DeserializeObject<List<Cards.Booster>>(json);
            json = File.ReadAllText(@"Bot/Settings/badge.json");
            CurrentData.badges = JsonConvert.DeserializeObject<List<Cards.Badge>>(json);
            json = File.ReadAllText(@"Bot/Settings/shoppages.json");
            Modules.Shop.shopPages = JsonConvert.DeserializeObject<List<Modules.ShopPage>>(json);
            json = File.ReadAllText(@"Bot/Settings/dailymonthprizes.json");
            CurrentData.dailyMonthPrizes = JsonConvert.DeserializeObject<List<Modules.DailyMonthPrizes>>(json);
            json = File.ReadAllText(@"Bot/Settings/tradeReq.json");
            CurrentData.tradeRequests = JsonConvert.DeserializeObject<List<Modules.TradeRequest>>(json);
        }

        public static void BackUpDailyMonthlyPrize()
        {
            Modules.DailyMonthPrizes newDmp = new Modules.DailyMonthPrizes()
            { 
                name = GetMonthAsString(DateTime.Now.Month),
                month = DateTime.Now.Month,
                prizesPerDay = new List<Modules.Prize>()
            };
            for (int i = 0; i < 9; i++)
            {
                newDmp.prizesPerDay.Add(new Modules.Prize()
                {
                    prizeType = Modules.PrizeType.COINS,
                    name = "Coins",
                    emoji = ":coin:",
                    coinAmount = 5.0
                });
            }
            newDmp.prizesPerDay.Add(new Modules.Prize()
            {
                prizeType = Modules.PrizeType.BOOSTER,
                name = "Normal Pack",
                emoji = "<:Normal_Pack:769405453565952050>",
                itemId = 0
            });
            for (int i = 0; i < 9; i++)
            {
                newDmp.prizesPerDay.Add(new Modules.Prize()
                {
                    prizeType = Modules.PrizeType.COINS,
                    name = "Coins",
                    emoji = ":coin:",
                    coinAmount = 10.0
                });
            }
            newDmp.prizesPerDay.Add(new Modules.Prize()
            {
                prizeType = Modules.PrizeType.BOOSTER,
                name = "Rare Pack",
                emoji = "<:Rare_Pack:774504252223258636>",
                itemId = 1
            });
            for (int i = 0; i < 7; i++)
            {
                newDmp.prizesPerDay.Add(new Modules.Prize()
                {
                    prizeType = Modules.PrizeType.COINS,
                    name = "Coins",
                    emoji = ":coin:",
                    coinAmount = 15.0
                });
            }
            newDmp.prizesPerDay.Add(new Modules.Prize()
            {
                prizeType = Modules.PrizeType.BOOSTER,
                name = "Legendary Pack",
                emoji = "<:Legend_Pack:809946496774045727>",
                itemId = 2
            });
            int m = DateTime.Now.Month;
            if (m==1|| m == 3 || m == 5 || m == 7 || m == 8 || m == 10 || m == 12)
            {
                for (int i = 0; i < 3; i++)
                {
                    newDmp.prizesPerDay.Add(new Modules.Prize()
                    {
                        prizeType = Modules.PrizeType.COINS,
                        name = "Coins",
                        emoji = ":coin:",
                        coinAmount = 20.0
                    });
                }
            }else if (m == 4 || m == 6 || m == 9 || m == 11)
            {
                for (int i = 0; i < 2; i++)
                {
                    newDmp.prizesPerDay.Add(new Modules.Prize()
                    {
                        prizeType = Modules.PrizeType.COINS,
                        name = "Coins",
                        emoji = ":coin:",
                        coinAmount = 20.0
                    });
                }
            }
            CurrentData.dailyMonthPrizes.Add(newDmp);
        }

        public static string GetMonthAsString(int month)
        {
            switch (month)
            {
                default:
                    return "";
                case 1:
                    return "January";
                case 2:
                    return "Febuary";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
            }
        }
    }
}
