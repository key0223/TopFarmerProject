using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Item : MonoBehaviour
{
    [SerializeField]
    int _itemId;
    public int ItemId { get { return _itemId; } set { _itemId = value; } }

    [SerializeField]
    SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();

    }
    private void Start()
    {
        if(ItemId != 0)
        {
            Init(ItemId);
        }
    }
    public void Init(int itemId)
    {
        if(itemId != 0)
        {
            _itemId = itemId;
            ItemData itemData = null;

            if(Managers.Data.ItemDict.TryGetValue(itemId, out itemData))
            {
                //gameObject.name = itemData.itemName;
                _renderer.sprite = Managers.Data.SpriteDict[itemData.itemSpritePath];

                if (itemData.itemType == ItemType.ITEM_REAPABLE_SCENARY)
                {
                    Util.GetOrAddComponent<ItemNudge>(gameObject);
                }
            }

        }
    }


}
