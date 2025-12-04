using System.Collections;
using UnityEngine;

public class GhostFader : MonoBehaviour
{
    [Header("Timing")]
    public float minVisibleTime = 1f;
    public float maxVisibleTime = 3f;
    public float minInvisibleTime = 0.5f;
    public float maxInvisibleTime = 2f;

    [Header("Fade")]
    public float fadeDuration = 0.3f;

    [Header("Options")]
    public bool disableCollidersWhenInvisible = true;

    Renderer[] meshRenderers;
    SpriteRenderer[] spriteRenderers;
    Collider[] colliders;

    Coroutine fadeRoutine;

    static readonly int ColorID = Shader.PropertyToID("_Color");
    static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    void Awake()
    {
        meshRenderers = GetComponentsInChildren<Renderer>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider>();

        Debug.Log($"{name}: GhostFader found {meshRenderers.Length} mesh renderers and {spriteRenderers.Length} sprite renderers.");
    }

    void OnEnable()
    {
        SetAlpha(1f);
        SetCollidersEnabled(true);
        fadeRoutine = StartCoroutine(FadeLoop());
    }

    void OnDisable()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        SetAlpha(1f);
        SetCollidersEnabled(true);
    }

    IEnumerator FadeLoop()
    {
        while (true)
        {
            float visibleTime = Random.Range(minVisibleTime, maxVisibleTime);
            yield return new WaitForSeconds(visibleTime);

            yield return StartCoroutine(FadeTo(0f));

            float invisibleTime = Random.Range(minInvisibleTime, maxInvisibleTime);
            yield return new WaitForSeconds(invisibleTime);

            yield return StartCoroutine(FadeTo(1f));
        }
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = GetCurrentAlpha();
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = fadeDuration > 0f ? t / fadeDuration : 1f;
            float a = Mathf.Lerp(startAlpha, targetAlpha, lerp);

            SetAlpha(a);

            if (disableCollidersWhenInvisible)
            {
                if (a <= 0.01f) SetCollidersEnabled(false);
                else if (a >= 0.99f) SetCollidersEnabled(true);
            }

            yield return null;
        }

        SetAlpha(targetAlpha);

        if (disableCollidersWhenInvisible)
            SetCollidersEnabled(targetAlpha > 0.01f);
    }

    float GetCurrentAlpha()
    {
        if (spriteRenderers != null && spriteRenderers.Length > 0)
            return spriteRenderers[0].color.a;

        if (meshRenderers != null && meshRenderers.Length > 0)
        {
            var mat = meshRenderers[0].material;

            if (mat.HasProperty(ColorID))
                return mat.color.a;

            if (mat.HasProperty(BaseColorID))
                return mat.GetColor(BaseColorID).a;
        }

        return 1f;
    }

    void SetAlpha(float alpha)
    {
        if (spriteRenderers != null)
        {
            foreach (var sr in spriteRenderers)
            {
                if (sr == null) continue;
                var c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
        }

        if (meshRenderers != null)
        {
            foreach (var r in meshRenderers)
            {
                if (r == null) continue;
                var mat = r.material;

                if (mat.HasProperty(ColorID))
                {
                    var c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
                else if (mat.HasProperty(BaseColorID))
                {
                    var c = mat.GetColor(BaseColorID);
                    c.a = alpha;
                    mat.SetColor(BaseColorID, c);
                }
            }
        }
    }

    void SetCollidersEnabled(bool enabled)
    {
        if (colliders == null) return;
        foreach (var col in colliders)
        {
            if (col == null) continue;
            col.enabled = enabled;
        }
    }
}
