using UnityEngine;
using TMPro;
public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;
    [Header("Villages")]
    public VillageDelivery[] villages;
    [Header("Current Delivery UI")]
    public TMP_Text destinationText;
    private VillageDelivery currentVillage;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ChooseRandomVillage();
    }
    public void ChooseRandomVillage()
    {
        if (villages == null || villages.Length == 0)
        {
            Debug.LogError(
                "DeliveryManager: No villages assigned!"
            );
            return;
        }
        int randomIndex =
            Random.Range(0, villages.Length);
        currentVillage =
            villages[randomIndex];
        foreach (VillageDelivery village in villages)
        {
            if (village != null)
            {
                village.SetDeliveryActive(
                    village == currentVillage
                );
            }
        }
        UpdateDestinationUI();
        Debug.Log(
            "New delivery destination: " +
            currentVillage.villageName
        );
    }
    void UpdateDestinationUI()
    {
        if (destinationText == null ||
            currentVillage == null)
        {
            return;
        }
        destinationText.text =
            "Deliver to: " +
            currentVillage.villageName;
    }
    public bool IsCurrentVillage(
        VillageDelivery village
    )
    {
        return village == currentVillage;
    }
    public void DeliveryCompleted()
    {
        Debug.Log(
            "Delivery completed at " +
            currentVillage.villageName
        );
        ChooseRandomVillage();
    }
    public VillageDelivery GetCurrentVillage()
    {
        return currentVillage;
    }
}