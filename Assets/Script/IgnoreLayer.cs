/*using System.Collections;
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
            // ��� Player �h������
            int playerLayer = LayerMask.NameToLayer("Player");

            // �����P�Ҧ��h���I���A���F Player �h
            for (int i = 0; i < 32; i++)
            {
                if (i != playerLayer) // �T�O������ Player �h
                {
                    Physics.IgnoreLayerCollision(myCollider.gameObject.layer, i, true);
                }
            }
        }
    }
}
*/