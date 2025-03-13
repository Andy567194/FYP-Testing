using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlatformRotate90 : NetworkBehaviour
{
    public void Rotate90()
    {
        transform.Rotate(0, 90, 0);
    }
}
