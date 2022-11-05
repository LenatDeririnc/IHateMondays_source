using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpContainer : MonoBehaviour
{
    public Slider HpSlider;
    public float MaxHp;
    public float CurrentHP;

    public void TakeDamage(float damage)
    {
        CurrentHP -= damage;
        HpSlider.value = CurrentHP;
    }

    private void Awake()
    {
        CurrentHP = MaxHp;
        HpSlider.maxValue = MaxHp;
        HpSlider.value = CurrentHP;
    }
}
