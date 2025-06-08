using System.Collections;
using UnityEngine;

public class TapTile : Tile
{
    private bool inZone = false;
    [SerializeField] private GameObject hitOverlay;
    private Vector3 initialScale;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    public override void Activate()
    {
        base.Activate();
        isHit = false;

        if (hitOverlay != null)
        {
            hitOverlay.SetActive(false);
        }
        transform.localScale = initialScale;

    }
    public override void OnEnterHitZone()
    {
        inZone = true;
        TileManager.Instance.AddTileToZone(this);
    }

    public override void OnExitHitZone()
    {
        inZone = false;
        TileManager.Instance.RemoveTileFromZone(this);
        if (!isHit)
        {
            EffectController.SpawnEffect(EffectController.EffectType.Miss);
            ScoreController.Instance.AddScore("Miss");
        }
        Deactivate();
    }

    public override void OnInputDown()
    {
        if (!inZone || isHit) return;

        float dist = Mathf.Abs(transform.position.y - HitLine.Instance.transform.position.y);
        if (dist < 0.2f)
        {
            ScoreController.Instance.AddScore("Perfect");
            EffectController.SpawnEffect(EffectController.EffectType.Perfect);
        }
        else if (dist < 0.5f)
        {
            ScoreController.Instance.AddScore("Good");
            EffectController.SpawnEffect(EffectController.EffectType.Good);
        }
        else
        {
            ScoreController.Instance.AddScore("Bad");
            EffectController.SpawnEffect(EffectController.EffectType.Bad);
        }

        isHit = true;
        StartCoroutine(PlayHitEffectAndDeactivate());
    }

    public override void OnInputUp() { }

    private IEnumerator PlayHitEffectAndDeactivate()
    {
        hitOverlay.SetActive(true);

        Vector3 targetScale = initialScale * 0.8f;
        float duration = 0.15f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }

        Deactivate();
    }
}
