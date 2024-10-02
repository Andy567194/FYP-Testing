using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectile;
    bool active = true;
    float cooldown = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (active && cooldown < 0) {
            Shoot();
            cooldown = 1f;
        }
    }

    public void Shoot()
    {
        Instantiate(projectile, transform.position, Quaternion.identity);
       
    }
}
