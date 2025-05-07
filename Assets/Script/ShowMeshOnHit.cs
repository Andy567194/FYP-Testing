using UnityEngine;
using Fusion;

public class ShowMeshOnHit : NetworkBehaviour
{
    void OnCollisionStay(Collision other)
    {
        if (GetComponent<MeshRenderer>().enabled == false)
        {
            if (other.gameObject.CompareTag("TimeStoppable"))
            {
                // Show the mesh when the player collides with this object
                Rpc_ShowMesh();
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ShowMesh()
    {
        // Show the mesh when the player collides with this object
        GetComponent<MeshRenderer>().enabled = true;
    }
}
