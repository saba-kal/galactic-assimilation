using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider _slider;
    private int _maxHealth;
    private int _currentHealth;

    public void UpdateHealthBar(int maxHealth, int currentHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = currentHealth;

        _slider.minValue = 0;
        _slider.maxValue = _maxHealth;
        _slider.value = _currentHealth;
    }
}
