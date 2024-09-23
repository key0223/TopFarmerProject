using System;
using static Define;

[Serializable]
public class ItemData 
{
    public int itemId;
    public ItemType itemType;
    public string itemSpritePath;
    public string itemName;
    public string itemDescription;
    public float itemUseGridRadius;
    public float itemUseRadius;
    public bool isStartingItem;
    public bool canBePickedUp;
    public bool canBeDropped;
    public bool canBeEaten;
    public bool canBeCarried;
}
