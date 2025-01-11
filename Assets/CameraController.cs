using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public IEnumerator CameraShake(float duration, float magnitude, float shakeSpeed = 0.1f)
    {
        Vector3 originalPosition = transform.localPosition; // Store the original position of the camera

        float elapsed = 0.0f;

        Vector3 targetPosition = originalPosition; // Initialize the target position for smooth shaking

        while (elapsed < duration)
        {
            float dampingFactor = 1 - (elapsed / duration); // Reduces magnitude over time

            targetPosition = new Vector3(
                originalPosition.x + Random.Range(-1f, 1f) * magnitude * dampingFactor,
                originalPosition.y + Random.Range(-1f, 1f) * magnitude * dampingFactor,
                originalPosition.z
            );

            // Smoothly interpolate towards the target position
            float lerpTime = 0f; // Time taken for each lerp to complete
            while (lerpTime < shakeSpeed)
            {
                lerpTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, lerpTime / shakeSpeed);
                yield return null; // Wait for the next frame
            }

            elapsed += shakeSpeed;
        }

        // Reset the camera position to the original
        transform.localPosition = originalPosition;
    }
}
