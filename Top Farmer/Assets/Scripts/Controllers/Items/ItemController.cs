using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : ObjectController
{
    public Item Item { get; set; }

    public ItemController ()
    {
        ObjectType = Define.ObjectType.OBJECT_TYPE_ITEM;
    }


}
