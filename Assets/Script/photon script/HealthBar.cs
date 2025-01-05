using Fusion;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthFill = null;

    public void SetHealth(int Hp, int maxHP)
    {
        healthFill.fillAmount = (float)Hp / maxHP;
    }
}