using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItSelf : MonoBehaviour
{
    public void Destory(float time)
    {
        Invoke("CallDestroy", time);
    }

    private void CallDestroy()
    {
        Managers.Resource.Destroy(gameObject);
    }
}
