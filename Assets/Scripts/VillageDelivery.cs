using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
public class VillageDelivery : MonoBehaviour
{
    [Header("Village")]
    public string villageName;
    [Header("Interaction")]
    public TMP_Text deliveryPrompt;
    [Header("Delivery")]
    public int suppliesRequired = 1;
    [Header("Delivery UI")]
    public GameObject deliveryOverlay;
    public TMP_Text deliveryMessage;
    [Header("Fade Settings")]
    public float fadeTime = 1.5f;
    public float messageTime = 2f;
    [Header("Trust")]
    public float trustGain = 20f;
    private bool playerNearby = false;
    private bool isDelivering = false;
    private bool deliveryActive = false;
    private CanvasGroup overlayGroup;
    void Start()
    {
        if (deliveryPrompt != null)
        {
            deliveryPrompt.gameObject.SetActive(false);
        }
        if (deliveryOverlay != null)
        {
            overlayGroup =
                deliveryOverlay.GetComponent<CanvasGroup>();
            if (overlayGroup == null)
            {
                overlayGroup =
                    deliveryOverlay.AddComponent<CanvasGroup>();
            }
            overlayGroup.alpha = 0f;
            deliveryOverlay.SetActive(false);
        }
    }
    void Update()
    {
        if (playerNearby &&
            deliveryActive &&
            !isDelivering &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(DeliverySequence());
        }
    }
    public void SetDeliveryActive(bool active)
    {
        deliveryActive = active;
        if (!active)
        {
            playerNearby = false;
            if (deliveryPrompt != null)
            {
                deliveryPrompt.gameObject.SetActive(false);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (deliveryActive &&
                deliveryPrompt != null &&
                !isDelivering)
            {
                deliveryPrompt.gameObject.SetActive(true);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (deliveryPrompt != null)
            {
                deliveryPrompt.gameObject.SetActive(false);
            }
        }
    }
    IEnumerator DeliverySequence()
    {
        isDelivering = true;
        if (deliveryPrompt != null)
        {
            deliveryPrompt.gameObject.SetActive(false);
        }
        if (InventoryManager.Instance == null)
        {
            Debug.LogError(
                "VillageDelivery: InventoryManager not found!"
            );
            isDelivering = false;
            yield break;
        }
        bool removed =
            InventoryManager.Instance.RemoveSupplies(
                suppliesRequired
            );
        if (!removed)
        {
            Debug.Log("Not enough supplies!");
            if (deliveryPrompt != null)
            {
                deliveryPrompt.gameObject.SetActive(true);
            }
            isDelivering = false;
            yield break;
        }
        if (TrustManager.Instance != null)
        {
            TrustManager.Instance.IncreaseTrust(
                trustGain
            );
        }
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.DeliveryCompleted();
        }
        if (deliveryOverlay != null)
        {
            deliveryOverlay.SetActive(true);
        }
        if (deliveryMessage != null)
        {
            deliveryMessage.text =
                "Supplies Delivered!";
        }
        yield return StartCoroutine(
            FadeOverlay(
                0f,
                1f,
                fadeTime
            )
        );
        yield return new WaitForSeconds(
            messageTime
        );
        if (deliveryMessage != null)
        {
            deliveryMessage.text =
                "Trust Gained!";
        }
        yield return new WaitForSeconds(
            1.5f
        );
        yield return StartCoroutine(
            FadeOverlay(
                1f,
                0f,
                fadeTime
            )
        );
        if (deliveryOverlay != null)
        {
            deliveryOverlay.SetActive(false);
        }
        isDelivering = false;
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
                Mathf.Clamp01(
                    timer / duration
                );
            if (overlayGroup != null)
            {
                overlayGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        endAlpha,
                        progress
                    );
            }
            yield return null;
        }
        if (overlayGroup != null)
        {
            overlayGroup.alpha = endAlpha;
        }
    }
}