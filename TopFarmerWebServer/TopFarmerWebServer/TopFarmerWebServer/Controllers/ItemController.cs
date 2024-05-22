using Microsoft.AspNetCore.Mvc;
using static TopFarmerWebServer.DB.DataModel;
using TopFarmerWebServer.DB;
using TopFarmerWebServer.Utils;
using static Define;
using Newtonsoft.Json;
using TopFarmerWebServer.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TopFarmerWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class ItemController : ControllerBase
    {
        AppDbContext _context;
        IConfiguration _configuration;
        //IDatabase _redis;
        public ItemController(AppDbContext context, IConfiguration configuration /*,IDatabase redis*/)
        {
            _context = context;
            _configuration = configuration;
            //_redis = redis;
        }

        [HttpPost]
        [Route("addItem")]
        public AddItemPacketRes AddItem([FromBody] AddItemPacketReq req)
        {
            List<ItemDb> findItems = _context.Items.Where(i => i.OwnerDbId == req.PlayerDbId).ToList();
            AddItemPacketRes res = new AddItemPacketRes();

            if (CheckStackableItem((req.TemplatedId)))
            {
                ItemData itemData = null;
                DataManager.ItemDict.TryGetValue(req.TemplatedId, out itemData);
                ItemDb findDbItem = findItems.FirstOrDefault(i => i.TemplatedId == req.TemplatedId);

                //ItemInfo itemInfo = RedisFindItem(i => i.templatedId == req.TemplatedId, req.PlayerDbId);

                if (findDbItem != null && findDbItem.Count < itemData.maxStack)
                {
                    findDbItem.Count += req.Count;

                    res.Item = new ItemInfo()
                    {
                        itemDbId = findDbItem.ItemDbId,
                        templatedId = findDbItem.TemplatedId,
                        slot = findDbItem.Slot,
                        count = findDbItem.Count + req.Count,
                        equipped = false,
                    };

                    _context.SaveChangesEx();
                    //RedisItemInfoUpdate(req.PlayerDbId, itemInfo.slot, res.Item);
                }
                else
                {
                    ItemDb newItemDb = new ItemDb()
                    {
                        TemplatedId = req.TemplatedId,
                        Slot = req.Slot,
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

                    //RedisItemInfoUpdate(req.PlayerDbId, newItemDb.Slot, res.Item);
                }
            }

            return res;
        }

        [HttpPost]
        [Route("updateItems")]
        public UpdateDatabaseItemsRes UpdateDatabaseItems([FromBody] UpdateDatabaseItemsReq req)
        {
            List<ItemDb> findItems = _context.Items.Where(i => i.OwnerDbId == req.PlayerDbId).ToList();

            UpdateDatabaseItemsRes res = new UpdateDatabaseItemsRes();
            if (req.ItemInfos == null)
                res.UpdatedOk = false;
            else
            {
                foreach (ItemInfo item in req.ItemInfos)
                {
                   
                    ItemDb findDbItem = findItems.FirstOrDefault(i => i.TemplatedId == item.templatedId);

                    ItemData itemData = null;
                    DataManager.ItemDict.TryGetValue(item.templatedId, out itemData);

                    // Db에 아이템이 있고, 가진 아이템 갯수가 최대치를 넘지 않았다면
                    if (findDbItem != null && item.count < itemData.maxStack)
                    {
                        findDbItem.Count = item.count;
                    }

                    _context.SaveChangesEx();

                    //// 스택이 가능한가
                    //if (CheckStackableItem((item.templatedId)))
                    //{
                    //    ItemDb findDbItem = findItems.FirstOrDefault(i => i.TemplatedId == item.templatedId);

                    //    ItemData itemData = null;
                    //    DataManager.ItemDict.TryGetValue(item.templatedId, out itemData);

                    //    // Db에 아이템이 있고, 가진 아이템 갯수가 최대치를 넘지 않았다면
                    //    if (findDbItem != null && item.count < itemData.maxStack)
                    //    {
                    //        findDbItem.Count = item.count;
                    //    }
                    //    _context.SaveChangesEx();
                    //}
                    //else
                    //{
                    //    ItemDb newItemDb = new ItemDb()
                    //    {
                    //        TemplatedId = item.templatedId,
                    //        Slot = item.slot,
                    //        Count = item.count,
                    //        OwnerDbId = req.PlayerDbId,
                    //    };

                    //    _context.Items.Add(newItemDb);
                    //    _context.SaveChangesEx();

                    //}
                    res.UpdatedOk = true;
                }
            }
            return res;
        }

        public bool CheckStackableItem(int templatedId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templatedId, out itemData);

            if (itemData.maxStack > 1)
                return true;

            return false;
        }

        /*
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
           */
    }
}
