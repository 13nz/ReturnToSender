using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LighthouseLight : MonoBehaviour
{
    [Header("light settings")]
    [SerializeField] private Light2D lighthouseLight;

    [Header("rotation settings")]
    [SerializeField] private float rotationSpeed = 25f;

    [Header("fade settings")]
    [SerializeField] private float maximumIntensity = 1f;
    [SerializeField] private float minimumIntensity = 0f;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;

    [Header("time settings")]
    [SerializeField] private bool useGameTime = true;
    [SerializeField] private float nightStartTime = 18f;
    [SerializeField] private float nightEndTime = 6f;

    private float fadeTimer;
    private bool fadingIn;

    private void Awake()
    {
        if (lighthouseLight == null)
            lighthouseLight = GetComponent<Light2D>();

        if (lighthouseLight == null)
        {
            Debug.LogWarning("no light2d component was found on this object.");
            enabled = false;
            return;
        }

        lighthouseLight.enabled = true;
        lighthouseLight.intensity = minimumIntensity;

        fadeTimer = 0f;
        fadingIn = true;
    }

    private void Update()
    {
        // rotates the spotlight around the lighthouse.
        transform.Rotate(
            Vector3.forward,
            -rotationSpeed * Time.deltaTime,
            Space.Self
        );

        // when disabled, the lighthouse can be tested at any time.
        if (useGameTime && !IsNighttime())
        {
            lighthouseLight.intensity = minimumIntensity;
            fadeTimer = 0f;
            fadingIn = true;
            return;
        }

        FadeLight();
    }

    private void FadeLight()
    {
        if (fadingIn)
        {
            fadeTimer += Time.deltaTime;

            float fadeAmount = Mathf.Clamp01(
                fadeTimer / fadeInDuration
            );

            lighthouseLight.intensity = Mathf.Lerp(
                minimumIntensity,
                maximumIntensity,
                fadeAmount
            );

            if (fadeAmount >= 1f)
            {
                fadingIn = false;
                fadeTimer = 0f;
            }
        }
        else
        {
            fadeTimer += Time.deltaTime;

            float fadeAmount = Mathf.Clamp01(
                fadeTimer / fadeOutDuration
            );

            lighthouseLight.intensity = Mathf.Lerp(
                maximumIntensity,
                minimumIntensity,
                fadeAmount
            );

            if (fadeAmount >= 1f)
            {
                fadingIn = true;
                fadeTimer = 0f;
            }
        }
    }

    private bool IsNighttime()
    {
        // supports a nighttime period that crosses midnight.
        if (nightStartTime > nightEndTime)
        {
            return Time.time >= nightStartTime ||
                   Time.time < nightEndTime;
        }

        return Time.time >= nightStartTime &&
               Time.time < nightEndTime;
    }
}