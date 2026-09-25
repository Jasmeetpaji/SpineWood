using UnityEngine;
public class DayNightManager : MonoBehaviour
{
    [Header("Time")]
    public float dayLength = 300f;
    [Range(0f, 1f)]
    public float startTime = 0.25f;
    private float currentTime;
    [Header("Sun")]
    public Light sun;
    public float maxSunIntensity = 1.2f;
    public float nightSunIntensity = 0.05f;
    [Header("Skyboxes")]
    public Material daySky;
    public Material eveningSky;
    public Material nightSky;
    [Header("Sky Timing")]
    [Range(0f, 1f)]
    public float morningStart = 0.20f;
    [Range(0f, 1f)]
    public float eveningStart = 0.65f;
    [Range(0f, 1f)]
    public float nightStart = 0.78f;
    void Start()
    {
        currentTime = startTime;
        UpdateSun();
        UpdateSky();
    }
    void Update()
    {
        UpdateTime();
        UpdateSun();
        UpdateSky();
    }
    void UpdateTime()
    {
        currentTime += Time.deltaTime / dayLength;
        if (currentTime >= 1f)
        {
            currentTime -= 1f;
        }
    }
    void UpdateSun()
    {
        if (sun == null)
            return;
        float sunAngle =
            currentTime * 360f - 90f;
        sun.transform.rotation =
            Quaternion.Euler(
                sunAngle,
                170f,
                0f
            );
        float sunHeight =
            Mathf.Sin(
                currentTime * Mathf.PI * 2f
            );
        if (sunHeight > 0f)
        {
            sun.intensity =
                sunHeight * maxSunIntensity;
        }
        else
        {
            sun.intensity =
                nightSunIntensity;
        }
    }
    void UpdateSky()
    {
        if (daySky == null ||
            eveningSky == null ||
            nightSky == null)
        {
            return;
        }
        if (currentTime >= nightStart ||
            currentTime < morningStart)
        {
            RenderSettings.skybox = nightSky;
        }
        else if (currentTime >= eveningStart)
        {
            RenderSettings.skybox = eveningSky;
        }
        else
        {
            RenderSettings.skybox = daySky;
        }
        DynamicGI.UpdateEnvironment();
    }
    public void SleepForHours(float hours)
    {
        float timeToAdd = hours / 24f;
        currentTime += timeToAdd;
        if (currentTime >= 1f)
        {
            currentTime -= 1f;
        }
        UpdateSun();
        UpdateSky();
        Debug.Log(
            "Player slept for " +
            hours +
            " hours."
        );
    }
    public float GetCurrentTime()
    {
        return currentTime;
    }
}