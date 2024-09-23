using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ShopItemData 
{
    public int itemId;
    public bool purchasable;
    public bool sellable;
    public int purchasePrice;
    public int sellPrice;
}
