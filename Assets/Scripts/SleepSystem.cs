using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
public class SleepSystem : MonoBehaviour
{
    [Header("Sleep Settings")]
    public float sleepHours = 8f;
    [Header("Day Night Manager")]
    public DayNightManager dayNightManager;
    [Header("Sleep UI")]
    public GameObject sleepPrompt;
    public GameObject sleepOverlay;
    public TMP_Text sleepingText;
    [Header("Fade Settings")]
    public float fadeTime = 1.5f;
    public float sleepDuration = 5.5f;
    private bool playerNearby = false;
    private bool isSleeping = false;
    private CanvasGroup overlayGroup;
    void Start()
    {
        if (sleepPrompt != null)
        {
            sleepPrompt.SetActive(false);
        }
        if (sleepOverlay != null)
        {
            overlayGroup = sleepOverlay.GetComponent<CanvasGroup>();
            if (overlayGroup == null)
            {
                overlayGroup =
                    sleepOverlay.AddComponent<CanvasGroup>();
            }
            overlayGroup.alpha = 0f;
            sleepOverlay.SetActive(false);
        }
        if (sleepingText != null)
        {
            sleepingText.text = "Sleeping...";
        }
    }
    void Update()
    {
        if (playerNearby &&
            !isSleeping &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(SleepSequence());
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (sleepPrompt != null &&
                !isSleeping)
            {
                sleepPrompt.SetActive(true);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (sleepPrompt != null)
            {
                sleepPrompt.SetActive(false);
            }
        }
    }
    IEnumerator SleepSequence()
    {
        isSleeping = true;
        if (sleepPrompt != null)
        {
            sleepPrompt.SetActive(false);
        }
        if (sleepOverlay != null)
        {
            sleepOverlay.SetActive(true);
        }
        if (sleepingText != null)
        {
            sleepingText.text = "Sleeping...";
        }
        yield return StartCoroutine(
            FadeOverlay(0f, 1f, fadeTime)
        );
        yield return new WaitForSeconds(sleepDuration);
        if (dayNightManager != null)
        {
            dayNightManager.SleepForHours(sleepHours);
        }
        else
        {
            Debug.LogError(
                "SleepSystem: DayNightManager is not assigned!"
            );
        }
        if (sleepingText != null)
        {
            sleepingText.text = "8 hours later...";
        }
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(
            FadeOverlay(1f, 0f, fadeTime)
        );
        if (sleepOverlay != null)
        {
            sleepOverlay.SetActive(false);
        }
        isSleeping = false;
    }
    IEnumerator FadeOverlay(
        float startAlpha,
        float endAlpha,
        float duration
    )
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress =
                Mathf.Clamp01(timer / duration);
            overlayGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    progress
                );
            yield return null;
        }
        overlayGroup.alpha = endAlpha;
    }
}