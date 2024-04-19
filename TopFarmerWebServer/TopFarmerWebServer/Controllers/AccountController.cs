﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using TopFarmerWebServer.DB;
using TopFarmerWebServer.Game;
using TopFarmerWebServer.Utils;
using static Define;
using static TopFarmerWebServer.DB.DataModel;

namespace TopFarmerWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        AppDbContext _context;
        IConfiguration _configuration;
        IDatabase _redis;
        public AccountController(AppDbContext context, IConfiguration configuration ,IDatabase redis)
        {
            _context = context;
            _configuration = configuration;
            _redis = redis;
        }

        [HttpPost]
        [Route("create")]
        public CreateAccountPacketRes CreateAccount([FromBody] CreateAccountPacketReq req)
        {
            CreateAccountPacketRes res = new CreateAccountPacketRes();

            AccountDb account = _context.Accounts
                                    .AsNoTracking() // read only
                                    .Where(a => a.AccountName == req.AccountName)
                                    .FirstOrDefault();


            if (account == null)
            {
                // Create Account
                _context.Accounts.Add(new AccountDb()
                {
                    AccountName = req.AccountName,
                    Password = req.Password,
                });
                bool success = _context.SaveChangesEx();
                res.CreateOk = success;
            }
            else
            {
                res.CreateOk = false;
            }
            return res;
        }

        [HttpPost]
        [Route("login")]
        public LoginAccountPacketRes LoginAccount([FromBody] LoginAccountPacketReq req)
        {
            LoginAccountPacketRes res = new LoginAccountPacketRes();

            //AccountDb account = _context.Accounts
            //    .Include(a => a.Players)
            //        .Where(a => a.AccountName == req.AccountName && a.Password == req.Password)
            //        .FirstOrDefault();

            AccountDb account = _context.Accounts.Include(a => a.Players)
                .Where(a => a.AccountName == req.AccountName).FirstOrDefault();


            if (account == null)
            {
                res.LoginResult = SYSTEM_MESSAGE.Game_NoAddress;
            }
            else
            {
                if(account.Password == req.Password)
                {

                    res.Players = new List<PlayerInfo>();
                    if (account.Players.Count != 0)
                    {
                        foreach (PlayerDb playerDb in account.Players)
                        {
                            PlayerInfo player = new PlayerInfo()
                            {
                                PlayerName = playerDb.PlayerName,
                                PlayerDbId = playerDb.PlayerDbId,
                                Coin = 500,
                            };

                            res.Players.Add(player);
                        }

                    }
                    else
                    {
                        // create PlayerDb
                        PlayerDb newPlayerDb = new PlayerDb()
                        {
                            PlayerName = req.AccountName,
                            AccountDbId = account.AccountDbId,
                            Coin = 500, // 초기 자금
                        };
                        _context.Players.Add(newPlayerDb);
                        _context.SaveChangesEx();

                        PlayerInfo newPlayer = new PlayerInfo()
                        {
                            PlayerName = req.AccountName,
                            PlayerDbId = newPlayerDb.PlayerDbId,
                            Coin = newPlayerDb.Coin,
                        };

                        // 초기 지급 아이템
                        ItemDb itemDb = new ItemDb()
                        {
                            TemplatedId = 105,
                            Count = 1,
                            Slot = 0,
                            Equipped = false,
                            OwnerDbId = newPlayerDb.PlayerDbId,
                        };
                        _context.Items.Add(itemDb);
                        _context.SaveChangesEx();
                        res.Players.Add(newPlayer);

                        // Save to Redis
                        // [key],[field],[value]
                        for (int slot = 0; slot < 18; slot++)
                        {
                            string slotString = slot.ToString();
                            ItemInfo info = new ItemInfo();
                            string infoStr = JsonConvert.SerializeObject(info);
                            _redis.HashSet($"{newPlayerDb.PlayerDbId}", slotString, infoStr);

                        }
                        string slotStr = itemDb.Slot.ToString();
                        ItemInfo itemInfo = new ItemInfo()
                        {
                            itemDbId = itemDb.ItemDbId,
                            templatedId = itemDb.TemplatedId,
                            count = itemDb.Count,
                            equipped = itemDb.Equipped,
                        };

                        string infoJson = JsonConvert.SerializeObject(itemInfo);
                        _redis.HashSet($"{newPlayerDb.PlayerDbId}", slotStr, infoJson);
                    }

                    res.AccountId = account.AccountDbId;
                    res.LoginResult = SYSTEM_MESSAGE.Game_LoginOK;
                }
                else
                {
                    res.LoginResult = SYSTEM_MESSAGE.Game_WrongPassword;
                }
            }

            /*
            else
            {
                res.Players = new List<PlayerInfo>();
                if (account.Players.Count != 0)
                {
                    foreach (PlayerDb playerDb in account.Players)
                    {
                        PlayerInfo player = new PlayerInfo()
                        {
                            PlayerName = playerDb.PlayerName,
                            PlayerDbId = playerDb.PlayerDbId,
                            Coin = 500,
                        };

                        res.Players.Add(player);
                    }

                }
                else
                {
                    // create PlayerDb
                    PlayerDb newPlayerDb = new PlayerDb()
                    {
                        PlayerName = req.AccountName,
                        AccountDbId = account.AccountDbId,
                        Coin = 500, // 초기 자금
                    };
                    _context.Players.Add(newPlayerDb);
                    _context.SaveChangesEx();

                    PlayerInfo newPlayer = new PlayerInfo()
                    {
                        PlayerName = req.AccountName,
                        PlayerDbId = newPlayerDb.PlayerDbId,
                        Coin= newPlayerDb.Coin,
                    };

                    // 초기 지급 아이템
                    ItemDb itemDb = new ItemDb()
                    {
                        TemplatedId = 105,
                        Count = 1,
                        Slot =0,
                        Equipped = false,
                        OwnerDbId = newPlayerDb.PlayerDbId,
                    };
                    _context.Items.Add(itemDb);
                    _context.SaveChangesEx();
                    res.Players.Add(newPlayer);

                    // Save to Redis
                    // [key],[field],[value]
                    for (int slot = 0; slot < 18; slot++)
                    {
                        string slotString = slot.ToString();
                        ItemInfo info = new ItemInfo();
                        string infoStr = JsonConvert.SerializeObject(info);
                        _redis.HashSet($"{newPlayerDb.PlayerDbId}", slotString, infoStr);

                    }
                    string slotStr = itemDb.Slot.ToString();
                    ItemInfo itemInfo = new ItemInfo()
                    {
                        itemDbId = itemDb.ItemDbId,
                        templatedId =itemDb.TemplatedId,
                        count = itemDb.Count,
                        equipped = itemDb.Equipped,
                    };

                    string infoJson = JsonConvert.SerializeObject(itemInfo);
                    _redis.HashSet($"{newPlayerDb.PlayerDbId}", slotStr, infoJson);
                }

                res.LoginOk = true;
                res.AccountId = account.AccountDbId;
            }
            */

            return res;
        }

        [HttpPost]
        [Route("getplayerdata")]
        public GetPlayerDataPacketRes GetPlayerData([FromBody] GetPlayerDataPacketReq req)
        {
            GetPlayerDataPacketRes res = new GetPlayerDataPacketRes();

            List<ItemDb> findItems = _context.Items
                   .Where(i => i.OwnerDbId == req.PlayerDbId).ToList();

            // Initialize Redis
            // [key],[field],[value]
            for (int slot = 0; slot < 18; slot++)
            {
                string slotString = slot.ToString();
                ItemInfo info = new ItemInfo();
                string infoStr = JsonConvert.SerializeObject(info);
                _redis.HashSet($"{req.PlayerDbId}", slotString, infoStr);

            }
            if (findItems.Count >0)
            {
                res.Items = new List<ItemInfo>();

                foreach (ItemDb itemDb in findItems)
                {

                    ItemInfo info = new ItemInfo();
                    info.itemDbId = itemDb.ItemDbId;
                    info.templatedId = itemDb.TemplatedId;
                    info.count = itemDb.Count;
                    info.slot = itemDb.Slot;
                    info.equipped = itemDb.Equipped;

                    res.Items.Add(info);

                    // Redis Update

                    string slotStr = itemDb.Slot.ToString();
                    string infoJson = JsonConvert.SerializeObject(info);
                    _redis.HashSet($"{itemDb.OwnerDbId}", slotStr, infoJson);
                }
            }
            return res;
        }
    }
}