using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsPanelManager : MonoBehaviour
{

    public TextMeshProUGUI healthCount;
    public TextMeshProUGUI manaCount;
    public TextMeshProUGUI spellPowerCount;
    public TextMeshProUGUI charmCount;

    public void setStatCounts (int hp, int mana, int spellPower, int charm)
    {
        healthCount.text = hp.ToString();
        manaCount.text = mana.ToString();
        spellPowerCount.text = spellPower.ToString();
        charmCount.text = charm.ToString();
    }
}
