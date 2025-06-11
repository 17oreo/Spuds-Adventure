using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FogController : MonoBehaviour
{
    public static FogController Instance;
    [SerializeField] private Image fogImage;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerFogEffect()
    {
        StartCoroutine(FogUp());
    }

    IEnumerator FogUp()
    {
        float duration = 0.5f;
        float maxAlpha = 130f / 255f;
        float t = 0f;
        Color color = fogImage.color;

        // Fade in
        while (t < duration)
        {
            if (GameManager.Instance.CurrentState == GameState.GameOver)
                yield break;

            float alpha = Mathf.Lerp(0f, maxAlpha, t / duration);
            fogImage.color = new Color(color.r, color.g, color.b, alpha);
            t += Time.deltaTime;
            yield return null;
        }

        fogImage.color = new Color(color.r, color.g, color.b, maxAlpha);

        // Wait while fog is fully visible
        float timer = 0f;
        while (timer < 1f)
        {
            if (GameManager.Instance.CurrentState == GameState.GameOver)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < duration)
        {
            if (GameManager.Instance.CurrentState == GameState.GameOver)
                yield break;

            float alpha = Mathf.Lerp(maxAlpha, 0f, t / duration);
            fogImage.color = new Color(color.r, color.g, color.b, alpha);
            t += Time.deltaTime;
            yield return null;
        }

        fogImage.color = new Color(color.r, color.g, color.b, 0f);
    }
}
