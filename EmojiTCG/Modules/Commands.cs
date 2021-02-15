using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EmojiTCG.Cards;

namespace EmojiTCG.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        // trt76 create new daily month thing

        [Command("help")]
        public async Task Help()
        {
            string reply = "";
            reply += "```diff\n+ All EmojiTCG Commands:\n  help - All these commands.\n  tutorial - Get a detailed explanation of how to use this bot.\n  status - Status of the bot.\n  inventory/inv (@mention) - Check your inventory. Only add @mention to check another inventory.\n  shop - Browse the shop.\n  open ID = Open a booster pack with a specific id.\n  daily - Get your daily reward.\n  melt cardID - Melt a card into a coin.\n  card ID - Check a cards stats.\nX deck - Command for your decks.\nX battle - Command to battle.\nX invite - Invite the bot to your server with this.\n- Administrator Commands\n  setup - Setup the server.\n  serverid - Get the server ID of this server.\n  unlink (serverid) - Only add server ID only if servers are linked to this server.\n```";
            await ReplyAsync(reply);
        }

        [Command("tutorial")]
        [Alias("tut")]
        public async Task Tutorial()
        {
            try
            {
                string reply = "Hello, welcome to the world of emojis! This bot allows you to collect, trade and battle emoji cards!\n\nIf you dont want to read this wall of text, you can listen to this video: <https://www.youtube.com/watch?v=dQw4w9WgXcQ>\n\nStart off by checking out your inventory with `=inv`!\nThis shows you everything you have including coins, badges, booster packs and cards. To look at other pages do `=inv #` *(# is page number)*\n\n**__Coins :coin: :__**\nCoins are the main currency. You use them to buy booster packs and to trade with others, but more on that later.\nYou earn coins by both chatting in the server and being in voice chat.\n**__Booster packs <:Normal_Pack:769405453565952050> :__**\nBooster packs are the main way to obtain cards. You can buy them in the shop. Once purchased they will appear in your inventory and you will be able to open them. To open one you do `=open <id>` where '<id>' is the booster packs id *(Which can be found next to the packs name in your inventory)*\nEach booster pack can give you certain cards at certain rarities. For example a normal booster pack will have a higher chance to give you lower tear cards, but has the benefit of giving you most of the available cards.";
                await Context.User.SendMessageAsync(reply);
                reply = "**__Cards <a:Halloween_2020_Card:769690597337858098> :__**\nCards are the main focus of this bot. You must collect them all! You can use cards to battle with other players, trade with other players or if you have duplicates, melt back into coins.\n**__The Shop :shopping_cart: :__**\nThe shop is where you can buy things with your coins. To access it do `=shop`. There are many categories in the shop which sell different things. You can access them using `=shop category` to get a list of all category ids and names, then `=shop <id/name>`. You can either use the shops ID or name given in the list.\nThe shop is updated every so often with new packs and items, so keep an eye on it.\nWhen you've found something to buy look for the ID of the item, which is usualy below the items image. With that, do `=shop buy <id>` where '<id>' is the ID of the item.";
                await Context.User.SendMessageAsync(reply);
                reply = "**__Trading <:trade:774445393047584790> :__**\nYou can trade cards and coins with other users. To do so you must create a trade request first using `=trade @mention <yourOffer> <recievingItems>` where '<yourOffer>' is a list of cards and coin amount you want to offer and '<recievingItems>' is a list of cards and coins you want in return.\nThe formatting for this is as follows `cardid,cardid,cardid` etc. If you want to offer coins, insert coin amount and a 'c' at the end like this: `100c`. If you want both add the coin amount to the card list like so: `cardid,cardid,cardid,100c`.\nHere is an example: `=trade @mention 1,2,3 4,5,50c`\nThis command says your offering cards 1, 2 and 3 for cards 4, 5 and 50 coins.\nOnce a trade request has been sent, it can be canceled with `=trade cancel`. The recipient of the request would now be able to do `=trade accept/a` to accept or `=trade reject/r` to reject.";
                await Context.User.SendMessageAsync(reply);
                reply = "**__Battling :crossed_swords: :__**\nYou can battle with other players with your cards. To get a tutorial on that, you can do `=battle tutorial`\n**__Melting Cards :fire: :__**\nYou can melt a card into 1 coin with the command `=melt <cardid>` where '<cardid>' is the ID of the card found next to its name in your inventory.\n**__Badges <:Bot_Creator:767154542939865089> :__**\nBadges are essentialy achievements, you get them for completing certain quotas. For example, you can get the Gen 1 badge for collecting all generation 1 cards.\n**__Daily Prizes <:Whats_Inside:769409287239499797> :__**\nYou can get daily rewards for doing `=daily` every day. The higher your streak, the better rewards.\nThere are also monthly daily prizes which can give more than just coins.\n**__Other bits 'n bobs :__**\nIf you want to invite this bot to your server you can do so with the invite link when doing `=invite`\nYou can get a list of all commands with `=help`\nYou can check the bots status with `=status`";
                await Context.User.SendMessageAsync(reply);
            }
            catch { await ReplyAsync("```diff\n- DMs failed to send. You may need to allow DMs from server members in your settings.\n```"); return; }
            await ReplyAsync("```diff\n+ DMs sent.\n  If you didnt recieve anything, you may need to enable DMs from non-friends.\n```");
        }

        [Command("status")]
        public async Task Status()
        {
            string str = "";
            if (CurrentData.maintenance)
            {
                str += "Maintenance is underway.\n\n";
            }
            else
            {
                str += "All modules running.\n\n";
            }
            str += CurrentData.statusText;
            await ReplyAsync(str);
        }

        [Command("invite")]
        public async Task Invite()
        {
            //await ReplyAsync("You can invite the bot with this link:\nhttps://discord.com/oauth2/authorize?client_id=766474551433232405&scope=bot&permissions=8");
            await ReplyAsync("Bot is not ready for public use yet. It will be announced when its public.");
        }

        [Command("shop")]
        public async Task Shop(params String[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                List<string> pages = new List<string>();
                try
                {
                    if (args[0] == "") { }
                }
                catch
                {
                    pages = Modules.Shop.shopPages[0].text.Split('§').ToList();
                    await ReplyAsync(pages[0]);
                }
                switch (args[0])
                {
                    case "category":
                        string str = "```diff\n+ All Shop Categories\n";
                        foreach (ShopPage _shoppage in Modules.Shop.shopPages)
                        {
                            str += "  " + _shoppage.id + " " + _shoppage.category.ToString().ToLower() + "\n";
                        }
                        str += "- =shop id/categoryName pageNum\n```";
                        await ReplyAsync(str);
                        break;
                    case "buy":
                        uint _slotId = 0;
                        bool parsedSlot = false;
                        try
                        {
                            parsedSlot = uint.TryParse(args[1], out _slotId);
                        }
                        catch { }
                        if (parsedSlot)
                        {
                            ShopSlot shopSlot = new ShopSlot();
                            bool slotFound = false;
                            foreach (ShopPage _shopPage in Modules.Shop.shopPages)
                            {
                                foreach (ShopSlot _slot in _shopPage.slots)
                                {
                                    if (_slot.id == _slotId)
                                    {
                                        shopSlot = _slot;
                                        slotFound = true;
                                        break;
                                    }
                                }
                                if (slotFound)
                                    break;
                            }
                            if (slotFound)
                            {
                                for (int i = 0; i < CurrentData.userData.Count; i++)
                                {
                                    if (CurrentData.userData[i].userId == Context.User.Id)
                                    {
                                        if (CurrentData.userData[i].coins >= shopSlot.price)
                                        {
                                            IEmote emote = new Emoji("\U00002705");
                                            switch (shopSlot.type)
                                            {
                                                case ShopSlotType.BOOSTER:
                                                    CurrentData.userData[i].coins -= shopSlot.price;
                                                    CurrentData.userData[i].inventoryBoosters.Add(shopSlot.itemId);
                                                    await Context.Message.AddReactionAsync(emote);
                                                    break;
                                                case ShopSlotType.CARD:
                                                    CurrentData.userData[i].coins -= shopSlot.price;
                                                    CurrentData.userData[i].inventoryCards.Add(shopSlot.itemId);
                                                    await Context.Message.AddReactionAsync(emote);
                                                    break;
                                                case ShopSlotType.BADGE:
                                                    CurrentData.userData[i].coins -= shopSlot.price;
                                                    CurrentData.userData[i].inventoryBadges.Add(shopSlot.itemId);
                                                    await Context.Message.AddReactionAsync(emote);
                                                    break;
                                                default:
                                                    string str2 = "```diff\n- An error has occured, failed to purchase item.\n```";
                                                    await ReplyAsync(str2);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            await ReplyAsync("```diff\n- You dont have enough coins!\n```");
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        int _id = 0;
                        int _page = 0;
                        bool parsedCategoryId = int.TryParse(args[0], out _id);
                        bool parsedPage = false;
                        try
                        {
                            parsedPage = int.TryParse(args[1], out _page);
                        }
                        catch { }
                        bool pagesFound = false;
                        if (parsedCategoryId)
                        {
                            pages = Modules.Shop.shopPages[_id].text.Split('§').ToList();
                            pagesFound = true;
                        }
                        else
                        {
                            foreach (ShopPage _shopPage in Modules.Shop.shopPages)
                            {
                                if (_shopPage.category.ToString().ToLower() == args[0])
                                {
                                    pages = _shopPage.text.Split('§').ToList();
                                    pagesFound = true;
                                    break;
                                }
                            }
                        }
                        if (parsedPage && _page > 0 && _page <= pages.Count && pagesFound)
                        {
                            await ReplyAsync(pages[_page - 1]);
                        }
                        else if (pagesFound)
                        {
                            await ReplyAsync(pages[0]);
                        }
                        else
                        {
                            await ReplyAsync(args[0] + " could not be found.");
                        }
                        break;
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use !status to check the bot status.\n```");
            }
        }

        [Command("inventory")]
        [Alias("inv")]
        public async Task Inventory(params String[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                int arg0 = 1;
                ulong userID = 0;
                bool userExistsInData = false;
                if (args.Any())
                {
                    try { arg0 = int.Parse(args[0]); }
                    catch
                    {
                        if (args[0].Contains("<@!"))
                        {
                            List<char> charList = args[0].ToCharArray().ToList();
                            string idString = "";
                            foreach (char chara in charList)
                            {
                                if (chara == '<' || chara == '>' || chara == '@' || chara == '!')
                                {
                                }
                                else
                                {
                                    idString += chara;
                                }
                            }
                            bool ulongParsed = ulong.TryParse(idString, out userID);
                            if (ulongParsed)
                            {

                                foreach (Data.UserData _userData in CurrentData.userData)
                                {
                                    if (_userData.userId == userID)
                                    {
                                        userExistsInData = true;
                                        break;
                                    }
                                }
                                try { arg0 = int.Parse(args[1]); }
                                catch { }
                            }
                        }
                    }
                }
                string name = "";
                if (userExistsInData)
                {
                    name = args[0];
                }
                else
                {
                    name = Context.User.Username;
                    userID = Context.User.Id;
                }
                string str = "**" + name.ToUpper() + " 'S INVENTORY**\n";
                List<Card> cards = new List<Card>();
                List<Badge> badges = new List<Badge>();
                List<Booster> boosters = new List<Booster>();
                List<uint> cardIds = new List<uint>();
                List<uint> badgeIds = new List<uint>();
                List<uint> boosterIds = new List<uint>();
                double coins = 0;
                bool userExists = false;
                foreach (Data.UserData _userdata in CurrentData.userData)
                {
                    if (_userdata.userId == userID)
                    {
                        userExists = true;
                        cardIds = _userdata.inventoryCards;
                        boosterIds = _userdata.inventoryBoosters;
                        badgeIds = _userdata.inventoryBadges;
                        coins = _userdata.coins;
                        break;
                    }
                }
                if (userExists)
                {
                    foreach (uint _id in cardIds)
                    {
                        foreach (Card _card in CurrentData.cards)
                        {
                            if (_card.id == _id)
                            {
                                cards.Add(_card);
                                break;
                            }
                        }
                    }
                    //sort cards
                    List<Card> sortedCards = new List<Card>();
                    for (int i = CardTypes.cardTypes.Count - 1; i >= 0; i--)
                    {
                        List<Card> _sortedCards = new List<Card>();
                        foreach (Card _card in cards)
                        {
                            if (_card.type == (CardType)i)
                            {
                                _sortedCards.Add(_card);
                            }
                        }
                        _sortedCards = _sortedCards.OrderBy(o => o.id).ToList();
                        foreach (Card _card in _sortedCards)
                        {
                            sortedCards.Add(_card);
                        }
                    }
                    cards = sortedCards;
                    foreach (uint _id in boosterIds)
                    {
                        foreach (Booster _booster in CurrentData.boosters)
                        {
                            if (_booster.id == _id)
                            {
                                boosters.Add(_booster);
                                break;
                            }
                        }
                    }
                    foreach (uint _id in badgeIds)
                    {
                        foreach (Badge _badge in CurrentData.badges)
                        {
                            if (_badge.id == _id)
                            {
                                badges.Add(_badge);
                                break;
                            }
                        }
                    }
                    str += "Coins: " + coins.ToString("0") + "<:cointcg:766805857035354145>\n\n";
                    int badgePage = 0;
                    int boosterPage = 0;
                    int cardPage = 0;
                    List<string> invPages = new List<string>();

                    string itemText = "";
                    for (int badgePos = 1; badgePos <= badges.Count; badgePos++)
                    {
                        if (badgePos == 1)
                            itemText += "__Badges__:\n";
                        itemText += badges[badgePos - 1].emoji + "       ";
                        if (badgePos % 5 == 0 && badgePos % 25 != 0)
                            itemText += "\n\n";
                        if (badgePos % 25 == 0 || badgePos == badges.Count)
                        {
                            badgePage++;
                            itemText += "\n*Page: " + badgePage;
                            invPages.Add(itemText);
                            itemText = "";
                        }
                    }
                    for (int boosterPos = 1; boosterPos <= boosters.Count; boosterPos++)
                    {
                        if (boosterPos == 1)
                            itemText += "__Booster Packs__:\n";
                        itemText += boosters[boosterPos - 1].emoji + " " + boosters[boosterPos - 1].id + " " + boosters[boosterPos - 1].name + "\n";
                        if (boosterPos % 10 == 0 || boosterPos == boosters.Count)
                        {
                            boosterPage++;
                            itemText += "*Page: " + (badgePage + boosterPage);
                            invPages.Add(itemText);
                            itemText = "";
                        }
                    }
                    for (int cardpos = 1; cardpos <= cards.Count; cardpos++)
                    {
                        if (cardpos == 1)
                            itemText += "__Cards__:\n";
                        itemText += cards[cardpos - 1].emoji + " " + cards[cardpos - 1].id + " " + cards[cardpos - 1].name + "\n";
                        if (cardpos % 10 == 0 || cardpos == cards.Count)
                        {
                            cardPage++;
                            itemText += "*Page: " + (badgePage + boosterPage + cardPage);
                            invPages.Add(itemText);
                            itemText = "";
                        }
                    }
                    if (arg0 < 1 || arg0 > invPages.Count)
                        arg0 = 1;

                    if (invPages.Count != 0)
                        str += invPages[arg0 - 1] + " of " + invPages.Count + " `=inv pageNum`*";
                    else
                        str += "*Page 1 of 1 `=inv pageNum`*";
                    await ReplyAsync(str);
                }
                else if (userExistsInData)
                {
                    await ReplyAsync("```diff\n- Uh-oh, he doesn't exist :o\n```");
                }
                else
                {
                    await ReplyAsync("```diff\n- Uh-oh, you dont exist :o\n  You should now exist :D\n  Do the command again\n```");
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use =status to check the bot status.\n```");
            }
        }

        [Command("trade")]
        public async Task Trade(params String[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                if (!args.Any())
                {
                    await ReplyAsync("```diff\n+ Trade command:\n  TRADE @MENTION yourcardid,yourcardid,yourcoinamountC theircardid,theircardid,theircoinamountC\n  You dont need to have specificaly 2 canrd IDs and 1 coin amount, you can change that.\n  Example: trade @user 23,45 100c = Youll give user card 23 and 45 for 100 coins.\n```");//trade help
                }
                else
                {
                    if (args[0].Contains("<@!"))
                    {
                        bool hasRequestAlready = false;
                        foreach (TradeRequest _tradeReq in CurrentData.tradeRequests)
                        {
                            if (_tradeReq.offererId == Context.User.Id)
                            {
                                hasRequestAlready = true;
                                break;
                            }
                        }
                        if (hasRequestAlready)
                        {
                            await ReplyAsync("```diff\n- You already have a trade request. Do TRADE CANCEl to  cancel it.\n```");
                        }
                        else
                        {
                            List<char> charList = args[0].ToCharArray().ToList();
                            string idString = "";
                            foreach (char chara in charList)
                            {
                                if (chara == '<' || chara == '>' || chara == '@' || chara == '!')
                                {
                                }
                                else
                                {
                                    idString += chara;
                                }
                            }
                            ulong id;
                            List<uint> offererCardIds = new List<uint>();
                            double offererCoins = 0;
                            List<uint> accepterCardIds = new List<uint>();
                            double accepterCoins = 0;
                            bool parsed = ulong.TryParse(idString, out id);
                            bool heHasReq = false;
                            if (parsed)
                            {
                                foreach (TradeRequest _tradeReq in CurrentData.tradeRequests)
                                {
                                    if (_tradeReq.accepterId == id)
                                    {
                                        heHasReq = true;
                                        break;
                                    }
                                }
                            }
                            if (heHasReq)
                            {
                                await ReplyAsync("```diff\n- Someone else sent a request to them already.\n```");
                                return;
                            }
                            if (parsed && !heHasReq)
                            {
                                Data.UserData offererUserData = new Data.UserData();
                                Data.UserData accepterUserData = new Data.UserData();
                                foreach (Data.UserData _userdata in CurrentData.userData)
                                {
                                    if (_userdata.userId == Context.User.Id)
                                    {
                                        offererUserData = _userdata;
                                        break;
                                    }
                                }
                                foreach (Data.UserData _userdata in CurrentData.userData)
                                {
                                    if (_userdata.userId == id)
                                    {
                                        accepterUserData = _userdata;
                                        break;
                                    }
                                }
                                List<string> strs = args[1].Split(',').ToList();
                                foreach (string str in strs)
                                {
                                    uint cardid = 4000000000;
                                    bool cardidParsed = uint.TryParse(str, out cardid);
                                    if (cardid == 4000000000)
                                        cardidParsed = false;
                                    if (cardidParsed)
                                    {
                                        offererCardIds.Add(cardid);
                                    }
                                    else if (str.ToLower().Contains("c"))
                                    {
                                        List<char> _chars = str.ToLower().ToCharArray().ToList();
                                        string newStr = "";
                                        foreach (char _char in _chars)
                                        {
                                            if (_char != 'c')
                                            {
                                                newStr += _char;
                                            }
                                        }
                                        int coinsInt = -1;
                                        bool coinsParsed = int.TryParse(newStr, out coinsInt);
                                        double coins = coinsInt;
                                        if (coins < 0)
                                            coinsParsed = false;
                                        if (coinsParsed)
                                        {
                                            offererCoins = coins;
                                        }
                                    }

                                }
                                strs = args[2].Split(',').ToList();
                                foreach (string str in strs)
                                {
                                    uint cardid = 4000000000;
                                    bool cardidParsed = uint.TryParse(str, out cardid);
                                    if (cardid == 4000000000)
                                        cardidParsed = false;
                                    if (cardidParsed)
                                    {
                                        accepterCardIds.Add(cardid);
                                    }
                                    else if (str.ToLower().Contains("c"))
                                    {
                                        List<char> _chars = str.ToLower().ToCharArray().ToList();
                                        string newStr = "";
                                        foreach (char _char in _chars)
                                        {
                                            if (_char != 'c')
                                            {
                                                newStr += _char;
                                            }
                                        }
                                        int coinsInt = -1;
                                        bool coinsParsed = int.TryParse(newStr, out coinsInt);
                                        double coins = coinsInt;
                                        if (coins < 0)
                                            coinsParsed = false;
                                        if (coinsParsed)
                                        {
                                            accepterCoins = coins;
                                        }
                                    }

                                }

                                TradeRequest tradeReq = new TradeRequest()
                                {
                                    offererId = Context.User.Id,
                                    accepterId = id,
                                    offeredCoins = offererCoins,
                                    acceptedCoins = accepterCoins,
                                    offererCardIds = offererCardIds,
                                    accepterCardIds = accepterCardIds
                                };
                                List<uint> offererCardInv = new List<uint>(offererUserData.inventoryCards);
                                List<uint> accepterCardInv = new List<uint>(accepterUserData.inventoryCards);
                                foreach (uint _cardid in tradeReq.offererCardIds)
                                {
                                    if (offererCardInv.Contains(_cardid))
                                    {
                                        offererCardInv.Remove(_cardid);
                                    }
                                    else
                                    {
                                        await ReplyAsync("```diff\n- You dont have one or more of those cards.\n```");
                                        return;
                                    }
                                }
                                foreach (uint _cardid in tradeReq.accepterCardIds)
                                {
                                    if (accepterCardInv.Contains(_cardid))
                                    {
                                        accepterCardInv.Remove(_cardid);
                                    }
                                    else
                                    {
                                        await ReplyAsync("```diff\n- They dont have one or more of those cards.\n```");
                                        return;
                                    }
                                }
                                if (offererUserData.coins < tradeReq.offeredCoins)
                                {
                                    await ReplyAsync("```diff\n- You dont have that many coins.\n```");
                                    return;
                                }
                                if (accepterUserData.coins < tradeReq.acceptedCoins)
                                {
                                    await ReplyAsync("```diff\n- They dont have that many coins.\n```");
                                    return;
                                }
                                CurrentData.tradeRequests.Add(tradeReq);
                                await ReplyAsync("```diff\n+ Trade Request made\n  The other must do =trade accept/reject```");
                                return;
                            }
                            else
                            {
                                await ReplyAsync("```diff\n- Thats not a valid ID\n```");
                            }
                        }
                    }
                    else if (args[0] == "cancel")
                    {
                        bool hasRequest = false;
                        for (int j = 0; j < CurrentData.tradeRequests.Count; j++)
                        {
                            if (CurrentData.tradeRequests[j].offererId == Context.User.Id)
                            {
                                hasRequest = true;
                                CurrentData.tradeRequests.Remove(CurrentData.tradeRequests[j]);
                                IEmote emote = new Emoji("\U00002705");
                                await Context.Message.AddReactionAsync(emote);
                                return;
                            }
                        }
                        if (!hasRequest)
                        {
                            await ReplyAsync("```diff\n- You have no requests to cancel.\n```");
                        }
                    }
                    else if (args[0] == "accept" || args[0] == "a")
                    {
                        bool hasRequestToAccept = false;
                        for (int j = 0; j < CurrentData.tradeRequests.Count; j++)
                        {
                            if (CurrentData.tradeRequests[j].accepterId == Context.User.Id)
                            {
                                hasRequestToAccept = true;
                                for (int k = 0; k < CurrentData.userData.Count; k++)
                                {
                                    if (CurrentData.userData[k].userId == CurrentData.tradeRequests[j].offererId)
                                    {
                                        if (CurrentData.userData[k].coins < CurrentData.tradeRequests[j].offeredCoins)
                                        {
                                            await ReplyAsync("```diff\n- The offerer no longer has the needed amount of coins for this trade.\n  Trade request automaticaly rejected.\n```");
                                            CurrentData.tradeRequests.Remove(CurrentData.tradeRequests[j]);
                                            return;
                                        }
                                    }
                                    else if (CurrentData.userData[k].userId == CurrentData.tradeRequests[j].accepterId)
                                    {
                                        if (CurrentData.userData[k].coins < CurrentData.tradeRequests[j].acceptedCoins)
                                        {
                                            await ReplyAsync("```diff\n- The accepter no longer has the needed amount of coins for this trade.\n  Trade request automaticaly rejected.\n```");
                                            CurrentData.tradeRequests.Remove(CurrentData.tradeRequests[j]);
                                            return;
                                        }
                                    }

                                    if (CurrentData.userData[k].userId == CurrentData.tradeRequests[j].offererId)
                                    {
                                        if (CurrentData.tradeRequests[j].offeredCoins > 0)
                                            CurrentData.userData[k].coins -= CurrentData.tradeRequests[j].offeredCoins;
                                        if (CurrentData.tradeRequests[j].acceptedCoins > 0)
                                            CurrentData.userData[k].coins += CurrentData.tradeRequests[j].acceptedCoins;
                                        foreach (uint _cardid in CurrentData.tradeRequests[j].offererCardIds)
                                        {
                                            for (int l = 0; l < CurrentData.userData[k].inventoryCards.Count; l++)
                                            {
                                                if (CurrentData.userData[k].inventoryCards[l] == _cardid)
                                                {
                                                    CurrentData.userData[k].inventoryCards.Remove(CurrentData.userData[k].inventoryCards[l]);
                                                    break;
                                                }
                                            }
                                        }
                                        foreach (uint _cardid in CurrentData.tradeRequests[j].accepterCardIds)
                                        {
                                            CurrentData.userData[k].inventoryCards.Add(_cardid);
                                        }
                                    }
                                    else if (CurrentData.userData[k].userId == CurrentData.tradeRequests[j].accepterId)
                                    {
                                        if (CurrentData.tradeRequests[j].acceptedCoins > 0)
                                            CurrentData.userData[k].coins -= CurrentData.tradeRequests[j].acceptedCoins;
                                        if (CurrentData.tradeRequests[j].offeredCoins > 0)
                                            CurrentData.userData[k].coins += CurrentData.tradeRequests[j].offeredCoins;
                                        foreach (uint _cardid in CurrentData.tradeRequests[j].accepterCardIds)
                                        {
                                            for (int l = 0; l < CurrentData.userData[k].inventoryCards.Count; l++)
                                            {
                                                if (CurrentData.userData[k].inventoryCards[l] == _cardid)
                                                {
                                                    CurrentData.userData[k].inventoryCards.Remove(CurrentData.userData[k].inventoryCards[l]);
                                                    break;
                                                }
                                            }
                                        }
                                        foreach (uint _cardid in CurrentData.tradeRequests[j].offererCardIds)
                                        {
                                            CurrentData.userData[k].inventoryCards.Add(_cardid);
                                        }
                                    }
                                }
                                CurrentData.tradeRequests.Remove(CurrentData.tradeRequests[j]);
                                IEmote emote = new Emoji("\U00002705");
                                await Context.Message.AddReactionAsync(emote);
                                return;
                            }
                        }
                        if (!hasRequestToAccept)
                        {
                            await ReplyAsync("```diff\n- You have no requests to accept.\n```");
                        }
                    }
                    else if (args[0] == "reject" || args[0] == "r")
                    {
                        bool hasRequestToReject = false;
                        for (int j = 0; j < CurrentData.tradeRequests.Count; j++)
                        {
                            if (CurrentData.tradeRequests[j].accepterId == Context.User.Id)
                            {
                                hasRequestToReject = true;
                                CurrentData.tradeRequests.Remove(CurrentData.tradeRequests[j]);
                                IEmote emote = new Emoji("\U000026D4");
                                await Context.Message.AddReactionAsync(emote);
                                return;
                            }
                        }
                        if (!hasRequestToReject)
                        {
                            await ReplyAsync("```diff\n- You have no requests to reject.\n```");
                        }
                    }
                    else
                    {
                        await ReplyAsync("```diff\n- Thats not a valid mention\n```");
                    }
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use !status to check the bot status.\n```");
            }
        }

        [Command("open")]
        public async Task Open(params string[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                if (!args[0].Any())
                {
                    await ReplyAsync("``diff\n+ Open Booster Pack command:\n  open boosterId - open the specific booster\n```");
                }
                else
                {
                    uint id = 4000000000;
                    bool idParsed = uint.TryParse(args[0], out id);
                    uint boosterIdToOpen = 0;
                    Booster boosterToOpen = new Booster();
                    if (idParsed)
                    {
                        bool boosterOwned = false;
                        foreach (Data.UserData _userData in CurrentData.userData)
                        {
                            if (_userData.userId == Context.User.Id)
                            {
                                foreach (uint _boosterId in _userData.inventoryBoosters)
                                {
                                    if (_boosterId == id)
                                    {
                                        boosterOwned = true;
                                        boosterIdToOpen = _boosterId;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        if (boosterOwned)
                        {
                            foreach (Booster _booster in CurrentData.boosters)
                            {
                                if (_booster.id == boosterIdToOpen)
                                {
                                    boosterToOpen = _booster;
                                    break;
                                }
                            }
                            List<Card> cardsUcanGet = new List<Card>();
                            foreach (uint _cardId in boosterToOpen.cardsUcanGet)
                            {
                                foreach (Card _card in CurrentData.cards)
                                {
                                    if (_card.id == _cardId)
                                    {
                                        cardsUcanGet.Add(_card);
                                        break;
                                    }
                                }
                            }
                            List<Card> cardUget = new List<Card>();
                            Random rnd = new Random();
                            foreach (BuildUp _buildUp in boosterToOpen.possibleBuildup)
                            {
                                List<char> types = _buildUp.types.ToCharArray().ToList();
                                List<string> chancesStr = _buildUp.chances.Split('-').ToList();
                                List<int> chances = new List<int>();
                                foreach (string chanceStr in chancesStr)
                                {
                                    chances.Add(int.Parse(chanceStr));
                                }
                                List<char> chooseType = new List<char>();
                                for (int i = 0; i < chances.Count; i++)
                                {
                                    for (int j = 0; j < chances[i]; j++)
                                    {
                                        chooseType.Add(types[i]);
                                    }
                                }
                                int type = int.Parse(chooseType[rnd.Next(0, chooseType.Count)].ToString());
                                List<Card> cardsToChooseFrom = new List<Card>();
                                foreach (Card _card in cardsUcanGet)
                                {
                                    if (_card.type == (CardType)type)
                                    {
                                        cardsToChooseFrom.Add(_card);
                                    }
                                }
                                cardUget.Add(cardsToChooseFrom[rnd.Next(0, cardsToChooseFrom.Count)]);
                            }
                            for (int j = 0; j < CurrentData.userData.Count; j++)
                            {
                                if (CurrentData.userData[j].userId == Context.User.Id)
                                {
                                    foreach (Card _card in cardUget)
                                    {
                                        CurrentData.userData[j].inventoryCards.Add(_card.id);
                                    }
                                    CurrentData.userData[j].inventoryBoosters.Remove(boosterIdToOpen);
                                    break;
                                }
                            }
                            string reply = "**You opened:**\n" + boosterToOpen.emoji + " " + boosterToOpen.name + "\n\n**You got:**\n";
                            foreach (Card _card in cardUget)
                            {
                                reply += _card.emoji + " " + _card.name + "\n";
                            }
                            reply += "\n*Items added to your inventory.*";
                            await ReplyAsync(reply);
                        }
                        else
                        {
                            await ReplyAsync("```diff\n- You dont own that booster.\n```");
                        }
                    }

                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use !status to check the bot status.\n```");
            }
        }

        [Command("daily")]
        public async Task Daily()
        {
            if (CurrentData.commandsEnabled)
            {
                bool userExists = false;
                for (int j = 0; j < CurrentData.userData.Count; j++)
                {
                    if (CurrentData.userData[j].userId == Context.User.Id)
                    {
                        userExists = true;
                        if (CurrentData.userData[j].lastDaily == null)
                        {
                            CurrentData.userData[j].lastDaily = new DateTime();
                        }
                        if (CurrentData.userData[j].lastDaily.Day != DateTime.Now.Day)
                        {
                            //string coins = "";
                            string dailyStreak = "";
                            if (CurrentData.userData[j].lastDaily.Date == DateTime.Now.AddDays(-1).Date)
                            {
                                CurrentData.userData[j].dailyStreak++;
                                dailyStreak = "You daily streak is now " + CurrentData.userData[j].dailyStreak + "!";
                            }
                            else
                            {
                                CurrentData.userData[j].dailyStreak = 0;
                                dailyStreak = "Your daily streak was reset D:";
                            }
                            string text = "You have claimed your daily reward!\n\n";
                            //dailyStreak prizes here
                            try
                            {
                                if (CurrentData.dailyMonthPrizes.Any() && CurrentData.userData[j].dailyMonthStreak < CurrentData.dailyMonthPrizes[0].prizesPerDay.Count)
                                {
                                    bool noMonth = false;
                                    if (CurrentData.dailyMonthPrizes[0].month != DateTime.Now.Month)
                                    {
                                        Console.WriteLine("Moving to next month daily prizes.");
                                        CurrentData.dailyMonthPrizes.RemoveAt(0);
                                        for (int l = 0; l < CurrentData.userData.Count; l++)
                                        {
                                            CurrentData.userData[l].dailyMonthStreak = 0;
                                        }
                                        if (!CurrentData.dailyMonthPrizes.Any())
                                            noMonth = true;
                                        if (noMonth)
                                        {
                                            Functions.BackUpDailyMonthlyPrize();
                                            var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                                            await adminChannel.SendMessageAsync("<@!350279720144863244> Theres no month prizes!!");// trt76 Create new month
                                            noMonth = false;
                                        }
                                    }
                                    if (!noMonth)
                                    {
                                        CurrentData.userData[j].dailyMonthStreak += 1;
                                        int currentDayPos = CurrentData.userData[j].dailyMonthStreak - 1;
                                        List<Prize> _prizes = CurrentData.dailyMonthPrizes[0].prizesPerDay;
                                        string emotes = "__" + CurrentData.dailyMonthPrizes[0].name + "__\n";
                                        for (int k = 0; k < _prizes.Count; k++)
                                        {
                                            string emoji = _prizes[k].emoji;
                                            if (k < currentDayPos)
                                            {
                                                emoji = ":ballot_box_with_check:";
                                            }
                                            else if (k == currentDayPos)
                                            {
                                                emoji = ":white_check_mark:";
                                            }
                                            if (k % 5 == 0 && k != 0)
                                            {
                                                emotes += "\n";
                                            }
                                            emotes += emoji;
                                        }
                                        text += emotes + "\n\n";
                                        switch (_prizes[currentDayPos].prizeType)
                                        {
                                            default:
                                                break;
                                            case PrizeType.COINS:
                                                CurrentData.userData[j].coins += _prizes[currentDayPos].coinAmount;
                                                text += "**+ " + _prizes[currentDayPos].coinAmount + "** :coin:\n";
                                                break;
                                            case PrizeType.CARD:
                                                Card card = new Card();
                                                foreach (Card _card in CurrentData.cards)
                                                {
                                                    if (_card.id == _prizes[currentDayPos].itemId)
                                                    {
                                                        card = _card;
                                                        break;
                                                    }
                                                }
                                                CurrentData.userData[j].inventoryCards.Add(card.id);
                                                text += "**+** " + card.emoji + "\n";
                                                break;
                                            case PrizeType.BOOSTER:
                                                Booster booster = new Booster();
                                                foreach (Booster _booster in CurrentData.boosters)
                                                {
                                                    if (_booster.id == _prizes[currentDayPos].itemId)
                                                    {
                                                        booster = _booster;
                                                        break;
                                                    }
                                                }
                                                CurrentData.userData[j].inventoryBoosters.Add(booster.id);
                                                text += "**+** " + booster.emoji + "\n";
                                                break;
                                            case PrizeType.BADGE:
                                                Badge badge = new Badge();
                                                foreach (Badge _badge in CurrentData.badges)
                                                {
                                                    if (_badge.id == _prizes[currentDayPos].itemId)
                                                    {
                                                        badge = _badge;
                                                        break;
                                                    }
                                                }
                                                CurrentData.userData[j].inventoryBadges.Add(badge.id);
                                                text += "**+** " + badge.emoji + "\n";
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    bool noMonth = false;
                                    bool resetOccured = false;
                                    if (CurrentData.dailyMonthPrizes[0].month != DateTime.Now.Month)
                                    {
                                        Console.WriteLine("Moving to next month daily prizes.");
                                        CurrentData.dailyMonthPrizes.RemoveAt(0);
                                        for (int l = 0; l < CurrentData.userData.Count; l++)
                                        {
                                            CurrentData.userData[l].dailyMonthStreak = 0;
                                        }
                                        resetOccured = true;
                                        if (!CurrentData.dailyMonthPrizes.Any())
                                            noMonth = true;
                                        if (noMonth)
                                        {
                                            Functions.BackUpDailyMonthlyPrize();
                                            var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                                            await adminChannel.SendMessageAsync("<@!350279720144863244> Theres no month prizes!!");// trt76 Create new month
                                            noMonth = false;
                                        }
                                    }
                                    if (!noMonth && !resetOccured)
                                    {
                                        List<Prize> _prizes = CurrentData.dailyMonthPrizes[0].prizesPerDay;
                                        string emotes = "";
                                        for (int k = 0; k < _prizes.Count; k++)
                                        {
                                            string emoji = ":ballot_box_with_check:";
                                            if (k % 5 == 0 && k != 0)
                                            {
                                                emotes += "\n";
                                            }
                                            emotes += emoji;
                                        }

                                        text += emotes + "\n\n";
                                    }
                                    else if (!noMonth && resetOccured)
                                    {
                                        CurrentData.userData[j].dailyMonthStreak += 1;
                                        int currentDayPos = CurrentData.userData[j].dailyMonthStreak - 1;
                                        List<Prize> _prizes = CurrentData.dailyMonthPrizes[0].prizesPerDay;
                                        string emotes = "__" + CurrentData.dailyMonthPrizes[0].name + "__\n";
                                        for (int k = 0; k < _prizes.Count; k++)
                                        {
                                            string emoji = _prizes[k].emoji;
                                            if (k < currentDayPos)
                                            {
                                                emoji = ":ballot_box_with_check:";
                                            }
                                            else if (k == currentDayPos)
                                            {
                                                emoji = ":white_check_mark:";
                                            }
                                            if (k % 5 == 0 && k != 0)
                                            {
                                                emotes += "\n";
                                            }
                                            emotes += emoji;
                                        }
                                        text += emotes + "\n\n";
                                        switch (_prizes[currentDayPos].prizeType)
                                        {
                                            default:
                                                break;
                                            case PrizeType.COINS:
                                                CurrentData.userData[j].coins += _prizes[currentDayPos].coinAmount;
                                                text += "**+ " + _prizes[currentDayPos].coinAmount + "** :coin:\n";
                                                break;
                                            case PrizeType.CARD:
                                                Card card = new Card();
                                                foreach (Card _card in CurrentData.cards)
                                                {
                                                    if (_card.id == _prizes[currentDayPos].itemId)
                                                    {
                                                        card = _card;
                                                        break;
                                                    }
                                                }
                                                CurrentData.userData[j].inventoryCards.Add(card.id);
                                                text += "**+** " + card.emoji + "\n";
                                                break;
                                            case PrizeType.BOOSTER:
                                                Booster booster = new Booster();
                                                foreach (Booster _booster in CurrentData.boosters)
                                                {
                                                    if (_booster.id == _prizes[currentDayPos].itemId)
                                                    {
                                                        booster = _booster;
                                                        break;
                                                    }
                                                }
                                                CurrentData.userData[j].inventoryBoosters.Add(booster.id);
                                                text += "**+** " + booster.emoji + "\n";
                                                break;
                                            case PrizeType.BADGE:
                                                Badge badge = new Badge();
                                                foreach (Badge _badge in CurrentData.badges)
                                                {
                                                    if (_badge.id == _prizes[currentDayPos].itemId)
                                                    {
                                                        badge = _badge;
                                                        break;
                                                    }
                                                }
                                                CurrentData.userData[j].inventoryBadges.Add(badge.id);
                                                text += "**+** " + badge.emoji + "\n";
                                                break;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Functions.BackUpDailyMonthlyPrize();
                                var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                                await adminChannel.SendMessageAsync("<@!350279720144863244> Theres no month prizes!!");// trt76 create mew thing
                                await ReplyAsync("```diff\n- An error has occured. Error code: DMP1\n  Please try again\n```");
                                return;
                            }
                            /*/------
                            if (CurrentData.userData[j].dailyStreak < 10)
                            {
                                CurrentData.userData[j].coins += 5;
                                coins = "5";
                            }
                            else if (CurrentData.userData[j].dailyStreak >= 10 && CurrentData.userData[j].dailyStreak < 20)
                            {
                                CurrentData.userData[j].coins += 10;
                                coins = "10";
                            }
                            else if (CurrentData.userData[j].dailyStreak >= 20)
                            {
                                CurrentData.userData[j].coins += 15;
                                coins = "15";
                            }*/
                            CurrentData.userData[j].lastDaily = DateTime.Now;
                            text += "\n*" + dailyStreak + "*";
                            await ReplyAsync(text);
                        }
                        else
                        {
                            await ReplyAsync("```diff\n- You can't claim the daily bonus until tomorow.\n```");
                        }
                        break;
                    }
                }
                if (!userExists)
                {
                    await ReplyAsync("```diff\n- Uh-oh, looks like you dont exist! Lets fix that!\n  Try again\n```");
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use !status to check the bot status.\n```");
            }
        }

        [Command("melt")]
        public async Task Melt(params string[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                uint arg0 = 4000000000;
                bool parsed;
                if (args.Any())
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        parsed = false;
                        parsed = uint.TryParse(args[i], out arg0);
                        if (parsed)
                        {
                            for (int j = 0; j < CurrentData.userData.Count; j++)
                            {
                                if (CurrentData.userData[j].userId == Context.User.Id)
                                {
                                    bool hasCard = false;
                                    foreach (uint _card in CurrentData.userData[j].inventoryCards)
                                    {
                                        if (_card == arg0)
                                        {
                                            CardType type = CardType.OTHER;
                                            CurrentData.userData[j].inventoryCards.Remove(_card);
                                            foreach (Card _Card in CurrentData.cards)
                                            {
                                                if (_Card.id == _card)
                                                {
                                                    type = _Card.type;
                                                    break;
                                                }
                                            }
                                            double monee;
                                            switch (type)
                                            {
                                                default:
                                                    monee = 1;
                                                    break;
                                                case CardType.COMMON:
                                                    monee = 1;
                                                    break;
                                                case CardType.UNCOMMON:
                                                    monee = 2;
                                                    break;
                                                case CardType.RARE:
                                                    monee = 3;
                                                    break;
                                                case CardType.ULTRARARE:
                                                    monee = 4;
                                                    break;
                                                case CardType.LENGENDARY:
                                                    monee = 5;
                                                    break;
                                            }

                                            CurrentData.userData[j].coins += monee;
                                            hasCard = true;
                                            IEmote emote = new Emoji("\U0001FA99");
                                            IEmote emote2 = new Emoji("\U0001F525");
                                            await ReplyAsync("You got " + monee + " :coin: from melting that card.");
                                            await Context.Message.AddReactionAsync(emote2);
                                            await Context.Message.AddReactionAsync(emote);
                                            break;
                                        }
                                    }
                                    if (!hasCard)
                                    {
                                        await ReplyAsync("```diff\n- You dont have card " + arg0 + ".\n```");
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            string str = "```diff\n- " + args[0] + " is not a valid card ID.\n```";
                            await ReplyAsync(str);
                        }
                    }
                }
                else
                {
                    await ReplyAsync("```diff\n+ Melt command:\n  Melt a card into a coin. () means optional.\n  melt cardid (cardid) (cardid) (cardid)...\n```");
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use !status to check the bot status.\n```");
            }
        }

        [Command("card")]
        public async Task Card(params string[] args)
        {
            uint id = 4000000000;
            if (args.Any())
            {
                bool parsed = uint.TryParse(args[0], out id);
                if (parsed)
                {
                    Card card = new Card();
                    foreach (Card _card in CurrentData.cards)
                    {
                        if (_card.id == id)
                        {
                            card = _card;
                            break;
                        }
                    }
                    List<string> noteList = card.notes.Split('§').ToList();
                    string canBattle = noteList[0];
                    string type = noteList[1];
                    string health = noteList[2];
                    string attacks = noteList[3];
                    string fullEmoji = card.fullEmoji;
                    string description = card.desc;

                    string reply = fullEmoji + "\n\n**" + card.name + "** [ID: " + card.id + "]\n" + card.type.ToString() + " " + card.emoji + "\n";
                    if (canBattle == "1")
                    {
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
                        reply += "\n";
                    }
                    else
                    {
                        reply += "\n";
                    }
                    reply += description;
                    await ReplyAsync(reply);
                }
            }
            else
            {
                await ReplyAsync("```diff\n+ Card command:\n  card id - Get information about a specific card.\n```");
            }
        }

        [Command("admin"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task Admin(params String[] stringArray)
        {
            if (Context.Guild.Id == CurrentData.settings.adminServerID)
            {
                IEmote tick = new Emoji("\U00002705");
                IEmote noEntry = new Emoji("\U000026D4");
                switch (stringArray[0])
                {
                    default:
                        await Context.Message.AddReactionAsync(noEntry);
                        break;
                    case "help":
                        await ReplyAsync("```diff\n+ Admin Arguments:\n  help\n  save\n  load\n  commandsoff\n  commandson\n  maintenancetrue\n  maintenancefalse\n  maintenancetext <text>\n  shutdown\n  kill\n  test - Test announcemnts\n  announce important/notify\n```");
                        break;
                    case "save":
                        Functions.SaveAllData();
                        await Context.Message.AddReactionAsync(tick);
                        break;
                    case "load":
                        Functions.LoadAllData();
                        await Context.Message.AddReactionAsync(tick);
                        break;
                    case "commandsoff":
                        if (!CurrentData.commandsEnabled) { await ReplyAsync("```diff\n- Commands are already off.\n```"); return; }
                        CurrentData.commandsEnabled = false;
                        await CurrentData.client.SetStatusAsync(UserStatus.DoNotDisturb);
                        await Context.Message.AddReactionAsync(tick);
                        break;
                    case "commandson":
                        if (CurrentData.commandsEnabled) { await ReplyAsync("```diff\n- Commands are already on.\n```"); return; }
                        CurrentData.commandsEnabled = true;
                        await CurrentData.client.SetStatusAsync(UserStatus.Online);
                        await Context.Message.AddReactionAsync(tick);
                        break;
                    case "maintenancetrue":
                        if (CurrentData.maintenance) { await ReplyAsync("```diff\n- Maintenance is already true.\n```"); return; }
                        CurrentData.maintenance = true;
                        await Context.Message.AddReactionAsync(tick);
                        break;
                    case "maintenancefalse":
                        if (!CurrentData.maintenance) { await ReplyAsync("```diff\n- Maintenance is already false.\n```"); return; }
                        CurrentData.maintenance = false;
                        await Context.Message.AddReactionAsync(tick);
                        break;
                    case "maintenancetext":
                        string str = "";
                        foreach (string _str in stringArray)
                        {
                            str += _str + " ";
                        }
                        CurrentData.statusText = str;
                        await Context.Message.AddReactionAsync(tick);
                        break;
                    case "shutdown":
                        Functions.SaveAllData();
                        Emote shtdwn = Emote.Parse("<:powerBtnOn:768910368474005545>");
                        await Context.Message.AddReactionAsync(shtdwn);
                        Environment.Exit(0);
                        break;
                    case "kill":
                        Emote kil = Emote.Parse("<:powerBtn:768907604176011314>");
                        await Context.Message.AddReactionAsync(kil);
                        Environment.Exit(0);
                        break;
                    case "test":
                        await Context.Message.AddReactionAsync(tick);
                        break;
                }
            }
        }
    }
}
