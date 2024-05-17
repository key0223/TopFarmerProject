using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class OvenController : ObjectController
{

    void Start()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();
        SetObject();
       
    }

    UI_StateMark _mark;

    public void OnInteract()
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = gameSceneUI.InvenUI;
        UI_Oven ovenUI = gameSceneUI.OvenUI;

        if(ovenUI.gameObject.activeSelf)
        {
            invenUI.gameObject.SetActive(false);
            ovenUI.gameObject.SetActive(false);

            invenUI.State = InventoryState.Inventory;
        }
        else
        {
            invenUI.State = InventoryState.Oven;
            invenUI.gameObject.SetActive(true);
            ovenUI.gameObject.SetActive(true);
        }
    }

    public void SetObject()
    {
        CellPos = new Vector3Int(0, 2);
        transform.position = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);

        //ObjectType = ObjectType.OBJECT_TYPE_INTERACTABLE_OBJECT;
        InteractableObject interactableObject = Util.GetOrAddComponent<InteractableObject>(gameObject);
        interactableObject.InteractableId = InteractableObjectType.INTERACTABLE_OBEJCT_TYPE_KITCHEN;

    }
   
    public void SetStateMark(StateMarkState state)
    {
        // Instantiate StateMark
        UI_StateMark activeMark = GetComponentInChildren<UI_StateMark>();

        if (activeMark != null)
        {
            _mark = activeMark;
        }
        else
        {
            UI_StateMark mark = Managers.UI.MakeWorldSpaceUI<UI_StateMark>(transform);
            _mark = mark;
        }
        _mark.gameObject.SetActive(true);
        _mark.SetStateMark(state);
        
   }
   
    public void DestroyStateMark()
    {
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Oven ovenUI = gameSceneUI.OvenUI;

        bool allStovesIdle = true;

        for (int i = 0; i < ovenUI.Stoves.Length; i++)
        {
            UI_Stove stove = ovenUI.Stoves[i];

            switch(stove.State)
            {
                case StoveState.Using:
                    _mark.SetStateMark(StateMarkState.ProgressingMark);
                    allStovesIdle = false;
                    break;
                case StoveState.Completed:
                    _mark.SetStateMark(StateMarkState.ExclamationMark);
                    allStovesIdle = false;
                    break;
                default:
                    continue;
            }
        }
        if (allStovesIdle)
            Managers.Resource.Destroy(_mark.gameObject);
    }
}
