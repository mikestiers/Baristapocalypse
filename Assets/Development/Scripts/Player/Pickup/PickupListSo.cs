using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName ="PickupScriptableObjects", menuName ="ScriptableObjects/PickupsList")]
public class PickupListSo : ScriptableObject
{
   public List<PickupSO> PickupListSO;
}
