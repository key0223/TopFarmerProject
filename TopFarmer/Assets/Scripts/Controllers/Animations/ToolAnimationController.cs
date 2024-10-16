using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static Define;

public class ToolAnimationController : MonoBehaviour
{

    Animator _animator;
    SpriteRenderer _sprite;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        Managers.Event.ToolAnimationEvent += UpdateAnimation;
    }
    private void OnDisable()
    {
        Managers.Event.ToolAnimationEvent -= UpdateAnimation;
    }
    void UpdateAnimation(MoveDir dir,ItemType itemType)
    {
        string animationName = GetToolAnimationName(itemType);

        switch (dir)
        {
            case MoveDir.Up:
                _animator.Play($"{animationName}_BACK");
                _sprite.flipX = false;
                break;
            case MoveDir.Down:
                _animator.Play($"{animationName}_FRONT");
                _sprite.flipX = false;
                break;
            case MoveDir.Left:
                {
                    if (itemType == ItemType.ITEM_WEAPON)
                    {
                        _animator.Play($"{animationName}_LEFT");
                    }
                    else
                    {
                        _animator.Play($"{animationName}_RIGHT");
                        _sprite.flipX = true;
                    }
                    
                }
                break;
            case MoveDir.Right:
                _animator.Play($"{animationName}_RIGHT");
                _sprite.flipX = false;
                break;
            case MoveDir.None:
                _animator.Play($"TOOL_NONE");
                break;
        }
    }

    string GetToolAnimationName(ItemType itemType)
    {
        string animationName = "";

        switch (itemType)
        {
            case ItemType.ITEM_TOOL_WATERING:
                animationName = "WATERING";
                break;
            case ItemType.ITEM_TOOL_HOEING:
                animationName = "HOE";
                break;
            case ItemType.ITEM_TOOL_AXE:
                animationName = "AXE";
                break;
            case ItemType.ITEM_TOOL_PICKAXE:
                animationName = "PICKAXE";
                break;
            case ItemType.ITEM_TOOL_SCYTHE:
                animationName = "SCYTHE";
                break;
            case ItemType.ITEM_TOOL_COLLECTING:
                animationName = "HARVEST";
                break;
            case ItemType.ITEM_WEAPON:
                animationName = "STABBING";
                break;

        }

        return animationName;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Monster"))
        {
            PlayerController.Instance.OnMonsterTriggered(coll);
        }
    }
}