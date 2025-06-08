using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int poolSize = 10;
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetFromPool()
    {
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(true);
            return obj;
        }

        GameObject item = pool.Dequeue();
        item.SetActive(true);
        return item;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (pool.Contains(obj)) return; 
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public void ClearPool()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                ReturnToPool(child.gameObject);
            }
        }
    }
}
