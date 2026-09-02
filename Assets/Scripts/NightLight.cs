using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NightLight : MonoBehaviour
{
    private Light2D light2D;

    private void Awake()
    {
        // gets the point light on this object
        light2D = GetComponent<Light2D>();
    }

    private void Start()
    {
        // updates the light when the scene loads
        UpdateLight();
    }

    public void UpdateLight()
    {
        // makes sure the game manager and light are available before updating
        if (GameManager.Instance == null || light2D == null)
            return;

        // only turns the light on during evening/night
        light2D.enabled =
            GameManager.Instance.CurrentTime == GameTime.Evening ||
            GameManager.Instance.CurrentTime == GameTime.Night ||
            GameManager.Instance.CurrentTime == GameTime.LateNight;
    }
}