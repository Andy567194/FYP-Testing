using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ObjectTimeout : NetworkBehaviour
{
    public float timeout = 5.0f;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = timeout;
    }

    // Update is called once per frame
    void Update()
    {
        if (Object.HasStateAuthority)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Runner.Despawn(Object);
            }
        }
    }
}
