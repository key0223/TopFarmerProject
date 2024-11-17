using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ShirtsAnimationMovement : AnimationMovement
{
    public Dictionary<int, int[]> _shirtsDict = new Dictionary<int, int[]>();
    public string _shirtsSpritePath = "shrits";
    public int _currentShirtsId = 1;

    Sprite[] _spriteSheet;

    int _front = 0;
    int _right = 1;
    int _back = 2;

    public void Awake()
    {
        // https://blog.naver.com/delloti/222929875619
        _spriteSheet = Resources.LoadAll<Sprite>($"Textures/Object/Creatures/Farmer/{_shirtsSpritePath}");
        _shirtsDict.Add(1, new int[] { 0, 16, 48 });
        _shirtsDict.Add(2, new int[] { 7, 23, 55 });
        _shirtsDict.Add(3, new int[] { 8, 24, 56 });
        _shirtsDict.Add(4, new int[] { 15, 31, 63 });
        _shirtsDict.Add(5, new int[] { 71, 87, 119 });
        _shirtsDict.Add(6, new int[] { 73, 89, 121 });
        _shirtsDict.Add(7, new int[] { 128, 144, 176 });
        _shirtsDict.Add(8, new int[] { 140, 156, 188 });
        _shirtsDict.Add(9, new int[] { 203, 219, 251 });
        _shirtsDict.Add(10, new int[] { 320, 336, 368 });
        _shirtsDict.Add(11, new int[] { 262, 278, 310 });
    }

    public override void UpdateAnimation(CreatureState state, MoveDir dir, MoveDir lastDir, string itemType)
    {
        if (state == CreatureState.Idle)
        {
            switch (lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.sprite = _spriteSheet[_shirtsDict[_currentShirtsId][_back]];
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.sprite = _spriteSheet[_shirtsDict[_currentShirtsId][_front]];
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.sprite = _spriteSheet[_shirtsDict[_currentShirtsId][_right]];
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.sprite = _spriteSheet[_shirtsDict[_currentShirtsId][_right]];
                    _sprite.flipX = false;
                    break;

            }
        }
        else if (state == CreatureState.Moving)
        {
            switch (dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.sprite = _spriteSheet[_shirtsDict[_currentShirtsId][_back]];
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.sprite = _spriteSheet[_shirtsDict[_currentShirtsId][_front]];
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.sprite = _spriteSheet[_shirtsDict[_currentShirtsId][_right]];
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.sprite = _spriteSheet[_shirtsDict[_currentShirtsId][_right]];
                    _sprite.flipX = false;
                    break;
                case MoveDir.None:

                    break;
            }
        }
        else if (state == CreatureState.ClickInput)
        {
            switch (lastDir)
            {
                case MoveDir.Up:
                    _animator.Play($"{itemType}_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play($"{itemType}_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play($"{itemType}_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play($"{itemType}_RIGHT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.None:

                    break;
            }
        }

    }
}
