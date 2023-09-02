using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spill : MessBase
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Interact(PlayerStateMachine player)
    {
        Debug.Log("Interacting" + GetMessSO());
    }

}
