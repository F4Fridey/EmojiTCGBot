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
using EmojiTCG.ServerData;

namespace EmojiTCG.Modules
{
    public class Commands: ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            await ReplyAsync("```diff\n+ All EmojiTCG Commands:\n  help - All these commands.\n  status - Status of the bot.\n  inventory/inv (@mention) - Check your inventory. Only add @mention to check another inventory.\n  shop - Browse the shop.\n  open ID = Open a booster pack with a specific id.\n  daily - Get your daily reward.\n  melt cardID - Melt a card into a coin.\n card ID - Check a cards stats.\n  deck - Command for your decks.\n  battle - Command to battle.\n  invite - Invite the bot to your server with this.\n- Administrator Commands\n  setup - Setup the server.\n  serverid - Get the server ID of this server.\n  unlink (serverid) - Only add server ID only if servers are linked to this server.\n```");
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

        [Command("serverid")]
        public async Task ServerId()
        {
            await ReplyAsync("This Servers ID: " + Context.Guild.Id.ToString());
        }

        [Command("setup"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetUp()
        {
            if (CurrentData.commandsEnabled)
            {
                bool alreadySettingUp = false;
                bool alreadySetUp = false;
                foreach (SettingUp _settingUp in CurrentData.settingUpList)
                {
                    if (_settingUp.serverId == Context.Guild.Id)
                    {
                        alreadySettingUp = true;
                        break;
                    }
                }
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)//!!!!!!
                    {
                        alreadySetUp = true;
                        break;
                    }
                }
                if (!alreadySettingUp && !alreadySetUp)
                {
                    SettingUp settingUp = new SettingUp()
                    {
                        serverId = Context.Guild.Id,
                        state = 0,
                        user = Context.User,
                        channel = Context.Channel,
                        settingUp = true
                    };
                    CurrentData.settingUpList.Add(settingUp);
                    ServerData.ServerData newServerData = new ServerData.ServerData()
                    {
                        serverId = Context.Guild.Id,
                        tradeRequests = new List<TradeRequest>(),
                        userData = new List<UserData>(),
                        battles = new List<Battle>(),
                        leaderboard = new List<LeaderBoardPlace>(),
                        allowNotifications = true,
                        allowAnnouncements = true
                    };
                    CurrentData.serverData.Add(newServerData);
                    await ReplyAsync("```diff\n+ Set up process begun, reply to all questions.\n```\n```diff\nWould you like to link this server to another server (if not, repy with =no)? Do =serverid in the other ser to get the ID then answer with =linkto THATID\n```");
                }
                else
                {
                    await ReplyAsync("```diff\n- You are already setting this server up or it has already been set up.\n```");
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use !status to check the bot status.\n```");
            }
        }

        [Command("no")]
        public async Task No()
        {
            if (CurrentData.commandsEnabled)
            {
                bool settingUpBool = false;
                SettingUp settingUp = new SettingUp();
                foreach (SettingUp _settingup in CurrentData.settingUpList)
                {
                    if (_settingup.serverId == Context.Guild.Id && _settingup.settingUp)
                    {
                        settingUpBool = true;
                        settingUp = _settingup;
                        break;
                    }
                }
                if (settingUpBool)
                {
                    switch (settingUp.state)
                    {
                        default:

                            break;
                        case 0:
                            settingUp.state += 1;
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].linkedToThisServerIds = new List<ulong>();
                                    CurrentData.serverData[i].userData = new List<ServerData.UserData>();
                                    break;
                                }
                            }
                            await ReplyAsync("```diff\nNot linked. You can change this later if need be.\n```\n```diff\nAre servers linked to this server? If yes get their server ids and do =linkto THATID. If multiple servers, continue doing =linkto THOSEIDS until your done. When done, do =no\n```");
                            break;
                        case 1:
                            /*for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].prefix = "=";
                                    CurrentData.serverData[i].setup = true;
                                    foreach (SettingUp _settingup in CurrentData.settingUpList)
                                    {
                                        if (_settingup.serverId == Context.Guild.Id)
                                        {
                                            CurrentData.settingUpList.Remove(_settingup);
                                            break;
                                        }
                                    }
                                }
                            }*/
                            settingUp.state += 1;
                            await ReplyAsync("```diff\n+ Link data set up completed.\n```\n```diff\nDo you want your members to recieve announcements DMs regarding the Emoji TCG bot? Such as \"New update!\" and \"X mas event now live!\" This is defaulted to true and helps keep users using the bot. You can change this later on aswell.\nUsers can turn this off for themselves if this is enabled.\n- Its recommended to reply with =yes\n+ However you may disable it with =no\n```");
                            /*Functions.SaveAllData();
                            await ReplyAsync("```diff\n+ Set up complete. Have fun :D\n```");
                            var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                            await adminChannel.SendMessageAsync("Bot setup in new server: " + Context.Guild.Name);*/
                            break;
                        case 2:
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].allowAnnouncements = false;
                                    break;
                                }
                            }
                            settingUp.state += 1;
                            await ReplyAsync("```diff\n+ Users will not recieve announcement DMs from the bot.\n```\n```diff\nDo you want your members to recieve notification DMs regarding the Emoji TCG bot? Such as \"New Item in Shop!\" or \"New daily prizes for this month!\" This is defaulted to true and helps keep users using the bot. You can change this later on aswell.\nUsers can turn this off for themselves if this is enabled.\n- Its recommended to reply with =yes\n+ However you may disable it with =no\n```");
                            break;
                        case 3:
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].allowNotifications = false;
                                    break;
                                }
                            }
                            settingUp.state += 1;
                            await ReplyAsync("```diff\n+ Users will not recieve announcement DMs from the bot.\n```");
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].prefix = "=";
                                    CurrentData.serverData[i].setup = true;
                                    foreach (SettingUp _settingup in CurrentData.settingUpList)
                                    {
                                        if (_settingup.serverId == Context.Guild.Id)
                                        {
                                            CurrentData.settingUpList.Remove(_settingup);
                                            break;
                                        }
                                    }
                                }
                            }
                            Functions.SaveAllData();
                            await ReplyAsync("```diff\n+ Set up complete. Have fun :D\n```");
                            var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                            await adminChannel.SendMessageAsync("Bot setup in new server: " + Context.Guild.Name);
                            break;
                    }
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use =status to check the bot status.\n```");
            }
        }

        [Command("yes")]
        public async Task Yes()
        {
            if (CurrentData.commandsEnabled)
            {
                bool settingUpBool = false;
                SettingUp settingUp = new SettingUp();
                foreach (SettingUp _settingup in CurrentData.settingUpList)
                {
                    if (_settingup.serverId == Context.Guild.Id && _settingup.settingUp)
                    {
                        settingUpBool = true;
                        settingUp = _settingup;
                        break;
                    }
                }
                if (settingUpBool)
                {
                    switch (settingUp.state)
                    {
                        default:

                            break;
                        case 2:
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].allowAnnouncements = true;
                                    break;
                                }
                            }
                            settingUp.state += 1;
                            await ReplyAsync("```diff\n+ Users will recieve announcement DMs from the bot if they have them enabled.\n```\n```diff\nDo you want your members to recieve notification DMs regarding the Emoji TCG bot? Such as \"New Item in Shop!\" or \"New daily prizes for this month!\" This is defaulted to true and helps keep users using the bot. You can change this later on aswell.\nUsers can turn this off for themselves if this is enabled.\n- Its recommended to reply with =yes\n+ However you may disable it with =no\n```");
                            break;
                        case 3:
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].allowNotifications = true;
                                    break;
                                }
                            }
                            settingUp.state += 1;
                            await ReplyAsync("```diff\n+ Users will recieve announcement DMs from the bot if they have them enabled.\n```");
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].prefix = "=";
                                    CurrentData.serverData[i].setup = true;
                                    foreach (SettingUp _settingup in CurrentData.settingUpList)
                                    {
                                        if (_settingup.serverId == Context.Guild.Id)
                                        {
                                            CurrentData.settingUpList.Remove(_settingup);
                                            break;
                                        }
                                    }
                                }
                            }
                            Functions.SaveAllData();
                            await ReplyAsync("```diff\n+ Set up complete. Have fun :D\n```");
                            var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                            await adminChannel.SendMessageAsync("Bot setup in new server: " + Context.Guild.Name);
                            break;
                    }
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use =status to check the bot status.\n```");
            }
        }

        [Command("linkto")]
        public async Task LinkTo(params String[] stringArray)
        {
            if (CurrentData.commandsEnabled)
            {
                bool settingUpBool = false;
                SettingUp settingUp = new SettingUp();
                foreach (SettingUp _settingup in CurrentData.settingUpList)
                {
                    if (_settingup.serverId == Context.Guild.Id && _settingup.settingUp)
                    {
                        settingUpBool = true;
                        settingUp = _settingup;
                        break;
                    }
                }
                if (settingUpBool)
                {
                    if (settingUp.state == 0)
                    {
                        ulong _serverid;
                        bool parsed = ulong.TryParse(stringArray[0], out _serverid);
                        if (parsed)
                        {
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].linkedToThisServerIds = new List<ulong>();
                                    CurrentData.serverData[i].linkedToServerId = _serverid;
                                    CurrentData.serverData[i].userData = new List<ServerData.UserData>();
                                    break;
                                }
                            }
                            settingUp.state += 200;
                            Functions.SaveAllData();
                            await ReplyAsync("```diff\n+ Succesfully linked.\n```");
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].prefix = "=";
                                    CurrentData.serverData[i].setup = true;
                                    foreach (SettingUp _settingup in CurrentData.settingUpList)
                                    {
                                        if (_settingup.serverId == Context.Guild.Id)
                                        {
                                            CurrentData.settingUpList.Remove(_settingup);
                                            break;
                                        }
                                    }
                                }
                            }
                            settingUp.state += 1;
                            Functions.SaveAllData();
                            await ReplyAsync("```diff\n+ Set up complete. Have fun :D\n```");
                            var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                            await adminChannel.SendMessageAsync("Bot setup in new server: " + Context.Guild.Name);
                        }
                        else
                        {
                            await ReplyAsync("```diff\n- Error, that is not a number. Try again.\n```");
                        }
                    }
                    else if (settingUp.state == 1)
                    {
                        ulong _serverid;
                        bool parsed = ulong.TryParse(stringArray[0], out _serverid);
                        if (parsed)
                        {
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                                {
                                    CurrentData.serverData[i].linkedToThisServerIds.Add(_serverid);
                                    break;
                                }
                            }
                            await ReplyAsync("```diff\n+ Succesfully linked.\n  Do it again to link another server or do =no to move on.\n```");//next question here
                        }
                        else
                        {
                            await ReplyAsync("```diff\n- Error, that is not a number. Try again.\n```");
                        }
                    }
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use =status to check the bot status.\n```");
            }
        }

        [Command("unlink")]
        public async Task Unlink(params string[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                for (int i = 0; i < CurrentData.serverData.Count; i++)
                {
                    if (CurrentData.serverData[i].serverId == Context.Guild.Id)
                    {
                        if (CurrentData.serverData[i].linkedToServerId != 0)
                        {
                            CurrentData.serverData.Remove(CurrentData.serverData[i]);
                            await ReplyAsync("```diff\n+ Unlinked.\n  Users will no longer be able to access their inventories from the other server here.\n- You need to re-setup this server with =setup\n```");
                        }
                        else if (CurrentData.serverData[i].linkedToThisServerIds.Any())
                        {
                            if (!args.Any())
                            {
                                await ReplyAsync("```diff\n- This server is not linked to any servers\n  However others may be linked to this server. To remove them please use their server id as the first argument.\n  EG: =unlink 8383247982479827432\n```");
                            }
                            else
                            {
                                ulong _id = 0;
                                bool _parsed = ulong.TryParse(args[0], out _id);
                                if (_id <= 0)
                                    _parsed = false;
                                if (_parsed)
                                {
                                    if (CurrentData.serverData[i].linkedToThisServerIds.Contains(_id))
                                    {
                                        CurrentData.serverData[i].linkedToThisServerIds.Remove(_id);
                                        await ReplyAsync("```diff\n+ Unlinked.\n  Users from that server will no longer be able to access their inventories here.\n```");
                                    }
                                    else
                                        await ReplyAsync("```diff\n- That server isnt linked to this server.\n```");
                                }
                                else
                                    await ReplyAsync("```diff\n- The 1st argument must be a number.\n```");
                            }
                        }
                        else
                        {
                            await ReplyAsync("```diff\n- This server has no servers to unlink.\n```");
                        }
                        break;
                    }
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use !status to check the bot status.\n```");
            }
        }

        [Command("shop")]
        public async Task Shop(params String[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(Context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                    break;
                                }
                                else
                                {
                                    contextGuildId = Context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = Context.Guild.Id;
                        }
                        break;
                    }
                }

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
                                for (int j = 0; j < CurrentData.serverData.Count; j++)
                                {
                                    if (CurrentData.serverData[j].serverId == contextGuildId)
                                    {
                                        for (int i = 0; i < CurrentData.serverData[j].userData.Count; i++)
                                        {
                                            if (CurrentData.serverData[j].userData[i].userId == Context.User.Id)
                                            {
                                                if (CurrentData.serverData[j].userData[i].coins >= shopSlot.price)
                                                {
                                                    IEmote emote = new Emoji("\U00002705");
                                                    switch (shopSlot.type)
                                                    {
                                                        case ShopSlotType.BOOSTER:
                                                            CurrentData.serverData[j].userData[i].coins -= shopSlot.price;
                                                            CurrentData.serverData[j].userData[i].inventoryBoosters.Add(shopSlot.itemId);
                                                            await Context.Message.AddReactionAsync(emote);
                                                            break;
                                                        case ShopSlotType.CARD:
                                                            CurrentData.serverData[j].userData[i].coins -= shopSlot.price;
                                                            CurrentData.serverData[j].userData[i].inventoryCards.Add(shopSlot.itemId);
                                                            await Context.Message.AddReactionAsync(emote);
                                                            break;
                                                        case ShopSlotType.BADGE:
                                                            CurrentData.serverData[j].userData[i].coins -= shopSlot.price;
                                                            CurrentData.serverData[j].userData[i].inventoryBadges.Add(shopSlot.itemId);
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
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(Context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                    break;
                                }
                                else
                                {
                                    contextGuildId = Context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = Context.Guild.Id;
                        }
                        break;
                    }
                }

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
                                
                                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                                {
                                    if (_serverData.serverId == contextGuildId)
                                    {
                                        foreach (UserData _userData in _serverData.userData)
                                        {
                                            if (_userData.userId == userID)
                                            {
                                                userExistsInData = true;
                                                break;
                                            }
                                        }
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
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == contextGuildId)
                    {
                        foreach (ServerData.UserData _userdata in _serverData.userData)
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
                        foreach (Card _card in cards)
                        {
                            if (_card.type == (CardType)i)
                            {
                                sortedCards.Add(_card);
                            }
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
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(Context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                    break;
                                }
                                else
                                {
                                    contextGuildId = Context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = Context.Guild.Id;
                        }
                        break;
                    }
                }

                if (!args.Any())
                {
                    await ReplyAsync("```diff\n+ Trade command:\n  TRADE @MENTION yourcardid,yourcardid,yourcoinamountC theircardid,theircardid,theircoinamountC\n  You dont need to have specificaly 2 canrd IDs and 1 coin amount, you can change that.\n  Example: trade @user 23,45 100c = Youll give user card 23 and 45 for 100 coins.\n```");//trade help
                }
                else
                {
                    if (args[0].Contains("<@!"))
                    {
                        bool hasRequestAlready = false;
                        foreach (ServerData.ServerData _serverdata in CurrentData.serverData)
                        {
                            if (_serverdata.serverId == contextGuildId)
                            {
                                foreach (TradeRequest _tradeReq in _serverdata.tradeRequests)
                                {
                                    if (_tradeReq.offererId == Context.User.Id)
                                    {
                                        hasRequestAlready = true;
                                        break;
                                    }
                                }
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
                                foreach (ServerData.ServerData _serverdata in CurrentData.serverData)
                                {
                                    if (_serverdata.serverId == contextGuildId)
                                    {
                                        foreach (TradeRequest _tradeReq in _serverdata.tradeRequests)
                                        {
                                            if (_tradeReq.accepterId == id)
                                            {
                                                heHasReq = true;
                                                break;
                                            }
                                        }
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
                                UserData offererUserData = new UserData();
                                UserData accepterUserData = new UserData();
                                foreach (ServerData.ServerData _serverdata in CurrentData.serverData)
                                {
                                    if (_serverdata.serverId == contextGuildId)
                                    {
                                        foreach (ServerData.UserData _userdata in _serverdata.userData)
                                        {
                                            if (_userdata.userId == Context.User.Id)
                                            {
                                                offererUserData = _userdata;
                                                break;
                                            }
                                        }
                                        foreach (ServerData.UserData _userdata in _serverdata.userData)
                                        {
                                            if (_userdata.userId == id)
                                            {
                                                accepterUserData = _userdata;
                                                break;
                                            }
                                        }
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
                                    }else if (str.ToLower().Contains("c"))
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
                                for (int f = 0; f < CurrentData.serverData.Count; f++)
                                {
                                    if (CurrentData.serverData[f].serverId == contextGuildId)
                                    {
                                        CurrentData.serverData[f].tradeRequests.Add(tradeReq);
                                        await ReplyAsync("```diff\n+ Trade Request made\n  The other must do =trade accept/reject```");
                                        return;
                                    }
                                }
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
                        for (int i = 0; i < CurrentData.serverData.Count; i++)
                        {
                            if (CurrentData.serverData[i].serverId == contextGuildId)
                            {
                                for (int j = 0; j < CurrentData.serverData[i].tradeRequests.Count; j++)
                                {
                                    if (CurrentData.serverData[i].tradeRequests[j].offererId == Context.User.Id)
                                    {
                                        hasRequest = true;
                                        CurrentData.serverData[i].tradeRequests.Remove(CurrentData.serverData[i].tradeRequests[j]);
                                        IEmote emote = new Emoji("\U00002705");
                                        await Context.Message.AddReactionAsync(emote);
                                        return;
                                    }
                                }
                                break;
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
                        for (int i = 0; i < CurrentData.serverData.Count; i++)
                        {
                            if (CurrentData.serverData[i].serverId == contextGuildId)
                            {
                                for (int j = 0; j < CurrentData.serverData[i].tradeRequests.Count; j++)
                                {
                                    if (CurrentData.serverData[i].tradeRequests[j].accepterId == Context.User.Id)
                                    {
                                        hasRequestToAccept = true;
                                        for (int k = 0; k < CurrentData.serverData[i].userData.Count; k++)
                                        {
                                            if (CurrentData.serverData[i].userData[k].userId == CurrentData.serverData[i].tradeRequests[j].offererId)
                                            {
                                                if (CurrentData.serverData[i].userData[k].coins < CurrentData.serverData[i].tradeRequests[j].offeredCoins)
                                                {
                                                    await ReplyAsync("```diff\n- The offerer no longer has the needed amount of coins for this trade.\n  Trade request automaticaly rejected.\n```");
                                                    CurrentData.serverData[i].tradeRequests.Remove(CurrentData.serverData[i].tradeRequests[j]);
                                                    return;
                                                }
                                            }else if (CurrentData.serverData[i].userData[k].userId == CurrentData.serverData[i].tradeRequests[j].accepterId)
                                            {
                                                if (CurrentData.serverData[i].userData[k].coins < CurrentData.serverData[i].tradeRequests[j].acceptedCoins)
                                                {
                                                    await ReplyAsync("```diff\n- The accepter no longer has the needed amount of coins for this trade.\n  Trade request automaticaly rejected.\n```");
                                                    CurrentData.serverData[i].tradeRequests.Remove(CurrentData.serverData[i].tradeRequests[j]);
                                                    return;
                                                }
                                            }

                                            if (CurrentData.serverData[i].userData[k].userId == CurrentData.serverData[i].tradeRequests[j].offererId)
                                            {
                                                if (CurrentData.serverData[i].tradeRequests[j].offeredCoins > 0)
                                                    CurrentData.serverData[i].userData[k].coins -= CurrentData.serverData[i].tradeRequests[j].offeredCoins;
                                                if (CurrentData.serverData[i].tradeRequests[j].acceptedCoins > 0)
                                                    CurrentData.serverData[i].userData[k].coins += CurrentData.serverData[i].tradeRequests[j].acceptedCoins;
                                                foreach (uint _cardid in CurrentData.serverData[i].tradeRequests[j].offererCardIds)
                                                {
                                                    for (int l = 0; l < CurrentData.serverData[i].userData[k].inventoryCards.Count; l++)
                                                    {
                                                        if (CurrentData.serverData[i].userData[k].inventoryCards[l] == _cardid)
                                                        {
                                                            CurrentData.serverData[i].userData[k].inventoryCards.Remove(CurrentData.serverData[i].userData[k].inventoryCards[l]);
                                                            break;
                                                        }
                                                    }
                                                }
                                                foreach (uint _cardid in CurrentData.serverData[i].tradeRequests[j].accepterCardIds)
                                                {
                                                    CurrentData.serverData[i].userData[k].inventoryCards.Add(_cardid);
                                                }
                                            }
                                            else if (CurrentData.serverData[i].userData[k].userId == CurrentData.serverData[i].tradeRequests[j].accepterId)
                                            {
                                                if (CurrentData.serverData[i].tradeRequests[j].acceptedCoins > 0)
                                                    CurrentData.serverData[i].userData[k].coins -= CurrentData.serverData[i].tradeRequests[j].acceptedCoins;
                                                if (CurrentData.serverData[i].tradeRequests[j].offeredCoins > 0)
                                                    CurrentData.serverData[i].userData[k].coins += CurrentData.serverData[i].tradeRequests[j].offeredCoins;
                                                foreach (uint _cardid in CurrentData.serverData[i].tradeRequests[j].accepterCardIds)
                                                {
                                                    for (int l = 0; l < CurrentData.serverData[i].userData[k].inventoryCards.Count; l++)
                                                    {
                                                        if (CurrentData.serverData[i].userData[k].inventoryCards[l] == _cardid)
                                                        {
                                                            CurrentData.serverData[i].userData[k].inventoryCards.Remove(CurrentData.serverData[i].userData[k].inventoryCards[l]);
                                                            break;
                                                        }
                                                    }
                                                }
                                                foreach (uint _cardid in CurrentData.serverData[i].tradeRequests[j].offererCardIds)
                                                {
                                                    CurrentData.serverData[i].userData[k].inventoryCards.Add(_cardid);
                                                }
                                            }
                                        }
                                        CurrentData.serverData[i].tradeRequests.Remove(CurrentData.serverData[i].tradeRequests[j]);
                                        IEmote emote = new Emoji("\U00002705");
                                        await Context.Message.AddReactionAsync(emote);
                                        return;
                                    }
                                }
                                break;
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
                        for (int i = 0; i < CurrentData.serverData.Count; i++)
                        {
                            if (CurrentData.serverData[i].serverId == contextGuildId)
                            {
                                for (int j = 0; j < CurrentData.serverData[i].tradeRequests.Count; j++)
                                {
                                    if (CurrentData.serverData[i].tradeRequests[j].accepterId == Context.User.Id)
                                    {
                                        hasRequestToReject = true;
                                        CurrentData.serverData[i].tradeRequests.Remove(CurrentData.serverData[i].tradeRequests[j]);
                                        IEmote emote = new Emoji("\U000026D4");
                                        await Context.Message.AddReactionAsync(emote);
                                        return;
                                    }
                                }
                                break;
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
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(Context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                    break;
                                }
                                else
                                {
                                    contextGuildId = Context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = Context.Guild.Id;
                        }
                        break;
                    }
                }

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
                        foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                        {
                            if (_serverData.serverId == contextGuildId)
                            {
                                foreach (ServerData.UserData _userData in _serverData.userData)
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
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == contextGuildId)
                                {
                                    for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                    {
                                        if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                                        {
                                            foreach (Card _card in cardUget)
                                            {
                                                CurrentData.serverData[i].userData[j].inventoryCards.Add(_card.id);
                                            }
                                            CurrentData.serverData[i].userData[j].inventoryBoosters.Remove(boosterIdToOpen);
                                            break;
                                        }
                                    }
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
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(Context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                    break;
                                }
                                else
                                {
                                    contextGuildId = Context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = Context.Guild.Id;
                        }
                        break;
                    }
                }

                bool userExists = false;
                for (int i = 0; i < CurrentData.serverData.Count; i++)
                {
                    if (CurrentData.serverData[i].serverId == contextGuildId)
                    {
                        for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                        {
                            if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                            {
                                userExists = true;
                                if (CurrentData.serverData[i].userData[j].lastDaily == null)
                                {
                                    CurrentData.serverData[i].userData[j].lastDaily = new DateTime();
                                }
                                if (CurrentData.serverData[i].userData[j].lastDaily.Day != DateTime.Now.Day)
                                {
                                    string coins = "";
                                    string dailyStreak = "";
                                    if (CurrentData.serverData[i].userData[j].lastDaily.Date == DateTime.Now.AddDays(-1).Date)
                                    {
                                        CurrentData.serverData[i].userData[j].dailyStreak++;
                                        dailyStreak = "You daily streak is now " + CurrentData.serverData[i].userData[j].dailyStreak + "!";
                                    }
                                    else
                                    {
                                        CurrentData.serverData[i].userData[j].dailyStreak = 0;
                                        dailyStreak = "Your daily streak was reset D:";
                                    }
                                    string text = "You have claimed your daily reward!\n\n";
                                    //dailyStreak prizes here
                                    try
                                    {
                                        if (CurrentData.dailyMonthPrizes.Any() && CurrentData.serverData[i].userData[j].dailyMonthStreak < CurrentData.dailyMonthPrizes[0].prizesPerDay.Count)
                                        {
                                            bool noMonth = false;
                                            if (CurrentData.dailyMonthPrizes[0].month != DateTime.Now.Month)
                                            {
                                                Console.WriteLine("Moving to next month daily prizes.");
                                                CurrentData.dailyMonthPrizes.RemoveAt(0);
                                                for (int k = 0; k < CurrentData.serverData.Count; k++)
                                                {
                                                    for (int l = 0; l < CurrentData.serverData[k].userData.Count; l++)
                                                    {
                                                        CurrentData.serverData[k].userData[l].dailyMonthStreak = 0;
                                                    }
                                                }
                                                if (!CurrentData.dailyMonthPrizes.Any())
                                                    noMonth = true;
                                                if (noMonth)
                                                {
                                                    var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                                                    await adminChannel.SendMessageAsync("<@!350279720144863244> Theres no month prizes!!");
                                                }
                                            }
                                            if (!noMonth)
                                            {
                                                CurrentData.serverData[i].userData[j].dailyMonthStreak += 1;
                                                int currentDayPos = CurrentData.serverData[i].userData[j].dailyMonthStreak - 1;
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
                                                        CurrentData.serverData[i].userData[j].coins += _prizes[currentDayPos].coinAmount;
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
                                                        CurrentData.serverData[i].userData[j].inventoryCards.Add(card.id);
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
                                                        CurrentData.serverData[i].userData[j].inventoryBoosters.Add(booster.id);
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
                                                        CurrentData.serverData[i].userData[j].inventoryBadges.Add(badge.id);
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
                                                for (int k = 0; k < CurrentData.serverData.Count; k++)
                                                {
                                                    if (CurrentData.serverData[k].serverId == contextGuildId)
                                                    {
                                                        for (int l = 0; l < CurrentData.serverData[k].userData.Count; l++)
                                                        {
                                                            CurrentData.serverData[k].userData[l].dailyMonthStreak = 0;
                                                        }
                                                        resetOccured = true;
                                                        break;
                                                    }
                                                }
                                                if (!CurrentData.dailyMonthPrizes.Any())
                                                    noMonth = true;
                                                if (noMonth)
                                                {
                                                    var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                                                    await adminChannel.SendMessageAsync("<@!350279720144863244> Theres no month prizes!!");
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
                                                CurrentData.serverData[i].userData[j].dailyMonthStreak += 1;
                                                int currentDayPos = CurrentData.serverData[i].userData[j].dailyMonthStreak - 1;
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
                                                        CurrentData.serverData[i].userData[j].coins += _prizes[currentDayPos].coinAmount;
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
                                                        CurrentData.serverData[i].userData[j].inventoryCards.Add(card.id);
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
                                                        CurrentData.serverData[i].userData[j].inventoryBoosters.Add(booster.id);
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
                                                        CurrentData.serverData[i].userData[j].inventoryBadges.Add(badge.id);
                                                        text += "**+** " + badge.emoji + "\n";
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        var adminChannel = CurrentData.client.GetChannel(767370970914619403) as IMessageChannel;
                                        await adminChannel.SendMessageAsync("<@!350279720144863244> Theres no month prizes!!");
                                    }
                                    //------
                                    if (CurrentData.serverData[i].userData[j].dailyStreak < 10)
                                    {
                                        CurrentData.serverData[i].userData[j].coins += 5;
                                        coins = "5";
                                    }
                                    else if (CurrentData.serverData[i].userData[j].dailyStreak >= 10 && CurrentData.serverData[i].userData[j].dailyStreak < 20)
                                    {
                                        CurrentData.serverData[i].userData[j].coins += 10;
                                        coins = "10";
                                    }
                                    else if (CurrentData.serverData[i].userData[j].dailyStreak >= 20)
                                    {
                                        CurrentData.serverData[i].userData[j].coins += 15;
                                        coins = "15";
                                    }
                                    CurrentData.serverData[i].userData[j].lastDaily = DateTime.Now;
                                    text += "**+ " + coins + "** :coin:\n\n*" + dailyStreak + "*";
                                    await ReplyAsync(text);
                                }
                                else
                                {
                                    await ReplyAsync("```diff\n- You can claim the daily bonus until tomorow.\n```");
                                }
                                break;
                            }
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
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(Context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                    break;
                                }
                                else
                                {
                                    contextGuildId = Context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = Context.Guild.Id;
                        }
                        break;
                    }
                }

                uint arg0 = 4000000000;
                bool parsed = false;
                if (args.Any())
                {
                    parsed = uint.TryParse(args[0], out arg0);
                    if (parsed)
                    {
                        for (int i = 0; i < CurrentData.serverData.Count; i++)
                        {
                            if (CurrentData.serverData[i].serverId == contextGuildId)
                            {
                                for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                {
                                    if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                                    {
                                        bool hasCard = false;
                                        foreach (uint _card in CurrentData.serverData[i].userData[j].inventoryCards)
                                        {
                                            if (_card == arg0)
                                            {
                                                CurrentData.serverData[i].userData[j].inventoryCards.Remove(_card);
                                                CurrentData.serverData[i].userData[j].coins += 1;
                                                hasCard = true;
                                                IEmote emote = new Emoji("\U0001FA99");
                                                IEmote emote2 = new Emoji("\U0001F525");
                                                await Context.Message.AddReactionAsync(emote2);
                                                await Context.Message.AddReactionAsync(emote);
                                                break;
                                            }
                                        }
                                        if (!hasCard)
                                        {
                                            await ReplyAsync("```diff\n- You dont have that card.\n```");
                                        }
                                        break;
                                    }
                                    break;
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        string str = "```diff\n- " + args[0] + " is not a valis card ID.\n```";
                        await ReplyAsync(str);
                    }
                }
                else
                {
                    await ReplyAsync("```diff\n+ Melt command:\n  Melt a card into a coin.\n  melt cardid\n```");
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use !status to check the bot status.\n```");
            }
        }

        [Command("deck")]
        public async Task Deck(params string[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(Context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                    break;
                                }
                                else
                                {
                                    contextGuildId = Context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = Context.Guild.Id;
                        }
                        break;
                    }
                }

                if (args.Any())
                {
                    switch (args[0])
                    {
                        default:
                            await ReplyAsync("```diff\n+ Deck command:\n  Use this command to manage your decks.\n- deck create name id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id\n  Use the above command to create a deck. Replace \"id\" with your card IDs. Note you need 20 cards per deck and the name can NOT HAVE SPACES, use _ instead.\n  You can only have 5 decks!\n- deck delete name\n  Delete a deck. Name cant have spaces.\n- deck list\n  List your decks.\n- deck view name\n  View the decks composition.\n```");
                            break;
                        case "create":
                            string name = "";
                            string idString = "";
                            List<uint> cardIds = new List<uint>();
                            try
                            {
                                name = args[1];
                                idString = args[2];
                            }
                            catch
                            {
                                await ReplyAsync("```diff\n- Incorrect usage.\n```");
                                return;
                            }
                            List<string> strs = idString.Split(',').ToList();
                            List<uint> invIds = new List<uint>();
                            foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                            {
                                if (_serverData.serverId == contextGuildId)
                                {
                                    foreach (UserData _userData in _serverData.userData)
                                    {
                                        if (_userData.userId == Context.User.Id)
                                        {
                                            invIds = new List<uint>(_userData.inventoryCards);
                                            if (_userData.decks.Count > 4)
                                            {
                                                await ReplyAsync("```diff\n- You already have 5 decks.\n```");
                                                return;
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            foreach (string str in strs)
                            {
                                uint id = 4000000000;
                                bool parsed = uint.TryParse(str, out id);
                                if (id == 4000000000)
                                    parsed = false;
                                if (parsed)
                                {
                                    bool hasCard = false;
                                    foreach (uint _card in invIds)
                                    {
                                        if (_card == id)
                                        {
                                            hasCard = true;
                                            invIds.Remove(id);
                                            break;
                                        }
                                    }
                                    if (hasCard)
                                    {
                                        cardIds.Add(id);
                                    }
                                    else
                                    {
                                        await ReplyAsync("```diff\n- You dont have one or mulitple of the cards specified.\n```");
                                        return;
                                    }
                                }
                                else
                                {
                                    await ReplyAsync("```diff\n- One ID is not a number.\n```");
                                    return;
                                }
                            }
                            if (cardIds.Count != 20)
                            {
                                
                                await ReplyAsync("```diff\n- You need to have 20 cards in a deck.\n```");
                                return;
                            }
                            Deck deck = new Deck()
                            {
                                name = name,
                                cards = cardIds
                            };
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == contextGuildId)
                                {
                                    for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                    {
                                        if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                                        {
                                            CurrentData.serverData[i].userData[j].decks.Add(deck);
                                            IEmote emote = new Emoji("\U00002705");
                                            await Context.Message.AddReactionAsync(emote);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        case "delete":
                            string _name = "";
                            try
                            {
                                _name = args[1];
                            }
                            catch
                            {
                                await ReplyAsync("```diff\n- Incorrect usage.\n```");
                            }
                            bool found = false;
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == contextGuildId)
                                {
                                    for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                    {
                                        if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                                        {
                                            for (int k = 0; k < CurrentData.serverData[i].userData[j].decks.Count; k++)
                                            {
                                                if (CurrentData.serverData[i].userData[j].decks[k].name == _name)
                                                {
                                                    found = true;
                                                    CurrentData.serverData[i].userData[j].decks.Remove(CurrentData.serverData[i].userData[j].decks[k]);
                                                    IEmote emote = new Emoji("\U00002705");
                                                    await Context.Message.AddReactionAsync(emote);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                    break;
                                }
                            }
                            if (!found)
                            {
                                await ReplyAsync("```diff\n- Could not find that deck.\n```");
                            }
                            break;
                        case "list":
                            string reply = "__**" + Context.User.Username + "'s Decks:**__\n══════════════════\n";
                            List<Deck> decks = new List<Deck>();
                            foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                            {
                                if (_serverData.serverId == contextGuildId)
                                {
                                    foreach (UserData _userData in _serverData.userData)
                                    {
                                        if (_userData.userId == Context.User.Id)
                                        {
                                            decks = _userData.decks;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            int num = 1;
                            foreach (Deck _deck in decks)
                            {
                                reply += "**" + num + "** " + _deck.name + "\n";
                                num += 1;
                            }
                            reply += "══════════════════\n";
                            await ReplyAsync(reply);
                            break;
                        case "view":
                            string __name = "";
                            try
                            {
                                __name = args[1];
                            }
                            catch
                            {
                                await ReplyAsync("```diff\n- Incorrect usage.\n```");
                            }
                            bool __found = false;
                            Deck __deck = new Deck();
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == contextGuildId)
                                {
                                    for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                    {
                                        if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                                        {
                                            for (int k = 0; k < CurrentData.serverData[i].userData[j].decks.Count; k++)
                                            {
                                                if (CurrentData.serverData[i].userData[j].decks[k].name == __name)
                                                {
                                                    __found = true;
                                                    __deck = CurrentData.serverData[i].userData[j].decks[k];
                                                }
                                            }
                                        }
                                        break;
                                    }
                                    break;
                                }
                            }
                            if (!__found)
                            {
                                await ReplyAsync("```diff\n- Could not find that deck.\n```");
                                return;
                            }
                            string __reply = "__**" + __deck.name + ":**__\n══════════════════\n";
                            foreach (uint __id in __deck.cards)
                            {
                                foreach (Card __card in CurrentData.cards)
                                {
                                    if (__card.id == __id)
                                    {
                                        __reply += "**" + __id + "** " + __card.name + "\n";
                                        break;
                                    }
                                }
                            }
                            __reply += "══════════════════";
                            await ReplyAsync(__reply);
                            break;
                    }
                }
                else
                {
                    await ReplyAsync("```diff\n+ Deck command:\n  Use this command to manage your decks.\n- deck create name id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id,id\n  Use the above command to create a deck. Replace \"id\" with your card IDs. Note you need 20 cards per deck and the name can NOT HAVE SPACES, use _ instead.\n  You can only have 5 decks!\n- deck delete name\n  Delete a deck. Name cant have spaces.\n- deck list\n  List your decks.\n- deck view name\n  View the decks composition.\n```");
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

        [Command("battle")]
        public async Task Battle(params string[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(Context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                    break;
                                }
                                else
                                {
                                    contextGuildId = Context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = Context.Guild.Id;
                        }
                        break;
                    }
                }
                if (!args.Any())
                {
                    await ReplyAsync("```diff\n+ Battle commands:\n  battle @mention deckName - Battle person with specific deck\n  battle cancel - Cancel request/forfeit\n  battle accept deckName - Accept battle request with specific deck\n  battle reject - Reject battle request\n```");
                }
                else
                {
                    switch (args[0])
                    {
                        default:
                            if (args[0].Contains("<@!"))
                            {
                                bool hasBattleAlready = false;
                                foreach (ServerData.ServerData _serverdata in CurrentData.serverData)
                                {
                                    if (_serverdata.serverId == contextGuildId)
                                    {
                                        foreach (Battle _battle in _serverdata.battles)
                                        {
                                            if (_battle.player1 == Context.User.Id || _battle.player2 == Context.User.Id)
                                            {
                                                hasBattleAlready = true;
                                                await ReplyAsync("```diff\n- You are already in a battle or someone has sent you a battle request.\n  Do battle cancel to cancel your current battle or battle reject to reject a battle request.\n```");
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                if (!hasBattleAlready)
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
                                    bool parsed = ulong.TryParse(idString, out id);
                                    if (parsed)
                                    {
                                        bool hasABattle = false;
                                        foreach (ServerData.ServerData _serverdata in CurrentData.serverData)
                                        {
                                            if (_serverdata.serverId == contextGuildId)
                                            {
                                                foreach (Battle _battle in _serverdata.battles)
                                                {
                                                    if (_battle.player1 == id || _battle.player2 == id)
                                                    {
                                                        hasABattle = true;
                                                        await ReplyAsync("```diff\n- That user already is in a battle or someone has sent them a battle request.\n  They must do battle cancel to cancel their current battle or battle reject to reject a battle request.\n```");
                                                        break;
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                        if (!hasABattle)
                                        {
                                            Battle battleReq = new Battle
                                            {
                                                player1 = Context.User.Id,
                                                player2 = id,
                                                turn = 0,
                                                player1deck = new BattleDeck() { battleCards = new List<BattleCard>(), itemOtherCards = new List<ItemOtherCard>() },
                                                player2deck = new BattleDeck() { battleCards = new List<BattleCard>(), itemOtherCards = new List<ItemOtherCard>() },
                                                player2accepted = false,
                                                player1activeCard = -1,
                                                player2activeCard = -1,
                                                player1activeSupportCard = new int[2],
                                                player2activeSupportCard = new int[2],
                                                player1deadCards = new List<int>(),
                                                player2deadCards = new List<int>()
                                            };
                                            string deckName = "";
                                            try { deckName = args[1]; }
                                            catch { await ReplyAsync("```diff\n- 2nd Argument invalid.\n```"); return; }
                                            bool hasDeck = false;
                                            Deck deck = new Deck();
                                            foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                                            {
                                                if (_serverData.serverId == contextGuildId)
                                                {
                                                    foreach (UserData _userData in _serverData.userData)
                                                    {
                                                        if (_userData.userId == Context.User.Id)
                                                        {
                                                            foreach (Deck _deck in _userData.decks)
                                                            {
                                                                if (_deck.name == deckName)
                                                                {
                                                                    hasDeck = true;
                                                                    deck = _deck;
                                                                    break;
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            if (!hasDeck)
                                            {
                                                await ReplyAsync("```diff\n- You dont have a deck with that name.\n```");
                                            }
                                            else
                                            {
                                                foreach (uint _cardId in deck.cards)
                                                {
                                                    foreach (Card _card in CurrentData.cards)
                                                    {
                                                        if (_cardId == _card.id)
                                                        {
                                                            List<string> noteList = _card.notes.Split('§').ToList();
                                                            string canBattle = noteList[0];
                                                            string type = noteList[1];
                                                            int health = int.Parse(noteList[2]);
                                                            if (canBattle == "1")
                                                            {
                                                                if (type == "DPS")
                                                                {
                                                                    battleReq.player1deck.battleCards.Add(new BattleCard() { card = _card, health = health, activeEffects = new List<string>() });
                                                                }else if (type == "ITEM")
                                                                {
                                                                    battleReq.player1deck.itemOtherCards.Add(new ItemOtherCard() { card = _card });
                                                                }
                                                                else
                                                                {
                                                                    await ReplyAsync("```diff\n- Unexpected error. Error code ERC001-" + _card.id + ". Please contact the bot administrator.\n```");//If this error occurs it means the cards note data is incorrect.
                                                                    return;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                string reply = "```diff\n- One of your cards in your deck can not be used in battle. Card: " + _card.name + " [" + _card.id + "]\n```";
                                                                await ReplyAsync(reply);
                                                                return;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                                for (int i = 0; i < CurrentData.serverData.Count; i++)
                                                {
                                                    if (CurrentData.serverData[i].serverId == contextGuildId)
                                                    {
                                                        CurrentData.serverData[i].battles.Add(battleReq);
                                                        await ReplyAsync("```diff\n+ Battle request sent.\n  Do battle accept deckName to accept the challence or battle reject to reject it.\n  The battle can always be canceld with battle cancel.\n```");
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await ReplyAsync("``diff\n- User invalid\n```");
                                    }
                                }
                            }
                            else
                                await ReplyAsync("```diff\n- Invalid argument.\n```");
                            break;
                        case "cancel":
                            bool hasBattle = false;
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == contextGuildId)
                                {
                                    for (int j = 0; j < CurrentData.serverData[i].battles.Count; j++)
                                    {
                                        if (CurrentData.serverData[i].battles[j].player1 == Context.User.Id || (CurrentData.serverData[i].battles[j].player2 == Context.User.Id && CurrentData.serverData[i].battles[j].player2accepted))
                                        {
                                            hasBattle = true;
                                            CurrentData.serverData[i].battles.Remove(CurrentData.serverData[i].battles[j]);
                                            IEmote emote = new Emoji("\U00002705");
                                            await Context.Message.AddReactionAsync(emote);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            if (!hasBattle)
                            {
                                await ReplyAsync("```diff\n- You have no Battles to cancel.\n```");
                            }
                            break;
                        case "reject":
                            bool hasBattleReq = false;
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == contextGuildId)
                                {
                                    for (int j = 0; j < CurrentData.serverData[i].battles.Count; j++)
                                    {
                                        if (CurrentData.serverData[i].battles[j].player2 == Context.User.Id)
                                        {
                                            hasBattleReq = true;
                                            CurrentData.serverData[i].battles.Remove(CurrentData.serverData[i].battles[j]);
                                            IEmote emote = new Emoji("\U00002705");
                                            await Context.Message.AddReactionAsync(emote);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            if (!hasBattleReq)
                            {
                                await ReplyAsync("```diff\n- You have no Battle Requests to reject.\n```");
                            }
                            break;
                        case "accept":
                            bool hasBattleRequest = false;
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                if (CurrentData.serverData[i].serverId == contextGuildId)
                                {
                                    for (int j = 0; j < CurrentData.serverData[i].battles.Count; j++)
                                    {
                                        if (CurrentData.serverData[i].battles[j].player2 == Context.User.Id && !CurrentData.serverData[i].battles[j].player2accepted)
                                        {
                                            hasBattleRequest = true;
                                            string deckname = "";
                                            try { deckname = args[1]; }
                                            catch { await ReplyAsync("```diff\n- 2nd Argument invalid. Please name the deck you want to use.\n```"); return; }
                                            Deck deck = new Deck();
                                            bool hasTheDeck = false;
                                            foreach (UserData _userdata in CurrentData.serverData[i].userData)
                                            {
                                                if (_userdata.userId == Context.User.Id)
                                                {
                                                    foreach (Deck _deck in _userdata.decks)
                                                    {
                                                        if (_deck.name == deckname)
                                                        {
                                                            hasTheDeck = true;
                                                            deck = _deck;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            if (!hasTheDeck)
                                            {
                                                await ReplyAsync("```diff\n- You dont have a deck named that.\n```");
                                                return;
                                            }
                                            else
                                            {
                                                foreach (uint _cardId in deck.cards)
                                                {
                                                    foreach (Card _card in CurrentData.cards)
                                                    {
                                                        if (_cardId == _card.id)
                                                        {
                                                            List<string> noteList = _card.notes.Split('§').ToList();
                                                            string canBattle = noteList[0];
                                                            string type = noteList[1];
                                                            int health = int.Parse(noteList[2]);
                                                            if (canBattle == "1")
                                                            {
                                                                if (type == "DPS")
                                                                {
                                                                    CurrentData.serverData[i].battles[j].player2deck.battleCards.Add(new BattleCard() { card = _card, health = health, activeEffects = new List<string>() });
                                                                }
                                                                else if (type == "ITEM")
                                                                {
                                                                    CurrentData.serverData[i].battles[j].player2deck.itemOtherCards.Add(new ItemOtherCard() { card = _card });
                                                                }
                                                                else
                                                                {
                                                                    await ReplyAsync("```diff\n- Unexpected error. Error code ERC001-" + _card.id + ". Please contact the bot administrator.\n```");//If this error occurs it means the cards note data is incorrect.
                                                                    return;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                string reply = "```diff\n- One of your cards in your deck can not be used in battle. Card: " + _card.name + " [" + _card.id + "]\n```";
                                                                await ReplyAsync(reply);
                                                                return;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            CurrentData.serverData[i].battles[j].player2accepted = true;
                                            CurrentData.serverData[i].battles[j].turn += 1;
                                            IEmote emote = new Emoji("\U00002705");
                                            await Context.Message.AddReactionAsync(emote);
                                            await ReplyAsync("**The battle is about to begin!**\n*During the battle you can do `=b` to list all commands.*\n__You'll only be able to do commands during your turn.__\nBefore we start both of you need to select a starter card. To do this each of you must do `=b select id`. You can also do `=b list` to see your available cards. (They will be DMed)\nPlease now select your starter cards.");
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            if (!hasBattleRequest)
                            {
                                await ReplyAsync("```diff\n- You have no Battle Requests to accept.\n```");
                            }
                            break;
                    }
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled. Use !status to check the bot status.\n```");
            }
        }

        [Command("b")]
        public async Task BattleActions(params string[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                ulong contextGuildId = 0;
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == Context.Guild.Id)
                    {
                        if (_serverData.linkedToServerId != 0)
                        {
                            foreach (ServerData.ServerData __serverData in CurrentData.serverData)
                            {
                                if (__serverData.linkedToThisServerIds.Contains(Context.Guild.Id))
                                {

                                    contextGuildId = _serverData.linkedToServerId;
                                    break;
                                }
                                else
                                {
                                    contextGuildId = Context.Guild.Id;
                                }
                            }
                        }
                        else
                        {
                            contextGuildId = Context.Guild.Id;
                        }
                        break;
                    }
                }
                bool inBattle = false;
                bool isTurn = false;
                Battle battle = new Battle();
                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                {
                    if (_serverData.serverId == contextGuildId)
                    {
                        foreach (Battle _battle in _serverData.battles)
                        {
                            if ((_battle.player1 == Context.User.Id || _battle.player2 == Context.User.Id) && _battle.player2accepted)
                            {
                                inBattle = true;
                                battle = _battle;
                            }
                            if (_battle.player1 == Context.User.Id && _battle.turn % 2 == 0 && inBattle)
                            {
                                isTurn = true;
                            }
                            else if (_battle.player2 == Context.User.Id && _battle.turn % 2 != 0 && inBattle)
                            {
                                isTurn = true;
                            }
                        }
                        break;
                    }
                }
                if (!args.Any())
                {
                    await ReplyAsync("```diff\n+ Battle action commands:\n  b list - List your deck (In DMs) and et card IDs\n  b select id - Select a card as your active card\n  b use itemId - Use one of you item cards\n  b attack 1,2,3 - Attack with the 1st, 2nd or 3rd attack\n```");
                    return;
                }
                switch (args[0])
                {
                    default:
                        await ReplyAsync("```diff\n- Unknown argument.\n```");
                        break;
                    case "list":
                        if (inBattle)
                        {
                            string reply = "**__Your DPS cards:__**\n";
                            await Context.User.SendMessageAsync(reply);
                            int incremental = 0;
                            if (battle.player1 == Context.User.Id)
                            {
                                reply = "";
                                for (int i = 0; i < battle.player1deck.battleCards.Count; i++)
                                {
                                    List<string> noteList = battle.player1deck.battleCards[i].card.notes.Split('§').ToList();
                                    string canBattle = noteList[0];
                                    string type = noteList[1];
                                    string health = noteList[2];
                                    string attacks = noteList[3];
                                    string fullEmoji = noteList[4];
                                    string description = noteList[5];

                                    reply += "**" + battle.player1deck.battleCards[i].card.name + "** __**[ID: " + i + "]**__\n" + battle.player1deck.battleCards[i].card.type.ToString() + " " + battle.player1deck.battleCards[i].card.emoji + "\n";
                                    if (canBattle == "1" && type == "DPS")
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
                                    }
                                    if (incremental > 9 || i == battle.player1deck.battleCards.Count - 1)
                                    {
                                        await Context.User.SendMessageAsync(reply);
                                        reply = "";
                                        incremental = 0;
                                    }
                                    else
                                    {
                                        incremental += 1;
                                    }
                                }
                                reply = "``` ```\n**__Your ITEM cards:__**";
                                await Context.User.SendMessageAsync(reply);
                                reply = "";
                                incremental = 0;
                                for (int i = 0; i < battle.player1deck.itemOtherCards.Count; i++)
                                {
                                    List<string> noteList = battle.player1deck.itemOtherCards[i].card.notes.Split('§').ToList();
                                    string canBattle = noteList[0];
                                    string type = noteList[1];
                                    string health = noteList[2];
                                    string attacks = noteList[3];
                                    string fullEmoji = noteList[4];
                                    string description = noteList[5];

                                    reply += "**" + battle.player1deck.itemOtherCards[i].card.name + "** __**[ID: " + (i + battle.player1deck.battleCards.Count) + "]**__\n" + battle.player1deck.itemOtherCards[i].card.type.ToString() + " " + battle.player1deck.itemOtherCards[i].card.emoji + "\n";
                                    if (canBattle == "1" && type == "ITEM")
                                    {
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
                                    if (incremental > 9 || i == battle.player1deck.itemOtherCards.Count - 1)
                                    {
                                        await Context.User.SendMessageAsync(reply);
                                        reply = "";
                                        incremental = 0;
                                    }
                                    else
                                    {
                                        incremental += 1;
                                    }
                                }
                            }
                            if (battle.player2 == Context.User.Id)
                            {
                                reply = "";
                                for (int i = 0; i < battle.player2deck.battleCards.Count; i++)
                                {
                                    List<string> noteList = battle.player2deck.battleCards[i].card.notes.Split('§').ToList();
                                    string canBattle = noteList[0];
                                    string type = noteList[1];
                                    string health = noteList[2];
                                    string attacks = noteList[3];
                                    string fullEmoji = noteList[4];
                                    string description = noteList[5];

                                    reply += "**" + battle.player2deck.battleCards[i].card.name + "** __**[ID: " + i + "]**__\n" + battle.player2deck.battleCards[i].card.type.ToString() + " " + battle.player2deck.battleCards[i].card.emoji + "\n";
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
                                    }
                                    if (incremental > 9 || i == battle.player2deck.battleCards.Count - 1)
                                    {
                                        await Context.User.SendMessageAsync(reply);
                                        reply = "";
                                        incremental = 0;
                                    }
                                    else
                                    {
                                        incremental += 1;
                                    }
                                }
                                reply = "``` ```\n**__Your ITEM cards:__**";
                                await Context.User.SendMessageAsync(reply);
                                reply = "";
                                for (int i = 0; i < battle.player2deck.itemOtherCards.Count; i++)
                                {
                                    List<string> noteList = battle.player2deck.itemOtherCards[i].card.notes.Split('§').ToList();
                                    string canBattle = noteList[0];
                                    string type = noteList[1];
                                    string health = noteList[2];
                                    string attacks = noteList[3];
                                    string fullEmoji = noteList[4];
                                    string description = noteList[5];

                                    reply += "**" + battle.player2deck.itemOtherCards[i].card.name + "** __**[ID: " + (i + battle.player2deck.battleCards.Count) + "]**__\n" + battle.player2deck.itemOtherCards[i].card.type.ToString() + " " + battle.player2deck.itemOtherCards[i].card.emoji + "\n";
                                    if (canBattle == "1" && type == "ITEM")
                                    {
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
                                    if (incremental > 9 || i == battle.player2deck.itemOtherCards.Count - 1)
                                    {
                                        await Context.User.SendMessageAsync(reply);
                                        reply = "";
                                        incremental = 0;
                                    }
                                    else
                                    {
                                        incremental += 1;
                                    }
                                }
                            }
                        }
                        else
                            await ReplyAsync("```diff\n- You are not in a battle currently.\n```");
                        break;
                    case "select":
                        string arg1 = "";
                        try { arg1 = args[1]; }catch { await ReplyAsync("```diff\n- Use the IDs found next to the name cards when doing =b list\n```"); return; }
                        int id = -1;
                        bool parsed = int.TryParse(arg1, out id);
                        if (parsed && id > -1 && id < 20)
                        {
                            Random rnd = new Random();
                            for (int a = 0; a < CurrentData.serverData.Count; a++)
                            {
                                if (CurrentData.serverData[a].serverId == contextGuildId)
                                {
                                    for (int b = 0; b < CurrentData.serverData[a].battles.Count; b++)
                                    {
                                        if (CurrentData.serverData[a].battles[b].player1 == Context.User.Id)
                                        {
                                            bool isBattleCard = false;
                                            for (int c = 0; c < CurrentData.serverData[a].battles[b].player1deck.battleCards.Count; c++)
                                            {
                                                if (c == id)
                                                {
                                                    isBattleCard = true;
                                                    break;
                                                }
                                            }
                                            if (CurrentData.serverData[a].battles[b].turn == 1 && isBattleCard)
                                            {
                                                CurrentData.serverData[a].battles[b].player1activeCard = id;
                                                IEmote tick = new Emoji("\U00002705");
                                                await Context.Message.AddReactionAsync(tick);
                                                if (CurrentData.serverData[a].battles[b].player2activeCard != -1 && CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                                {
                                                    uint startingTurn = (uint)rnd.Next(1, 3);
                                                    CurrentData.serverData[a].battles[b].turn = startingTurn + 1;
                                                    Console.WriteLine(">:( " + startingTurn + " " + CurrentData.serverData[a].battles[b].turn);
                                                    string str = "<@!";
                                                    if (CurrentData.serverData[a].battles[b].turn % 2 == 0)
                                                        str += CurrentData.serverData[a].battles[b].player1;
                                                    else
                                                        str += CurrentData.serverData[a].battles[b].player2;
                                                    str += ">";
                                                    await ReplyAsync("```diff\n+ Let the battle begin!\n```\n" + str + " Begins!\n Do `=b` for a list of action commands!");
                                                }
                                                break;
                                            }
                                            else if (isTurn && isBattleCard)
                                            {
                                                CurrentData.serverData[a].battles[b].player1activeCard = id;
                                                string battleGround = "__**The Battleground:**__\n\n";
                                                battleGround += "<@!" + CurrentData.serverData[a].battles[b].player1 + ">'s active card:\n";
                                                if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                                {
                                                    battleGround += CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health + ":heart:\n";
                                                    if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Any())
                                                    {
                                                        battleGround += "__Effects:__\n";
                                                        foreach (string effect in CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects)
                                                        {
                                                            battleGround += effect.ToLower() + "\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    battleGround += "NONE\n";
                                                }
                                                battleGround += "\n<@!" + CurrentData.serverData[a].battles[b].player2 + ">'s active card:\n";
                                                if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                                {
                                                    battleGround += CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health + ":heart:\n";
                                                    if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Any())
                                                    {
                                                        battleGround += "__Effects:__\n";
                                                        foreach (string effect in CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects)
                                                        {
                                                            battleGround += effect.ToLower() + "\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    battleGround += "NONE\n";
                                                }
                                                battleGround += "\nDo `=b` for more commands you can do.";
                                                await ReplyAsync(battleGround);
                                            }
                                            else if (isBattleCard)
                                            {
                                                await ReplyAsync("```diff\n- Its not your turn\n```");
                                                return;
                                            }
                                            else
                                            {
                                                await ReplyAsync("```diff\n- Thats not a card you can set as you active card. IDs may have updated, do =b list again.\n```");
                                                return;
                                            }
                                            break;
                                        }
                                        if (CurrentData.serverData[a].battles[b].player2 == Context.User.Id)
                                        {
                                            bool isBattleCard = false;
                                            for (int c = 0; c < CurrentData.serverData[a].battles[b].player2deck.battleCards.Count; c++)
                                            {
                                                if (c == id)
                                                {
                                                    isBattleCard = true;
                                                    break;
                                                }
                                            }
                                            if (CurrentData.serverData[a].battles[b].turn == 1 && isBattleCard)
                                            {
                                                CurrentData.serverData[a].battles[b].player2activeCard = id;
                                                IEmote tick = new Emoji("\U00002705");
                                                await Context.Message.AddReactionAsync(tick);
                                                if (CurrentData.serverData[a].battles[b].player1activeCard != -1 && CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                                {
                                                    uint startingTurn = (uint)rnd.Next(1, 3);
                                                    CurrentData.serverData[a].battles[b].turn = startingTurn + 1;
                                                    Console.WriteLine(">:( " + startingTurn + " " + CurrentData.serverData[a].battles[b].turn);
                                                    string str = "<@!";
                                                    if (CurrentData.serverData[a].battles[b].turn % 2 == 0)
                                                        str += CurrentData.serverData[a].battles[b].player1;
                                                    else
                                                        str += CurrentData.serverData[a].battles[b].player2;
                                                    str += ">";
                                                    await ReplyAsync("```diff\n+ Let the battle begin!\n```\n" + str + " Begins!");
                                                }
                                                break;
                                            }
                                            else if (isTurn && isBattleCard)
                                            {
                                                CurrentData.serverData[a].battles[b].player2activeCard = id;
                                                string battleGround = "__**The Battleground:**__\n\n";
                                                battleGround += "<@!" + CurrentData.serverData[a].battles[b].player1 + ">'s active card:\n";
                                                if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                                {
                                                    battleGround += CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health + ":heart:\n";
                                                    if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Any())
                                                    {
                                                        battleGround += "__Effects:__\n";
                                                        foreach (string effect in CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects)
                                                        {
                                                            battleGround += effect.ToLower() + "\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    battleGround += "NONE\n";
                                                }
                                                battleGround += "\n<@!" + CurrentData.serverData[a].battles[b].player2 + ">'s active card:\n";
                                                if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                                {
                                                    battleGround += CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health + ":heart:\n";
                                                    if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Any())
                                                    {
                                                        battleGround += "__Effects:__\n";
                                                        foreach (string effect in CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects)
                                                        {
                                                            battleGround += effect.ToLower() + "\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    battleGround += "NONE\n";
                                                }
                                                battleGround += "\nDo `=b` for more commands you can do.";
                                                await ReplyAsync(battleGround);
                                            }
                                            else if (isBattleCard)
                                            {
                                                await ReplyAsync("```diff\n- Its not your turn\n```");
                                                return;
                                            }
                                            else
                                            {
                                                await ReplyAsync("```diff\n- That not a card you can set as you active card.\n```");
                                                return;
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            await ReplyAsync("```diff\n- Use the IDs found next to the name cards when doing =b list\n```");
                        }
                        break;
                    case "use":
                        string _arg1 = "";
                        try { _arg1 = args[1]; } catch { await ReplyAsync("```diff\n- Use the IDs found next to the name cards when doing =b list\n```"); return; }
                        int _id = -1;
                        bool _parsed = int.TryParse(_arg1, out _id);
                        if (_parsed && _id > -1 && _id < 20)
                        {
                            Random rnd = new Random();
                            for (int a = 0; a < CurrentData.serverData.Count; a++)
                            {
                                if (CurrentData.serverData[a].serverId == contextGuildId)
                                {
                                    for (int b = 0; b < CurrentData.serverData[a].battles.Count; b++)
                                    {
                                        if (CurrentData.serverData[a].battles[b].player1 == Context.User.Id && CurrentData.serverData[a].battles[b].turn % 2 == 0 && CurrentData.serverData[a].battles[b].turn != 1)
                                        {
                                            if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                            {
                                                _id -= CurrentData.serverData[a].battles[b].player1deck.battleCards.Count;
                                                if (_id > -1)
                                                {
                                                    List<string> notes = CurrentData.serverData[a].battles[b].player1deck.itemOtherCards[_id].card.notes.Split('§').ToList();
                                                    List<string> attacks = notes[3].Split('#').ToList();
                                                    foreach (string _attacks in attacks)
                                                    {
                                                        List<string> attack = _attacks.Split('$').ToList();
                                                        switch (attack[4])
                                                        {
                                                            default:
                                                                break;
                                                            case "0"://to me
                                                                switch (attack[0])
                                                                {
                                                                    default:
                                                                        break;
                                                                    case "HEAL":
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health += int.Parse(attack[2]);
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Add(attack[3]);
                                                                        break;
                                                                    case "DPS":
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health -= int.Parse(attack[2]);
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Add(attack[3]);
                                                                        break;
                                                                }
                                                                break;
                                                            case "1"://to enemy
                                                                switch (attack[0])
                                                                {
                                                                    default:
                                                                        break;
                                                                    case "HEAL":
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health += int.Parse(attack[2]);
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Add(attack[3]);
                                                                        break;
                                                                    case "DPS":
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health -= int.Parse(attack[2]);
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Add(attack[3]);
                                                                        break;
                                                                }
                                                                break;
                                                        }
                                                    }
                                                    await ReplyAsync("```diff\n+ You used that card.\n```");
                                                    CurrentData.serverData[a].battles[b].player1deck.itemOtherCards.RemoveAt(_id);
                                                    // show battle ground
                                                    string battleGround = "__**The Battleground:**__\n\n";
                                                    battleGround += "<@!" + CurrentData.serverData[a].battles[b].player1 + ">'s active card:\n";
                                                    if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                                    {
                                                        battleGround += CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health + ":heart:\n";
                                                        if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Any())
                                                        {
                                                            battleGround += "__Effects:__\n";
                                                            foreach (string effect in CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects)
                                                            {
                                                                battleGround += effect.ToLower() + "\n";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        battleGround += "NONE\n";
                                                    }
                                                    battleGround += "\n<@!" + CurrentData.serverData[a].battles[b].player2 + ">'s active card:\n";
                                                    if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                                    {
                                                        battleGround += CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health + ":heart:\n";
                                                        if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Any())
                                                        {
                                                            battleGround += "__Effects:__\n";
                                                            foreach (string effect in CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects)
                                                            {
                                                                battleGround += effect.ToLower() + "\n";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        battleGround += "NONE\n";
                                                    }
                                                    battleGround += "\nDo `=b` for more commands you can do.";
                                                    await ReplyAsync(battleGround);
                                                }
                                                else
                                                {
                                                    await ReplyAsync("```diff\n- That card is not an ITEM card.\n");
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                await ReplyAsync("```diff\n- You need to select an active card. Use =b select id, find the id in =b list\n```");
                                                break;
                                            }
                                        }else if (CurrentData.serverData[a].battles[b].player2 == Context.User.Id && CurrentData.serverData[a].battles[b].turn % 2 != 0 && CurrentData.serverData[a].battles[b].turn != 1)
                                        {
                                            if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                            {
                                                _id -= CurrentData.serverData[a].battles[b].player2deck.battleCards.Count;
                                                if (_id > -1)
                                                {
                                                    List<string> notes = CurrentData.serverData[a].battles[b].player2deck.itemOtherCards[_id].card.notes.Split('§').ToList();
                                                    List<string> attacks = notes[3].Split('#').ToList();
                                                    //-----------------
                                                    foreach (string _attacks in attacks)
                                                    {
                                                        List<string> attack = _attacks.Split('$').ToList();
                                                        switch (attack[4])
                                                        {
                                                            default:
                                                                break;
                                                            case "1"://to enemy
                                                                switch (attack[0])
                                                                {
                                                                    default:
                                                                        break;
                                                                    case "HEAL":
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health += int.Parse(attack[2]);
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Add(attack[3]);
                                                                        break;
                                                                    case "DPS":
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health -= int.Parse(attack[2]);
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Add(attack[3]);
                                                                        break;
                                                                }
                                                                break;
                                                            case "0"://to me
                                                                switch (attack[0])
                                                                {
                                                                    default:
                                                                        break;
                                                                    case "HEAL":
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health += int.Parse(attack[2]);
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Add(attack[3]);
                                                                        break;
                                                                    case "DPS":
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health -= int.Parse(attack[2]);
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Add(attack[3]);
                                                                        break;
                                                                }
                                                                break;
                                                        }
                                                    }
                                                    await ReplyAsync("```diff\n+ You used that item card.\n```");
                                                    CurrentData.serverData[a].battles[b].player1deck.itemOtherCards.RemoveAt(_id);
                                                    // show battle ground
                                                    string battleGround = "__**The Battleground:**__\n\n";
                                                    battleGround += "<@!" + CurrentData.serverData[a].battles[b].player1 + ">'s active card:\n";
                                                    if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                                    {
                                                        battleGround += CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health + ":heart:\n";
                                                        if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Any())
                                                        {
                                                            battleGround += "__Effects:__\n";
                                                            foreach (string effect in CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects)
                                                            {
                                                                battleGround += effect.ToLower() + "\n";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        battleGround += "NONE\n";
                                                    }
                                                    battleGround += "\n<@!" + CurrentData.serverData[a].battles[b].player2 + ">'s active card:\n";
                                                    if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                                    {
                                                        battleGround += CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health + ":heart:\n";
                                                        if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Any())
                                                        {
                                                            battleGround += "__Effects:__\n";
                                                            foreach (string effect in CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects)
                                                            {
                                                                battleGround += effect.ToLower() + "\n";
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        battleGround += "NONE\n";
                                                    }
                                                    battleGround += "\nDo `=b` for more commands you can do.";
                                                    await ReplyAsync(battleGround);
                                                }
                                                else
                                                {
                                                    await ReplyAsync("```diff\n- That card is not an ITEM card.\n");
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                await ReplyAsync("```diff\n- You need to select an active card. Use =b select id, find the id in =b list\n```");
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    case "attack":
                        string __arg1 = "";
                        try { __arg1 = args[1]; } catch { await ReplyAsync("```diff\n- Use 1,2 or 3 to select an attack from you active card. Do =b list to see your cards attacks.\n```"); return; }
                        int __id = -1;
                        bool __parsed = int.TryParse(__arg1, out __id);
                        if (__parsed && __id > 0 && __id < 4)
                        {
                            Random rnd = new Random();
                            for (int a = 0; a < CurrentData.serverData.Count; a++)
                            {
                                if (CurrentData.serverData[a].serverId == contextGuildId)
                                {
                                    for (int b = 0; b < CurrentData.serverData[a].battles.Count; b++)
                                    {
                                        if (CurrentData.serverData[a].battles[b].player1 == Context.User.Id && CurrentData.serverData[a].battles[b].turn % 2 == 0 && CurrentData.serverData[a].battles[b].turn != 1)
                                        {
                                            if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                            {
                                                List<string> notes = CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.notes.Split('§').ToList();
                                                List<string> attacks = notes[3].Split('#').ToList();
                                                List<string> attack = attacks[__id-1].Split('$').ToList();
                                                switch (attack[4])
                                                {
                                                    default:
                                                        break;
                                                    case "0"://to me
                                                        switch (attack[0])
                                                        {
                                                            default:
                                                                break;
                                                            case "HEAL":
                                                                CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health += int.Parse(attack[2]);
                                                                if (attack[3] != "0")
                                                                    CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Add(attack[3]);
                                                                break;
                                                            case "DPS":
                                                                CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health -= int.Parse(attack[2]);
                                                                if (attack[3] != "0")
                                                                    CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Add(attack[3]);
                                                                break;
                                                        }
                                                        break;
                                                    case "1"://to enemy
                                                        switch (attack[0])
                                                        {
                                                            default:
                                                                break;
                                                            case "HEAL":
                                                                CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health += int.Parse(attack[2]);
                                                                if (attack[3] != "0")
                                                                    CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Add(attack[3]);
                                                                break;
                                                            case "DPS":
                                                                CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health -= int.Parse(attack[2]);
                                                                if (attack[3] != "0")
                                                                    CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Add(attack[3]);
                                                                break;
                                                        }
                                                        break;
                                                }
                                                await ReplyAsync("```diff\n+ You used that attack.\n```");
                                                // move to next turn
                                                CurrentData.serverData[a].battles[b].turn += 1;
                                                if (CurrentData.serverData[a].battles[b].turn % 2 == 0)
                                                {
                                                    if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                                    {
                                                        await ReplyAsync("It is now <@!" + CurrentData.serverData[a].battles[b].player1 + ">'s turn.");
                                                        if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Any())
                                                        {
                                                            foreach (string effect in CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects)
                                                            {
                                                                switch (effect)
                                                                {
                                                                    default:
                                                                        break;
                                                                    case "POISON":
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health -= 1;
                                                                        break;
                                                                }
                                                            }
                                                        }
                                                        if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health < 1)
                                                        {
                                                            CurrentData.serverData[a].battles[b].player1deck.battleCards.RemoveAt(CurrentData.serverData[a].battles[b].player1activeCard);
                                                            CurrentData.serverData[a].battles[b].player1activeCard = -1;
                                                            await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player1 + ">, you card has 0 HP, select a new one with `=b select id`. Get the list of ids with `=b list`.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await ReplyAsync("It is now <@!" + CurrentData.serverData[a].battles[b].player1 + ">'s turn.");
                                                        await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player1 + ">, you card has 0 HP, select a new one with `=b select id`. Get the list of ids with `=b list`.");
                                                    }
                                                }
                                                else
                                                {
                                                    if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                                    {
                                                        await ReplyAsync("It is now <@!" + CurrentData.serverData[a].battles[b].player2 + ">'s turn.");
                                                        if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Any())
                                                        {
                                                            foreach (string effect in CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects)
                                                            {
                                                                switch (effect)
                                                                {
                                                                    default:
                                                                        break;
                                                                    case "POISON":
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health -= 1;
                                                                        break;
                                                                }
                                                            }
                                                        }
                                                        if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health < 1)
                                                        {
                                                            CurrentData.serverData[a].battles[b].player2deck.battleCards.RemoveAt(CurrentData.serverData[a].battles[b].player2activeCard);
                                                            CurrentData.serverData[a].battles[b].player2activeCard = -1;
                                                            await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player2 + ">, you card has 0 HP, select a new one with `=b select id`. Get the list of ids with `=b list`.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await ReplyAsync("It is now <@!" + CurrentData.serverData[a].battles[b].player2 + ">'s turn.");
                                                        await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player2 + ">, you card has 0 HP, select a new one with `=b select id`. Get the list of ids with `=b list`.");
                                                    }
                                                }
                                                if (!CurrentData.serverData[a].battles[b].player1deck.battleCards.Any())
                                                {
                                                    await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player1 + "> has no more cards left.");
                                                    await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player2 + "> **WINS!**");
                                                    for (int d = 0; d < CurrentData.serverData[a].userData.Count; d++)
                                                    {
                                                        if (CurrentData.serverData[a].userData[d].userId == CurrentData.serverData[a].battles[b].player2)
                                                        {
                                                            CurrentData.serverData[a].userData[d].battleWins += 1;
                                                            break;
                                                        }
                                                    }
                                                    CurrentData.serverData[a].battles.RemoveAt(b);
                                                    return;
                                                }else if (!CurrentData.serverData[a].battles[b].player2deck.battleCards.Any())
                                                {
                                                    await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player2 + "> has no more cards left.");
                                                    await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player1 + "> **WINS!**");
                                                    for (int d = 0; d < CurrentData.serverData[a].userData.Count; d++)
                                                    {
                                                        if (CurrentData.serverData[a].userData[d].userId == CurrentData.serverData[a].battles[b].player1)
                                                        {
                                                            CurrentData.serverData[a].userData[d].battleWins += 1;
                                                            break;
                                                        }
                                                    }
                                                    CurrentData.serverData[a].battles.RemoveAt(b);
                                                    return;
                                                }
                                                // show battle ground
                                                string battleGround = "__**The Battleground:**__\n\n";
                                                battleGround += "<@!" + CurrentData.serverData[a].battles[b].player1 + ">'s active card:\n";
                                                if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                                {
                                                    battleGround += CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health + ":heart:\n";
                                                    if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Any())
                                                    {
                                                        battleGround += "__Effects:__\n";
                                                        foreach (string effect in CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects)
                                                        {
                                                            battleGround += effect.ToLower() + "\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    battleGround += "NONE\n";
                                                }
                                                battleGround += "\n<@!" + CurrentData.serverData[a].battles[b].player2 + ">'s active card:\n";
                                                if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                                {
                                                    battleGround += CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health + ":heart:\n";
                                                    if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Any())
                                                    {
                                                        battleGround += "__Effects:__\n";
                                                        foreach (string effect in CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects)
                                                        {
                                                            battleGround += effect.ToLower() + "\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    battleGround += "NONE\n";
                                                }
                                                battleGround += "\nDo `=b` for more commands you can do.";
                                                await ReplyAsync(battleGround);
                                                break;
                                            }
                                            else
                                            {
                                                await ReplyAsync("```diff\n- You need to select an active card. Use =b select id, find the id in =b list\n```");
                                                break;
                                            }
                                        }
                                        else if (CurrentData.serverData[a].battles[b].player2 == Context.User.Id && CurrentData.serverData[a].battles[b].turn % 2 != 0 && CurrentData.serverData[a].battles[b].turn != 1)
                                        {
                                            if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                            {
                                                List<string> notes = CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.notes.Split('§').ToList();
                                                List<string> attacks = notes[3].Split('#').ToList();
                                                List<string> attack = attacks[__id - 1].Split('$').ToList();
                                                switch (attack[4])
                                                {
                                                    default:
                                                        break;
                                                    case "1"://to enemy
                                                        switch (attack[0])
                                                        {
                                                            default:
                                                                break;
                                                            case "HEAL":
                                                                CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health += int.Parse(attack[2]);
                                                                if (attack[3] != "0")
                                                                    CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Add(attack[3]);
                                                                break;
                                                            case "DPS":
                                                                CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health -= int.Parse(attack[2]);
                                                                if (attack[3] != "0")
                                                                    CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Add(attack[3]);
                                                                break;
                                                        }
                                                        break;
                                                    case "0"://to me
                                                        switch (attack[0])
                                                        {
                                                            default:
                                                                break;
                                                            case "HEAL":
                                                                CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health += int.Parse(attack[2]);
                                                                if (attack[3] != "0")
                                                                    CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Add(attack[3]);
                                                                break;
                                                            case "DPS":
                                                                CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health -= int.Parse(attack[2]);
                                                                if (attack[3] != "0")
                                                                    CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Add(attack[3]);
                                                                break;
                                                        }
                                                        break;
                                                }
                                                await ReplyAsync("```diff\n+ You used that attack.\n```");
                                                // move to next turn
                                                CurrentData.serverData[a].battles[b].turn += 1;
                                                if (CurrentData.serverData[a].battles[b].turn % 2 == 0)
                                                {
                                                    if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                                    {
                                                        await ReplyAsync("It is now <@!" + CurrentData.serverData[a].battles[b].player1 + ">'s turn.");
                                                        if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Any())
                                                        {
                                                            foreach (string effect in CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects)
                                                            {
                                                                switch (effect)
                                                                {
                                                                    default:
                                                                        break;
                                                                    case "POISON":
                                                                        CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health -= 1;
                                                                        break;
                                                                }
                                                            }
                                                        }
                                                        if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health < 1)
                                                        {
                                                            CurrentData.serverData[a].battles[b].player1deck.battleCards.RemoveAt(CurrentData.serverData[a].battles[b].player1activeCard);
                                                            CurrentData.serverData[a].battles[b].player1activeCard = -1;
                                                            await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player1 + ">, you card has 0 HP, select a new one with `=b select id`. Get the list of ids with `=b list`.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await ReplyAsync("It is now <@!" + CurrentData.serverData[a].battles[b].player1 + ">'s turn.");
                                                        await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player1 + ">, you card has 0 HP, select a new one with `=b select id`. Get the list of ids with `=b list`.");
                                                    }
                                                    if (!CurrentData.serverData[a].battles[b].player1deck.battleCards.Any())
                                                    {
                                                        await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player1 + "> has no more cards left.");
                                                        await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player2 + "> **WINS!**");
                                                        for (int d = 0; d < CurrentData.serverData[a].userData.Count; d++)
                                                        {
                                                            if (CurrentData.serverData[a].userData[d].userId == CurrentData.serverData[a].battles[b].player2)
                                                            {
                                                                CurrentData.serverData[a].userData[d].battleWins += 1;
                                                                break;
                                                            }
                                                        }
                                                        CurrentData.serverData[a].battles.RemoveAt(b);
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                                    {
                                                        await ReplyAsync("It is now <@!" + CurrentData.serverData[a].battles[b].player2 + ">'s turn.");
                                                        if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Any())
                                                        {
                                                            foreach (string effect in CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects)
                                                            {
                                                                switch (effect)
                                                                {
                                                                    default:
                                                                        break;
                                                                    case "POISON":
                                                                        CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health -= 1;
                                                                        break;
                                                                }
                                                            }
                                                        }
                                                        if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health < 1)
                                                        {
                                                            CurrentData.serverData[a].battles[b].player2deck.battleCards.RemoveAt(CurrentData.serverData[a].battles[b].player2activeCard);
                                                            CurrentData.serverData[a].battles[b].player2activeCard = -1;
                                                            await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player2 + ">, you card has 0 HP, select a new one with `=b select id`. Get the list of ids with `=b list`.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        await ReplyAsync("It is now <@!" + CurrentData.serverData[a].battles[b].player2 + ">'s turn.");
                                                        await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player2 + ">, you card has 0 HP, select a new one with `=b select id`. Get the list of ids with `=b list`.");
                                                    }
                                                    if (!CurrentData.serverData[a].battles[b].player2deck.battleCards.Any())
                                                    {
                                                        await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player2 + "> has no more cards left.");
                                                        await ReplyAsync("<@!" + CurrentData.serverData[a].battles[b].player1 + "> **WINS!**");
                                                        for (int d = 0; d < CurrentData.serverData[a].userData.Count; d++)
                                                        {
                                                            if (CurrentData.serverData[a].userData[d].userId == CurrentData.serverData[a].battles[b].player1)
                                                            {
                                                                CurrentData.serverData[a].userData[d].battleWins += 1;
                                                                break;
                                                            }
                                                        }
                                                        CurrentData.serverData[a].battles.RemoveAt(b);
                                                        return;
                                                    }
                                                }
                                                // show battle ground
                                                string battleGround = "__**The Battleground:**__\n\n";
                                                battleGround += "<@!" + CurrentData.serverData[a].battles[b].player1 + ">'s active card:\n";
                                                if (CurrentData.serverData[a].battles[b].player1activeCard != -1)
                                                {
                                                    battleGround += CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].health + ":heart:\n";
                                                    if (CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects.Any())
                                                    {
                                                        battleGround += "__Effects:__\n";
                                                        foreach (string effect in CurrentData.serverData[a].battles[b].player1deck.battleCards[CurrentData.serverData[a].battles[b].player1activeCard].activeEffects)
                                                        {
                                                            battleGround += effect.ToLower() + "\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    battleGround += "NONE\n";
                                                }
                                                battleGround += "\n<@!" + CurrentData.serverData[a].battles[b].player2 + ">'s active card:\n";
                                                if (CurrentData.serverData[a].battles[b].player2activeCard != -1)
                                                {
                                                    battleGround += CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.name + " " + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].card.emoji + "\n" + CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].health + ":heart:\n";
                                                    if (CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects.Any())
                                                    {
                                                        battleGround += "__Effects:__\n";
                                                        foreach (string effect in CurrentData.serverData[a].battles[b].player2deck.battleCards[CurrentData.serverData[a].battles[b].player2activeCard].activeEffects)
                                                        {
                                                            battleGround += effect.ToLower() + "\n";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    battleGround += "NONE\n";
                                                }
                                                battleGround += "\nDo `=b` for more commands you can do.";
                                                await ReplyAsync(battleGround);
                                                break;
                                            }
                                            else
                                            {
                                                await ReplyAsync("```diff\n- You need to select an active card.\n```");
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            await ReplyAsync("```diff\n- Its not your turn.\n```");
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            await ReplyAsync("```diff\n- Use 1,2 or 3 to select an attack from you active card. Do =b list to see your cards attacks.\n```");
                        }
                        break;
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled.\n  Dont worry, you battle is just paused.\n```");
            }
        }

        [Command("announcements")]
        public async Task Announcements(params string[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                if (!args.Any())
                {
                    await ReplyAsync("diff\n- Error. Please specify.\n  Use =announcments on\n  Or  =announcements off\n```");
                }
                else
                {
                    switch (args[0])
                    {
                        default:
                            await ReplyAsync("diff\n- Error. Please specify.\n  Use =announcments on\n  Or  =announcements off\n```");
                            break;
                        case "on":
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                {
                                    if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                                    {
                                        CurrentData.serverData[i].userData[j].importantAnnouncements = true;
                                        break;
                                    }
                                }
                            }
                            await Context.Message.AddReactionAsync(new Emoji("\U00002705"));
                            break;
                        case "off":
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                {
                                    if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                                    {
                                        CurrentData.serverData[i].userData[j].importantAnnouncements = false;
                                        break;
                                    }
                                }
                            }
                            await Context.Message.AddReactionAsync(new Emoji("\U00002705"));
                            break;
                    }
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled.\n  Dont worry, you battle is just paused.\n```");
            }
        }

        [Command("notifications")]
        public async Task Notifications(params string[] args)
        {
            if (CurrentData.commandsEnabled)
            {
                if (!args.Any())
                {
                    await ReplyAsync("diff\n- Error. Please specify.\n  Use =notifications on\n  Or  =notifications off\n```");
                }
                else
                {
                    switch (args[0])
                    {
                        default:
                            await ReplyAsync("diff\n- Error. Please specify.\n  Use =notifications on\n  Or  =notifications off\n```");
                            break;
                        case "on":
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                {
                                    if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                                    {
                                        CurrentData.serverData[i].userData[j].notifyAnnouncements = true;
                                        break;
                                    }
                                }
                            }
                            await Context.Message.AddReactionAsync(new Emoji("\U00002705"));
                            break;
                        case "off":
                            for (int i = 0; i < CurrentData.serverData.Count; i++)
                            {
                                for (int j = 0; j < CurrentData.serverData[i].userData.Count; j++)
                                {
                                    if (CurrentData.serverData[i].userData[j].userId == Context.User.Id)
                                    {
                                        CurrentData.serverData[i].userData[j].notifyAnnouncements = false;
                                        break;
                                    }
                                }
                            }
                            await Context.Message.AddReactionAsync(new Emoji("\U00002705"));
                            break;
                    }
                }
            }
            else
            {
                await ReplyAsync("```diff\n- Commands Currently Disabled.\n  Dont worry, you battle is just paused.\n```");
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
                        if (!CurrentData.maintenance) { await ReplyAsync("```diff\n- Maintenance is already false.\n```");return; }
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
                        string testAnnouncement = "> **Emoji TCG Announcment:**\n*Do `=announcements off` in ";
                        testAnnouncement += "GuildName if you dont want to see these.*\n\n";
                        testAnnouncement += CurrentData.settings.announcement;
                        await ReplyAsync(testAnnouncement);
                        string testNotification = "> **Emoji TCG Notification:**\n*Do `=notifications off` in ";
                        testNotification += "GuildName if you dont want to see these.*\n\n";
                        testNotification += CurrentData.settings.notification;
                        await ReplyAsync(testNotification);
                        await Context.Message.AddReactionAsync(tick);
                        break;
                    case "announce":
                        switch (stringArray[1])
                        {
                            default:
                                break;
                            case "important":
                                List<ulong> idsSentTo = new List<ulong>();
                                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                                {
                                    foreach (UserData _userData in _serverData.userData)
                                    {
                                        if (!idsSentTo.Contains(_userData.userId) && _userData.importantAnnouncements && _serverData.allowAnnouncements)
                                        {
                                            string announcement = "> **Emoji TCG Announcment:**\n*Do `=announcements off` in ";
                                            var user = CurrentData.client.GetUser(_userData.userId);
                                            var guild = CurrentData.client.GetGuild(_serverData.serverId);
                                            announcement += guild.Name + " if you dont want to see these.*\n\n";
                                            announcement += CurrentData.settings.announcement;
                                            idsSentTo.Add(_userData.userId);
                                            try { await user.SendMessageAsync(announcement); } catch { }
                                        }
                                    }
                                }
                                await Context.Message.AddReactionAsync(tick);
                                break;
                            case "notify":
                                List<ulong> idsSentTo2 = new List<ulong>();
                                foreach (ServerData.ServerData _serverData in CurrentData.serverData)
                                {
                                    foreach (UserData _userData in _serverData.userData)
                                    {
                                        if (!idsSentTo2.Contains(_userData.userId) && _userData.notifyAnnouncements && _serverData.allowNotifications)
                                        {
                                            string notification = "> **Emoji TCG Notification:**\n*Do `=notifications off` in ";
                                            var user = CurrentData.client.GetUser(_userData.userId);
                                            var guild = CurrentData.client.GetGuild(_serverData.serverId);
                                            notification += guild.Name + " if you dont want to see these.*\n\n";
                                            notification += CurrentData.settings.notification;
                                            idsSentTo2.Add(_userData.userId);
                                            await user.SendMessageAsync(notification);
                                        }
                                    }
                                }
                                await Context.Message.AddReactionAsync(tick);
                                break;
                        }
                        break;
                }
            }
        }
    }
}
