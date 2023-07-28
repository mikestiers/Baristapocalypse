using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAppearanceRandomizer : MonoBehaviour
{
    public List<GameObject> heads = new List<GameObject>();
    public List<GameObject> bodies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        int headIndex = Random.Range(0, heads.Count);
        int bodyIndex = Random.Range(0, bodies.Count);
        heads[headIndex].SetActive(true);
        bodies[bodyIndex].SetActive(true);
    }
}

