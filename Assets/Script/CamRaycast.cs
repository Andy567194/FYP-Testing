using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRaycast : MonoBehaviour
{
    public float rayLength = 100f;
   
    void Update()
    {
        // Define the ray from the camera
        GlobalVar.CameraACenter = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Draw the ray in the editor (for debugging)
        Debug.DrawRay(GlobalVar.CameraACenter.origin, GlobalVar.CameraACenter.direction * rayLength, Color.red);

        // Perform the raycast
        if (Physics.Raycast(GlobalVar.CameraACenter, out hit, rayLength))
        {
            // Handle what happens when the ray hits an object
            Debug.Log("Hit: " + hit.collider.name);
        }
    }

    
}
