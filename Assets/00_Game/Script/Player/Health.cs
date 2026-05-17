using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;           

    [Header("Events")]
    public UnityEvent<float> OnHealthPercentChanged = new UnityEvent<float>();


    void Awake()
    {
        SetupInitialHealth();
    }


    private void SetupInitialHealth()
    {
        _currentHealth = maxHealth;
        NotifyChange();
    }



    public void TakeDamage(float amount)
    {
        //Tính toán tr? máu th?c t?
        ApplyDamageCalculation(amount);

        //thông báo cho UIHealth c?p nh?t l?i thanh Slider
        NotifyChange();
    }



    private void ApplyDamageCalculation(float amount)
    {
        _currentHealth -= amount;
        // Hàm Mathf.Clamp giúp ép _currentHealth không bao gi? b? âm d??i 0 và không bao gi? v??t quá maxHealth
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }


    private void NotifyChange()
    {
        // Ch?n l?i chia cho s? 0 n?u vô tình trong Inspector b?n nh?p Max Health = 0
        if (maxHealth <= 0) return;
        float percent = _currentHealth / maxHealth;

        //Invoke() s? phát tín hi?u ?i kèm theo d? li?u ph?n tr?m máu
        // B?t k? ??i t??ng nào ??ng ký nh?n s? ki?n này ? ngoài Inspector (nh? Slider thanh máu) s? t? ??ng ch?y theo.
        OnHealthPercentChanged.Invoke(percent);
    }
}