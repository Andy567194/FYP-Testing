using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthFill;

    public void SetHealth(float healthPercentage)
    {
        healthFill.fillAmount = healthPercentage;
    }
}