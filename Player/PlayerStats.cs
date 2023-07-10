using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int Charm { get; set; }
    public int spellPower;
    public int maxHealth;
    public int currentHealth;
    public int maxMana;
    public int currentMana;
    [Tooltip("Amount of mana regenerated per second.")]
    public int manaRegenRate = 5;
    // Used to accumulate mana regeneration over time.
    private float manaRegenAccumulator = 0.0f;

    private int baseMaxHealth = 50;
    private int baseMaxMana = 50;
    private int baseSpellPower = 10;
    private int baseCharm = 10;

    public Slider healthSlider;
    public Slider manaSlider;
    public StatsPanelManager statsPanelManager;
    private PlayerInventory playerInventory;

    void Start()
    {
        spellPower = baseSpellPower;
        Charm = baseCharm;
        maxMana = baseMaxMana;
        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;
        currentMana = maxMana;
        healthSlider.maxValue = maxHealth;
        manaSlider.maxValue = maxMana;

        playerInventory = GetComponent<PlayerInventory>();
        statsPanelManager.setStatCounts(maxHealth, maxMana, spellPower, Charm);
    }

    private void Update()
    {
        if(currentMana < maxMana)
        {
            // Accumulate the fraction of mana to be regenerated this frame.
            manaRegenAccumulator += manaRegenRate * Time.deltaTime;

            // If the accumulated value is greater than or equal to 1, we can regenerate some mana.
            if (manaRegenAccumulator >= 1.0f)
            {
                int manaToRegen = Mathf.FloorToInt(manaRegenAccumulator);

                // Increase currentMana and make sure it does not exceed maxMana.
                currentMana = Mathf.Min(currentMana + manaToRegen, maxMana);

                // Subtract the whole number portion from the accumulator.
                manaRegenAccumulator -= manaToRegen;
            }
        }
        if(currentHealth <= 0)
        {
            // Die
        }
    }

    void LateUpdate()
    {
        handleUpdateUISliders();
    }

    public void adjustHealth(int value)
    {
        currentHealth += value;
    }

    public void adjustMana(int value)
    {
        currentMana += value;
    }

    public int GetSpellPower()
    {
        return spellPower;
    }

    void handleUpdateUISliders()
    {
        healthSlider.value = currentHealth;
        manaSlider.value = currentMana;
    }

    public void UpdateStatsBasedOnEquipment()
    {
        maxHealth = baseMaxHealth;
        maxMana = baseMaxMana;
        Charm = baseCharm;
        spellPower = baseSpellPower;

        // Assuming playerInventory is a reference to your PlayerInventory instance
        foreach (var item in playerInventory.GetAllEquippedItems())
        {
            maxHealth += item.healthBoost;
            maxMana += item.manaBoost;
            Charm += item.charmBoost;
            spellPower += item.spellPowerBoost;
        }

        // Ensure current health does not exceed max health
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // Ensure current mana does not exceed max mana
        currentMana = Mathf.Min(currentMana, maxMana);

        // Update health slider's max value
        healthSlider.maxValue = maxHealth;

        // Update mana slider's max value
        manaSlider.maxValue = maxMana;

        statsPanelManager.setStatCounts(maxHealth, maxMana, spellPower, Charm);
    }

}
