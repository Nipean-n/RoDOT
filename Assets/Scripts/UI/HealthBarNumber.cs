using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBarNumber : MonoBehaviour
{
    private TMP_Text healthText;

    private void Awake()
    {
        healthText = GetComponent<TMP_Text>();
        if (healthText == null)
        {
            Debug.LogWarning($"Î´ÕÒµ½TMP_Text×é¼þ£º{name}");
        }
    }

    public void UpdateHealthNumber(int currentHealth, int maxHealth)
    {
        if (healthText == null) return;
        healthText.text = $"{currentHealth} / {maxHealth}";
    }
}