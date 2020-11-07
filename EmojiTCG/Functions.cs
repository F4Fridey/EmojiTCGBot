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
        //commands
        public static ulong CheckServerLinked(ulong contextServerId)
        {
            ulong contextGuildId = 0;
            foreach (ServerData.ServerData _serverData in CurrentData.serverData)
            {
                if (_serverData.serverId == contextServerId)
                {
                    if (_serverData.linkedToServerId != 0)
                    {
                        foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                        {
                            if (__serverData.linkedToThisServerIds.Contains(contextServerId))
                            {

                                contextGuildId = _serverData.linkedToServerId;
                                break;
                            }
                            else
                            {
                                contextGuildId = contextServerId;
                            }
                        }
                    }
                    else
                    {
                        contextGuildId = contextServerId;
                    }
                    break;
                }
            }
            return contextGuildId;
        }

        public static bool GetServerDataIndex(ulong contextServerId, out int i)
        {
            for (int _i = 0; _i < CurrentData.serverData.Count; _i++)
            {
                if (CurrentData.serverData[_i].serverId == contextServerId)
                {
                    i = _i;
                    return true;
                }
            }
            i = 0;
            return false;
        }

        public static bool GetUserDataIndex(ulong contextUserId, int serverDataIndex, out int i)
        {
            i = 0;
            for (int _i = 0; _i < CurrentData.serverData[serverDataIndex].userData.Count; _i++)
            {
                if (CurrentData.serverData[serverDataIndex].userData[_i].userId == contextUserId)
                {
                    i = _i;
                    return true;
                }
            }
            return false;
        }

        //Battle
        public static bool GetBattleIndex(ulong contextUserId, int serverDataIndex, out int i)
        {
            i = 0;
            for (int _i = 0; _i < CurrentData.serverData[serverDataIndex].battles.Count; _i++)
            {
                if (CurrentData.serverData[serverDataIndex].battles[_i].player1 == contextUserId || CurrentData.serverData[serverDataIndex].battles[_i].player2 == contextUserId)
                {
                    i = _i;
                    return true;
                }
            }
            return false;
        }

        public static string GetCardInfoFormatted(List<string> noteList, BattleDeck playerDeck, int i)
        {
            string reply = "";
            string canBattle = noteList[0];
            string type = noteList[1];
            string health = noteList[2];
            string attacks = noteList[3];

            if (canBattle == "1" && type == "DPS")
            {
                reply += "**" + playerDeck.battleCards[i].card.name + "** __**[ID: " + i + "]**__\n" + playerDeck.battleCards[i].card.type.ToString() + " " + playerDeck.battleCards[i].card.emoji + "\n";
                reply += "**HP:** " + health + " **Type:** " + type + "\n";
                List<string> attackLines = attacks.Split('#').ToList();
                foreach (string str in attackLines)
                {
                    List<string> attackData = str.Split('$').ToList();
                    reply += "__" + attackData[1] + "__  [DMG: " + attackData[2] + "]\n";
                    if (attackData[3] != "0")
                    {
                        reply += "+ " + attackData[3] + "\n";
                    }
                }
            }
            else if (canBattle == "1" && (type == "ITEM" || type == "SUPPORT"))
            {
                reply += "**" + playerDeck.itemOtherCards[i].card.name + "** __**[ID: " + (i + playerDeck.battleCards.Count) + "]**__\n" + playerDeck.itemOtherCards[i].card.type.ToString() + " " + playerDeck.itemOtherCards[i].card.emoji + "\n";
                reply += "**Type:** " + type + "\n";
                List<string> attackLines = attacks.Split('#').ToList();
                foreach (string str in attackLines)
                {
                    List<string> attackData = str.Split('$').ToList();
                    reply += "__" + attackData[1] + "__  [DMG: " + attackData[2] + "]\n";
                    if (attackData[3] != "0")
                    {
                        reply += "+ " + attackData[3] + "\n";
                    }
                }
            }

            return reply;
        }

        public static bool CheckIfTurn(ulong contextUserId, int serverDataIndex, int battleIndex)
        {
            bool isPlayer1 = false;
            bool isPlayer1Turn = false;
            if (CurrentData.serverData[serverDataIndex].battles[battleIndex].player1 == contextUserId)
            {
                isPlayer1 = true;
            }
            if (CurrentData.serverData[serverDataIndex].battles[battleIndex].turn % 2 == 0)
            {
                isPlayer1Turn = true;
            }
            if ((isPlayer1 && isPlayer1Turn) || (!isPlayer1 && !isPlayer1Turn))
            {
                return true;
            }
            return false;
        }

        public static void UseItemCard(int serverDataIndex, int battleIndex, bool isPlayer1, List<string> cardNotes)
        {
            int p1card = CurrentData.serverData[serverDataIndex].battles[battleIndex].player1activeCard;
            int p2card = CurrentData.serverData[serverDataIndex].battles[battleIndex].player2activeCard;

            List<string> attacks = cardNotes[3].Split('#').ToList();
            foreach (string attack in attacks)
            {
                List<string> attackData = attack.Split('$').ToList();
                if (attackData[4] == "0")
                {
                    bool hasEffects = attackData[3] != "0";
                    if (attackData[0] == "HEAL")
                    {
                        if (isPlayer1)
                        {
                            CurrentData.serverData[serverDataIndex].battles[battleIndex].player1deck.battleCards[p1card].health += int.Parse(attackData[2]);
                            if (hasEffects)
                                CurrentData.serverData[serverDataIndex].battles[battleIndex].player1deck.battleCards[p1card].activeEffects.Add(attackData[3]);
                        }
                        else
                        {
                            CurrentData.serverData[serverDataIndex].battles[battleIndex].player1deck.battleCards[p2card].health += int.Parse(attackData[2]);
                            if (hasEffects)
                                CurrentData.serverData[serverDataIndex].battles[battleIndex].player1deck.battleCards[p2card].activeEffects.Add(attackData[3]);
                        }
                    }else if (attackData[0] == "DPS")
                    {
                        if (isPlayer1)
                        {
                            CurrentData.serverData[serverDataIndex].battles[battleIndex].player1deck.battleCards[p1card].health -= int.Parse(attackData[2]);
                            if (hasEffects)
                            {
                                CurrentData.serverData[serverDataIndex].battles[battleIndex].player1deck.battleCards[p1card].activeEffects.Add(attackData[3]);
                            }
                            if (CurrentData.serverData[serverDataIndex].battles[battleIndex].player1deck.battleCards[p1card].health < 1)
                            {
                                CurrentData.serverData[serverDataIndex].battles[battleIndex].player1deadCards.Add(p1card);
                                CurrentData.serverData[serverDataIndex].battles[battleIndex].player1activeCard = -1;
                            }
                        }
                        else
                        {
                            CurrentData.serverData[serverDataIndex].battles[battleIndex].player2deck.battleCards[p2card].health -= int.Parse(attackData[2]);
                            if (hasEffects)
                            {
                                CurrentData.serverData[serverDataIndex].battles[battleIndex].player2deck.battleCards[p2card].activeEffects.Add(attackData[3]);
                            }
                            if (CurrentData.serverData[serverDataIndex].battles[battleIndex].player2deck.battleCards[p2card].health < 1)
                            {
                                CurrentData.serverData[serverDataIndex].battles[battleIndex].player2deadCards.Add(p2card);
                                CurrentData.serverData[serverDataIndex].battles[battleIndex].player2activeCard = -1;
                            }
                        }
                    }
                }
            }
        }

        public static void UseSupportCard(int serverDataIndex, int battleIndex, bool isPlayer1, int id, int pos)
        {
            Battle battle = CurrentData.serverData[serverDataIndex].battles[battleIndex];
            pos -= 1;
            if (isPlayer1)
            {
                if (battle.player1activeSupportCard[pos] != -1)
                {
                    CurrentData.serverData[serverDataIndex].battles[battleIndex].player1deadCards.Add(battle.player1activeSupportCard[pos]);
                }
                CurrentData.serverData[serverDataIndex].battles[battleIndex].player1activeSupportCard[pos] = id;
            }else
            {
                if (battle.player2activeSupportCard[pos] != -1)
                {
                    CurrentData.serverData[serverDataIndex].battles[battleIndex].player2deadCards.Add(battle.player2activeSupportCard[pos]);
                }
                CurrentData.serverData[serverDataIndex].battles[battleIndex].player2activeSupportCard[pos] = id;
            }
        }

        public static string ReplyBattlefield(int serverDataIndex, int battleIndex)
        {
            Battle battle = CurrentData.serverData[serverDataIndex].battles[battleIndex];
            string reply = "**The Battlefield:**\n";
            reply += "<@!" + battle.player1 + "> :\n";
            reply += "*Support:*\n";
            if (battle.player1activeSupportCard[0] != -1)
            {
                string name = battle.player1deck.itemOtherCards[battle.player1activeSupportCard[0] - battle.player1deck.battleCards.Count].card.name;
                string emoji = battle.player1deck.itemOtherCards[battle.player1activeSupportCard[0] - battle.player1deck.battleCards.Count].card.emoji;
                reply += name + " " + emoji + "\n";
            }
            if (battle.player1activeSupportCard[1] != -1)
            {
                string name = battle.player1deck.itemOtherCards[battle.player1activeSupportCard[1] - battle.player1deck.battleCards.Count].card.name;
                string emoji = battle.player1deck.itemOtherCards[battle.player1activeSupportCard[1] - battle.player1deck.battleCards.Count].card.emoji;
                reply += name + " " + emoji + "\n";
            }
            reply += "\n*Active:*\n";
            if (battle.player1activeCard != -1)
            {
                BattleCard card = battle.player1deck.battleCards[battle.player1activeCard];
                reply += card.card.name + " " + card.card.emoji + " [" + card.health + " :heart:]\n*Effects:*\n";
                foreach (string effect in card.activeEffects)
                {
                    if (effect != "0")
                    {
                        reply += effect + "\n";
                    }
                }
            }
            else
            {
                reply += "NONE\n";
            }
            reply += "\n";
            reply += "<@!" + battle.player2 + "> :\n";
            reply += "\n*Active:*\n";
            if (battle.player2activeCard != -1)
            {
                BattleCard card = battle.player2deck.battleCards[battle.player2activeCard];
                reply += card.card.name + " " + card.card.emoji + " [" + card.health + " :heart:]\n*Effects:*\n";
                foreach (string effect in card.activeEffects)
                {
                    if (effect != "0")
                    {
                        reply += effect + "\n";
                    }
                }
            }
            else
            {
                reply += "NONE\n";
            }
            reply += "*Support:*\n";
            if (battle.player2activeSupportCard[0] != -1)
            {
                string name = battle.player2deck.itemOtherCards[battle.player2activeSupportCard[0] - battle.player2deck.battleCards.Count].card.name;
                string emoji = battle.player2deck.itemOtherCards[battle.player2activeSupportCard[0] - battle.player2deck.battleCards.Count].card.emoji;
                reply += name + " " + emoji + "\n";
            }
            if (battle.player2activeSupportCard[1] != -1)
            {
                string name = battle.player2deck.itemOtherCards[battle.player2activeSupportCard[1] - battle.player2deck.battleCards.Count].card.name;
                string emoji = battle.player2deck.itemOtherCards[battle.player2activeSupportCard[1] - battle.player2deck.battleCards.Count].card.emoji;
                reply += name + " " + emoji + "\n";
            }
            reply += "\n*Do `=b` for a list of all battle actions*";
            return reply;
        }
    }
}
