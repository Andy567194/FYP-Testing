using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ObjectTimeout : NetworkBehaviour
{
    public float timeout = 5.0f;
    private float timer;
    TimeControl timeControl;
    TimeRewind timeRewind;

    // Start is called before the first frame update
    void Start()
    {
        timer = timeout;
        timeControl = GetComponent<TimeControl>();
        timeRewind = GetComponent<TimeRewind>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HasStateAuthority)
        {
            if (timeControl != null && timeRewind != null)
            {
                if (!timeControl.timeStopped && !timeRewind.isRewinding)
                {
                    timer -= Time.deltaTime;
                }
            }
            else if (timeControl != null)
            {
                if (!timeControl.timeStopped)
                {
                    timer -= Time.deltaTime;
                }
            }
            else if (timeRewind != null)
            {
                if (!timeRewind.isRewinding)
                {
                    timer -= Time.deltaTime;
                }
            }
            else
            {
                timer -= Time.deltaTime;
            }
            if (timer <= 0)
            {
                Runner.Despawn(Object);
            }
        }
    }
}
