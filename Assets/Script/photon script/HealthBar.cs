using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    public Image healthFill;

    public void SetHealth(float healthPercentage)
    {
        healthFill.fillAmount = healthPercentage;
    }
}