using Newtonsoft.Json;
using StackExchange.Redis;
using static Define;

namespace TopFarmerWebServer.Controllers
{
    public partial class ItemController
    {

        public void RedisClear(int playerDbId)
        {
            // Initialize Redis
            // [key],[field],[value]
            for (int slot = 0; slot < 18; slot++)
            {
                string slotString = slot.ToString();
                ItemInfo info = new ItemInfo();
                string infoStr = JsonConvert.SerializeObject(info);
                _redis.HashSet($"{playerDbId}", slotString, infoStr);

            }
        }
        public void RedisItemInfoUpdate(int playerId, int slot, ItemInfo itemInfo)
        {
            string slotStr = slot.ToString();
            string infoJson = JsonConvert.SerializeObject(itemInfo);
            _redis.HashSet($"{playerId}", slotStr, infoJson);
        }
        public ItemInfo RedisFindItem(Func<ItemInfo, bool> condition, int playerDbId)
        {
            HashEntry[] slots = _redis.HashGetAll($"{playerDbId}");
            foreach (HashEntry itemInfo in slots)
            {
                ItemInfo info = JsonConvert.DeserializeObject<ItemInfo>(itemInfo.Value);
                if (condition.Invoke(info))
                    return info;
            }

            return null;
        }
        public int? GetEmptySlot(int playerDbId)
        {
            HashEntry[] slots = _redis.HashGetAll($"{playerDbId}");
            Array.Sort(slots, (left, right) => left.Name.CompareTo(right.Name));

            foreach (HashEntry itemInfo in slots)
            {
                ItemInfo info = JsonConvert.DeserializeObject<ItemInfo>(itemInfo.Value);

                if (info.templatedId == 0)
                    return (int)itemInfo.Name;

            }
            return null;
        }
    }
}
