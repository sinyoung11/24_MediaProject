using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance { get; private set; }

    [SerializeField] private Image fadeImage; // 페이드 효과를 위한 이미지
    [SerializeField] private float fadeDuration = 0.35f; // 페이드 지속 시간

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 페이드 효과 호출
    /// </summary>
    /// <param name="color">페이드 색상</param>
    /// <param name="intensity">페이드 강도 (0 ~ 1)</param>
    public void Fade(Color color, float intensity)
    {
        if (fadeImage != null)
        {
            StartCoroutine(FadeCoroutine(color, intensity));
        }
        else
        {
            Debug.LogWarning("Fade Image is not assigned!");
        }
    }

    private System.Collections.IEnumerator FadeCoroutine(Color color, float intensity)
    {
        // 페이드 인
        float elapsed = 0f;
        fadeImage.color = new Color(color.r, color.g, color.b, 0f);

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, intensity, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, intensity);

        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(intensity, 0f, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 0f);
    }
}