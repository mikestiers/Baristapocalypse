using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName ="MessScriptableObjects", menuName ="ScriptableObjects/MessList")]

public class MessListSO : ScriptableObject
{
    public List<MessSO> MessSoList;
}
