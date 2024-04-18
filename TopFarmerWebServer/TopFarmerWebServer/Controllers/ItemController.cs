using Microsoft.AspNetCore.Mvc;
using static TopFarmerWebServer.DB.DataModel;
using TopFarmerWebServer.DB;
using TopFarmerWebServer.Utils;
using StackExchange.Redis;
using static Define;
using Newtonsoft.Json;
using TopFarmerWebServer.Data;

namespace TopFarmerWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class ItemController : ControllerBase
    {
        AppDbContext _context;
        IConfiguration _configuration;
        IDatabase _redis;
        public ItemController(AppDbContext context, IConfiguration configuration ,IDatabase redis)
        {
            _context = context;
            _configuration = configuration;
            _redis = redis;
        }

        [HttpPost]
        [Route("addItem")]
        public AddItemPacketRes AddItem([FromBody] AddItemPacketReq req)
        {
            AddItemPacketRes res = new AddItemPacketRes();


            if (CheckStackableItem((req.TemplatedId)))
            {
                ItemData itemData = null;
                DataManager.ItemDict.TryGetValue(req.TemplatedId, out itemData);

                ItemInfo itemInfo = RedisFindItem(i => i.templatedId == req.TemplatedId,req.PlayerDbId);

                if(itemInfo != null && itemInfo.count< itemData.maxStack )
                {
                    res.Item = new ItemInfo()
                    {
                        itemDbId = itemInfo.itemDbId,
                        templatedId = itemInfo.templatedId,
                        slot = itemInfo.slot,
                        count = itemInfo.count + req.Count,
                        equipped = false,
                    };

                    RedisItemInfoUpdate(req.PlayerDbId, itemInfo.slot, res.Item);
                }
                else
                {
                    int? slot = GetEmptySlot(req.PlayerDbId);
                    ItemDb newItemDb = new ItemDb()
                    {
                        TemplatedId = req.TemplatedId,
                        Slot = slot.Value,
                        Count = req.Count,
                        OwnerDbId = req.PlayerDbId,
                    };

                    _context.Items.Add(newItemDb);
                    _context.SaveChangesEx();


                    res.Item = new ItemInfo()
                    {
                        itemDbId = newItemDb.ItemDbId,
                        templatedId = newItemDb.TemplatedId,
                        slot = newItemDb.Slot,
                        count = newItemDb.Count,
                        equipped = false,
                    };

                    RedisItemInfoUpdate(req.PlayerDbId, newItemDb.Slot, res.Item);
                }
            }
            
            return res;
        }

        [HttpPost]
        [Route("UpdateRedisItems")]
        public UpdateRedisItemsPacketRes UpdateRedisItem([FromBody] UpdateRedisItemsPacketReq req)
        {
            UpdateRedisItemsPacketRes res = new UpdateRedisItemsPacketRes();

            if (req.ItemInfos == null)
                res.UpdatedOk = false;
            else
            {
                RedisClear(req.PlayerDbId);

                foreach (ItemInfo itemInfo in req.ItemInfos)
                {
                    string slotStr;
                    string infoJson;

                    // Redis Update
                    if (itemInfo.count == 0)
                    {
                        ItemInfo newInfo = new ItemInfo();
                        slotStr = itemInfo.slot.ToString();
                        infoJson = JsonConvert.SerializeObject(newInfo);
                        _redis.HashSet($"{req.PlayerDbId}", slotStr, infoJson);
                    }
                    else
                    {
                        slotStr = itemInfo.slot.ToString();
                        infoJson = JsonConvert.SerializeObject(itemInfo);
                        _redis.HashSet($"{req.PlayerDbId}", slotStr, infoJson);
                    }

                    res.UpdatedOk = true;
                }
            }

            return res;
        }

        
        public bool CheckStackableItem(int templatedId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue (templatedId, out itemData);

            if (itemData.maxStack > 1)
                return true;

            return false;
        }

    }

}
