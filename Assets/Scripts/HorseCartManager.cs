using System;
using System.Collections.Generic;
using UnityEngine;

public class HorseCartManager : MonoBehaviour
{
    public static HorseCartManager Instance;

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
    }


    [Serializable]
    public class HorseData
    {
        public string horseName;

        public int horseLevel;

        public int experience;

        public int experienceToNextLevel;

        public float health;

        public float maxHealth;

        public float stamina;

        public float maxStamina;

        public float happiness;

        public float maxHappiness;

        public float baseSpeed;

        public float sprintSpeed;

        public float staminaDrainRate;

        public float staminaRecoveryRate;

        public float hunger;

        public float maxHunger;

        public float fatigue;

        public float maxFatigue;

        public int purchasePrice;

        public int totalDeliveries;

        public int successfulDeliveries;

        public int failedDeliveries;

        public int perfectDeliveries;

        public int totalDistanceTravelled;

        public bool isOwned;

        public bool isTired;

        public bool isHungry;

        public bool isInjured;

        public bool isSprinting;

        public bool isAvailable;


        public HorseData()
        {
            horseName = "Unnamed Horse";

            horseLevel = 1;

            experience = 0;

            experienceToNextLevel = 100;

            health = 100f;

            maxHealth = 100f;

            stamina = 100f;

            maxStamina = 100f;

            happiness = 100f;

            maxHappiness = 100f;

            baseSpeed = 5f;

            sprintSpeed = 8f;

            staminaDrainRate = 10f;

            staminaRecoveryRate = 6f;

            hunger = 0f;

            maxHunger = 100f;

            fatigue = 0f;

            maxFatigue = 100;

            purchasePrice = 500;

            totalDeliveries = 0;

            successfulDeliveries = 0;

            failedDeliveries = 0;

            perfectDeliveries = 0;

            totalDistanceTravelled = 0;

            isOwned = false;

            isTired = false;

            isHungry = false;

            isInjured = false;

            isSprinting = false;

            isAvailable = true;
        }
    }

    [Serializable]
    public class CartData
    {
        public string cartName;

        public int cartLevel;

        public float maxCapacity;

        public float currentWeight;

        public float durability;

        public float maxDurability;

        public float baseSpeedPenalty;

        public int purchasePrice;

        public int totalDeliveries;

        public bool isOwned;

        public CartData()
        {
            cartName = "Wooden Delivery Cart";
            cartLevel = 1;
            maxCapacity = 100f;
            currentWeight = 0f;
            durability = 100f;
            maxDurability = 100f;
            baseSpeedPenalty = 0f;
            purchasePrice = 300;
            totalDeliveries = 0;
            isOwned = false;
        }
    }

    public List<HorseData> horses = new List<HorseData>();

    public List<CartData> carts = new List<CartData>();

    public int currentHorseIndex = -1;

    public int currentCartIndex = -1;

    public int playerGold = 1000;

    public float minimumDeliveryWeight = 1f;

    public float maximumDeliveryWeight = 1000f;

    public float cargoWeightPerItem = 1f;

    public int baseExperiencePerDelivery = 25;

    public int perfectDeliveryExperienceBonus = 20;

    public int failedDeliveryExperience = 5;
    
    public int maximumHorseLevel = 20;
    public float levelHealthIncrease = 5f;
    public float levelStaminaIncrease = 5f;
    public float levelSpeedIncrease = 0.15f;

    public float normalStaminaRecovery = 6f;
    public float sprintStaminaDrain = 10f;
    public float tirednessThreshold = 20f;

    public float hungerIncreasePerMinute = 2f;
    public float hungryThreshold = 70f;
    public float happinessLossWhenHungary = 2f;

    public float fatigueIncreasePerMinute = 1f;
    public float fatigueThreshold = 80f;

    public bool debugMessages = true;


    private void InitializeSystem()
    {
        if (horses == null)
        {
            horses = new List<HorseData>();
        }
        if (carts == null)
        {
            carts = new List<CartData>();
        }
        if (horses.Count == 0)
        {
            CreateStarterHorse();
        }
        if (carts.Count == 0)
        {
            CreateStarterCart();
        }
        if (currentHorseIndex < 0 && horses.Count > 0)
        {
            currentHorseIndex = 0;
        }

        if (currentCartIndex < 0 && carts.Count > 0)
        {
            currentCartIndex = 0;
        }


        DebugMessage(
            "Horse and Cart System Initializef."
        );
    }

    
    public void CreateStarterHorse()
    {
        HorseData starterHorse = new HorseData();

        starterHorse.horseName = "Jimmy Long";

        starterHorse.isOwned = true;

        starterHorse.purchasePrice = 0;

        horses.Add(starterHorse);

        currentHorseIndex = horses.Count - 1;

        DebugMessage(
            "Starter horse created: "
            + starterHorse.horseName
        );
    }

    public void CreateStarterCart()
    {
        CartData starterCart = new CartData();

        starterCart.cartName = "Cart of hopes and dreams";

        starterCart.isOwned = true;
        
        starterCart.purchasePrice = 0;

        carts.Add(starterCart);

        currentCartIndex = carts.Count - 1;

        DebugMessage(
            "Starter cart created."
        );
    }


    public HorseData GetCurrentHorse()
    {
        if (currentHorseIndex < 0)
        {
            return null;
        }
        if (currentHorseIndex >= horses.Count)
        {
            return null;
        }

        return horses[currentHorseIndex];
    }


    public CartData GetCurrentCart()
    {
        if (currentCartIndex < 0)
        {
            return null;
        }

        if (currentCartIndex >= carts.Count)
        {
            return null;
        }

        return carts[currentCartIndex];
    }

    public void RenameCurrentHorse(
        string newName)
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            return;
        }

        horse.horseName = newName.Trim();

        DebugMessage(
            "Horse renamed to "
            + horse.horseName
        );
    }


    public bool BuyHorse(
        string horseName,
        int price)
    {
        if (price < 0)
        {
            return false;
        }

        if (playerGold < price)
        {
            DebugMessage(
                "Not enough gold to buy horse."
            );
            return false;
        }

        HorseData newHorse = new HorseData();

        newHorse.horseName = horseName;

        newHorse.purchasePrice = price;

        newHorse.isOwned = true; 

        playerGold -= price;

        horses.Add(newHorse);

        currentHorseIndex = horses.Count - 1;

        DebugMessage(
            "Purchased horse: "
            + horseName
        );

        return true;
    }

    public bool SellCurrentHorse()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return false;
        }

        if (horses.Count <= 1)
        {
            DebugMessage(
                "You cannot sell your only horse."
            );

            return false;
        }

        int salePrice = Mathf.RoundToInt(
            horse.purchasePrice * 0.5f
        );

        playerGold += salePrice;

        horses.RemoveAt(
            currentHorseIndex
        );

        currentHorseIndex = 0;

        DebugMessage(
            "Horse sold for"
            + salePrice
            + " gold."
        );

        return true;
    }


    public bool FeedHorse(
        float hungerReduction,
        int foodCost)
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return false;
        }
        if (playerGold < foodCost)
        {
            return false;
        }

        playerGold -= foodCost;

        horse.hunger -= hungerReduction;

        horse.hunger = Mathf.Clamp(
            horse.hunger,
            0f,
            horse.maxHunger
        );

        horse.happiness += 5f;

        horse.happiness = Mathf.Clamp(
            horse.happiness,
            0f,
            horse.maxHappiness
        );

        UpdateHorseConditions();

        DebugMessage(
            "Horse fed."
        );

        return true;
    }

    public void RestHorse(
        float restAmount)
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        horse.stamina += restAmount;

        horse.stamina = Mathf.Clamp(
            horse.stamina,
            0f,
            horse.maxStamina
        );

        horse.fatigue -= restAmount;

        horse.fatigue = Mathf.Clamp(
            horse.fatigue,
            0f,
            horse.maxFatigue
        );

        horse.health += restAmount * 0.25f;

        horse.health = Mathf.Clamp(
            horse.health,
            0f,
            horse.maxHealth
        );

        UpdateHorseConditions();

        DebugMessage(
            "Horse rested."
        );
    }

    public bool StartSprint()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return false;
        }

        if (horse.isTired)
        {
            DebugMessage(
                "Horse is too tired to sprint."
            );
            return false;
        }

        if (horse.stamina <= 0f)
        {
            return false;
        }

        horse.isSprinting = true;

        return true;
    }

    
    public void StopSprint()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        horse.isSprinting = false;
    }


    public float GetCurrentHorseSpeed()
    {
        HorseData horse = GetCurrentHorse();

        CartData cart = GetCurrentCart();

        if (horse == null)
        {
            return 0f;
        }

        float speed;

        if (horse.isSprinting)
        {
            speed = horse.sprintSpeed;
        }
        else
        {
            speed = horse.baseSpeed;
        }

        if (cart != null)
        {
            float weightRatio = 
            cart.currentWeight / cart.maxCapacity;

            float weightPenalty = weightRatio * 2f;

            speed -= weightPenalty;
        }

        if (horse.isTired)
        {
            speed *= 0.7f;
        }

        if (horse.isHungry)
        {
            speed *= 0.9f;
        }

        if (horse.isInjured)
        {
            speed *= 0.6f;
        }

        if (speed < 0.5f)
        {
            speed = 0.5f;
        }

        return speed;
    }

    private void Update()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        UpdateHorseStamina();

        UpdateHorseHunger();

        UpdateHorseFatigue();

        UpdateHorseConditions();
    }

    private void UpdateHorseStamina()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        if (horse.isSprinting)
        {
            horse.stamina -= sprintStaminaDrain 
            * Time.deltaTime;
        }
        else
        {
            horse.stamina += normalStaminaRecovery * Time.deltaTime;
        }

        horse.stamina = Mathf.Clamp(
            horse.stamina,
            0f,
            horse.maxStamina
        );

        if (horse.stamina <= 0f)
        {
            horse.isSprinting = false;
        }
    }

    private void UpdateHorseHunger()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        horse.hunger += hungerIncreasePerMinute 
        * Time.deltaTime
        / 60f;

        horse.hunger = Mathf.Clamp(
            horse.hunger,
            0f,
            horse.maxHunger
        );
    }

    private void UpdateHorseFatigue()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        if (horse.isSprinting)
        {
            horse.fatigue += 
            fatigueIncreasePerMinute 
            * Time.deltaTime;
        }
        else
        {
            horse.fatigue -= 
            fatigueIncreasePerMinute
            * 0.25f
            * Time.deltaTime;
        }

        horse.fatigue = Mathf.Clamp(
            horse.fatigue,
            0f,
            horse.maxFatigue
        );
    }

    private void UpdateHorseConditions()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        horse.isTired = horse.stamina <= tirednessThreshold;

        horse.isHungry = horse.hunger >= hungryThreshold;

        horse.isInjured = horse.health < horse.maxHealth * 0.5f;

        if (horse.fatigue >= fatigueThreshold)
        {
            horse.isTired = true;
        }
    }

    public void AddHorseExperience(
        int amount)
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        horse.experience += amount;

        CheckForHorseLevelUp();
    }


    private void CheckForHorseLevelUp()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        while (
            horse.experience >= 
            horse.experienceToNextLevel
            &&
            horse.horseLevel <
            maximumHorseLevel)
        {
            horse.experience -= horse.experienceToNextLevel;

            horse.horseLevel++;

            horse.experienceToNextLevel = 
            CalculateNextExperienceRequirement(
                horse.horseLevel
            );

            LevelUpHorse();
        }
    }

    private int CalculateNextExperienceRequirement(
        int level)
    {
        return 100 + ((level - 1) * 50);
    }

    
    private void LevelUpHorse()
    {
        HorseData horse = GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        horse.maxHealth += levelHealthIncrease;

        horse.health = horse.maxHealth;

        horse.maxStamina += levelStaminaIncrease;

        horse.stamina = horse.maxStamina;

        horse.baseSpeed += levelSpeedIncrease;

        horse.sprintSpeed += levelSpeedIncrease;

        horse.happiness = horse.maxHappiness;

        DebugMessage(
            "Horse leveled up to level "
            + horse.horseLevel
        );
    }


    public void RegisterDelivery(
        bool successful,
        bool perfect,
        float distanceTravelled)
    {
        HorseData horse = GetCurrentHorse();

        CartData cart = GetCurrentCart();

        if (horse == null)
        {
            return;
        }

        horse.totalDeliveries++;

        if (successful)
        {
            horse.successfulDeliveries++;

            AddHorseExperience(
                baseExperiencePerDelivery
            );
        }
        else
        {
            horse.failedDeliveries++;

            AddHorseExperience(
                failedDeliveryExperience
            );
        }

        if (perfect)
        {
            horse.perfectDeliveries++;

            AddHorseExperience(
                perfectDeliveryExperienceBonus
            );
        }

        horse.totalDistanceTravelled += 
        Mathf.RoundToInt(
            distanceTravelled
        );

        if (cart != null)
        {
            cart.totalDeliveries++;

            DamageCartAfterDelivery(
                distanceTravelled
            );
        }

        DebugMessage(
            "Horse delivery registered"
        );
    }


    private void DamageCartAfterDelivery(
        float distance)
    {
        CartData cart = GetCurrentCart();

        if (cart == null)
        {
            return;
        }

        float damage = distance * 0.02f;

        cart.durability -= damage;

        cart.durability = Mathf.Clamp(
            cart.durability,
            0f,
            cart.maxDurability
        );
    }


    public bool RepairCart(
        int repairAmount,
        int goldCost)
    {
        CartData cart = GetCurrentCart();

        if (cart == null)
        {
            return false;
        }

        if (playerGold < goldCost)
        {
            return false;
        }

        playerGold -= goldCost;

        cart.durability += repairAmount;

        cart.durability = Mathf.Clamp(
            cart.durability,
            0f,
            cart.maxDurability
        );

        DebugMessage(
            "Cart repaired."
        );

        return true;
    }

    public bool AddCargo(
        float weight)
    {
        CartData cart =
            GetCurrentCart();

        if (cart == null)
        {
            return false;
        }

        if (weight <= 0f)
        {
            return false;
        }

        if (
            cart.currentWeight +
            weight >
            cart.maxCapacity)
        {
            DebugMessage(
                "Cart cannot hold that much cargo."
            );

            return false;
        }

        cart.currentWeight +=
            weight;

        return true;
    }



    public bool RemoveCargo(
        float weight)
    {
        CartData cart =
            GetCurrentCart();

        if (cart == null)
        {
            return false;
        }

        if (weight <= 0f)
        {
            return false;
        }

        cart.currentWeight -=
            weight;

        cart.currentWeight =
            Mathf.Clamp(
                cart.currentWeight,
                0f,
                cart.maxCapacity
            );

        return true;
    }


    public void EmptyCart()
    {
        CartData cart =
            GetCurrentCart();

        if (cart == null)
        {
            return;
        }

        cart.currentWeight = 0f;

        DebugMessage(
            "Cart emptied."
        );
    }


    public float GetCargoPercentage()
    {
        CartData cart =
            GetCurrentCart();

        if (cart == null)
        {
            return 0f;
        }

        if (cart.maxCapacity <= 0f)
        {
            return 0f;
        }

        return
            cart.currentWeight /
            cart.maxCapacity;
    }


    public bool UpgradeCart(
        int goldCost,
        float capacityIncrease)
    {
        CartData cart =
            GetCurrentCart();

        if (cart == null)
        {
            return false;
        }

        if (playerGold < goldCost)
        {
            DebugMessage(
                "Not enough gold for cart upgrade."
            );

            return false;
        }

        playerGold -=
            goldCost;

        cart.cartLevel++;

        cart.maxCapacity +=
            capacityIncrease;

        cart.maxDurability +=
            10f;

        cart.durability =
            cart.maxDurability;

        DebugMessage(
            "Cart upgraded to level "
            + cart.cartLevel
        );

        return true;
    }


    public bool HealHorse(
        float healthAmount,
        int goldCost)
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            return false;
        }

        if (playerGold < goldCost)
        {
            return false;
        }

        if (horse.health >=
            horse.maxHealth)
        {
            return false;
        }

        playerGold -=
            goldCost;

        horse.health +=
            healthAmount;

        horse.health =
            Mathf.Clamp(
                horse.health,
                0f,
                horse.maxHealth
            );

        UpdateHorseConditions();

        DebugMessage(
            "Horse healed."
        );

        return true;
    }



    public void ChangeHorseHappiness(
        float amount)
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            return;
        }

        horse.happiness +=
            amount;

        horse.happiness =
            Mathf.Clamp(
                horse.happiness,
                0f,
                horse.maxHappiness
            );
    }



    public string GetHorseCondition()
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            return "No Horse";
        }

        if (horse.isInjured)
        {
            return "Injured";
        }

        if (horse.isTired)
        {
            return "Tired";
        }

        if (horse.isHungry)
        {
            return "Hungry";
        }

        if (horse.happiness >= 80f)
        {
            return "Happy";
        }

        if (horse.happiness >= 50f)
        {
            return "Content";
        }

        return "Unhappy";
    }


    public int GetHorseLevel()
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            return 0;
        }

        return horse.horseLevel;
    }


    public int GetHorseExperience()
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            return 0;
        }

        return horse.experience;
    }


    public float GetHorseExperienceProgress()
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            return 0f;
        }

        if (horse.experienceToNextLevel <= 0)
        {
            return 0f;
        }

        return
            (float)horse.experience /
            horse.experienceToNextLevel;
    }



    public float GetHorseStaminaPercentage()
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            return 0f;
        }

        return
            horse.stamina /
            horse.maxStamina;
    }


    public float GetHorseHealthPercentage()
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            return 0f;
        }

        return
            horse.health /
            horse.maxHealth;
    }


    public float GetHorseHappinessPercentage()
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            return 0f;
        }

        return
            horse.happiness /
            horse.maxHappiness;
    }


    public float GetCartDurabilityPercentage()
    {
        CartData cart =
            GetCurrentCart();

        if (cart == null)
        {
            return 0f;
        }

        return
            cart.durability /
            cart.maxDurability;
    }



    public float GetCurrentCargoWeight()
    {
        CartData cart =
            GetCurrentCart();

        if (cart == null)
        {
            return 0f;
        }

        return cart.currentWeight;
    }


    public float GetMaximumCargoWeight()
    {
        CartData cart =
            GetCurrentCart();

        if (cart == null)
        {
            return 0f;
        }

        return cart.maxCapacity;
    }


    public int GetPlayerGold()
    {
        return playerGold;
    }


    public void AddGold(
        int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        playerGold +=
            amount;
    }


    public bool SpendGold(
        int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (playerGold < amount)
        {
            return false;
        }

        playerGold -=
            amount;

        return true;
    }


    [ContextMenu(
        "Debug: Add 100 Horse XP")]
    private void DebugAddHorseXP()
    {
        AddHorseExperience(100);

        Debug.Log(
            "Added 100 horse XP."
        );
    }



    [ContextMenu(
        "Debug: Feed Horse")]
    private void DebugFeedHorse()
    {
        FeedHorse(
            25f,
            10
        );

        Debug.Log(
            "Fed current horse."
        );
    }

    [ContextMenu(
        "Debug: Rest Horse")]
    private void DebugRestHorse()
    {
        RestHorse(50f);

        Debug.Log(
            "Rested current horse."
        );
    }


    [ContextMenu(
        "Debug: Add 25 Cargo")]
    private void DebugAddCargo()
    {
        AddCargo(25f);

        Debug.Log(
            "Added 25 cargo weight."
        );
    }


    [ContextMenu(
        "Debug: Empty Cart")]
    private void DebugEmptyCart()
    {
        EmptyCart();

        Debug.Log(
            "Cart emptied."
        );
    }


    [ContextMenu(
        "Debug: Start Sprint")]
    private void DebugStartSprint()
    {
        StartSprint();

        Debug.Log(
            "Horse sprint started."
        );
    }


    [ContextMenu(
        "Debug: Stop Sprint")]
    private void DebugStopSprint()
    {
        StopSprint();

        Debug.Log(
            "Horse sprint has stopped."
        );
    }


    [ContextMenu(
        "Debug: Perfect Delivery")]
    private void DebugPerfectDelivery()
    {
        RegisterDelivery(
            true,
            true,
            100f
        );

        Debug.Log(
            "Perfect horse delivery registered."
        );
    }


    [ContextMenu(
        "Debug: Print Horse Info")]
    private void DebugPrintHorseInfo()
    {
        HorseData horse =
            GetCurrentHorse();

        if (horse == null)
        {
            Debug.Log(
                "No current horses available for use."
            );

            return;
        }

        Debug.Log(
            "HORSE INFO\n"
            + "Name: "
            + horse.horseName
            + "\nLevel: "
            + horse.horseLevel
            + "\nXP: "
            + horse.experience
            + "\nHealth: "
            + horse.health
            + "\nStamina: "
            + horse.stamina
            + "\nHappiness: "
            + horse.happiness
            + "\nHunger: "
            + horse.hunger
            + "\nFatigue: "
            + horse.fatigue
            + "\nCondition: "
            + GetHorseCondition()
        );
    }


    [ContextMenu(
        "Debug: Print Cart Info")]
    private void DebugPrintCartInfo()
    {
        CartData cart =
            GetCurrentCart();

        if (cart == null)
        {
            Debug.Log(
                "No current cart."
            );

            return;
        }

        Debug.Log(
            "CART INFO\n"
            + "Name: "
            + cart.cartName
            + "\nLevel: "
            + cart.cartLevel
            + "\nCapacity: "
            + cart.currentWeight
            + " / "
            + cart.maxCapacity
            + "\nDurability: "
            + cart.durability
        );
    }


    private void DebugMessage(
        string message)
    {
        if (!debugMessages)
        {
            return;
        }

        Debug.Log(
            "Horse Cart Manager "
            + message
        );
    }
}