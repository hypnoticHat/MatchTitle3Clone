using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public ObjectPool normalTilePool;
    public ObjectPool longTilePool;

    void Awake()
    {
        Instance = this;
    }

    public GameObject GetTile(bool isLong)
    {
        return isLong ? longTilePool.GetFromPool() : normalTilePool.GetFromPool();
    }
}
