using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class NpcController : CreatureController
{
    public Npc Npc { get;  set; }

    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;

    }
    protected override void UpdateController()
    {
        base.UpdateController();
    }
    protected virtual void SetNpc()
    {
        //Npc = Npc.Init(templatedId);
        CellPos = new Vector3Int(4, -2);
        transform.position = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        if (ObjectType == ObjectType.OBJECT_TYPE_INTERACTABLE_OBJECT)
        {
            InteractableObject merchantInteract = Util.GetOrAddComponent<InteractableObject>(gameObject);
            merchantInteract.InteractableId = InteractableObjectType.INTERACTABLE_OBEJCT_TYPE_NPC;
        }
    }
}
