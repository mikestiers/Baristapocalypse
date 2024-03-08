using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MopBehavior : MonoBehaviour
{
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Quaternion startRot;  
    [SerializeField] private bool isNotAtStartingTrans = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void ReturnMop()
    {
        StartCoroutine(DelayReturnMop());
    }

    public IEnumerator DelayReturnMop()
    {
        yield return new WaitForSeconds(2f);

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = startPos;
        transform.rotation = startRot;
    }

}
