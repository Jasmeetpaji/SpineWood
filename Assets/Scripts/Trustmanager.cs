using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TrustManager : MonoBehaviour
{
    public static TrustManager Instance;
    [Header("Trust Settings")]
    [Range(0f, 100f)]
    public float trust = 50f;
    public float trustDecreaseAmount = 5f;
    public float trustDecreaseInterval = 30f;
    public float trustGainPerDelivery = 20f;
    [Header("UI")]
    public Slider trustBar;
    public TMP_Text trustText;
    private float decreaseTimer;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        UpdateUI();
    }
    void Update()
    {
        decreaseTimer += Time.deltaTime;
        if (decreaseTimer >= trustDecreaseInterval)
        {
            decreaseTimer = 0f;
            DecreaseTrust(trustDecreaseAmount);
        }
    }
    public void IncreaseTrust(float amount)
    {
        trust += amount;
        trust = Mathf.Clamp(trust, 0f, 100f);
        UpdateUI();
        Debug.Log(
            "Trust increased to " +
            trust
        );
    }
    public void DecreaseTrust(float amount)
    {
        trust -= amount;
        trust = Mathf.Clamp(trust, 0f, 100f);
        UpdateUI();
        Debug.Log(
            "Trust decreased to " +
            trust
        );
    }
    void UpdateUI()
    {
        if (trustBar != null)
        {
            trustBar.value = trust;
        }
        if (trustText != null)
        {
            trustText.text =
                "Trust: " +
                Mathf.RoundToInt(trust) +
                "%";
        }
    }
}