using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarControl : MonoBehaviour
{
    public void UpdateHealthBar(float healthPercentage)
    {
        // Ensure healthPercentage is between 0 and 1
        healthPercentage = Mathf.Clamp01(healthPercentage);
        // Update the scale of the health bar based on health percentage
        GetComponent<RectTransform>().localScale = new Vector3(healthPercentage, 1, 1);
    }
}
