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
            res.ExtraItems = new List<ItemInfo>();

            if (CheckStackableItem((req.TemplatedId)))
            {
                ItemData itemData = null;
                if(DataManager.ItemDict.TryGetValue(req.TemplatedId, out itemData))
                {
                    ItemDb findDbItem = findItems.FirstOrDefault(i => i.TemplatedId == req.TemplatedId);
                    if(findDbItem != null )
                    {
                        if(findDbItem.Count + req.Count<= itemData.maxStack)
                        {
                            HandleExistingItem(findDbItem, itemData, req, res);
                        }
                        else
                        {
                            ItemDb newItemDb = AddNewItem(req, req.Count);
                            res.Item = CreateItemInfo(newItemDb);
                        }
                    }
                }
                else
                {
                    // ItemData == null
                     res.Item = null;
                }
            }
            _context.SaveChangesEx();  
            return res;
        }

        private void HandleExistingItem(ItemDb findDbItem, ItemData itemData, AddItemPacketReq req, AddItemPacketRes res)
        {
            if (req.Count == 0)
            {
                // 클라이언트로부터 받은 갯수가 0이면 삭제
                _context.Items.Remove(findDbItem);
                res.Item = null;
            }
            else if (req.Count <= itemData.maxStack)
            {
                // 클라이언트로 받은 갯수로 맞춤
                findDbItem.Count = req.Count;
                res.Item = CreateItemInfo(findDbItem);
            }
            else
            {
                // 최대치를 초과하는 경우
                findDbItem.Count = itemData.maxStack;
                int remainingCount = req.Count - itemData.maxStack;
                res.Item = CreateItemInfo(findDbItem);

                ItemDb newItemDb = AddNewItem(req, remainingCount);
                res.ExtraItems.Add(CreateItemInfo(newItemDb));
            }
        }
        private ItemDb AddNewItem(AddItemPacketReq req, int count)
        {
            ItemDb newItemDb = new ItemDb()
            {
                TemplatedId = req.TemplatedId,
                Slot = req.Slot,
                Count = count,
                OwnerDbId = req.PlayerDbId,
            };

            _context.Items.Add(newItemDb);
            _context.SaveChangesEx();

            return newItemDb;
        }

        private ItemInfo CreateItemInfo(ItemDb itemDb)
        {
            return new ItemInfo()
            {
                itemDbId = itemDb.ItemDbId,
                templatedId = itemDb.TemplatedId,
                slot = itemDb.Slot,
                count = itemDb.Count,
                equipped = false,
            };
        }

        [HttpPost]
        [Route("updateItems")]
        public UpdateDatabaseItemsRes UpdateDatabaseItems([FromBody] UpdateDatabaseItemsReq req)
        {
            UpdateDatabaseItemsRes res = new UpdateDatabaseItemsRes();

            if (req.ItemInfos == null)
            {
                res.UpdatedOk = false;
                return res;

            }

            List<ItemDb> findItems = _context.Items.Where(i => i.OwnerDbId == req.PlayerDbId).ToList();

            foreach (ItemInfo item in req.ItemInfos)
            {
               ProcessItemUpdate(req.PlayerDbId,findItems, item);
            }
            _context.SaveChangesEx();
            res.UpdatedOk = true;
            return res;
        }

        private void ProcessItemUpdate(int playerDbId, List<ItemDb> findItems, ItemInfo item)
        {
            if(item.count == 0)
            {
                RemoveItem(findItems, item.itemDbId);
            }
            else
            {
                UpdateItem(playerDbId, findItems, item);
            }
        }

        private void UpdateItem(int playerDbId, List<ItemDb> findItems,ItemInfo item)
        {
            ItemDb findDbItem = findItems.FirstOrDefault(i => i.TemplatedId == item.templatedId);
            ItemData itemData;
            DataManager.ItemDict.TryGetValue(item.templatedId, out itemData);

            if (findDbItem != null && itemData != null)
            {
                if (item.count <= itemData.maxStack)
                {
                    findDbItem.Count = item.count;
                }
                else
                {
                    findDbItem.Count = itemData.maxStack;
                    int remainingCount = item.count - itemData.maxStack;
                    AddNewItem(new AddItemPacketReq { TemplatedId = item.templatedId, Slot = item.slot, PlayerDbId = playerDbId }, remainingCount);
                }
            }
            else if (itemData != null)
            {
                AddNewItem(new AddItemPacketReq { TemplatedId = item.templatedId, Slot = item.slot, PlayerDbId = playerDbId }, item.count);
            }
        }
        private void RemoveItem(List<ItemDb> findItems, int itemDbId)
        {
            ItemDb itemDb = findItems.FirstOrDefault(i => i.ItemDbId == itemDbId);
            if (itemDb != null)
            {
                _context.Items.Remove(itemDb);
            }
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
