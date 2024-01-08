using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachedHead : MonoBehaviour
{
    public GameObject blood;
    private bool bloodSpilled;
    private bool initialized;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground" && !bloodSpilled)
        {
            Vector3 spawnPosition = transform.position; // + new Vector3 (0,10,0);

            // Leave this commented out until the map is complete
            //if (Physics.Raycast(spawnPosition, -Vector3.up, out RaycastHit hit, 100, LayerMask.NameToLayer("Ground")))
            //{
            //    spawnPosition = hit.point;
            //}
            spawnPosition = new Vector3(spawnPosition.x, -2.22f, spawnPosition.z); // cheating because spills spawn slightly below the floor
            Instantiate(blood, spawnPosition, Quaternion.identity);
            bloodSpilled = true;
        }
    }

    public void Initialize()
    {
        if (initialized)
            return;
        initialized = true;
        StartCoroutine(StartHeadDetach());
    }

    IEnumerator StartHeadDetach()
    {
        transform.SetParent(null);
        Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<SphereCollider>();
        rigidBody.useGravity = true;
        rigidBody.AddForce(transform.up * 350);
        rigidBody.AddForce(transform.right * 350);
        yield return new WaitForSeconds(5);
        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }
}
