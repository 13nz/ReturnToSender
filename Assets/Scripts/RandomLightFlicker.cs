using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RandomLightFlicker : MonoBehaviour
{
    [Header("flicker settings")]
    [SerializeField] private float minimumIntensityMultiplier = 0.85f;
    [SerializeField] private float maximumIntensityMultiplier = 1.05f;

    [SerializeField] private float minimumWaitTime = 0.05f;
    [SerializeField] private float maximumWaitTime = 0.35f;

    [SerializeField] private float minimumChangeDuration = 0.03f;
    [SerializeField] private float maximumChangeDuration = 0.12f;

    private void Start()
    {
        Light2D[] lights = GetComponentsInChildren<Light2D>(true);

        foreach (Light2D light in lights)
        {
            StartCoroutine(FlickerLight(light));
        }
    }

    private IEnumerator FlickerLight(Light2D light)
    {
        float originalIntensity = light.intensity;

        while (light != null)
        {
            float waitTime = Random.Range(
                minimumWaitTime,
                maximumWaitTime
            );

            yield return new WaitForSeconds(waitTime);

            float intensityMultiplier = Random.Range(
                minimumIntensityMultiplier,
                maximumIntensityMultiplier
            );

            float targetIntensity = originalIntensity * intensityMultiplier;

            float changeDuration = Random.Range(
                minimumChangeDuration,
                maximumChangeDuration
            );

            float startingIntensity = light.intensity;
            float elapsedTime = 0f;

            while (elapsedTime < changeDuration)
            {
                if (light == null)
                    yield break;

                elapsedTime += Time.deltaTime;

                float progress = elapsedTime / changeDuration;

                light.intensity = Mathf.Lerp(
                    startingIntensity,
                    targetIntensity,
                    progress
                );

                yield return null;
            }

            light.intensity = targetIntensity;
        }
    }
}