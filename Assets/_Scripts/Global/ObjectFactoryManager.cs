using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactoryManager : MonoBehaviourSingleton<ObjectFactoryManager>
{
    public GameObject CreateInstance(GameObject prefab)
    {
        if(prefab != null)
        {
            return Instantiate(prefab);
        }

        return null;
    }

    public GameObject CreateInstance(GameObject prefab, Transform parent)
    {
        if (prefab != null)
        {
            return Instantiate(prefab, parent);
        }

        return null;
    }

    public GameObject CreateInstance(GameObject prefab, Vector3 position, Transform parent)
    {
        if (prefab != null)
        {
            return Instantiate(prefab, position, Quaternion.identity, parent);
        }

        return null;
    }

    public GameObject CreateInstance(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (prefab != null)
        {
            return Instantiate(prefab, position, rotation, parent);
        }

        return null;
    }

    public T CreateInstance<T>(GameObject prefab)
    {
        if (prefab != null)
        {
            return Instantiate(prefab).GetComponent<T>();
        }

        return default(T);
    }

    public T CreateInstance<T>(GameObject prefab, Transform parent)
    {
        if (prefab != null)
        {
            return Instantiate(prefab, parent).GetComponent<T>();
        }

        return default(T);
    }
}