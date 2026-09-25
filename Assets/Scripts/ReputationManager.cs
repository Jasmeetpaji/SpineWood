using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReputationManager : MonoBehaviour
{
    // Reputation Manager

    public static ReputationManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance == this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSystem();
        LoadReputation();
    }

    //Enums

    public enum ReputationLevel
    {
        Stranger,
        Newcomer,
        Familiar,
        Accepted,
        Trusted,
        Respected,
        Honored,
        Beloved,
        Legendary
    }

    public enum DeliveryResult
    {
        Perfect,
        Successful,
        Late,
        Damaged,
        Failed,
        Abandoned
    }

    //Reputation event

    [Serializable]
    public class ReputationEvent
    {
        public string eventName;

        public int reputationChange;

        public string description;

        public string date;

        public ReputationEvent()
        {
        }

        public ReputationEvent(
            string newEventName,
            int newReputationChange,
            string newDescription)
        {
            eventName = newEventName;
            reputationChange = newReputationChange;
            description = newDescription;

            date = DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss");
        }
    }

    // Reputation milestone

    [Serializable]
    public class ReputationMilestone
    {
        public string milestoneName;

        public string description;

        public int requiredReputation;

        public bool unlocked;

        public ReputationMilestone(
            string newName,
            string newDescription,
            int requiredValue)
        {
            milestoneName = newName;

            description = newDescription;

            requiredReputation = requiredValue;

            unlocked = false;
        }
    }

    //Villgae reputation data

    [Serializable]
    public class VillageReputation
    {
        [Header("Village Information")]

        public string villageName;
        
        public string villageID;

        [Header("Reputation")]

        [Range(0, 100)]
        public int reputation;

        public int lifetimeReputationEarned;

        public int lifetimeReputationLost;

        [Header("Delivery Statistics")]

        public int totalDeliveries;

        public int successfulDeliveries;

        public int perfectDeliveries;

        public int lateDeliveries;

        public int damagedDeliveries;

        public int failedDeliveries;

        public int abandonedDeliveries;

        [Header("Delivery Streak")]

        public int currentDeliveryStreak;

        public int bestDeliveryStreak;

        [Header("Money Statistics")]

        public int totalGoldEarned;
        public int totalBonusGoldEarned;

        [Header("Village Status")]

        public bool villageUnlocked;

        public bool specialContractsUnlocked;

        public bool merchantDiscountUnlocked;

        public bool premiumContractsUnlocked;

        [Header("Reputation History")]

        public List<ReputationEvent> reputationHistory =
        new List<ReputationEvent>();

        [Header("Milestones")]

        public List<ReputationMilestone> milestones =
        new List<ReputationMilestone>();
    }

    //Inspector settings

    [Header("Village Reputation Settings")]

    [SerializeField]
    private List<VillageReputation> villages =
    new List<VillageReputation>();


    [Header("General Reputation Settings")]
    
    [SerializeField]
    private int minimumReputation = 0;

    [SerializeField]
    private int maximumReputation = 100;

    [Header("Save Settings")]

    [SerializeField]
    private bool saveAutomatically = true;

    [Header("Debug Settings")]

    [SerializeField]
    private bool enableDebugMessages = true;

    //Reputation Events

    [Header("Unity Events")]

    public UnityEvent onReputationChanged;

    public UnityEvent onVillageLevelChanged;

    public UnityEvent onMilestoneUnlocked;

    //Initialization

    private void InitializeSystem()
    {
        if (villages == null)
        {
            villages = new List<VillageReputation>();
        }

        if (villages.Count == 0)
        {
            CreateDefualtVillages();
        }

        foreach (VillageReputation village in villages)
        {
            SetupVillage(village);
        }
    }
    
}

