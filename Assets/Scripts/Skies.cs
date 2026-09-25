using UnityEngine;
public class Skies : MonoBehaviour
{
    [Header("Skyboxes")]
    public Material daySky;
    public Material eveningSky;
    public Material nightSky;
    [Header("Sky Times")]
    [Range(0f, 1f)]
    public float eveningStart = 0.65f;
    [Range(0f, 1f)]
    public float nightStart = 0.78f;
    [Range(0f, 1f)]
    public float morningStart = 0.20f;
    [Header("Current Time")]
    [Range(0f, 1f)]
    public float currentTime = 0.25f;
    void Start()
    {
        UpdateSky();
    }
    void Update()
    {
        UpdateSky();
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
}