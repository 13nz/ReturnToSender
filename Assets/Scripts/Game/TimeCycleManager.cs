using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeCycleManager : MonoBehaviour
{
    [Header("global light")]
    [SerializeField] private Light2D globalLight;

    [Header("morning")]
    [SerializeField] private Color morningColor = Color.white;
    [SerializeField] private float morningIntensity = 1f;

    [Header("late morning")]
    [SerializeField] private Color lateMorningColor = new Color(1f, 0.98f, 0.92f);
    [SerializeField] private float lateMorningIntensity = 1f;

    [Header("noon")]
    [SerializeField] private Color noonColor = Color.white;
    [SerializeField] private float noonIntensity = 1f;

    [Header("afternoon")]
    [SerializeField] private Color afternoonColor = new Color(1f, 0.82f, 0.62f);
    [SerializeField] private float afternoonIntensity = 0.9f;

    [Header("late afternoon")]
    [SerializeField] private Color lateAfternoonColor = new Color(1f, 0.68f, 0.48f);
    [SerializeField] private float lateAfternoonIntensity = 0.75f;

    [Header("evening")]
    [SerializeField] private Color eveningColor = new Color(0.72f, 0.62f, 0.85f);
    [SerializeField] private float eveningIntensity = 0.55f;

    [Header("night")]
    [SerializeField] private Color nightColor = new Color(0.45f, 0.58f, 0.85f);
    [SerializeField] private float nightIntensity = 0.35f;

    [Header("late night")]
    [SerializeField] private Color lateNightColor = new Color(0.25f, 0.35f, 0.65f);
    [SerializeField] private float lateNightIntensity = 0.2f;

    private void Start()
    {
        // applies the current game time when the main scene loads
        ApplyCurrentTime();
    }

    public void ApplyCurrentTime()
    {
        // prevents the lighting system from failing if the global light is missing
        if (globalLight == null)
        {
            Debug.LogWarning("no global light 2d has been assigned to the time cycle manager.");
            return;
        }

        // prevents the lighting system from failing if the game manager is missing
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("no game manager was found for the time cycle manager.");
            return;
        }

        // changes the global light based on the current game time
        switch (GameManager.Instance.CurrentTime)
        {
            case GameTime.Morning:
                SetLighting(morningColor, morningIntensity);
                break;

            case GameTime.LateMorning:
                SetLighting(lateMorningColor, lateMorningIntensity);
                break;

            case GameTime.Noon:
                SetLighting(noonColor, noonIntensity);
                break;

            case GameTime.Afternoon:
                SetLighting(afternoonColor, afternoonIntensity);
                break;

            case GameTime.LateAfternoon:
                SetLighting(lateAfternoonColor, lateAfternoonIntensity);
                break;

            case GameTime.Evening:
                SetLighting(eveningColor, eveningIntensity);
                break;

            case GameTime.Night:
                SetLighting(nightColor, nightIntensity);
                break;

            case GameTime.LateNight:
                SetLighting(lateNightColor, lateNightIntensity);
                break;
        }
    }

    private void SetLighting(Color color, float intensity)
    {
        // applies the selected color and brightness to the global light
        globalLight.color = color;
        globalLight.intensity = intensity;
    }
}