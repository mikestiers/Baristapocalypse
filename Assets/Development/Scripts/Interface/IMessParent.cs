using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IMessParent
{
    Transform GetMessTransform();
    void SetMess(Mess mess);
    Mess GetMess();
    void ClearMess();
    bool HasMess();
}
