using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    //S? ki?n báo cho UIHealth
    public UnityEvent<float> OnHealthPercentChanged;
    private float _currentHealth;


    void Awake()
    {
        SetupInitialHealth();
    }


    //Thi?t l?p máu v? m?c t?i ?a và thông báo tr?ng thái
    private void SetupInitialHealth()
    {
        _currentHealth = maxHealth;
        NotifyChange();
    }

    public void TakeDamage(float amount)
    {
        ApplyDamageCalculation(amount);
        NotifyChange();
    }

    //Th?c hi?n phép toán tr? máu và gi?i h?n trong kho?ng [0, Max]
    private void ApplyDamageCalculation(float amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    //Tính toán t? l? % máu và "phát tín hi?u" qua Unity Event
    private void NotifyChange()
    {
        float percent = _currentHealth / maxHealth;
        OnHealthPercentChanged.Invoke(percent);
    }
}