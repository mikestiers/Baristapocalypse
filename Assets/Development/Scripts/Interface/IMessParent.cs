using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IMessParent
{
    public Transform GetMessTransform();
    public void SetMess(MessBase mess);
    public MessBase GetMess();
    public void ClearMess();
    public bool HasMess();
}
