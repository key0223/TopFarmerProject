using Assets.Scripts.Contents.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

#region Login
public class CreateAccountPacketReq
{
    public string AccountName;
    public string Password;
}
public class CreateAccountPacketRes
{
    public bool CreateOk;
}
public class LoginAccountPacketReq
{
    public string AccountName;
    public string Password;
}
public class LoginAccountPacketRes
{
    public SYSTEM_MESSAGE LoginResult;
    public int AccountId;
    public List<PlayerInfo> Players;
    //public int Token;
}

public class GetPlayerDataPacketReq
{
    public int PlayerDbId;
}
public class GetPlayerDataPacketRes
{
    public List<ItemInfo> Items;
}
#endregion
#region Item
public class AddItemPacketReq
{
    public int PlayerDbId;
    public int TemplatedId;
    public int Count;
    public int Slot;
}
public class AddItemPacketRes
{
    public ItemInfo Item;
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
    public int PlayerDbId;
    public List<ItemInfo> ItemInfos;
}

public class UpdateRedisItemsPacketRes
{
    public bool UpdatedOk;
}
*/
#endregion

