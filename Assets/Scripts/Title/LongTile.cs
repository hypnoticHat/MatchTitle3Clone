using System.Collections;
using UnityEngine;

public class LongTile : Tile
{
    private bool inZone = false;
    private bool isHolding = false;
    private float holdStartY;
    private float tileHeight;

    [SerializeField] private GameObject hitOverlay;
    [SerializeField] private Transform bodyTransform;

    private float initialOverlayScaleY;
    private float initialOverlayPosY;
    private Vector3 overlayStartLocalPos;

    public override void Activate()
    {
        base.Activate();
        isHolding = false;
        inZone = false;
        isHit = false;

        tileHeight = GetComponent<Renderer>().bounds.size.y;

        if (hitOverlay != null)
        {
            hitOverlay.SetActive(false);
            initialOverlayScaleY = hitOverlay.transform.localScale.y;
            initialOverlayPosY = hitOverlay.transform.localPosition.y;

            hitOverlay.transform.localScale = new Vector3(1f, 0.01f, 1f);
        }
    }

    protected override void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (isHolding && hitOverlay != null)
        {
            hitOverlay.SetActive(true);
            float heldDistance = holdStartY - transform.position.y;
            float percent = Mathf.Clamp01(heldDistance / tileHeight);

            float maxY = bodyTransform.localScale.y;
            float overlayScaleY = percent * maxY;

            hitOverlay.transform.localScale = new Vector3(1f, overlayScaleY, 1f);

            float offsetY = overlayScaleY / 2f; 
            hitOverlay.transform.localPosition = new Vector3(0f, overlayStartLocalPos.y - offsetY, 0f);
        }
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
            if (isHolding)
            {
                HandleReleaseScore();
            }
            else if ( gameObject.activeSelf)
            {
                EffectController.SpawnEffect(EffectController.EffectType.Miss);
                isHit = true;
                StartCoroutine(PlayHitEffectAndDeactivate());
            }
        }
    }

    public override void OnInputDown()
    {
        if (!inZone || isHit) return;

        isHolding = true;
        holdStartY = transform.position.y;

        if (hitOverlay != null)
        {
            hitOverlay.SetActive(true);

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float localY = transform.InverseTransformPoint(worldPoint).y;

            float halfHeight = tileHeight / 2f;
            localY = Mathf.Clamp(localY, -halfHeight, halfHeight);

            overlayStartLocalPos = new Vector3(0, localY, 0);
            hitOverlay.transform.localPosition = overlayStartLocalPos;

            hitOverlay.transform.localScale = new Vector3(1f, 0.01f, 1f);
        }
    }


    public override void OnInputUp()
    {
        if (!inZone || isHit) return;

        isHolding = false;
        HandleReleaseScore();
    }

    private void HandleReleaseScore()
    {
        float distanceHeld = holdStartY - transform.position.y;
        float holdPercent = distanceHeld / tileHeight;

        if (holdPercent >= 0.9f)
        {
            ScoreController.Instance.AddScore("Perfect");
            EffectController.SpawnEffect(EffectController.EffectType.Perfect);
        }
        else if (holdPercent >= 0.6f)
        {
            ScoreController.Instance.AddScore("Good");
            EffectController.SpawnEffect(EffectController.EffectType.Good);
        }
        else
        {
            EffectController.SpawnEffect(EffectController.EffectType.Bad);
            ScoreController.Instance.AddScore("Bad");
        }

        isHit = true;
        StartCoroutine(PlayHitEffectAndDeactivate());
    }

    private IEnumerator PlayHitEffectAndDeactivate()
    {
        yield return new WaitForSeconds(0.1f);
        hitOverlay?.SetActive(false);
        Deactivate();
    }
}
