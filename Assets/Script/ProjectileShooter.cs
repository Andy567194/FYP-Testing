using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectile;
    public GameObject ShootPosition;
    bool active = true;
    public float cooldown = 1f;

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
        Instantiate(projectile, ShootPosition.transform.position, ShootPosition.transform.rotation);
       
    }
}
