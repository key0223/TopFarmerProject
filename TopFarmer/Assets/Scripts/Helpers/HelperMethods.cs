using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public static class HelperMethods 
{

    public static bool GetComponentsAtCursorLocation<T> (out List<T> componentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;

        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);

        T tComponent = default(T);

        for (int i = 0; i< collider2DArray.Length; i++)
        {
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if(tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if(tComponent != null )
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        componentsAtPositionList = componentList;
        return found;
    }

    public static  bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
        bool found =false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tComponent !=null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if(tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
                
            }
        }

        listComponentsAtBoxPosition = componentList;

        return found;
    }

    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size, float angle)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];

        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);

        T tComponent = default(T);

        T[] componentArray = new T[collider2DArray.Length];

        for (int i = collider2DArray.Length - 1; i >= 0; i--)
        {
            if (collider2DArray[i] != null)
            {
                tComponent = collider2DArray[i].gameObject.GetComponent<T>();

                if (tComponent != null)
                {
                    componentArray[i] = tComponent;
                }
            }
        }

        return componentArray;
    }

    #region Animation
    public static AnimationClip GetAnimationClip(Animator animator, string name)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }
    public static float GetRemainingAnimationTime(Animator animator ,int layerIndex = 0)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        float clipLength = stateInfo.length;
        float normalizedTime = stateInfo.normalizedTime % 1f;

        // 남은 시간 계산
        float remainingTime = clipLength * (1f - normalizedTime);

        return remainingTime;
    }

    public static float GetAnimationClipLenth(Animator animator, string name)
    {
        AnimationClip clip = GetAnimationClip(animator,name);

        if (clip != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);

            foreach (AnimatorClipInfo clipInfo in clipInfos)
            {
                if (clipInfo.clip == clip)
                {
                    float clipLength = clip.length;
                    float clipSpeed = stateInfo.speed * animator.speed;

                    float actualLength = clipLength / clipSpeed;

                    return actualLength;
                }
            }
        }
        return 1;
    }

   public static string GetToolAnimationName(ItemType itemType)
    {
        string animationName = "";

        switch (itemType)
        {
            case ItemType.ITEM_TOOL_WATERING:
                animationName = "WATERING";
                break;
            case ItemType.ITEM_TOOL_HOEING:
                animationName = "BREAKING";
                break;
            case ItemType.ITEM_TOOL_AXE:
                animationName = "BREAKING";
                break;
            case ItemType.ITEM_TOOL_PICKAXE:
                animationName = "BREAKING";
                break;
            case ItemType.ITEM_TOOL_SCYTHE:
                animationName = "SCYTHE";
                break;
            case ItemType.ITEM_TOOL_COLLECTING:
                animationName = "HARVEST";
                break;
            case ItemType.ITEM_WEAPON:
                animationName = "SCYTHE";
                break;

        }

        return animationName;
    }
    #endregion
}
