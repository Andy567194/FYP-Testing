using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ManipulateEnergy : NetworkBehaviour
{
    private Camera _camera;
    public Material highlightMaterial;
    private Material originalMaterial;
    private GameObject lastHitObject;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        // Ensure this code runs only for the player with input authority
        if (!Object.HasInputAuthority)
        {
            return;
        }

        RaycastHit raycastHit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out raycastHit, 5) && raycastHit.collider.gameObject.CompareTag("TimeStoppable"))
        {
            GameObject hitObject = raycastHit.collider.gameObject;
            Renderer hitRenderer = hitObject.GetComponent<MeshRenderer>();

            if (hitRenderer != null)
            {
                if (lastHitObject != hitObject)
                {
                    if (lastHitObject != null)
                    {
                        lastHitObject.GetComponent<MeshRenderer>().material = originalMaterial;
                    }

                    originalMaterial = hitRenderer.material;
                    hitRenderer.material = highlightMaterial;
                    lastHitObject = hitObject;
                }
            }
        }
        else if (lastHitObject != null)
        {
            lastHitObject.GetComponent<MeshRenderer>().material = originalMaterial;
            lastHitObject = null;
        }
    }

    public void UseEnergy()
    {
        Debug.Log("Energy used");
    }
}
