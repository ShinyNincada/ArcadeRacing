using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;

    bool isShaking = false;
    
    
    public async void ShakeCamera()
    {
        isShaking = true;
        Vector3 originalPosition = transform.localPosition;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsedTime += Time.deltaTime;

            await Task.Yield();
        }

        transform.localPosition = originalPosition;
        isShaking = false;
    }
}
