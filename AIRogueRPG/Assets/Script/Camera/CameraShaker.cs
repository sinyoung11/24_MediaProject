using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    private static CameraShaker instance;
    public static CameraShaker Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    private Vector3 originalOffset; // 플레이어로부터의 기본 오프셋
    private bool isShaking = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        if (CameraController.Instance == null)
        {
            Debug.LogError("CameraController instance is missing! Attach CameraController to your camera.");
            return;
        }
        originalOffset = transform.localPosition - CameraController.Instance.transform.position;
    }

    public static void Shake(float duration, float magnitude)
    {
        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.ShakeCoroutine(duration, magnitude));
        }
        else
        {
            Debug.LogWarning("CameraShaker instance not found. Ensure it's attached to your camera.");
        }
    }

    private System.Collections.IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        if (isShaking) yield break;

        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // 흔들림 효과를 적용한 위치 계산
            Vector3 shakeOffset = new Vector3(offsetX, offsetY, 0f);
            transform.localPosition = CameraController.Instance.transform.position + originalOffset + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 원래 위치로 복구
        transform.localPosition = CameraController.Instance.transform.position + originalOffset;
        isShaking = false;
    }
}
