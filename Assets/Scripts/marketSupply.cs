using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class MarketSupply : MonoBehaviour
{
    [Header("Interaction")]
    public TMP_Text collectPrompt;
    [Header("Supplies")]
    public int suppliesToGive = 1;
    private bool playerNearby = false;
    void Start()
    {
        if (collectPrompt != null)
        {
            collectPrompt.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (playerNearby &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            CollectSupplies();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (collectPrompt != null)
            {
                collectPrompt.gameObject.SetActive(true);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (collectPrompt != null)
            {
                collectPrompt.gameObject.SetActive(false);
            }
        }
    }
    void CollectSupplies()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError(
                "MarketSupply: InventoryManager not found!"
            );
            return;
        }
        bool added =
            InventoryManager.Instance.AddSupplies(
                suppliesToGive
            );
        if (!added)
        {
            Debug.Log("Inventory is full!");
            return;
        }
        Debug.Log(
            "Collected " +
            suppliesToGive +
            " supply!"
        );
        if (collectPrompt != null)
        {
            collectPrompt.gameObject.SetActive(false);
        }
        playerNearby = false;
    }
}