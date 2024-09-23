using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ToolEffectAnimationController : MonoBehaviour
{
    Animator _animator;
    SpriteRenderer _sprite;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }
  
    public void UpdateAnimation(MoveDir dir, ItemType itemType, Vector3 gripPosition)
    {
        string animationName = GetToolAnimationName(itemType);

        switch (dir)
        {
            case MoveDir.Up:
                _animator.Play($"{animationName}_BACK");
                _sprite.flipX = false;
                gameObject.transform.position = gripPosition;
                break;
            case MoveDir.Down:
                _animator.Play($"{animationName}_FRONT");
                _sprite.flipX = false;
                gameObject.transform.position = gripPosition;
                break;
            case MoveDir.Left:
                _animator.Play($"{animationName}_RIGHT");
                _sprite.flipX = true;
                gameObject.transform.position = gripPosition;
                break;
            case MoveDir.Right:
                _animator.Play($"{animationName}_RIGHT");
                _sprite.flipX = false;
                gameObject.transform.position = gripPosition;
                break;
            case MoveDir.None:
                _animator.Play($"NONE");
                gameObject.transform.position = gripPosition;
                break;
        }
    }

    string GetToolAnimationName(ItemType itemType)
    {
        string animationName = "";

        switch (itemType)
        {
            case ItemType.ITEM_TOOL_WATERING:
                animationName = "WATERING_EFFECT";
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
                break;

        }

        return animationName;
    }
}
