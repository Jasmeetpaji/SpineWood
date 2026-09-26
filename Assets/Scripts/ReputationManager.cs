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
            Instance = this;
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

        public List<ReputationEvent> reputationHistory = new List<ReputationEvent>();

        [Header("Milestones")]

        public List<ReputationMilestone> milestones = new List<ReputationMilestone>();
    }

    //Inspector settings

    [Header("Village Reputation Settings")]

    [SerializeField]
    private List<VillageReputation> villages = new List<VillageReputation>();


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
            CreateDefaultVillagesForGame();
        }

        foreach (VillageReputation village in villages)
        {
            SetupVillage(village);
        }
    }

    //Creation for default villages for,Adjsut later

    private void CreateDefaultVillagesForGame()
    {
        CreateVillage(
            "Spinewood",
            "spinewood",
            0,
            true
        );

        CreateVillage(
            "Cranktown",
            "cranktown",
            0,
            true
        );
    }

    // Create Villgae

    public void CreateVillage(
        string villageName,
        string villageID,
        int startingReputation,
        bool unlocked)
    {
        if (GetVillage(villageID) != null)
        {
            Debug.LogWarning(
                "Village already exists: " + villageID);
            
            return;
        }

        VillageReputation newVillage = 
        new VillageReputation();

        newVillage.villageName = villageName;

        newVillage.villageID  = villageID;

        newVillage.reputation = 
        Mathf.Clamp(
            startingReputation,
            minimumReputation,
            maximumReputation
        );

        newVillage.villageUnlocked = unlocked;

        CreateMilestonesForVillage(newVillage);

        villages.Add(newVillage);

        DebugMessage(
            "Created village: " + villageName
        );
    }

    // Village setup

    private void SetupVillage(
        VillageReputation village)
    {
        if (village.reputationHistory == null)
        {
            village.reputationHistory = new List<ReputationEvent>();
        }

        if (village.milestones == null)
        {
            village.milestones = new List<ReputationMilestone>();
        }

        if (village.milestones.Count == 0)
        {
            CreateMilestonesForVillage(village);
        }

        village.reputation = 
        Mathf.Clamp(
            village.reputation,
            minimumReputation,
            maximumReputation
        );

        UpdateUnlocks(village);

        CheckMilestones(village);
    }

    //Create milestones

    private void CreateMilestonesForVillage(
        VillageReputation village)
    {
        village.milestones = new List<ReputationMilestone>();

        village.milestones.Add(
            new ReputationMilestone(
                "First Impression",
                "Reach 10 reputation with this village.",
                10
            )
        );

        village.milestones.Add(
            new ReputationMilestone(
                "Familiar Face",
                "Reach 25 reputation with this village.",
                25
            )
        );

        village.milestones.Add(
            new ReputationMilestone(
                "Trusted Courier",
                "Reach 50 reputation with this village.",
                50
            )
        );

        village.milestones.Add(
            new ReputationMilestone(
                "Respected Courier",
                "Reach 75 reputation with this villlage.",
                75
            )
        );

        village.milestones.Add(
            new ReputationMilestone(
                "Legend of the Village",
                "Reach maximum reputation.",
                100
            )
        );
    }

    // Get Village

   public VillageReputation GetVillage(
    string villageID)
   {
    foreach (VillageReputation village in villages)
    {
        if (village.villageID == villageID)
        {
            return village;
        }
    }

    return null;

   }

   
   // Get villages by name

   public VillageReputation GetVillageByName(string villageName)
   {
        foreach (VillageReputation village in villages)
        {
            if (village.villageName == villageName)
            {
                return village;
            }
        }

        Debug.LogWarning(
            "Could not find village by name: " + villageName
        );

        return null;
   } 

   //Add reputation system

   public void AddReputation(
       string villageID,
       int amount,
       string reason)

    {
        if (amount <= 0)
        {
            return;
        }

        ChangeReputation(
            villageID,
            amount,
            reason
        );
    }

    // Remove reputation

    public void RemoveReputation(
        string villageID,
        int amount,
        string reason)
    {
        if (amount <= 0)
        {
            return;
        }

        ChangeReputation(
            villageID,
            -amount,
            reason
        );
    }

    // Change reputation

    public void ChangeReputation(
        string villageID,
        int amount,
        string reason)
    {
        VillageReputation village = GetVillage(villageID);

        if (village == null)
        {
            Debug.LogWarning(
                "Cannot change reputation. Village not found: " 
                + villageID
            );

            return;
        }

        int oldReputation = village.reputation;

        ReputationLevel oldLevel = GetReputationLevel(oldReputation);

        village.reputation += amount;

        village.reputation = 
        Mathf.Clamp(
            village.reputation,
            minimumReputation,
            maximumReputation
        );

        int actualChange = village.reputation - oldReputation;

        if (actualChange > 0)
        {
            village.lifetimeReputationEarned += actualChange;
        }
        
        else if (actualChange < 0)
        {
            village.lifetimeReputationLost += Mathf.Abs(actualChange);
        }

        ReputationEvent newEvent = 
        new ReputationEvent(
            "Reputation Change",
            actualChange,
            reason
        );

        village.reputationHistory.Add(
            newEvent
        );

        LimitHistory(village);

        ReputationLevel newLevel = 
        GetReputationLevel(
            village.reputation
        );

        UpdateUnlocks(village);

        CheckMilestones(village);

        if (oldLevel != newLevel)
        {
            HandleLevelChanged(
                village,
                oldLevel,
                newLevel
            );
        }

        if (onReputationChanged != null)
        {
            onReputationChanged.Invoke();
        }

        if (saveAutomatically)
        {
            SaveReputation();
        }

        DebugMessage(
            village.villageName
            + " reputation changed by "
            + actualChange
            + ". Reason: "
            + ". New reputation: "
            + village.reputation
        );
    }

    // Limit History 

    private void LimitHistory(
        VillageReputation village)
    {
        int maximumHistoryEntries = 50;

        while (
            village.reputationHistory.Count
            > maximumHistoryEntries)
        {
            village.reputationHistory.RemoveAt(0);
        }
    }

    // Delivery Result

    public void RegisterDelivery(
        string villageID,
        DeliveryResult result,
        int goldEarned)
    {
        VillageReputation village = 
        GetVillage(villageID);

        if (village == null)
        {
            Debug.LogWarning(
                "Cannot register delivery. Village not found."
            );

            return;
        }

        village.totalDeliveries++;

        village.totalGoldEarned +=
        goldEarned;

        switch (result)
        {
            case DeliveryResult.Perfect:

            RegisterPerfectDelivery(
                villageID,
                goldEarned
            );

            break;

            case DeliveryResult.Successful:

            RegisterSuccessfulDelivery(
                villageID,
                goldEarned
            );

            break;

            case DeliveryResult.Late:

            RegisterLateDelivery(
                villageID,
                goldEarned
            );

            break;

            case DeliveryResult.Damaged:

            RegisterDamagedDelivery(
                villageID,
                goldEarned
            );

            break;

            case DeliveryResult.Failed:

            RegisterFailedDelivery(
                villageID
            );

            break;

            case DeliveryResult.Abandoned:

            RegisterAbandonedDelivery(
                villageID
            );

            break;

        }

        UpdateUnlocks(village);

        CheckMilestones(village);

        if (saveAutomatically)
        {
            SaveReputation();
        }
    }

    //Perfect Delivery

    public void RegisterPerfectDelivery(
        string villageID,
        int goldEarned)
    {
        VillageReputation village = GetVillage(villageID);

        if (village == null)
        {
            return;
        }

        village.successfulDeliveries++;

        village.perfectDeliveries++;

        village.currentDeliveryStreak++;

        if (
            village.currentDeliveryStreak
            > village.bestDeliveryStreak)
        {
            village.bestDeliveryStreak = village.currentDeliveryStreak;
        }

        int reputationReward = 8;

        int streakBonus = 
        CalculateStreakReputationBonus(
            village.currentDeliveryStreak
        );

        int totalReward = 
        reputationReward 
        + streakBonus;

        AddReputation(
            villageID,
            totalReward,
            "Perfect delivery"
        );
    }

        private int CalculateReputationGoldBonus(
            string villageID,
            int goldEarned)
        {
            VillageReputation village = GetVillage(villageID);

            if (village == null)
            {
                return 0;
            }

            float multiplier = GetGoldMultiplier(village.reputation);

            int totalGold = Mathf.RoundToInt(
                goldEarned * multiplier
            );

            int bonusGold = totalGold - goldEarned;

            return bonusGold;
        
    }

    public void RegisterSuccessfulDelivery(
        string villageID,
        int goldEarned)
    {
        VillageReputation village = GetVillage(villageID);

        if (village == null)
        {
            return;
        }

        village.successfulDeliveries++;

        village.currentDeliveryStreak++;

        if (
            village.currentDeliveryStreak
            > village.bestDeliveryStreak)
        {
            village.bestDeliveryStreak = 
            village.currentDeliveryStreak;
        }

        int reputationReward = 5;

        int streakBonus = CalculateStreakReputationBonus(
            village.currentDeliveryStreak
        );

        int totalReward = 
        reputationReward
        + streakBonus;

        AddReputation(
            villageID,
            totalReward,
            "Successful delivery"
        );

        int bonusGold = 
        CalculateReputationGoldBonus(
            villageID,
            goldEarned
        );

        village.totalBonusGoldEarned +=
        bonusGold;

        DebugMessage(
            "Successful delivery! Reputation gained: "
            + totalReward
        );
    }

    public void RegisterLateDelivery(
        string villageID,
        int goldEarned)
    {
        VillageReputation village = GetVillage(villageID);

        if (village == null)
        {
            return;
        }

        village.lateDeliveries++;

        village.currentDeliveryStreak = 0;

        AddReputation(
            villageID,
            -2,
            "Delivery arrived late"
        );

        DebugMessage(
            "Late delivery. Reputation lost: 2"
        );
    }


    public void RegisterDamagedDelivery(
        string villageID,
        int goldEarned)
    {
        VillageReputation village = GetVillage(villageID);

        if (village == null)
        {
            return;
        }

        village.damagedDeliveries++;

        village.currentDeliveryStreak = 0;

        AddReputation(
            villageID,
            -5,
            "Goods arrived damaged"
        );

        DebugMessage(
            "Damaged delivery. Reputation lost: 5"
        );

    }


    public void RegisterFailedDelivery(
        string villageID)
    {
        VillageReputation village = GetVillage(villageID);

        if (village == null)
        {
            return;
        }

        village.failedDeliveries++;

        village.currentDeliveryStreak = 0;

        AddReputation(
            villageID,
            -10,
            "Delivery failed"
        );

        DebugMessage(
            "Faied delivery. Reputation lost: 10"
        );
    }


    public void RegisterAbandonedDelivery(
        string villageID)
    {
        VillageReputation village = GetVillage(villageID);

        if (village == null)
        {
            return;
        }

        village.abandonedDeliveries++;

        village.currentDeliveryStreak = 0;

        AddReputation(
            villageID,
            -7,
            "Delivery was abandoned"
        );

        DebugMessage(
            "Delivery abandoned. Reputation lost: 7"
        );
    }


    private int CalculateStreakReputationBonus(
        int streak)
    {
        if (streak < 3)
        {
            return 0;
        }

        if (streak < 5)
        {
            return 1;
        }

        if (streak < 10)
        {
            return 2;
        }

        if (streak < 20)
        {
            return 3;
        }

        return 5;
    }


    public int CalcuateReputationGoldBonus(
        string villageID,
        int baseGold)
    {
        VillageReputation village = GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        float multiplier = 
        GetGoldMultiplier(
            village.reputation
        );

        int bonus = 
        Mathf.RoundToInt(
            baseGold * (multiplier - 1f)
        );

        return Mathf.Max(
            0,
            bonus
        );
    }


    public float GetGoldMultiplier(
        int reputation)
    {
        if (reputation < 10)
        {
            return 1.00f;
        }

        if (reputation < 25)
        {
            return 1.02f;
        }

        if (reputation < 50)
        {
            return 1.05f;
        }

        if (reputation < 75)
        {
            return 1.10f;
        }

        if (reputation < 90)
        {
            return 1.15f;
        }

        return 1.25f;
    }

    public ReputationLevel GetReputationLevel(
        int reputation)
    {
        if (reputation < 10)
        {
            return ReputationLevel.Stranger;
        }

        if (reputation < 25)
        {
            return ReputationLevel.Newcomer;
        }

        if (reputation < 40)
        {
            return ReputationLevel.Familiar;
        }

        if (reputation < 55)
        {
            return ReputationLevel.Accepted;
        }

        if (reputation < 70)
        {
            return ReputationLevel.Trusted;
        }

        if (reputation < 80)
        {
            return ReputationLevel.Respected;
        }

        if (reputation < 90)
        {
            return ReputationLevel.Honored;
        }

        if (reputation < 100)
        {
            return ReputationLevel.Beloved;
        }

        return ReputationLevel.Legendary;
    }


    public ReputationLevel GetVillageReputationLevel(
        string villageID)
    {
        VillageReputation village = 
        GetVillage(villageID);

        if (village == null)
        {
            return ReputationLevel.Stranger;
        }

        return GetReputationLevel(
            village.reputation
        );
    }


    public string GetReputationLevelName(
        string villageID)
    {
        ReputationLevel level = 
        GetVillageReputationLevel(
            villageID
        );

        return GetLevelDisplayName(level);
    }


    public string GetLevelDisplayName(ReputationLevel level)
    {
        switch (level)
        {
            case ReputationLevel.Stranger:
            return "Stranger";

            case ReputationLevel.Newcomer:
            return "Newcomer";

            case ReputationLevel.Familiar:
            return "Familiar Face";

            case ReputationLevel.Accepted:
            return "Accepted Courier";

            case ReputationLevel.Trusted:
            return "Trusted Courier";

            case ReputationLevel.Respected:
            return "Resptected Courier";

            case ReputationLevel.Honored:
            return "Honored Courier";

            case ReputationLevel.Beloved:
            return "Beloved Courier";

            case ReputationLevel.Legendary:
            return "Legendary Courier";
        }

        return "Unknown";
    }


    public ReputationLevel GetNextLevel(
        int reputation)
    {
        ReputationLevel currentLevel = 
        GetReputationLevel(
            reputation
        );
        
        if (
            currentLevel
            == ReputationLevel.Legendary)
        {
            return ReputationLevel.Legendary;
        }

        return currentLevel + 1;
    }
    
    

    public int GetLevelRequirment(
        ReputationLevel level)
    {
        switch (level)
        {
            case ReputationLevel.Stranger:
            return 0;

            case ReputationLevel.Newcomer:
            return 10;

            case ReputationLevel.Familiar:
            return 25;

            case ReputationLevel.Accepted:
            return 40;

            case ReputationLevel.Trusted:
            return 55;

            case ReputationLevel.Respected:
            return 70;

            case ReputationLevel.Honored:
            return 80;

            case ReputationLevel.Beloved:
            return 90;

            case ReputationLevel.Legendary:
            return 100;
        }

        return 0;
    }


    public float GetProgressToNextLevel(
        string villageID)
    {
        VillageReputation village = GetVillage(villageID);

        if (village == null)
        {
            return 0f;
        }

        ReputationLevel currentLevel = 
        GetReputationLevel(
            village.reputation
        );

        if (
            currentLevel
            == ReputationLevel.Legendary)
        {
            return 1f;
        }

        int currentRequirment = 
        GetLevelRequirment(
            currentLevel
        );

        ReputationLevel nextLevel = 
        GetNextLevel(
            village.reputation
        );

        int nextRequirment = 
        GetLevelRequirment(
            nextLevel
        );

        int range = 
        nextRequirment
        - currentRequirment;

        if (range <= 0)
        {
            return 1f;
        }

        int progress = 
        village.reputation
        - GetLevelRequirment(currentLevel);

        return Mathf.Clamp01(
            (float)progress / range
        );
    }

    public int GetReputationNeededForNextLevel(
        string villageID)
    {
        VillageReputation village = 
        GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        ReputationLevel nextLevel = 
        GetNextLevel(
            village.reputation
        );

        int requirment = 
        GetLevelRequirment(
            nextLevel
        );

        return Mathf.Max(
            0,
            requirment - village.reputation
        );
    }


    private void HandleLevelChanged(
        VillageReputation village,
        ReputationLevel oldLevel,
        ReputationLevel newLevel)
    {
        DebugMessage(
            "REPUTATION LEVEL UP!"
        );

        DebugMessage(
            village.villageName
            + " changed from "
            + GetLevelDisplayName(oldLevel)
            + " to "
            + GetLevelDisplayName(newLevel)
        );

        UpdateUnlocks(village);

        if (onVillageLevelChanged != null)
        {
            onVillageLevelChanged.Invoke();
        }
    }



    private void UpdateUnlocks(
        VillageReputation village)
    {
        int reputation =
            village.reputation;



        if (reputation >= 25)
        {
            if (!village.specialContractsUnlocked)
            {
                village.specialContractsUnlocked = true;

                DebugMessage(
                    village.villageName
                    + " unlocked Special Contracts!"
                );
            }
        }



        if (reputation >= 50)
        {
            if (!village.merchantDiscountUnlocked)
            {
                village.merchantDiscountUnlocked = true;

                DebugMessage(
                    village.villageName
                    + " unlocked Merchant Discount!"
                );
            }
        }



        if (reputation >= 75)
        {
            if (!village.premiumContractsUnlocked)
            {
                village.premiumContractsUnlocked = true;

                DebugMessage(
                    village.villageName
                    + " unlocked Premium Contracts!"
                );
            }
        }
    }


    private void CheckMilestones(
        VillageReputation village)
    {
        if (village.milestones == null)
        {
            return;
        }

        foreach (
            ReputationMilestone milestone
            in village.milestones)
        {
            if (!milestone.unlocked
                &&
                village.reputation
                >= milestone.requiredReputation)
            {
                UnlockMilestone(
                    village,
                    milestone
                );
            }
        }
    }


    private void UnlockMilestone(
        VillageReputation village,
        ReputationMilestone milestone)
    {
        milestone.unlocked = true;

        DebugMessage(
            "MILESTONE UNLOCKED!"
        );

        DebugMessage(
            village.villageName
            + ": "
            + milestone.milestoneName
        );

        if (onMilestoneUnlocked != null)
        {
            onMilestoneUnlocked.Invoke();
        }
    }


    public bool IsVillageUnlocked(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return false;
        }

        return village.villageUnlocked;
    }


    public void UnlockVillage(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return;
        }

        village.villageUnlocked = true;

        DebugMessage(
            "Village unlocked: "
            + village.villageName
        );

        if (saveAutomatically)
        {
            SaveReputation();
        }
    }


    public void LockVillage(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return;
        }

        village.villageUnlocked = false;

        if (saveAutomatically)
        {
            SaveReputation();
        }
    }



    public bool HasSpecialContracts(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return false;
        }

        return village.specialContractsUnlocked;
    }


    public bool HasMerchantDiscount(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return false;
        }

        return village.merchantDiscountUnlocked;
    }


    public bool HasPremiumContracts(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return false;
        }

        return village.premiumContractsUnlocked;
    }



    public int GetCurrentDeliveryStreak(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.currentDeliveryStreak;
    }


    public int GetBestDeliveryStreak(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.bestDeliveryStreak;
    }


    public int GetTotalDeliveries(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.totalDeliveries;
    }


    public int GetSuccessfulDeliveries(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.successfulDeliveries;
    }



    public int GetPerfectDeliveries(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.perfectDeliveries;
    }



    public int GetFailedDeliveries(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.failedDeliveries;
    }



    public int GetLateDeliveries(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.lateDeliveries;
    }



    public int GetDamagedDeliveries(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.damagedDeliveries;
    }


    public int GetTotalGoldEarned(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.totalGoldEarned;
    }



    public int GetTotalBonusGoldEarned(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.totalBonusGoldEarned;
    }


    public int GetLifetimeReputationEarned(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.lifetimeReputationEarned;
    }


    public int GetLifetimeReputationLost(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.lifetimeReputationLost;
    }


    public float GetReputationPercentage(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0f;
        }

        if (maximumReputation <= 0)
        {
            return 0f;
        }

        return (float)village.reputation
            / maximumReputation;
    }



    public bool IsReputationMaxed(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return false;
        }

        return village.reputation
            >= maximumReputation;
    }


    public bool HasLowReputation(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return false;
        }

        return village.reputation < 25;
    }


    public bool HasHighReputation(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return false;
        }

        return village.reputation >= 75;
    }


    public void ApplyReputationDecay(
        string villageID,
        int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        RemoveReputation(
            villageID,
            amount,
            "Reputation decay"
        );
    }


    public void RewardSpecialEvent(
        string villageID,
        int reputationReward,
        string eventDescription)
    {
        if (reputationReward <= 0)
        {
            return;
        }

        AddReputation(
            villageID,
            reputationReward,
            eventDescription
        );

        DebugMessage(
            "Special event completed!"
        );

        DebugMessage(
            "Reputation reward: "
            + reputationReward
        );
    }


    public void PenalizeSpecialEvent(
        string villageID,
        int reputationPenalty,
        string eventDescription)
    {
        if (reputationPenalty <= 0)
        {
            return;
        }

        RemoveReputation(
            villageID,
            reputationPenalty,
            eventDescription
        );

        DebugMessage(
            "Special event failed!"
        );

        DebugMessage(
            "Reputation penalty: "
            + reputationPenalty
        );
    }



    public int GetUnlockedMilestoneCount(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        int count = 0;

        foreach (
            ReputationMilestone milestone
            in village.milestones)
        {
            if (milestone.unlocked)
            {
                count++;
            }
        }

        return count;
    }



    public int GetTotalMilestoneCount(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return 0;
        }

        return village.milestones.Count;
    }



    [Serializable]
    public class ReputationSaveData
    {
        public List<VillageReputation> villages =
            new List<VillageReputation>();
    }



    public void SaveReputation()
    {
        ReputationSaveData saveData =
            new ReputationSaveData();

        saveData.villages = villages;

        string json =
            JsonUtility.ToJson(
                saveData
            );

        PlayerPrefs.SetString(
            "SPINEWOOD_REPUTATION_DATA",
            json
        );

        PlayerPrefs.Save();

        DebugMessage(
            "Reputation data saved."
        );
    }


    public void LoadReputation()
    {
        if (
            !PlayerPrefs.HasKey(
                "SPINEWOOD_REPUTATION_DATA"
            )
        )
        {
            DebugMessage(
                "No reputation save found. Using defaults."
            );

            return;
        }

        string json =
            PlayerPrefs.GetString(
                "SPINEWOOD_REPUTATION_DATA"
            );

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning(
                "Reputation save data was empty."
            );

            return;
        }

        ReputationSaveData saveData =
            JsonUtility.FromJson<ReputationSaveData>(
                json
            );

        if (saveData == null)
        {
            Debug.LogWarning(
                "Could not load reputation save."
            );

            return;
        }

        villages =
            saveData.villages;

        if (villages == null)
        {
            villages =
                new List<VillageReputation>();
        }

        foreach (
            VillageReputation village
            in villages)
        {
            SetupVillage(village);
        }

        DebugMessage(
            "Reputation data loaded."
        );
    }


    public void DeleteReputationSave()
    {
        if (
            PlayerPrefs.HasKey(
                "SPINEWOOD_REPUTATION_DATA"
            )
        )
        {
            PlayerPrefs.DeleteKey(
                "SPINEWOOD_REPUTATION_DATA"
            );

            PlayerPrefs.Save();
        }

        DebugMessage(
            "Reputation save deleted."
        );
    }



    public void ResetVillageReputation(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return;
        }

        village.reputation = 0;

        village.lifetimeReputationEarned = 0;

        village.lifetimeReputationLost = 0;

        village.totalDeliveries = 0;

        village.successfulDeliveries = 0;

        village.perfectDeliveries = 0;

        village.lateDeliveries = 0;

        village.damagedDeliveries = 0;

        village.failedDeliveries = 0;

        village.abandonedDeliveries = 0;

        village.currentDeliveryStreak = 0;

        village.bestDeliveryStreak = 0;

        village.totalGoldEarned = 0;

        village.totalBonusGoldEarned = 0;

        village.specialContractsUnlocked = false;

        village.merchantDiscountUnlocked = false;

        village.premiumContractsUnlocked = false;

        village.reputationHistory.Clear();

        foreach (
            ReputationMilestone milestone
            in village.milestones)
        {
            milestone.unlocked = false;
        }

        if (saveAutomatically)
        {
            SaveReputation();
        }

        DebugMessage(
            "Reset reputation for "
            + village.villageName
        );
    }



    public void ResetAllReputation()
    {
        foreach (
            VillageReputation village
            in villages)
        {
            village.reputation = 0;

            village.lifetimeReputationEarned = 0;

            village.lifetimeReputationLost = 0;

            village.totalDeliveries = 0;

            village.successfulDeliveries = 0;

            village.perfectDeliveries = 0;

            village.lateDeliveries = 0;

            village.damagedDeliveries = 0;

            village.failedDeliveries = 0;

            village.abandonedDeliveries = 0;

            village.currentDeliveryStreak = 0;

            village.bestDeliveryStreak = 0;

            village.totalGoldEarned = 0;

            village.totalBonusGoldEarned = 0;

            village.reputationHistory.Clear();

            foreach (
                ReputationMilestone milestone
                in village.milestones)
            {
                milestone.unlocked = false;
            }

            UpdateUnlocks(village);
        }

        if (saveAutomatically)
        {
            SaveReputation();
        }

        DebugMessage(
            "ALL reputation has been reset."
        );
    }


    public int GetVillageCount()
    {
        return villages.Count;
    }


    public string GetVillageName(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return "Unknown Village";
        }

        return village.villageName;
    }


    public string GetReputationSummary(
        string villageID)
    {
        VillageReputation village =
            GetVillage(villageID);

        if (village == null)
        {
            return "Village not found.";
        }

        ReputationLevel level =
            GetReputationLevel(
                village.reputation
            );

        string summary =
            "Village: "
            + village.villageName
            + "\n";

        summary +=
            "Reputation: "
            + village.reputation
            + "/"
            + maximumReputation
            + "\n";

        summary +=
            "Status: "
            + GetLevelDisplayName(level)
            + "\n";

        summary +=
            "Deliveries: "
            + village.totalDeliveries
            + "\n";

        summary +=
            "Successful: "
            + village.successfulDeliveries
            + "\n";

        summary +=
            "Perfect: "
            + village.perfectDeliveries
            + "\n";

        summary +=
            "Late: "
            + village.lateDeliveries
            + "\n";

        summary +=
            "Damaged: "
            + village.damagedDeliveries
            + "\n";

        summary +=
            "Failed: "
            + village.failedDeliveries
            + "\n";

        summary +=
            "Current Streak: "
            + village.currentDeliveryStreak
            + "\n";

        summary +=
            "Best Streak: "
            + village.bestDeliveryStreak
            + "\n";

        summary +=
            "Gold Earned: "
            + village.totalGoldEarned;

        return summary;
    }


    public void PrintReputationSummary(
        string villageID)
    {
        Debug.Log(
            GetReputationSummary(
                villageID
            )
        );
    }


    [ContextMenu("Debug: Add 10 Reputation to Spinewood")]
    private void DebugAddSpinewoodReputation()
    {
        AddReputation(
            "spinewood",
            10,
            "Debug test"
        );
    }
        

    [ContextMenu("Debug: Remove 10 Reputation from Spinewood")]
    private void DebugRemoveSpinewoodReputation()
    {
        RemoveReputation(
            "spinewood",
            10,
            "Debug test"
        );
    }

    [ContextMenu("Debug: Perfect Spinewood Delivery")]
    private void DebugPerfectDelivery()
    {
        RegisterDelivery(
            "spinewood",
            DeliveryResult.Perfect,
            100
        );
    }


    [ContextMenu("Debug: Failed Spinewood Delivery")]
    private void DebugFailedDelivery()
    {
        RegisterDelivery(
            "spinewood",
            DeliveryResult.Failed,
            0
        );
    }


    [ContextMenu("Debug: Print All Villages")]
    private void DebugPrintAllVillages()
    {
        foreach (
            VillageReputation village
            in villages)
        {
            Debug.Log(
                "--------------------------------"
            );

            Debug.Log(
                "Village: "
                + village.villageName
            );

            Debug.Log(
                "Reputation: "
                + village.reputation
            );

            Debug.Log(
                "Level: "
                + GetLevelDisplayName(
                    GetReputationLevel(
                        village.reputation
                    )
                )
            );

            Debug.Log(
                "Deliveries: "
                + village.totalDeliveries
            );

            Debug.Log(
                "Successful: "
                + village.successfulDeliveries
            );

            Debug.Log(
                "Failed: "
                + village.failedDeliveries
            );

            Debug.Log(
                "Streak: "
                + village.currentDeliveryStreak
            );

            Debug.Log(
                "Best Streak: "
                + village.bestDeliveryStreak
            );
        }
    }


    private void DebugMessage(
        string message)
    {
        if (!enableDebugMessages)
        {
            return;
        }

        Debug.Log(
            "[ReputationManager] "
            + message
        );
    }


    private void OnApplicationQuit()
    {
        if (saveAutomatically)
        {
            SaveReputation();
        }
    }


    private void OnApplicationPause(
        bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (saveAutomatically)
            {
                SaveReputation();
            }
        }
    }
}

