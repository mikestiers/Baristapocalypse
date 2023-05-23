using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBase : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;

    public float distThreshold;


    /// <summary> Suggested stuff in common
    /// timer for time to deal with? we can have a universal starting one then we can override it after in like different character classes
    /// name? for if we want to pursue name writing in cup and calling, or just to have it here in base class
    /// 
    /// </summary>



    // Start is called before the first frame update
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();


        if (distThreshold <= 0) distThreshold = 0.5f;

        //we can add the randomization of meshes or skins here then add more stuff in specific classes?

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(!target) return;

        

      

    }

    public virtual void CustomerLeave()
    {

    }

   
}
