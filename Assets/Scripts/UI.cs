using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UI : MonoBehaviour
{
    public AttributeSet PCAttributes;
    [SerializeField] Slider _healthBar;
    [SerializeField] Slider _manaBar;
    [SerializeField] Slider _staminaBar;


    private void Start()
    {
        // _healthBar.value = playerHP / playerMaxHP;
        // _manaBar.value = playerMana / playerMaxMana;
        // _staminaBar.value = playerStamina / playerMaxStamina;
        PCAttributes.UpdateCurrentValues();
    }

    void Update()
    {
        PCAttributes.UpdateCurrentValues();
        //playerHP = PCAttributes.GetBaseAttributeValue(HP);
        _healthBar.value = GetStat("HP") / GetStat("HP Max");
        _manaBar.value = GetStat("MP") / GetStat("MP Max");
    }

    float GetStat(string statName)
    {
        return PCAttributes.GetBaseAttributeValue(PCAttributes.GetAttributeType(statName));
    }

}
