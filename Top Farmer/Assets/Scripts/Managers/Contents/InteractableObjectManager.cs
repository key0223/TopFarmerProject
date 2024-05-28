using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class InteractableObjectManager 
{
    Dictionary<int,Action<InteractableObject>> _handler = new Dictionary<int, Action<InteractableObject>>();

    // InteractableObjectManager 클래스가 인스턴스화될 때 자동으로 호출합니다.
    public InteractableObjectManager()
    {
        Register();
    }

    void Register()
    {
        _handler.Add((int)InteractableObjectType.INTERACTABLE_OBJECT_NONE, NoneHandler);
        _handler.Add((int)InteractableObjectType.INTERACTABLE_OBEJCT_TYPE_STORAGE, StorageBoxHandler);
        _handler.Add((int)InteractableObjectType.INTERACTABLE_OBEJCT_TYPE_NPC,NpcHandler);
        _handler.Add((int)InteractableObjectType.INTERACTABLE_OBEJCT_TYPE_KITCHEN, OvenHandler);

    }

    public void OnInteract(InteractableObject interactableObject)
    {
        if (interactableObject.InteractableId < 0)
            return;

        Action<InteractableObject> action = null;

        if(_handler.TryGetValue((int)interactableObject.InteractableId,out action))
        {
            action.Invoke(interactableObject);
        }


    }

    void NoneHandler(InteractableObject obj)
    {

    }
    void StorageBoxHandler(InteractableObject obj)
    {
        Debug.Log("Storage Box");
    }
    void NpcHandler(InteractableObject obj)
    {
        NpcController nc = obj.gameObject.GetComponent<NpcController>();

        if(nc.Npc.NpcType == NpcType.NPC_TYPE_MERCHANT)
        {
            MerchantController mc = (MerchantController)nc;

            mc.OnInteract();
            
        }
        Debug.Log($"{nc.Npc.Name}");
    }
    void OvenHandler(InteractableObject obj)
    {
        OvenController oc = obj.gameObject.GetComponent<OvenController>();
        oc.OnInteract();
    }
}
