using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager 
{
    public MerchantController MerchantCtrl { get; private set; }

    public void Init()
    {
        MerchantCtrl = GameObject.FindAnyObjectByType<MerchantController>();
    }
}
