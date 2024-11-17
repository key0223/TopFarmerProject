using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HairAnimationMovement : AnimationMovement
{
    public Dictionary<int, int[]> _hairDict = new Dictionary<int, int[]>();
    public string _hairSpritePath = "hairstyles";
    public int _currentHairId = 1;

    Sprite[] _spriteSheet;

    int _front = 0;
    int _right = 1;
    int _back = 2;
    public void Awake()
    {
        // https://blog.naver.com/kkunma_09/222147644275
        _spriteSheet = Resources.LoadAll<Sprite>($"Textures/Object/Creatures/Farmer/{_hairSpritePath}");
        _hairDict.Add(1, new int[] { 0, 7, 14 });
        _hairDict.Add(2, new int[] { 1, 8, 15 });
        _hairDict.Add(3, new int[] { 2, 9, 16 });
        _hairDict.Add(4, new int[] { 3, 10, 17 });
        _hairDict.Add(5, new int[] { 4, 11, 18 });
        _hairDict.Add(6, new int[] { 5, 12, 19 });
        _hairDict.Add(7, new int[] { 6, 13, 20 });
    }

    public override void UpdateAnimation(CreatureState state, MoveDir dir, MoveDir lastDir, string itemType)
    {
        if (state == CreatureState.Idle)
        {
            switch (lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.sprite = _spriteSheet[_hairDict[_currentHairId][_back]];
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.sprite = _spriteSheet[_hairDict[_currentHairId][_front]];
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.sprite = _spriteSheet[_hairDict[_currentHairId][_right]];
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.sprite = _spriteSheet[_hairDict[_currentHairId][_right]];
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
                    _sprite.sprite = _spriteSheet[_hairDict[_currentHairId][_back]];
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.sprite = _spriteSheet[_hairDict[_currentHairId][_front]];
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.sprite = _spriteSheet[_hairDict[_currentHairId][_right]];
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.sprite = _spriteSheet[_hairDict[_currentHairId][_right]];
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
