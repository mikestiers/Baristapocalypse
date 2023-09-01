using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IMessParent
{
    Transform GetMessTransform();
    void SetMess(MessBase mess);
    MessBase GetMess();
    void ClearMess();
    bool HasMess();
}
