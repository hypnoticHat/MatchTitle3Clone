using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public bool isHit = false;

    public float fallSpeed = 5f;
    protected virtual void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }
    public virtual void Activate()
    {
        isHit = false;
        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
        if (this is TapTile)
            PoolManager.Instance.normalTilePool.ReturnToPool(gameObject);
        else if (this is LongTile)
            PoolManager.Instance.longTilePool.ReturnToPool(gameObject);
    }

    public abstract void OnEnterHitZone();
    public abstract void OnExitHitZone();
    public abstract void OnInputDown();
    public abstract void OnInputUp();
}

