using TopFarmerWebServer.Game;
using static Define;

#region Login
public class CreateAccountPacketReq
{
    public string AccountName { get; set; }
    public string Password { get; set; }
}
public class CreateAccountPacketRes
{
    public bool CreateOk { get; set; }
}

public class LoginAccountPacketReq
{
    public string AccountName { get; set; }
    public string Password { get; set; }
}
public class LoginAccountPacketRes
{
    public SYSTEM_MESSAGE LoginResult { get; set; }
    public int AccountId { get; set; }
    public List<PlayerInfo> Players { get; set; }
}

public class GetPlayerDataPacketReq
{
    public int PlayerDbId { get; set; }
}
public class GetPlayerDataPacketRes
{
    public List<ItemInfo> Items { get; set; } = new List<ItemInfo>();
}
#endregion
#region Item

public class AddItemPacketReq
{
    public int PlayerDbId { get; set; }
    public int TemplatedId { get; set; }
    public int Count { get; set; }
    public int Slot { get; set; } 
}
public class AddItemPacketRes
{
    public ItemInfo Item { get; set; }
}
public class UpdateDatabaseItemsReq
{
    public int PlayerDbId;
    public List<ItemInfo> ItemInfos;
}
public class UpdateDatabaseItemsRes
{
    public bool UpdatedOk;
}

/*
public class UpdateRedisItemsPacketReq
{
    public int PlayerDbId { get; set; }
    public List<ItemInfo> ItemInfos { get; set; }

}
public class UpdateRedisItemsPacketRes
{
    public bool UpdatedOk { get; set; }
}
*/
#endregion





