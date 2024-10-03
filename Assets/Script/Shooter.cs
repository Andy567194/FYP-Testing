using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject item;
    public float force = 1000;
    float cooldown = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.G))
        {
            var cube = Instantiate(item, transform.position, Quaternion.identity);
            cube.GetComponent<Rigidbody>().AddForce(Vector3.right * force);
            cooldown = 1;
        }

    }

    public void Shoot()
    {
        var cube = Instantiate(item, transform.position, Quaternion.identity);
        cube.GetComponent<Rigidbody>().AddForce(Vector3.right * force);
    }
}
