using System.Collections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public Transform cameraHolder;
    private Coroutine currentShake;
    
    public void StartShake(float duration, float magnitude)
    {
        if (currentShake != null)
            StopCoroutine(currentShake);
        currentShake = StartCoroutine(Shake(duration, magnitude));
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition; // Use localPosition relative to the holder, include z
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-0.2f, 0.2f) * magnitude;
            float y = Random.Range(-0.2f, 0.2f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f); // Apply offset preserving z

            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.localPosition = originalPos; // Return to original position after the shake
        currentShake = null;
    }
}
