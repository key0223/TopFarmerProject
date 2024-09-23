using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngineInternal;

public class ResourceManager
{
    public T Load<T> (string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            // 만들어진 오브젝트가 있으면 풀에서 땡겨온다.
            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T> (path);
    }
   
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject> ($"Prefabs/{path}");
        if(original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;
        
        // 풀링 대상이 아니라면
        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;

        
    }
    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        // 풀링이 필요하다면 poolManager에 위탁
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
