using System;
using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour
{
    public static EffectController Instance { get; private set; }

    public enum EffectType { Perfect, Good, Bad, Miss }
    [SerializeField] private SpriteRenderer comboGlowSprite;
    private Coroutine glowCoroutine;
    private Coroutine blinkCoroutine;

    [Serializable]
    public class EffectEvent
    {
        public EffectType Type;
        public EffectEvent(EffectType type) => Type = type;
    }

    [Serializable]
    public class EffectEventChannel
    {
        public Action<EffectEvent> OnEffectSpawned;
        public void RaiseEvent(EffectEvent e) => OnEffectSpawned?.Invoke(e);
    }

    [Header("Effect Prefabs")]
    public GameObject perfectPrefab;
    public GameObject goodPrefab;
    public GameObject badPrefab;
    public GameObject missPrefab;

    [Header("Effect Layer")]
    public Transform effectLayer;

    [Header("Effect Event Channel")]
    public EffectEventChannel effectChannel = new EffectEventChannel();

    private GameObject currentEffect;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        effectChannel.OnEffectSpawned += HandleEffect;
        ScoreController.OnComboChanged += HandleComboGlow;
    }

    private void OnDisable()
    {
        effectChannel.OnEffectSpawned -= HandleEffect;
        ScoreController.OnComboChanged -= HandleComboGlow;
    }

    private void HandleEffect(EffectEvent e)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (currentEffect != null)
        {
            Destroy(currentEffect);
            currentEffect = null;
        }

        GameObject prefab = null;
        switch (e.Type)
        {
            case EffectType.Perfect: prefab = perfectPrefab; break;
            case EffectType.Good: prefab = goodPrefab; break;
            case EffectType.Bad: prefab = badPrefab; break;
            case EffectType.Miss: prefab = missPrefab; break;
        }

        if (prefab != null)
        {
            currentEffect = Instantiate(prefab, effectLayer.position, Quaternion.identity, effectLayer);
            currentCoroutine = StartCoroutine(PlayEffectAnimation(currentEffect));
        }
    }

    private IEnumerator PlayEffectAnimation(GameObject effect)
    {
        if (effect == null) yield break;

        Transform tf = effect.transform;
        tf.localScale = Vector3.one * 1.2f;

        float shrinkDuration = 0.2f;
        float displayDuration = 1.5f;

        float elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;
            tf.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, t);
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        if (effect != null)
        {
            Destroy(effect);
        }

        currentEffect = null;
        currentCoroutine = null;
    }

    public static void SpawnEffect(EffectType type)
    {
        Instance?.effectChannel.RaiseEvent(new EffectEvent(type));
    }

    public void ClearEffect()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (currentEffect != null)
        {
            Destroy(currentEffect);
            currentEffect = null;
        }

        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
            glowCoroutine = null;
        }

        if (comboGlowSprite != null)
        {
            var c = comboGlowSprite.color;
            c.a = 150f / 255f;
            comboGlowSprite.color = c;
        }
    }

    private void HandleComboGlow(int combo)
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (combo >= 4)
        {
            blinkCoroutine = StartCoroutine(BlinkAlpha(100f / 255f, 255f / 255f));
        }
        else
        {
            float targetAlpha = 150f / 255f;
            if (combo == 2) targetAlpha = 200f / 255f;
            else if (combo == 3) targetAlpha = 255f / 255f;

            if (glowCoroutine != null) StopCoroutine(glowCoroutine);
            glowCoroutine = StartCoroutine(SmoothAlphaChange(targetAlpha));
        }
    }

    private IEnumerator SmoothAlphaChange(float targetAlpha, float duration = 0.3f)
    {
        if (comboGlowSprite == null) yield break;

        Color color = comboGlowSprite.color;
        float startAlpha = color.a;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            comboGlowSprite.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        comboGlowSprite.color = new Color(color.r, color.g, color.b, targetAlpha);
    }

    private IEnumerator BlinkAlpha(float minAlpha, float maxAlpha)
    {
        if (comboGlowSprite == null) yield break;

        float duration = 0.5f;
        while (true)
        {
            yield return SmoothAlphaChange(maxAlpha, duration / 2f);
            yield return SmoothAlphaChange(minAlpha, duration / 2f);
        }
    }
}
