using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int maxMana = 100;
    public int currentMana;
    [Tooltip("Amount of mana regenerated per second.")]
    public int manaRegenRate = 5;
    // Used to accumulate mana regeneration over time.
    private float manaRegenAccumulator = 0.0f; 

    public Slider healthSlider;
    public Slider manaSlider;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        healthSlider.maxValue = maxHealth;
        manaSlider.maxValue = maxMana;
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

    void handleUpdateUISliders()
    {
        healthSlider.value = currentHealth;
        manaSlider.value = currentMana;
    }
}
