using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SelectObject : NetworkBehaviour
{
    public GameObject selectedObject;
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
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out raycastHit, 4) && raycastHit.collider.gameObject.CompareTag("TimeStoppable"))
        {
            selectedObject = raycastHit.collider.gameObject;
            Renderer hitRenderer = selectedObject.GetComponent<MeshRenderer>();

            if (hitRenderer != null)
            {
                if (lastHitObject != selectedObject)
                {
                    if (lastHitObject != null)
                    {
                        lastHitObject.GetComponent<MeshRenderer>().material = originalMaterial;
                    }

                    originalMaterial = hitRenderer.material;
                    hitRenderer.material = highlightMaterial;
                    lastHitObject = selectedObject;
                }
            }
        }
        else if (lastHitObject != null)
        {
            lastHitObject.GetComponent<MeshRenderer>().material = originalMaterial;
            lastHitObject = null;
            selectedObject = null;
        }
    }

    public void UseEnergy()
    {
        Debug.Log("Energy used");
    }
}
