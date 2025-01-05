using UnityEngine;
using Fusion;

public class PlayerInteraction : NetworkBehaviour
{
    public Transform handTransform; // The transform where the object will be held
    private GameObject heldObject = null;

    [Networked]
    private NetworkButtons _PickUpPreviousButton { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData data))
        {
            var PickUpButtonPressed = data.PickUpButton.GetPressed(_PickUpPreviousButton);
            _PickUpPreviousButton = data.PickUpButton;

            if (PickUpButtonPressed.IsSet(InputButton.PickUp))
            {
                Debug.Log("PickUp button pressed");


                if (heldObject == null)
                {
                    TryPickUpObject();
                }
                else
                {
                    TryUseObject();
                }
            }
        }
        if (heldObject != null)
        {
            heldObject.transform.position = handTransform.position;
            heldObject.transform.rotation = handTransform.rotation;
        }

    }
    //void Update()
    //  {
    // if (heldObject != null)
    //  {
    //      heldObject.transform.position = handTransform.position;
    //    heldObject.transform.rotation = handTransform.rotation;
    //   }
    // }
    void TryPickUpObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldObject = hit.collider.gameObject;
                heldObject.transform.SetParent(handTransform);
                heldObject.transform.localPosition = Vector3.zero;
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false; // Disable gravity to prevent falling
                }
                Key key = heldObject.GetComponent<Key>();
                if (key != null)
                {
                    key.PickUp();
                }
            }
        }
    }

    void TryUseObject()
    {
        if (heldObject != null && heldObject.GetComponent<Key>() != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    // Unlock the door
                    hit.collider.GetComponent<Door>().Unlock();
                    // Destroy the key after use
                    Destroy(heldObject);
                    heldObject = null;
                }
            }
        }
    }
}