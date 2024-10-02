using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimPoint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float distance = 3f;
    public GameObject Cam;
    Camera cam;
    void Start()
    {
        //GameObject Cam = GameObject.FindWithTag("MainCamera");
        cam = Cam.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
        
        
    }
}
