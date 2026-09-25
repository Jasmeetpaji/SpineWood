using UnityEngine;
public class DayNightCycle : MonoBehaviour
{
    [Header("Time")]
    public float dayLength = 120f;
    [Range(0f, 1f)]
    public float startTime = 0.25f;
    private float currentTime;
    [Header("Sun")]
    public Light sun;
    [Header("Sun Settings")]
    public float sunIntensity = 1.2f;
    [Header("Ambient Light")]
    public Color dayAmbientColor = Color.white;
    public Color nightAmbientColor =
        new Color(0.05f, 0.07f, 0.15f);
    [Header("Sky Colors")]
    public Color daySkyColor =
        new Color(0.5f, 0.7f, 1f);
    public Color nightSkyColor =
        new Color(0.02f, 0.03f, 0.08f);
    void Start()
    {
        currentTime = startTime;
        UpdateSun();
        UpdateEnvironment();
    }
    void Update()
    {
        UpdateTime();
        UpdateSun();
        UpdateEnvironment();
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
        float sunRotation =
            currentTime * 360f - 90f;
        sun.transform.rotation =
            Quaternion.Euler(
                sunRotation,
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
                sunHeight * sunIntensity;
        }
        else
        {
            sun.intensity = 0f;
        }
    }
    void UpdateEnvironment()
    {
        float sunHeight =
            Mathf.Sin(
                currentTime * Mathf.PI * 2f
            );
        float dayAmount =
            Mathf.Clamp01(
                (sunHeight + 0.2f) / 0.7f
            );
        RenderSettings.ambientLight =
            Color.Lerp(
                nightAmbientColor,
                dayAmbientColor,
                dayAmount
            );
        if (Camera.main != null)
        {
            Camera.main.backgroundColor =
                Color.Lerp(
                    nightSkyColor,
                    daySkyColor,
                    dayAmount
                );
        }
    }
    public void SleepForHours(float hours)
    {
        float timeToAdd =
            hours / 24f;
        currentTime += timeToAdd;
        if (currentTime >= 1f)
        {
            currentTime -= 1f;
        }
        UpdateSun();
        UpdateEnvironment();
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