using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class UISkillManager : NetworkBehaviour
{
    [SerializeField] Sprite[] skillIcons;
    [SerializeField] Image[] skillObjects;
    public override void Spawned()
    {
        PlayerController playerController = GetComponentInParent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.timeControlPlayer)
            {
                skillObjects[0].sprite = skillIcons[0];
                skillObjects[1].sprite = skillIcons[1];
                skillObjects[2].sprite = skillIcons[2];
            }
            else
            {
                skillObjects[0].sprite = skillIcons[3];
                skillObjects[1].sprite = skillIcons[4];
                skillObjects[2].sprite = skillIcons[5];
            }
        }
    }
}
