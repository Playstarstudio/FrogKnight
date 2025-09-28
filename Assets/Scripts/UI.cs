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
        //playerHP = PCAttributes.GetCurrentAttributeValue(HP);
        _healthBar.value = PCAttributes.GetCurrentAttributeValue(PCAttributes.GetAttributeType("HP")) / PCAttributes.GetCurrentAttributeValue(PCAttributes.GetAttributeType("HP Max"));
        _manaBar.value = PCAttributes.GetCurrentAttributeValue(PCAttributes.GetAttributeType("MP")) / PCAttributes.GetCurrentAttributeValue(PCAttributes.GetAttributeType("MP Max"));
        _staminaBar.value = PCAttributes.GetCurrentAttributeValue(PCAttributes.GetAttributeType("STA")) / PCAttributes.GetCurrentAttributeValue(PCAttributes.GetAttributeType("STA Max"));
    }

}
