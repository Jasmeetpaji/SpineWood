using UnityEngine;
using TMPro;
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    [Header("Inventory")]
    public int supplies = 0;
    public int maxSupplies = 5;
    [Header("UI")]
    public TMP_Text suppliesText;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        UpdateUI();
    }
    public bool AddSupplies(int amount)
    {
        if (supplies >= maxSupplies)
        {
            return false;
        }
        supplies += amount;
        if (supplies > maxSupplies)
        {
            supplies = maxSupplies;
        }
        UpdateUI();
        return true;
    }
    public bool RemoveSupplies(int amount)
    {
        if (supplies < amount)
        {
            return false;
        }
        supplies -= amount;
        UpdateUI();
        return true;
    }
    void UpdateUI()
    {
        if (suppliesText != null)
        {
            suppliesText.text =
                "Supplies: " +
                supplies +
                " / " +
                maxSupplies;
        }
    }
}