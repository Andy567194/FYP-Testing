using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreLayer : MonoBehaviour
{
    private Collider myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider>();

        if (myCollider != null)
        {
            // 獲取 Player 層的索引
            int playerLayer = LayerMask.NameToLayer("Player");

            // 忽略與所有層的碰撞，除了 Player 層
            for (int i = 0; i < 32; i++)
            {
                if (i != playerLayer) // 確保不忽略 Player 層
                {
                    Physics.IgnoreLayerCollision(myCollider.gameObject.layer, i, true);
                }
            }
        }
    }
}
